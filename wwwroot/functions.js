window.downloadBase64File = function (fileName, base64String) {
    const blob = base64ToBlob(base64String);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
window.downloadStrFile = function (fileName, cString) {
    const blob = new Blob([cString], { type: 'text/plain' }); 
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob); 
    link.download = fileName; 
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
function base64ToBlob(base64String) {
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray]);
}

window.userInactiveTime = {
    timeout: null,
    init: function (callback, timeoutDuration) {
        window.addEventListener('mousemove', this.resetTimer);
        window.addEventListener('mousedown', this.resetTimer);
        window.addEventListener('keypress', this.resetTimer);
        window.addEventListener('touchmove', this.resetTimer);

        this.timeout = setTimeout(callback, timeoutDuration);
    },
    resetTimer: function () {
        clearTimeout(window.userInactiveTime.timeout);
        window.userInactiveTime.init(window.userInactiveTime.callback, window.userInactiveTime.timeoutDuration); // Reset timeout with the specified duration
    },
    callback: function (root) {
        // DotNet.invokeMethodAsync('Nat.Ux', 'NotifyInactive');
        window.location.href = "/login/?TimeOut=1";
    },
    setInactivityTimeout: function (timeoutDuration) {
        window.userInactiveTime.timeoutDuration = timeoutDuration;
        window.userInactiveTime.resetTimer();
    }
};
