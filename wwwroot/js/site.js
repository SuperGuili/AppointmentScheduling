
var routeUrl = location.protocol + "//" + location.host;

var table = document.getElementById('financeTable');

$(document).ready(function () { // Load Kendo date picker

    $("#startDatePicker").kendoDatePicker({
        dateInput: false,
        format: "dd/MM/yyyy HH:mm"  //Change the date format as needed
    })

    $("#endDatePicker").kendoDatePicker({
        dateInput: false,
        format: "dd/MM/yyyy HH:mm"  //Change the date format as needed
    })

    //Workaround to fix style bug on X.PagedList
    //Bootstrap 4 related
    // https://github.com/dncuug/X.PagedList/issues/127
    $('ul.pagination > li.disabled > a').addClass('page-link')

});

function loadDetailsModal(id, isAppointment) {

    if (isAppointment) {
        $.ajax({
            url: routeUrl + '/api/Finance/Details/' + id,
            type: 'GET',
            dataType: 'JSON',
            success: function (response) {

                if (response.status === 1 && response.dataenum !== undefined) { //status 1 success

                    onShowDetailsModal(response.dataenum, isAppointment);
                }

            },
            error: function (xhr) {
                $.notify("Error", "error");
            }
        });
    } else {
        $.ajax({
            url: routeUrl + '/api/Finance/DetailsExpense/' + id,
            type: 'GET',
            dataType: 'JSON',
            success: function (response) {

                if (response.status === 1 && response.dataenum !== undefined) { //status 1 success

                    onShowDetailsModal(response.dataenum, isAppointment, response.message);
                }

            },
            error: function (xhr) {
                $.notify("Error", "error");
            }
        });
    }
}

function onShowDetailsModal(obj, isAppointment, message) {

    if (isAppointment) {

        $("#title").html(obj.title);
        $("#description").html(obj.description);
        $("#lblPatientName").html(obj.patientName);
        $("#lblDoctorName").html(obj.doctorName);
        $("#startDate").html(obj.startDate);
        $("#duration").html(obj.duration + " minutes");
        $("#value").html("£ " +obj.value);
        $("#paidOn").html(obj.paidDate);
        $("#lblStatus").html(obj.isDoctorApproved ? "Confirmed" : "Not Confirmed");
        $("#endDate").html(obj.endDate);
        $("#AdminName").html(obj.adminName);
        $("#telephone").html(obj.telephone);

        if (!obj.isPaid) {
            $("#paidOn").html("Not Paid");
        }

    } else {

        var paidDate = new Date(obj.expenseDate); 

        paidDate = new Intl.DateTimeFormat('en-GB', { // Format date to local date
            year: 'numeric',
            month: 'numeric',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'

        }).format(paidDate);  

        $("#title").html(obj.expenseType);
        $("#description").html(obj.expenseDescription);
        $("#value").html("£ "+(obj.expenseAmount).toFixed(2)); //Decimal to string with .00
        $("#paidOn").html(paidDate);
        $("#AdminName").html(message);

    }

    $('#appointmentDetails').modal("show");
}

function closeDetailsModal() {

    //$("#appointmentDetailsForm")[0].reset();
    $("#title").val('');
    $("#description").html("");
    $("#lblPatientName").html("");
    $("#lblDoctorName").html("");
    $("#startDate").html("");
    $("#duration").html("");
    $("#value").val(0);
    $("#paidOn").val("");
    $("#lblStatus").html("");
    $("#endDate").html("");
    $("#AdminName").html("");
    $("#telephone").html("");


    $("#appointmentDetails").modal("hide");
}

//Function to sort the table by clicking the headers
$(function () {

    $('table').on('click', 'th', function () {

        var index = $(this).index(),
            rows = [],
            thClass = $(this).hasClass('asc') ? 'desc' : 'asc';

        $('#financeTable th').removeClass('asc desc');
        $(this).addClass(thClass);
        $('#financeTable tbody tr').each(function (index, row) {
            rows.push($(row).detach());
        });

        rows.sort(function (a, b) {

            var aValue = $(a).find('td').eq(index).text(),
                bValue = $(b).find('td').eq(index).text();

            return aValue > bValue ? 1 : aValue < bValue ? -1 : 0;
        });

        if ($(this).hasClass('desc')) {
            rows.reverse();
        }

        $.each(rows, function (index, row) {
            $('#financeTable tbody').append(row);
        });

    });

});