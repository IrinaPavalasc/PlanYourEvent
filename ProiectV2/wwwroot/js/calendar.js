var eventList = [];
var table = document.getElementById('daysTable');
var days = table.getElementsByTagName('tr');
var venueId = 0;

for (var day of days) {
    var item = day.getElementsByTagName('td');
    var color = '#87CEFA';
    venueId = item[4].innerText;
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

var calendarEl = document.getElementById('calendar');
var calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: 'dayGridMonth',
    selectable: true,
    eventDisplay: 'block',
    eventTextColor:'#2F4F4F',
    eventBorderColor: '#FFFFFF',
    eventMouseEnter: function (info) {
        var title = "Available  " + info.event.extendedProps.price + "$";
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
    events: eventList,
    select: function (info) {
        $('#editCalendar').modal('show');
        $('#saveChanges').click(function () {

            var price = $('#PricePerDay').val();
            var availability = true;
            if ($('#Availability').is(':checked')) {
                availability = false;
            }
            var startDate = info.startStr;
            var endDate = info.endStr;
            $.ajax({
                url: 'Calendar/Edit',
                type: 'POST',
                dataType: 'text',
                data: { price: price, availability: availability, start: startDate, end: endDate, venueId: venueId},
                success: function (status) {
                    console.log(status);
                    window.location.replace("Calendar?venueId="+venueId);
 
                },
                error: function (status) {
                    alert('Failed to edit the calendar!');
                    console.log(status);
                },
 
            });
        });
    },


   


});
calendar.render();
