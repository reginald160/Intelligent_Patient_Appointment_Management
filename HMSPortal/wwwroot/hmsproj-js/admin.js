$(document).ready(function () {
    $('#tblTable').DataTable();
});

function confirmDelete(id) {

  
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            deletePatient(id);
        }
    });
}

function deletePatient(id) {
    $.ajax({
        url: "/Admin/delete/",
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
                'There was an error deleting the admin.',
                'error'
            );
        }
    });
}

function confirmLock(id) {

    Swal.fire({
        title: 'Are you sure?',
        text: "Locking user will disble his access!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, disable it!'
    }).then((result) => {
        if (result.isConfirmed) {
            lockAdmin(id);
        }
    });
}

function lockAdmin(id) {
    $.ajax({
        url: "/Admin/Lock/",
        data:
        {
            "id": id
        },
        type: "DELETE",
        success: function (response) {
            if (response.success) {
                Swal.fire(
                    'Locked!',
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
                'There was an error locking the admin.',
                'error'
            );
        }
    });

 
}

function confirmUnLock(id) {

    Swal.fire({
        title: 'Are you sure?',
        text: "Locking user will enable his access!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, enable it!'
    }).then((result) => {
        if (result.isConfirmed) {
            UnlockAdmin(id);
        }
    });
}

function UnlockAdmin(id) {
    $.ajax({
        url: "/Admin/UnLock/",
        data:
        {
            "id": id
        },
        type: "DELETE",
        success: function (response) {
            if (response.success) {
                Swal.fire(
                    'Unlocked!',
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
                'There was an error Unlocking the admin.',
                'error'
            );
        }
    });
}