
var routeUrl = location.protocol + "//" + location.host;

$(document).ready(function () {

    $("#appointmentDate").kendoDateTimePicker({
        value: new Date(),
        dateInput: false,
        format: "dd/MM/yyyy HH:mm"  //Change the date format as needed
    })

    //dateTime.timeView.options.min = new Date(2000, 0, 1, 7, 30, 0);
    //dateTime.timeView.options.max = new Date(2100, 0, 1, 19, 30, 0);

    InitializeCalendar();
});

var calendar;

function InitializeCalendar() {

    try {
        //var calendar = null;
        var calendarEl = document.getElementById('calendar');
        if (calendarEl != null) {
            calendar = new FullCalendar.Calendar(calendarEl, {

                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev,next,today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay'
                },
                selectable: true,
                editable: false,
                select: function (event) {
                    onShowModal(event, null);
                },

                businessHours: {
                    // days of week. an array of zero-based day of week integers (0=Sunday)
                    daysOfWeek: [1, 2, 3, 4, 5], // Monday - Thursday

                    startTime: '09:00', // a start time (10am in this example)
                    endTime: '19:00', // an end time (6pm in this example)
                },

                eventTimeFormat: { // like '14:30'
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: false
                },

                eventDisplay: 'block',

                events: function (fetchInfo, successCallback, failureCallback) {

                    $.ajax({
                        url: routeUrl + '/api/Appointment/GetCalendarData?doctorId=' + $("#doctorId").val(),
                        type: 'GET',
                        dataType: 'JSON',
                        success: function (response) {

                            var events = [];

                            if (response.status === 1) { //status 1 success
                                // loop the response dataenum
                                $.each(response.dataenum, function (i, data) {
                                    // populate the events array
                                    events.push({
                                        title: data.patientName,
                                        description: data.description,
                                        start: data.startDate,
                                        end: data.endDate,
                                        backgroundColor: data.isDoctorApproved ? "#28a745" : "#dc3545",
                                        borderColor: data.isPaid ? "#28a745" : "#dc3545", //"#162466",
                                        textColor: "white",
                                        id: data.id
                                    });
                                })
                            }
                            successCallback(events);
                        },
                        error: function (xhr) {
                            $.notify("Error", "error");
                        }
                    });

                },

                eventClick: function (info) {
                    getEventDetailsByEventId(info.event);
                }

            });

            calendar.render();
        }
    }
    catch (e) {
        alert(e);
    }
}

function onShowModal(obj, isEventDetail) {

    if (isEventDetail != null) { // Edit/Update Event

        $("#title").val(obj.title);
        $("#description").val(obj.description);
        $("#appointmentDate").val(obj.startDate);
        $("#duration").val(obj.duration);
        $("#doctorId").val(obj.doctorId);
        $("#lblDoctorName").html(obj.doctorName);
        $("#patientId").val(obj.patientId);
        $("#lblPatientName").html(obj.patientName);
        $("#id").val(obj.id);
        $("#doctorApproved").val(obj.isDoctorApproved);
        $("#value").val(obj.value);
        $("#isPaid").val(obj.isPaid);
        $("#telephone").html(obj.telephone);


        if (obj.isDoctorApproved) {

            $("#lblStatus").html("Approved");
            $("#btnSubmit").addClass("d-none");
            $("#btnConfirm").addClass("d-none");
            $("#btnUpdate").removeClass("d-none");

        } else {

            $("#lblStatus").html("Pending");
            $("#btnConfirm").removeClass("d-none");
            $("#btnSubmit").addClass("d-none");
            $("#btnUpdate").removeClass("d-none");////

        }

        if (obj.isPaid) {
            $('#btnPay').attr('disabled', 'disabled');
            //$("#btnPay").addClass("d-none");
        } else {
            $('#btnPay').removeAttr('disabled');
            //$("#btnPay").removeClass("d-none");
        }

        $("#btnDelete").removeClass("d-none");
        $("#btnUpdate").removeClass("d-none");
        $("#btnSubmit").addClass("d-none");

    } else {

        //When creating the appointment
        var startTime = obj.startStr; //"2021-10-22" needs to format the string

        var day = startTime.substring(8);
        var month = startTime.substring(5, 7);
        var year = startTime.substring(0, 4);

        startTime = day + "/" + month + "/" + year;

        $("#appointmentDate").val(startTime + " " + new moment().format("HH:mm"));
        $("#Id").val(0);

        $("#duration").val("30"); // Default 30 minutes for a new appointment
        onDurationChange();

        $("#btnDelete").addClass("d-none");
        $("#btnSubmit").removeClass("d-none");
        $("#btnUpdate").addClass("d-none");
        $("#btnConfirm").addClass("d-none");
    }

    $('#appointmentInput').modal("show");

}

function onCloseModal() {
    // reset the form
    $("#appointmentForm")[0].reset();
    $("#id").val(0);

    $("#title").val('');
    $("#description").val('');
    $("#appointmentDate").val('');
    $("#doctorApproved").val(false);
    $("#lblStatus").html("");
    $("#isPaid").val(false);
    $("#telephone").html("");

    $("#value").removeClass('error');

    $("#appointmentInput").modal("hide");
}

function onSubmitForm() {

    if (checkValidation()) {

        var doctorApproved = $("#doctorApproved").val();
        var confirmAction = true;

        if (doctorApproved === "true") {

            confirmAction = confirm("\nIf you edit this confirmed appointment\n It must be confirmed again");
            if (confirmAction) {
                $.notify("This Appointment must be confirmed again!", "error");
                //alert("This Appointment must be confirmed again!")
            } else {
                confirmAction = false;
                $.notify("Action canceled", "success");
            }
        }

        if (confirmAction) {

            var requestData = {
                Id: parseInt($("#id").val()),
                Title: $("#title").val(),
                Description: $("#description").val(),
                StartDate: $("#appointmentDate").val(),
                Duration: $("#duration").val(),
                DoctorId: $("#doctorId").val(),
                PatientId: $("#patientId").val(),
                Value: $("#value").val()                

            };

            $.ajax({
                url: routeUrl + '/api/Appointment/SaveCalendarData',
                type: 'POST',
                data: JSON.stringify(requestData),
                contentType: 'application/json',
                success: function (response) {
                    if (response.status === 1 || response.status === 2) { // 1 = Update / 2 = Create
                        $.notify(response.message, "success");
                        onCloseModal();
                        calendar.refetchEvents();
                    } else {
                        $.notify(response.message, "error");
                    }
                },
                error: function (xhr) {
                    $.notify("Error", "error");
                }
            });
        } else {
            onCloseModal();
            calendar.refetchEvents();
        }
    }

}

function checkValidation() {

    var isValid = true;

    if ($("#title").val() === undefined || $("#title").val() === "") {
        isValid = false;
        $("#title").addClass('error');
        $.notify("Please Enter Title", "error");
    } else {
        $("#title").removeClass('error');
    }

    if ($("#appointmentDate").val() === undefined || $("#appointmentDate").val() === "") {
        isValid = false;
        $("#appointmentDate").addClass('error');
        $.notify("Please Enter Date", "error");
    } else {
        $("#appointmentDate").removeClass('error');
    }
    var valuecheck = $("#value").val();
    if ($("#value").val() === undefined || $("#value").val() === "" || $("#value").val() === null) {
        isValid = false;
        $("#value").addClass('error');
        $.notify("Please Enter Value", "error");
    } else {
        $("#value").removeClass('error');
    }

    return isValid;

}

function getEventDetailsByEventId(info) {

    $.ajax({
        url: routeUrl + '/api/Appointment/GetCalendarDataById/' + info.id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1 && response.dataenum !== undefined) { //status 1 success

                onShowModal(response.dataenum, true);
            }
            successCallback(events);
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });

}

function onDoctorChange() {
    calendar.refetchEvents();
}

function onDeleteAppointment() {

    var id = parseInt($("#id").val());

    $.ajax({
        url: routeUrl + '/api/Appointment/DeleteAppointment/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1) { //status 1 success
                $.notify(response.message, "success");
                onCloseModal();
                calendar.refetchEvents();
            } else {
                $.notify(response.message, "error");

            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}

function onConfirm() {


    var id = parseInt($("#id").val());

    $.ajax({
        url: routeUrl + '/api/Appointment/ConfirmAppointment/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1) { //status 1 success

                $.notify(response.message, "success");
                onCloseModal();
                calendar.refetchEvents();

            } else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });

}

function onDurationChange(obj) { //Change the value for the appointment as you change the duration

    $("#value").val("");

    if ($("#duration").val() === "15") { //// duration
        $("#value").val("0.00");        //// Value
    }

    if ($("#duration").val() === "30") {
        $("#value").val("80.00");
    }

    if ($("#duration").val() === "60") {
        $("#value").val("120.00");
    }

    if ($("#duration").val() === "90") {
        $("#value").val("150.00");
    }

    if ($("#duration").val() === "120") {
        $("#value").val("200.00");
    }

}

function onPay() {

    var id = parseInt($("#id").val());

    $.ajax({
        url: routeUrl + '/api/Appointment/ConfirmPayment/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1) { //status 1 success

                $.notify(response.message, "success");
                onCloseModal();
                calendar.refetchEvents();

            } else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}