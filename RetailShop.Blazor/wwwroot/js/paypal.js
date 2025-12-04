window.openPaypalPopup = function (url, dotNetObj) {
    const width = 500;
    const height = 700;

    const left = (window.screen.width / 2) - (width / 2);
    const top = (window.screen.height / 2) - (height / 2);

    const popup = window.open(
        url,
        "paypalPopup",
        `width=${width},height=${height},top=${top},left=${left},
         menubar=no,toolbar=no,location=no,status=no,resizable=yes,scrollbars=yes`
    );

    if (!popup) {
        alert("Popup blocked! Please allow popup in your browser.");
    }

    window.addEventListener("message", function (event) {
        if (!event.data) return;

        if (event.data.status === "success") {
            dotNetObj.invokeMethodAsync("OnPaypalSuccess", event.data.orderId);
        }
        if (event.data.status === "failed") {
            dotNetObj.invokeMethodAsync("OnPaypalFailed");
        }
    });

    return popup;
};
