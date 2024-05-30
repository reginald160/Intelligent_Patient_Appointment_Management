$(document).ready(function () {
    loaddatatable();
});

function loaddatatable() {

    dataTable = $('#tblTable').DataTable({
        "ajax": { url: '/doctor/getall' },

        "columns": [
            { data: 'firstName', "width": "15%" },
            { data: 'lastName', "width": "15%" },
            { data: 'phone', "width": "10%" },
            { data: 'address', "width": "15%" },
            { data: 'specialty', "width": "10%" },
            { data: 'yearsOfExperience', "width": "15%" },

            {
                data: 'id',//myMblTable

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