var Cart = function() {
    var itemsToRemove;
    function Cart() {
    }

    Cart.Init = function Init() {
        $('.update-btn').hide();
        initItemRemoval();
        initUpdateButton();
        initCheckoutButton();
        itemsToRemove = [];
    };

    var initItemRemoval = function() {
        $('.trash-btn').click(function () {
            var parent = $(this).parent().parent();
            var lineItem = parent.find('td').find('.line-item');
            lineItem.wrap("<strike>");
            var cartItemId = lineItem.find('.cart-item-id').text();
            parent.find('.text-right').find('span').wrap('<strike>');
            $(this).hide();
            $('.continue-btn').text("< Cancel");
            $('.checkout-btn').hide();
            $('.update-btn').show();
            itemsToRemove.push(cartItemId);
        });
    };

    var initUpdateButton = function() {
        $('.update-btn').click(function() {
                $.ajax({
                    url: "/reservation/deleteslotsfromcart",
                    type: "POST",
                    data: JSON.stringify(itemsToRemove),
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        location.reload();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log(xhr.responseText);
                        alert(xhr.responseText);
                    }
                });
        });
    };

    var initCheckoutButton = function () {
        $('.checkout-btn').click(function () {
            window.location = "/reservation/customerinfo";

        });
    };
    return Cart;
}();