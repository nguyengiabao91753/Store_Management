document.addEventListener("DOMContentLoaded", () => {

    // Calculate subtotal, VAT, total
    function parseCurrencyToNumber(text) {
        if (!text) return 0;
        const cleaned = text.replace(/[^0-9.\-]+/g, ''); // bỏ ký tự $, ₫, , ...
        const n = parseFloat(cleaned);
        return isNaN(n) ? 0 : n;
    }

    function recalcTotals() {
        let subtotal = 0;

        // chỉ tính các phần tử có class each-price-product
        $('.each-price-product').each(function () {
            const price = parseCurrencyToNumber($(this).text());
            subtotal += price;
        });

        const discount = parseFloat($('#DiscountAmount').val()) || 0;
        const vat = subtotal * 0.08;
        const total = subtotal + vat - discount;

        // cập nhật UI
        $('#subtotal').text(`$${subtotal.toFixed(2)}`);
        $('#vat').text(`$${vat.toFixed(2)}`);
        //$('#discount').text(`-$${discount.toFixed(2)}`);
        $('#total').text(`$${total.toFixed(2)}`);

        // cập nhật input hidden để gửi khi submit form
        $('#TotalAmount').val(total.toFixed(2));
    }


    recalcTotals();

    // Apply promo
    $('#btnApplyPromo').click(function () {
        const code = $('#promoCode').val();
        const subtotal = parseFloat($('#subtotal').text().replace(/[^0-9.-]+/g, "")); // lấy subtotal hiện có

        if (!code) {
            $('#promoMessage').text("Please enter a promo code.");
            return;
        }

        $.ajax({
            url: '/Promotion/CheckCode',
            type: 'POST',
            data: { code: code },
            success: function (res) {
                if (res.isValid) {
                    let discount = 0;

                    if (res.discountPercent > 0)
                        discount = subtotal * (res.discountPercent / 100);
                    else
                        discount = res.discountAmount;

                    const totalAfterDiscount = subtotal - discount;

                    $('#promoMessage').removeClass('text-danger').addClass('text-success')
                        .text(`Applied ${code}! `);
                    $('#discount').text(`-${discount.toLocaleString()}₫`);
                    $('#total').text(`${totalAfterDiscount.toLocaleString()}₫`);

                    $('#DiscountAmount').val(discount);
                    $('#PromoId').val(res.promoId);
                  
                    $('#discountBadge').removeClass('d-none').text(`${res.discountPercent}% Discount`);
                    recalcTotals();
                } else {
                    $('#promoMessage').text('Invalid or expired code!').addClass('text-danger');
                    $('#DiscountAmount').val(0);
                    $('#PromoId').val('');
                    recalcTotals();
                }
            },
            error: function () {
                alert('Error checking promo code.');
            }
        });
    });

    // Payment option toggle
    $('.payment-option').click(function () {
        $('.payment-option').removeClass('active');
        $(this).addClass('active');
        $(this).find('input[type=radio]').prop('checked', true);
    });
});
