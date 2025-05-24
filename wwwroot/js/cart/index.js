$(document).ready(function () {
    $('.qty-btn').on('click', function () {
        const $button = $(this);
        const $cartItem = $button.closest('.cart-item');
        const cartId = $cartItem.data('cart-id');
        const unitPrice = parseFloat($cartItem.data('unit-price'));
        const $qtySpan = $cartItem.find('.qty-value');
        const $totalSpan = $cartItem.find('.total-value');

        let quantity = parseInt($qtySpan.text());

        if ($button.hasClass('increase')) {
            quantity++;
        } else if ($button.hasClass('decrease') && quantity > 1) {
            quantity--;
        }

        $qtySpan.text(quantity);
        const newTotal = (quantity * unitPrice).toFixed(2);
        $totalSpan.text(newTotal);

        // Disable button temporarily
        $button.prop('disabled', true);
        console.log(cartId);
        console.log(quantity);
        $.ajax({
            url: '/api/cart/updatequantity',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                cartId: cartId,
                quantity: quantity
            }),
            success: function (response) {
                console.log("Cart updated:", response);
                // Optionally update cart total
            },
            error: function (xhr) {
                console.error("Error:", xhr.responseText);
                alert("Failed to update cart. Please try again.");
            },
            complete: function () {
                $button.prop('disabled', false);
            }
        });
    });
});


function removeCartItem() {

    $("#remove-cart-btn").click(function (e) {

        var button = $(this);
        var variantId = button.data("variant-id");

        $.ajax({
            url: "/api/cart/remove",
            type: "GET",
            data: { variantId },
            success: function (response) {
                if (response.success) {
                    renderCartCount('/api/cart/count');
                    window.location.reload();
                } else {
                    alert("Failed to remove item from cart.");
                }
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    });
}