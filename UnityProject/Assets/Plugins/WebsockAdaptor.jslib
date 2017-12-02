var LibraryWebsockAdaptor = {
$commons: {},
$game: {},

WebsockAdaptorStart: function() {
	var path = window.location.pathname;
	path = path.substr(0, path.lastIndexOf("/") + 1);
	var lobby = commons.lobby = new WebSocket("ws://" + location.host + path);
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
				SendMessage('Network_Manager', 'NewChannel', ev.data.substr(ADD_CHANNEL_TOKEN.length));
			}
			var REMOVE_CHANNEL_TOKEN = "-: ";
			if (ev.data.startsWith(REMOVE_CHANNEL_TOKEN)) {
				SendMessage('Network_Manager', 'RemoveChannel', ev.data.substr(REMOVE_CHANNEL_TOKEN.length));
			}
		}
	};
	var room = commons.room = new WebSocket("ws://" + location.host + "/" + location.hash);
	room.onopen = function() {
		// FIXME
	};
	room.onclose = function() {
		// FIXME
	};
	room.onmessage = function(ev) {
		if ("string" === typeof ev.data) {
		} else {
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				console.log("Receiving:: " + reader.result);
				SendMessage('Network_Manager', 'ReceivePacket', reader.result);
			});
			reader.readAsText(ev.data, "UTF-8");
		}
	};
},
WebsockAdaptorSend: function(str_ptr) {
	var str = Pointer_stringify(str_ptr);
	console.log("Sending:: " + str);
	commons.room.send(new Blob([str],  { encoding: "UTF-8" } ));
}
};

autoAddDeps(LibraryWebsockAdaptor, '$commons');
autoAddDeps(LibraryWebsockAdaptor, '$game');
mergeInto(LibraryManager.library, LibraryWebsockAdaptor);
