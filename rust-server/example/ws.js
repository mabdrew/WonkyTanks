(() => {
let mb = document.getElementById('responses');
let input = document.getElementById('input');
let encoder = new TextEncoder("utf-8");
let decoder = new TextDecoder("utf-8");
let identity;
let ws;

function log(type, message, node) {
    let date = document.createElement('span');
    date.appendChild(document.createTextNode(new Date().toLocaleString()));
    date.className = "date";
    let content = node || document.createElement('span');
    content.appendChild(document.createTextNode(message));
    let red = document.createElement('div');
    red.className = type;
    red.appendChild(date);
    red.appendChild(content);
    mb.insertBefore(red, mb.childNodes[0]);
}

function initWebSocket(hash) {
    return new WebSocket("ws://" + location.host + "/" + (hash || location.hash.substr(1)));
}

function hide() {
    input.style.visibility = 'hidden';
}

function onClose(socket) {
    return () => {
        if (socket === ws) {
            hide();
            log('warning', "Disconnected");
        } else {
            log('warning', "(Old) Disconnected");
        }
    }
}

function onError() {
    hide();
    log('warning', 'ERROR: ' + e);
}

function onOpen() {
    log('debug', "Connected");
    input.style.visibility = 'visible';
}

function init(newSocket) {
    ws = newSocket || initWebSocket();
    ws.onopen = onOpen;
    ws.onclose = onClose(ws);
    ws.onerror = onError;

    function onMessage(ev) {
        if ("string" === typeof ev.data) {
            const IDENTITY_TOKEN = "identity: ";
            if (ev.data.startsWith(IDENTITY_TOKEN)) {
                identity = ev.data.substr(IDENTITY_TOKEN.length) + ": ";
            }
            const ADD_TOKEN = "+: ";
            if (ev.data.startsWith(ADD_TOKEN)) {
                let token = ev.data.substr(ADD_TOKEN.length + 1);
                let link = document.createElement('a');
                link.href = `#${token}`;
                link.onclick = () => {
                    hide();
                    ws.close();
                    location.hash = link.href;
                    init(initWebSocket(token));
                };
                log('meta', ev.data, link);
                return;
            }
            log('meta', ev.data);
        } else {
            let reader = new FileReader();
            reader.onload = (event) => log('text', decoder.decode(event.target.result));
            reader.readAsArrayBuffer(ev.data)
        }
    }
    ws.onmessage = onMessage;
}

input.onkeydown = function(ev) {
    if (ev.which === 13) {
        let value = (identity || "") + input.value;
        ws.send(encoder.encode(value));
        log('self', value);
        input.value = '';
    }
};

init();
})()
