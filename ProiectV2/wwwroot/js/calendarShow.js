function CreateCalendar(){
    var eventList = [];
    var table = document.getElementById('daysTable');
    var days = table.getElementsByTagName('tr');

    for (var day of days) {
        var item = day.getElementsByTagName('td');
        var color = '#87CEFA';
        id = item[4].innerText;
        if (item[2].innerText == 'False') {
            color = '#ffa07a';
        }
        var eventObj = {

            title: item[3].innerText + '$',
            start: item[1].innerText,
            backgroundColor: color,
            availability: item[2].innerText,
            price: item[3].innerText,

        };
        eventList.push(eventObj);

    };

    var calendarEl = document.getElementById('calendarShow');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        selectable: true,
        headerToolbar :  {
            start: 'title', // will normally be on the left. if RTL, will be on the right
            center: '',
            end: 'prev,next' // will normally be on the right. if RTL, will be on the left
        },
        eventDisplay: 'block',
        eventTextColor: '#2F4F4F',
        eventBorderColor: '#FFFFFF',
        eventMouseEnter: function (info) {
            var title = "Available  " + info.event.extendedProps.price + "$/Day";
            if (info.event.extendedProps.availability == "False") {
                title = "Rezerved";
            }
            $(info.el).tooltip({
                title: title,
                placement: "top",
                trigger: "hover",
                container: "body"
            });
        },
        validRange: {
            start: eventList[0].start,
            end: '2023-01-01'
        },
        selectOverlap: function (event) {
            return event.extendedProps.availability === 'True';
        },
        events: eventList,
        select: function (info) {
            $("#start").val(info.startStr);
            $("#end").val(info.endStr);
            

        },

    });
    calendar.render();
}

CreateCalendar();
function MakeReservation() {
    var venueId = document.getElementById("venueId").value;
    var start = document.getElementById("start").value;
    var end = document.getElementById("end").value;
    if (start==null || start=="" || end==null ||end=="") {
        alert("Please pick the dates for your reservation!");
        
    }
    else {
        $.ajax({
            url: '/Client/Reservation/Show',
            type: 'POST',
            dataType: 'text',
            data: { venueId: venueId, start: start, end: end },
            success: function (status) {
                window.location.replace("/Client/Reservation/Show?venueId=" + venueId + "&start=" + start + "&end=" + end);
            },
            error: function (status, request, error) {
                $(document).ajaxError(function (event, jqxhr, settings, exception) {
                    if (jqxhr.status == 401) {
                        // unauthorized
                        window.location.replace('/Identity/Account/Login');
                        alert("Ypu must have a client account to make a reservation!");
                    }
                });

            },
        });
    }
}

