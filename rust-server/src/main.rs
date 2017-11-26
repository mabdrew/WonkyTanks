extern crate tokio_core;
extern crate futures;
extern crate tk_http;
extern crate tk_listen;
extern crate wonky_tanks_server;

use std::cell::RefCell;
use std::collections::hash_map::HashMap;
use std::fmt::Write;
use std::rc::Rc;

use tokio_core::reactor::Core;
use tokio_core::net::TcpListener;

use futures::{Stream, Future};
use futures::future::{FutureResult, ok};
use futures::sync::mpsc::{unbounded, UnboundedSender};

use tk_http::{Status};
use tk_http::server::buffered::{Request, BufferedDispatcher};
use tk_http::server::{Encoder, EncoderDone, Config as ServerConfig, Proto, Error as ServerError};
use tk_http::websocket::{Packet, Config as WebsocketConfig, Dispatcher, Frame, Loop, Error as WebsocketError};

use tk_listen::ListenExt;

use wonky_tanks_server::*;

const LOBBY: &'static str = "/";

type SharedData = Rc<RefCell<HashMap<CountedString, ChannelState>>>;

#[derive(Clone, Default)]
pub struct ChannelState {
    capacity: Option<u8>,
    identities: usize,
    participants: Vec<Rc<UnboundedSender<Packet>>>,
}

impl std::fmt::Debug for ChannelState {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
        f
            .debug_struct("ChannelState")
            .field("capacity", &self.capacity)
            .field("identities", &self.identities)
            .field(
                "participants",
                &format_args!(
                    "{:x}",
                    IterContentsDebug(||
                        self
                            .participants
                            .iter()
                            .map(|i| (&i) as &UnboundedSender<_> as *const _ as usize)
                    )
                )
            )
            .finish()
    }
}

fn service<S>(directory: &Directory, token: &mut Option<String>, req: Request, mut e: Encoder<S>) -> FutureResult<EncoderDone<S>, ServerError> {
    if let Some(ws) = req.websocket_handshake() {
        *token = Some(req.path().to_string());
        e.status(Status::SwitchingProtocol);
        e.add_header("Connection", "upgrade").unwrap();
        e.add_header("Upgrade", "websocket").unwrap();
        e.format_header("Sec-Websocket-Accept", &ws.accept).unwrap();
        e.done_headers().unwrap();
        return ok(e.done())
    }

    if let Some((ctype, data)) = directory.get(req.path()) {
        e.status(Status::Ok);
        e.add_length(data.len() as u64).unwrap();
        e.add_header("Content-Type", ctype).unwrap();
        if e.done_headers().unwrap() {
            e.write_body(&data);
        }
        return ok(e.done())
    }

    let not_found = "No data found".as_bytes();
    e.status(Status::NotFound);
    e.add_length(not_found.len() as u64).unwrap();
    e.add_header("Content-Type", "text/plain").unwrap();
    if e.done_headers().unwrap() {
        e.write_body(not_found);
    }
    ok(e.done())
}

struct WebsocketConnection {
    shared_data: SharedData,
    sender: Rc<UnboundedSender<Packet>>,
    token: CountedString,
}

impl WebsocketConnection {
    pub fn new(shared_data: SharedData, token: String, sender: Rc<UnboundedSender<Packet>>) -> Self {
        let token: CountedString  = token.into();
        let mut data: String = Default::default();
        match match {
            let shared_data = &mut *shared_data.borrow_mut();
            let shared_data = shared_data.entry(token.clone());
            #[cfg(debug_assertions)]
            println!("Previous data: {:?}", &shared_data);
            let &mut ChannelState { ref mut participants, capacity, ref mut identities } = shared_data.or_insert_with(Default::default);
            write!(data, "identity: {}", identities).unwrap();
            drop(sender.unbounded_send(Packet::Text(data.clone())));
            *identities += 1;
            participants.push(sender.clone());
            data.clear();
            write!(data, "count: {}", participants.len()).unwrap();
            let (last, participants) = participants.split_last().unwrap();
            for participant in participants {
                drop(participant.unbounded_send(Packet::Text(data.clone())));
            }
            drop(last.unbounded_send(Packet::Text(data.clone())));
            (capacity, participants.len())
        } {
            (Some(capacity), 1) if 1 < capacity => Some(true),
            (None, 1) => Some(true),
            (Some(capacity), count) if count == capacity as usize && count != 1 => Some(false),
            _ => None,
        } {
            Some(toggle) => {
                if let Some(lobby) = shared_data.try_borrow().unwrap().get(LOBBY) {
                    data.clear();
                    write!(data, "{}: {}", if toggle { "+" } else { "-" }, token).unwrap();
                    let (last, participants) = lobby.participants.split_last().unwrap();
                    for participant in participants {
                        drop(participant.unbounded_send(Packet::Text(data.clone())));
                    }
                    drop(last.unbounded_send(Packet::Text(data)));
                }
            },
            None => {},
        }
        #[cfg(debug_assertions)]
        println!("Created connection: {:x}/{}", (&sender) as &UnboundedSender<_> as *const _ as usize, token);
        WebsocketConnection { shared_data, sender, token }
    }

    fn is_lobby(&self) -> bool {
        LOBBY == &*self.token.0
    }
}

impl Drop for WebsocketConnection {
    fn drop(&mut self) {
        let &mut WebsocketConnection {
            ref mut shared_data,
            ref sender,
            ref token
        } = self;
        use std::collections::hash_map::Entry::Occupied;
        if Some(1) != {
            let mut map = shared_data.borrow_mut();
            let mut entry =
                if let Occupied(entry) = map.entry(token.clone()) {
                    entry
                } else {
                    panic!("Bad state; expected {} to be occupied: {:?}", token, shared_data)
                };
            if let (capacity, 0) = {
                let &mut ChannelState { ref mut participants, capacity, identities: _ } = entry.get_mut();
                participants.retain(|target| !Rc::ptr_eq(target, sender));
                if !participants.is_empty() {
                    let data = format!("count: {}", participants.len());
                    let (last, participants) = participants.split_last().unwrap();
                    for participant in participants {
                        drop(participant.unbounded_send(Packet::Text(data.clone())));
                    }
                    drop(last.unbounded_send(Packet::Text(data)));
                }
                (capacity, participants.len())
            } {
                entry.remove_entry();
                capacity
            } else {
                Some(1)
            }
        } {
            if let Some(lobby) = shared_data.try_borrow().unwrap().get(LOBBY) {
                let data = format!("-: {}", token);
                let (last, participants) = lobby.participants.split_last().unwrap();
                for participant in participants {
                    drop(participant.unbounded_send(Packet::Text(data.clone())));
                }
                drop(last.unbounded_send(Packet::Text(data)));
            }
        }
        #[cfg(debug_assertions)]
        println!("Dropped connection: {:x}/{}", (&self.sender) as &UnboundedSender<_> as *const _ as usize, token);
    }
}

impl Dispatcher for WebsocketConnection {
    type Future = FutureResult<(), WebsocketError>;

    fn frame(&mut self, frame: &Frame) -> FutureResult<(), WebsocketError> {
        if self.is_lobby() { return ok(()) }
        let &mut WebsocketConnection { ref shared_data, ref sender, ref token } = self;

        match frame {
            &Frame::Text(t) => {
                // FIXME - parse state changes
            },
            &Frame::Binary(b) => {
                let &ChannelState { ref participants, .. } = &(&*shared_data.borrow())[token];
                if participants.len() == 1 { return ok(()) }
                let mut participants = participants.iter().filter(|&participant| !Rc::ptr_eq(participant, sender));
                let last = participants.next_back().unwrap();
                let to_send = Packet::Binary(Vec::from(b));
                for participant in participants {
                    drop(participant.unbounded_send(to_send.clone()));
                }
                drop(last.unbounded_send(to_send));
            },
            _ => {},
        }

        ok(())
    }
}

fn main() {
    let directory = &Directory::new(std::env::current_dir().unwrap().as_path());
    let shared_data: SharedData = Default::default();
    let mut lp = Core::new().unwrap();
    let h1 = lp.handle();

    let addr = "0.0.0.0:5000".parse().unwrap();
    let listener = TcpListener::bind(&addr, &lp.handle()).unwrap();
    let cfg = ServerConfig::new().done();
    let wcfg = WebsocketConfig::new().done();

    let pipeline = listener.incoming();
    let pipeline =
        pipeline.map(
        move |(socket, addr)| {
            let wcfg = wcfg.clone();
            let h2 = h1.clone();

            let shared_data = shared_data.clone();
            let data = Rc::new(RefCell::new(None));
            let http_data = data.clone();
            let websocket_data = data;

            Proto::new(
                socket,
                &cfg,
                BufferedDispatcher::new_with_websockets(
                    addr,
                    &h1,
                    move |p1, p2| {
                        let mut http_data = http_data.borrow_mut();
                        let http_data: &mut Option<String> = &mut http_data;
                        if let Some(token) = http_data.take() {
                            panic!("http initializer called twice: {}", token);
                        }
                        let ret = service(
                            directory,
                            http_data,
                            p1,
                            p2
                        );
                        ret
                    },
                    move |outp, inp| {
                        let (sender, receiver) = unbounded();
                        let sender = Rc::new(sender);
                        Loop::server(
                            outp,
                            inp,
                            receiver.map_err(move |e| format!("{:?} : stream closed", e)),
                            WebsocketConnection::new(
                                shared_data.clone(),
                                websocket_data
                                    .borrow_mut()
                                    .take()
                                    .expect("http initializer not called yet"),
                                sender
                            ),
                            &wcfg,
                            &h2
                        )
                            .map_err(|e| println!("Websocket err: {:?}", e))
                    }
                ),
                &h1
            )
                .map_err(|e| {println!("Connection error: {:?}", e);})
                .then(|_| Ok(())) // don't fail, please
        }
    );
    let pipeline = pipeline.listen(1000);

    lp.run(pipeline).unwrap();
}