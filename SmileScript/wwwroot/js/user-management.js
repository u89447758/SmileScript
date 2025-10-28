$(function () {

    // Get the anti-forgery token from our hidden form as soon as the page loads.
    const token = $('#antiForgeryForm [name=__RequestVerificationToken]').val();

    // =================================================================
    // STEP 1: INITIALIZE DATATABLES TO HANDLE AJAX
    // =================================================================
    // We store the DataTable instance in a variable so we can access its API later (e.g., to reload data).
    const table = $('#usersTable').DataTable({
        "processing": true, // Shows a "processing" indicator during AJAX calls
        "serverSide": false, // We are doing client-side processing for this small dataset
        "ajax": {
            "url": "/UserManagement/GetUsers", // The endpoint to fetch data from
            "type": "GET",
            "dataSrc": "data" // Tells DataTables that the user array is in the 'data' property of the JSON response
        },
        "columns": [
            // Column 1: Email. The 'data' property must match the JSON property name from our ViewModel.
            { "data": "email" },
            // Column 2: Role
            { "data": "role" },
            // Column 3: Actions (Edit/Delete buttons)
            {
                "data": "id", // We pass the 'id' of the user to the render function
                "render": function (data, type, full, meta) {
                    // 'data' is the user's ID.
                    // 'full' is the entire user object for the row (so we can get 'full.email').
                    // We return the HTML for the buttons.
                    return `<button class="btn btn-primary btn-sm edit-btn" data-id="${data}" title="Edit"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-danger btn-sm delete-btn" data-id="${data}" data-email="${full.email}" title="Delete"><i class="bi bi-trash"></i></button>`;
                },
                "orderable": false, // The actions column should not be sortable
                "searchable": false // Or searchable
            }
        ]
    });

    // =================================================================
    // STEP 2: ALL OTHER EVENT HANDLERS REMAIN LARGELY THE SAME
    // The only change is that we call `table.ajax.reload()` instead of `loadUsers()`.
    // =================================================================

    $('#createUserBtn').on('click', function () {
        $.get('/UserManagement/GetUserForm', function (data) {
            $('#userFormModalLabel').text('Create New User');
            $('#userFormModalBody').html(data);
            $('#password-section').show();
            $('#Email').prop('readonly', false);
            $('#userFormModal').modal('show');
        });
    });

    $('#usersTable tbody').on('click', '.edit-btn', function () {
        var userId = $(this).data('id');
        $.get(`/UserManagement/GetUserForm?id=${userId}`, function (data) {
            $('#userFormModalLabel').text('Edit User');
            $('#userFormModalBody').html(data);
            $('#password-section').hide();
            $('#Email').prop('readonly', true);
            $('#userFormModal').modal('show');
        });
    });

    $('#saveUserBtn').on('click', function () {
        var form = $('#userForm');
        if (form.valid()) {
            var formData = form.serialize();
            $.ajax({
                url: '/UserManagement/SaveUser',
                type: 'POST',
                data: formData,
                headers: { 'RequestVerificationToken': token },
                success: function (response) {
                    if (response.success) {
                        $('#userFormModal').modal('hide');
                        showToast(response.message, true);
                        // THE FIX: Use the DataTables API to reload the data
                        table.ajax.reload(null, false); // 'null, false' keeps the user on the same page
                    } else {
                        showToast(response.message, false);
                    }
                },
                error: function () {
                    showToast('An unexpected error occurred. Please try again.', false);
                }
            });
        }
    });

    $('#usersTable tbody').on('click', '.delete-btn', function () {
        var userId = $(this).data('id');
        var userEmail = $(this).data('email');
        $('#userEmailToDelete').text(userEmail);
        $('#confirmDeleteBtn').data('id', userId);
        $('#deleteModal').modal('show');
    });

    $('#confirmDeleteBtn').on('click', function () {
        var userId = $(this).data('id');
        $.ajax({
            url: '/UserManagement/Delete',
            type: 'POST',
            data: {
                id: userId,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    showToast(response.message, true);
                    // THE FIX: Use the DataTables API to reload the data
                    table.ajax.reload(null, false);
                } else {
                    showToast(response.message, false);
                }
            },
            error: function () {
                showToast('An unexpected error occurred during deletion.', false);
            }
        });
    });

    // We no longer need to call loadUsers() here, as DataTables does it automatically on initialization.
});