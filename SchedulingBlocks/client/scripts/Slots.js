var Slots = (function () {
    function Slots() { }
    Slots.Init = function Init() {
        //check if we're moving back to the days view
        $(window).on("hashchange", function () {
            if (window.location.hash.length <= 0) {
                console.log(window.location.hash + " test");
                $.get("/reservation/days", function (msg) {
                    $("#content").html(msg);
                })
                .fail(function (xhr, ajaxOptions, thrownError) {
                    console.log(xhr.responseText);
                });
            }
        });

        $("#content").on("click", ".slot_select:not(.slot_reserved):not(.slot_selected)", function (e) {
            Slots.SlotClick(e);
        });
        $("#content").on("click", ".slot_selected", function (e) {
            Slots.SlotUnclick(e);
        });
        //$("#content").on("submit", "#submit_slots", function (e) {
        $("#content").on("click", ".slots_continue", function (e) {
            Slots.ContinueClick(e);
        });
    };
    Slots.SlotClick = function SlotClick(e) {
        $(e.currentTarget).addClass("slot_selected");
    };
    Slots.SlotUnclick = function SlotUnclick(e) {
        $(e.currentTarget).removeClass("slot_selected");
    };
    Slots.ContinueClick = function ContinueClick(e) {
        var slots = { Slots: [] },
        lastStart = 0,
        lastEnd = 0, 
        lastFacility = "";

        $(".slot_selected").each(function (i, el) {
            var thisFacility = $(el).data("slot-facility");
            var thisStart = $(el).data("slot-start");
            var thisEnd = $(el).data("slot-end");
            if (lastEnd == 0) {
                lastStart = thisStart;
                lastEnd = thisEnd;
                lastFacility = thisFacility;
            }
            else if (lastEnd == thisStart) {
                lastEnd = thisEnd;
            }
            else {
                var slot = {
                    StartTime: lastStart,
                    EndTime: lastEnd,
                    Facility: lastFacility  
                };
                slots.Slots.push(slot);
                lastStart = thisStart;
                lastEnd = thisEnd;
                lastFacility = thisFacility;
            }
        });
        //add last slot
        if (lastStart != 0) {
            var slot = {
                StartTime: lastStart,
                EndTime: lastEnd,
                Facility: lastFacility
            };
            slots.Slots.push(slot);
        }

        $.ajax({
            url: "/reservation/addslotstocart",
            dataType: 'json',
            type: "POST",
            data: JSON.stringify(slots),
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.result == 'Redirect') {
                    //redirecting to main page from here for the time being.
                    window.location = response.url;
                }
                else {
                    //show error
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr.responseText);
            }
        });
    };
    return Slots;
})();