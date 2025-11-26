
const keyStates = {};

// WebSocket connection to server for input
let ws;
function connect() {
    try {
        ws = new WebSocket((location.protocol === 'https:' ? 'wss://' : 'ws://') + location.host + '/');
        ws.addEventListener('open', () => {
            // send initial state
            sendToServer();
        });
        ws.addEventListener('close', () => {
            // try to reconnect after a short delay
            setTimeout(connect, 1000);
        });
        ws.addEventListener('error', () => {
            try { ws.close(); } catch (e) { }
        });
    } catch (e) {
        // ignore
    }
}
connect();

window.addEventListener("keydown", function (event) {
    keyStates[event.key] = true;
    sendToServer();
});
window.addEventListener("keyup", function (event) {
    keyStates[event.key] = false;
    sendToServer();
});

const sendToServer = () => {
    const pressedKeys = Object.keys(keyStates).filter(key => keyStates[key]);
    const payload = pressedKeys.join(',');
    if (ws && ws.readyState === WebSocket.OPEN) {
        ws.send(payload);
    }
};

const faceButton = (/** @type {string} */key) => {
    keyStates[key] = true;
    sendToServer();
    setTimeout(() => {
        keyStates[key] = false;
        sendToServer();
    }, 100);
};