(() => {
    let ws = new WebSocket("ws://" + location.host + "/chat");
    let mb = document.getElementById('responses');
    let input = document.getElementById('input');
    let encoder = new TextEncoder("utf-8");
    let decoder = new TextDecoder("utf-8");
    let identity;
    ws.onopen = function() {
        log('debug', "Connected");
        input.style.visibility = 'visible';
    };

    ws.onclose = function() {
        input.style.visibility = 'hidden';
        log('warning', "Disconnected");
    };

    ws.onerror = function(e) {
        input.style.visibility = 'hidden';
        log('warning', 'ERROR: ' + e);
    };
    ws.onmessage = function(ev) {
        if ("string" === typeof ev.data) {
            log('meta', ev.data);
            const IDENTITY_TOKEN = "identity: ";
            if (ev.data.startsWith(IDENTITY_TOKEN)) {
                identity = ev.data.substr(IDENTITY_TOKEN.length) + ": ";
            }
        } else {
            let reader = new FileReader();
            reader.onload = (event) => log('text', decoder.decode(event.target.result));
            reader.readAsArrayBuffer(ev.data)
        }
    };
    input.onkeydown = function(ev) {
        if(ev.which === 13) {
            let value = (identity || "") + input.value;
            ws.send(encoder.encode(value));
            log('self', value);
            input.value = '';
        }
    };

    function log(type, message) {
        let red = document.createElement('div');
        red.className = type;
        red.appendChild(document.createTextNode(message));
        mb.insertBefore(red, mb.childNodes[0]);
    }

})();
