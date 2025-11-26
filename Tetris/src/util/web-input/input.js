
const keyStates = {};
window.addEventListener("keydown", function(event) {
    keyStates[event.key] = true;
    sendToServer();
});
window.addEventListener("keyup", function(event) {
    keyStates[event.key] = false;
    sendToServer();
});

const sendToServer = () => {
    const pressedKeys = Object.keys(keyStates).filter(key => keyStates[key]);
    if (pressedKeys.length > 0) {
        fetch('/input', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ keys: pressedKeys })
        });
    }
}