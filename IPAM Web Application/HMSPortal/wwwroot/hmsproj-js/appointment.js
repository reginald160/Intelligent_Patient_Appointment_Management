
$(document).ready(function () {
    $('#tblTable').DataTable({
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'csvHtml5',
                text: '<span class="ti-download"></span> csv',
                className: 'btn btn-link'
            },
            {
                extend: 'excelHtml5',
                text: '<span class="ti-align-justify"></span> Excel',
                className: 'btn btn-link'
            },
            {
                extend: 'pdfHtml5',
                text: '<span class="ti-file"></span> PDF',
                className: 'btn btn-link'
            },
            {
                extend: 'print',
                text: '<span class="ti-printer"></span> print',
                className: 'btn btn-link'
            }
        ]
    });

 
});

function confirmDelete(id) {

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Cancel it!'
    }).then((result) => {
        if (result.isConfirmed) {
            deletePatient(id);
        }
    });
}

function deletePatient(id) {
    $.ajax({
        url: "/Patient/delete/",
        data:
        {
            "id": id
        },
        type: "DELETE",
        success: function (response) {
            if (response.success) {
                Swal.fire(
                    'Deleted!',
                    response.message,
                    'success'
                ).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire(
                    'Error!',
                    response.message,
                    'error'
                );
            }
        },
        error: function () {
            Swal.fire(
                'Error!',
                'There was an error while canceling the appointment.',
                'error'
            );
        }
    });
}


function ConfirmCancelling(id) {

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Cancel it!'
    }).then((result) => {
        if (result.isConfirmed) {
            CancelAppointment(id);
        }
    });
}

function CancelAppointment(id) {
    $.ajax({
        url: "/Appointment/Cancel/",
        data:
        {
            "id": id
        },
        type: "DELETE",
        success: function (response) {
            if (response.success) {
                Swal.fire(
                    'Appointment Cancelled!',
                    response.message,
                    'success'
                ).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire(
                    'Error!',
                    response.message,
                    'error'
                );
            }
        },
        error: function () {
            Swal.fire(
                'Error!',
                'There was an error while canceling the appointment.',
                'error'
            );
        }
    });
}