$(document).ready(function () {
    loaddatatable();
});

function loaddatatable() {

    dataTable = $('#myMblTable').DataTable({
        "ajax": { url: '/doctor/getall' },

        "columns": [
           
            { data: 'advancePaid', "width": "10%" },
            { data: 'discount', "width": "15%" },
            { data: 'serviceName', "width": "10%" },
            { data: 'paymentPaid', "width": "15%" },
            { data: 'doctor.firstName', "width": "15%" },
            { data: 'patient.firstName', "width": "15%" },

            {
                data: 'id',

                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/doctor/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-squar"></i>Edit</a>
                        <a href="/doctor/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                    </div>`
                },

                "width": "20%"
            }
        ]
    });
}