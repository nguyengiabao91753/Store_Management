<script src="https://www.paypal.com/sdk/js?client-id=AdTZ2ync1fLHI42NkgWL2vsv9d9hTwZH0hRMVszhy2AleQEZ8xGxJAnEpqzTIdFeEcTvTAsYxcXXCZGr&currency=USD"></script>

    window.initPayPalButton = (orderId, dotNetHelper) => {
        paypal.Buttons({
            style: {
                shape: 'rect',
                color: 'gold',
                layout: 'vertical',
                label: 'paypal',
                height: 45
            },

            createOrder: function(data, actions) {
                return fetch('/api/paypal/create-order', {
                    method: 'post',
                    headers: { 'content-type': 'application/json' },
                    body: JSON.stringify({ /* có thể gửi thêm dữ liệu nếu cần */ })
                })
                .then(res => res.json())
                .then(orderData => orderData.id);
            },

            onApprove: function(data, actions) {
                return fetch(`/api/paypal/capture-order/${data.orderID}`, {
                    method: 'post'
                })
                .then(res => res.json())
                .then(orderData => {
                    if (orderData.status === "COMPLETED") {
                        dotNetHelper.invokeMethodAsync('OnPaymentSuccess');
                    }
                });
            },

            onCancel: function(data) {
                dotNetHelper.invokeMethodAsync('OnPaymentCancel');
            },

            onError: function(err) {
                dotNetHelper.invokeMethodAsync('OnPaymentError', err.toString());
            }

        }).render('#paypal-button-container');
    };
