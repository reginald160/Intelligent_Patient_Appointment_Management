$(document).ready(function () {
    loaddatatable();
});

function loaddatatable() {

    dataTable = $('#tblTable').DataTable({
        "ajax": { url: '/patient/getall' },

        "columns": [
            { data: 'firstName', "width": "15%" },
            { data: 'lastName', "width": "15%" },
            { data: 'phone', "width": "10%" },
            { data: 'address', "width": "15%" },
            { data: 'dateOfBirth', "width": "10%" },
            { data: 'gender', "width": "15%" },

            {
                data: 'id',

                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/patient/delete?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-squar"></i>Edit</a>
                        <a href="/patient/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                    
                    </div>`
                },

                "width": "20%"
            }
        ]
    });
}