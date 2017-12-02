var LibraryWebsockAdaptor = {
$commons: {},
$game: {},

// SendMessage('Game', 'FunctionName', ...);

WebsockAdaptorStart: function() {
	var path = window.location.pathname;
	path = path.substr(0, path.lastIndexOf("/") + 1);
	var lobby = commons.lobby = new WebSocket("ws://" + location.host + path);
	var encoder = commons.encoder.new TextEncoder("utf-8");
	var decoder = commons.decoder.new TextDecoder("utf-8");
	lobby.onopen = function() {
		// FIXME
	};
	lobby.onclose = function() {
		// FIXME
	};
	lobby.onmessage = function(ev) {
		if ("string" === typeof ev.data) {
			var ADD_CHANNEL_TOKEN = "+: ";
			if (ev.data.startsWith(ADD_CHANNEL_TOKEN)) {
				SendMessage('Game', 'NewChannel', ev.data.substr(ADD_CHANNEL_TOKEN.length));
			}
			var REMOVE_CHANNEL_TOKEN = "-: ";
			if (ev.data.startsWith(REMOVE_CHANNEL_TOKEN)) {
				SendMessage('Game', 'RemoveChannel', ev.data.substr(REMOVE_CHANNEL_TOKEN.length));
			}
		}
	};
	var room = commons.room = new WebSocket("ws://" + location.host + "/example");
	room.onopen = function() {
		// FIXME
	};
	room.onclose = function() {
		// FIXME
	};
	room.onmessage = function(ev) {
		if ("string" === typeof ev.data) {
		} else {
			let reader = new FileReader();
			reader.onload = function (event) {
				SendMessage('Game', 'ReceivePacket', decoder.decode(event.target.result));
			}
			reader.readAsArrayBuffer(ev.data)
		}
	};
},

WebsockAdaptorSend: function(str_ptr) {
	let str = Pointer_stringify(str_ptr);
	commons.room.send(encoder.encode(str));
}

};

autoAddDeps(LibraryWebsockAdaptor, '$commons');
autoAddDeps(LibraryWebsockAdaptor, '$game');
mergeInto(LibraryManager.library, LibraryWebsockAdaptor);
