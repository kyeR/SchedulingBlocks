var Confirm = (function () {
    function Confirm() { 
        var reservationId = "";
        var reservationEmail = "";
    }
    Confirm.Init = function Init(jsonId, jsonEmail) {
        reservationId = jsonId;
        reservationEmail = jsonEmail;

        $(document).on("submit", "#confirm_form", function (e) {
            Confirm.ConfirmFormSubmit(e);
        });
    };
    Confirm.ConfirmFormSubmit = function ConfirmFormSubmit(e) {
        $.ajax({
            url: "/reservation/ordersubmission",
            type: "POST",
            data: JSON.stringify({ReservationId: reservationId, ReservationEmail: reservationEmail}),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.success != "true") {
                    alert("We're sorry. You're cart is invalid, possibly due to another customer reserving one of your time slots. Please try again.");
                    window.location = "/";
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.responseText);
                alert(xhr.responseText);
            }
        });
    };
    return Confirm;
})();