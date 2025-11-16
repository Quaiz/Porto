// cart.js - Cart functionality with AJAX
$(document).ready(function () {
    // Update cart count on page load
    updateCartCount();

    // Add to cart
    $('.add-to-cart').on('click', function () {
        var productId = $(this).data('id');
        addToCart(productId, 1);
    });

    // Update quantity
    $(document).on('change', '.quantity-input', function () {
        var productId = $(this).data('id');
        var quantity = parseInt($(this).val());
        if (quantity > 0) {
            updateQuantity(productId, quantity);
        }
    });

    // Remove item
    $(document).on('click', '.remove-item', function () {
        var productId = $(this).data('id');
        removeItem(productId);
    });

    function addToCart(productId, quantity) {
        $.post('/Cart/AddToCart', { productId: productId, quantity: quantity })
            .done(function (response) {
                if (response.success) {
                    updateCartCount(response.count);
                    updateGrandTotal(response.total);
                    showMessage('Product added to cart!', 'success');
                } else {
                    showMessage(response.message || 'Failed to add product', 'error');
                }
            })
            .fail(function () {
                showMessage('Error communicating with server', 'error');
            });
    }

    function updateQuantity(productId, quantity) {
        $.post('/Cart/UpdateQuantity', { productId: productId, quantity: quantity })
            .done(function (response) {
                if (response.success) {
                    updateGrandTotal(response.total);
                    updateCartCount();
                    // Update item total
                    var row = $('.quantity-input[data-id="' + productId + '"]').closest('tr');
                    var price = parseFloat(row.find('td:eq(1)').text().replace(/[^0-9.-]+/g, ""));
                    row.find('.item-total').text('$' + (price * quantity).toFixed(2));
                }
            });
    }

    function removeItem(productId) {
        $.post('/Cart/RemoveItem', { productId: productId })
            .done(function (response) {
                if (response.success) {
                    updateGrandTotal(response.total);
                    updateCartCount(response.count);
                    // Remove row
                    $('.remove-item[data-id="' + productId + '"]').closest('tr').remove();
                    if ($('tbody tr').length === 0) {
                        location.reload();
                    }
                }
            });
    }

    function updateCartCount(count) {
        if (count === undefined) {
            $.get('/Cart/GetCount', function (data) {
                $('#cart-count').text(data);
            });
        } else {
            $('#cart-count').text(count);
        }
    }

    function updateGrandTotal(total) {
        $('#grand-total').text(total);
    }

    function showMessage(message, type) {
        // Simple alert fallback - can be replaced with toastr or bootstrap toast
        if (typeof toastr !== 'undefined') {
            if (type === 'success') {
                toastr.success(message);
            } else {
                toastr.error(message);
            }
        } else {
            alert(message);
        }
    }
});