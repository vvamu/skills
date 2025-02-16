var calendarr;
var fuull;
var items = [];
! function (e) {
    "use strict";
    var t = function() {
        this.$body = e("body"),
        this.$modal = e("#event-modal"),

            this.$editModal = e("#edit-event-modal"),
            this.titleEvent = e("#title-event"),
        this.$event = "#external-events div.external-event",
        this.$calendar = e("#calendar"),
        this.$saveCategoryBtn = e(".save-category"),
        this.$categoryForm = e("#add-category form"),
        this.$extEvents = e("#external-events"),
            this.$calendarObj = null,
            calendarr = this.$calendar
    };
    t.prototype.onDrop = function(t, n) {
        var a = t.data("eventObject"),
            o = t.attr("data-class"),
            i = e.extend({}, a);
        i.start = n, o && (i.className = [o]), this.$calendar.fullCalendar("renderEvent", i, !0), e("#drop-remove").is(":checked") && t.remove()
    },
    t.prototype.onEventClick = function (t, n, a) {
        var o = this,
            i = e("<form id='createEventForm'></form>");
        this.titleEvent.html("Change event");
        console.log(t);

        i.append("<label>Change event name</label>"),
            i.append("<div class='input-group'><input class='form-control' type=text value='" + t.title + "' style='padding-left:10px' name='name'/>"),
            i.append("<div class='input-group'><input class='form-control' id='selectedDateUpdating' type='text' value='" + t.start._d + "' style='padding-left:10px' name='time' asp-for='Item1.Time'/></div><button onclick='CreateEvent()' type='submit' class='btn btn-success waves-effect waves-light'><i class='fa fa-check'></i> Save</button></div>"),
            i.append("<div class='input-group'><input class='form-control' id='selectedEndDateUpdating' type='text' value='" + t.end + "' style='padding-left:10px' name='time' asp-for='Item1.Time'/></div><button onclick='CreateEvent()' type='submit' class='btn btn-success waves-effect waves-light'><i class='fa fa-check'></i> Save</button></div>"),

            o.$modal.modal({
            backdrop: "static"
        }), o.$modal.find(".delete-event").show().end()
            .find(".save-event").hide().end()
            .find(".modal-body").empty().prepend(i).end()
            .find(".delete-event").unbind("click").on("click", function () {


            o.$calendarObj.fullCalendar("removeEvents", function(e) {
                return e._id == t._id,
                    e.start = t;
            }), o.$modal.modal("hide")
            }), o.$modal.find("form").on("submit", function () {

                var customTime = $("input#selectedDateUpdating").val(); // Your custom time in HH:mm format
                var customEndTime = $("input#selectedEndDateUpdating").val();
                var parts = customTime.split(':');
                /*
                t.set({
                    hour: parseInt(parts[0]), // Parse the hours as an integer
                    minute: parseInt(parts[1]) // Parse the minutes as an integer
                });*/

                return t.title = i.find("input[type=text]").val(),
                    t.start._d = customTime,
                    t.end = customEndTime,
                o.$calendarObj.fullCalendar("updateEvent", t),

                o.$modal.modal("hide"), !1
        })
        },
     t.prototype.onSelect = function (t, n, a) {
        var o = this;
        o.$modal.modal({
            backdrop: "static"
        });

        var i = e("<form id='createEventForm'></form>");
        this.titleEvent.html("Create event");
        i.append("<div class='row'></div>"), i.find(".row").append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Event Name</label><input class='form-control' placeholder='Insert Event Name' type='text' name='name' asp-for='Item1.Name' id='eventName'/></div></div>")
            .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Category</label><select class='form-control' asp-for='Item1.Category' id='eventCategory' name='category'></select></div></div>")
            .find("select[name='category']").append("<option value='bg-danger'>Danger</option>")
            .append("<option value='bg-success'>Success</option>")
            .append("<option value='bg-dark'>Dark</option>")
            .append("<option value='bg-primary'>Primary</option>")
            .append("<option value='bg-info'>Info</option>")
            .append("<option value='bg-warning'>Warning</option></div></div>"),
            i.find(".row").append("<div class='input-group'><input id='selectedDateCreating' class='form-control' type='time' value='" + Date.now() + "' style='padding-left:10px' /></div></div></div>");

        o.$modal.find(".delete-event").hide().end().find(".save-event").show().end().find(".modal-body").empty().prepend(i).end().find(".save-event").unbind("click").on("click", function () {
            i.submit();
            o.$calendar.select($("input#selectedDateCreating").val());
        })
, o.$modal.find("form").on("submit", function () {
            var e = i.find("input[name='name']").val(),
                a = (i.find("input[name='beginning']").val(),
                    i.find("input[name='ending']").val(),
            i.find("select[name='category'] option:checked").val());




    //console.log(t);

    //var _d = t._d;
    var customTime = $("input#selectedDateCreating").val(); // Your custom time in HH:mm format
    var parts = customTime.split(':'); // Split the custom time string into hours and minutes
    var result = t;
    result.set({
        hour: parseInt(parts[0]), // Parse the hours as an integer
        minute: parseInt(parts[1]) // Parse the minutes as an integer
    });
    


    //.setHours($("input#selectedDateCreating").val())
            return null !== e && 0 != e.length ? (o.$calendarObj.fullCalendar("renderEvent", {
                title: e,
                start: result,
                end: n,
                allDay: !1,
                className: a
            }, !0),
                o.$modal.modal("hide")) : alert("You have to give a title to your event"), !1
}), o.$calendarObj.fullCalendar("unselect")
    }, t.prototype.enableDrag = function() {
        e(this.$event).each(function() {
            var t = {
                title: e.trim(e(this).text())
            };
            e(this).data("eventObject", t), e(this).draggable({
                zIndex: 999,
                revert: !0,
                revertDuration: 0
            })
        })
        }, t.prototype.init = function () {

        this.enableDrag();
        $(document).ready(function () {
            console.log($("#sika").val());
            items = JSON.parse($("#sika").val());
            var item = items[0];
            console.log(items[0]);
            //o.$extEvents.append('<div class="external-event ' + items[0].className + '" data-class="' + items[0].className + '" style="position: relative;"><i class="fa fa-move"></i>' + items[0].title + "</div>"), o.enableDrag();

                //t.title = i.find("input[type=text]").val(),
                //t.start._d = customTime,
                //t.end = customEndTime,
            o.$calendarObj.fullCalendar("updateEvent", item);

            (o.$calendarObj.fullCalendar("renderEvent", item, !0));

        })
        console.log("-----------------\n" + ($("#sika").val()));
        var t = new Date,
            n = (t.getDate(), t.getMonth(), t.getFullYear(), new Date(e.now())),
            a = [],
            o = this;

        
        o.$calendarObj = o.$calendar.fullCalendar({
            slotDuration: "00:15:00",
            minTime: "04:00:00",
            maxTime: "24:00:00",
            defaultView: "month",
            handleWindowResize: !0,
            height: e(window).height() - 200,



            header: {
                left: "prev,next today",
                center: "title",
                right: "month,agendaWeek,agendaDay"
            },
            events: a,
            editable: !0,
            droppable: !0,
            eventLimit: !0,
            selectable: !0,
            drop: function(t) {
                o.onDrop(e(this), t)
            },
            select: function(e, t, n) {
                o.onSelect(e, t, n)
            },
            eventClick: function(e, t, n) {
                o.onEventClick(e, t, n)
            }
        }), fuull = o.$calendar.fullCalendar,

            this.$saveCategoryBtn.on("click", function () {
            var e = o.$categoryForm.find("input[name='category-name']").val(),
                t = o.$categoryForm.find("select[name='category-color']").val();
            null !== e && 0 != e.length && (o.$extEvents.append('<div class="external-event bg-' + t + '" data-class="bg-' + t + '" style="position: relative;"><i class="fa fa-move"></i>' + e + "</div>"), o.enableDrag())
        })
    }, e.CalendarApp = new t, e.CalendarApp.Constructor = t
}(window.jQuery),

function(e) {
    "use strict";
    e.CalendarApp.init();
    }(window.jQuery);

function getValues() {
    console.log("SEFFFEEF")
    $.getJSON("/CRM/GetEvents", null, getPeople);
    
}

function getPeople(people) {

    var list = "";
    for (var i = 0; i <= people.length; i++) {
        console.log(people[i]);
        var pp = { title: people[0].Title, start: '2023-09-21', end: '2023-09-22' }
        //console.log(Object.getPrototypeOf(calendarr).fullCalendar);
        console.log(fuull("getEvents"));
    }

}

