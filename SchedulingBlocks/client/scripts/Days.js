var Days = (function () {
    function Days() { }
    Days.Init = function Init() {
        //set each location_day_view button's click event
        $("#content").on("click", ".location_day_view:not(.day_full)", function (e) {
            Days.LocationDayViewClick(e);
        });

        $.get("/reservation/days", function (msg) {
            $("#content").html(msg);
        })
        .fail(function (xhr, ajaxOptions, thrownError) {
            console.log(xhr.responseText);
        });
    };
    Days.LocationDayViewClick = function LocationDayViewClick(e) {
        var date = $(e.currentTarget).attr("data-date");
        var action = "/reservation/slots/" + encodeURIComponent(date);
        $.get(action, function (msg) {
            $("#content").html(msg);
            window.location.hash = "#select-times";
        })
        .fail(function (xhr, ajaxOptions, thrownError) {
            console.log(xhr.responseText);
        });
    };
    return Days;
})();