$(function () {

    const token = $('#antiForgeryForm [name=__RequestVerificationToken]').val();
    let editor; // This will hold our single, persistent editor instance.

    // --- STEP 1: INITIALIZE THE TOAST UI EDITOR ONCE ---
    // We create the editor instance and keep it in memory. It is never destroyed.
    // The 'el' property points to our new container div.
    editor = new toastui.Editor({
        el: document.querySelector('#tui-editor-container'),
        height: '500px',
        initialEditType: 'markdown',
        previewStyle: 'vertical',
        // THE DEFINITIVE FIX FOR IMAGE UPLOADS: Implement the 'addImageBlobHook'.
        hooks: {
            addImageBlobHook: function (blob, callback) {
                const formData = new FormData();
                formData.append('image', blob); // The name 'image' must match our controller action's parameter.

                $.ajax({
                    url: '/BlogPosts/UploadImage', // The endpoint in our controller.
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    headers: { 'RequestVerificationToken': token }, // It's good practice to send the token.
                    success: function (response) {
                        // On success, call the editor's callback with the URL from the server.
                        callback(response.url, 'Image Alt Text');
                    },
                    error: function () {
                        // Handle errors if the upload fails.
                        console.error('Image upload failed.');
                        callback(null); // Pass null to the callback to indicate failure.
                    }
                });
            }
        }
    });

    // --- STEP 2: DATATABLES INITIALIZATION (UNCHANGED) ---
    const table = $('#blogPostsTable').DataTable({
        "processing": true,
        "ajax": { "url": "/BlogPosts/GetBlogPosts", "type": "GET", "dataSrc": "data" },
        "columns": [
            { "data": "title" }, { "data": "authorEmail" }, { "data": "categoryName" },
            {
                "data": "status",
                "render": function (data) {
                    let sClass = "", sText = "Draft";
                    switch (data) {
                        case 1: sClass = "status-pending"; sText = "PendingReview"; break;
                        case 2: sClass = "status-published"; sText = "Published"; break;
                        case 3: sClass = "status-rejected"; sText = "Rejected"; break;
                    }
                    return `<span class="badge ${sClass}">${sText}</span>`;
                }
            },
            { "data": "createdDate", "render": function (data) { return new Date(data).toLocaleDateString(); } },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `<button class="btn btn-primary btn-sm edit-btn" data-id="${data}" data-post='${JSON.stringify(row)}' title="Edit"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-danger btn-sm delete-btn" data-id="${data}" data-title="${row.title}" title="Delete"><i class="bi bi-trash"></i></button>`;
                },
                "orderable": false, "searchable": false
            }
        ]
    });

    // --- STEP 3: MODAL OPENING LOGIC ---
    function openPostModal(id, title, postData) {
        $('#postFormModalLabel').text(title);
        $.get(`/BlogPosts/GetPostForm?id=${id || ''}`, function (data) {
            $('#modal-form-fields-container').html(data);

            // Use the editor's official API to set its content.
            editor.setMarkdown(postData ? postData.content || '' : '');

            // Show the current image if it exists.
            if (postData && postData.headerImageUrl) {
                $('#current-image-container').html(`<p>Current Image:</p><img src="${postData.headerImageUrl}" style="max-width: 100%;" />`);
            } else {
                $('#current-image-container').html('');
            }

            $('#postFormModal').modal('show');
        });
    }

    $('#createPostBtn').on('click', function () {
        openPostModal(null, 'Create New Post', null);
    });

    $('#blogPostsTable tbody').on('click', '.edit-btn', function () {
        var postId = $(this).data('id');
        var postData = $(this).data('post');
        openPostModal(postId, 'Edit Blog Post', postData);
    });

    // --- STEP 4: SAVE POST LOGIC ---
    $('#savePostBtn').on('click', function () {
        var form = document.getElementById('postForm');
        var formData = new FormData(form);

        // Manually get the editor's content and append it to the form data.
        formData.append('BlogPost.Content', editor.getMarkdown());

        $.ajax({
            url: '/BlogPosts/SavePost', type: 'POST', data: formData,
            processData: false, contentType: false,
            headers: { 'RequestVerificationToken': token },
            success: function (response) {
                if (response.success) {
                    $('#postFormModal').modal('hide');
                    showToast(response.message, true);
                    table.ajax.reload(null, false);
                } else {
                    showToast(response.message, false);
                }
            },
            error: function () {
                showToast('An unexpected error occurred. Please try again.', false);
            }
        });
    });

    // --- STEP 5: DELETE POST LOGIC (UNCHANGED) ---
    $('#blogPostsTable tbody').on('click', '.delete-btn', function () {
        var postId = $(this).data('id');
        var postTitle = $(this).data('title');
        $('#postTitleToDelete').text(postTitle);
        $('#confirmDeleteBtn').data('id', postId);
        $('#deleteModal').modal('show');
    });

    $('#confirmDeleteBtn').on('click', function () {
        var postId = $(this).data('id');
        $.ajax({
            url: '/BlogPosts/Delete', type: 'POST',
            data: { id: postId, __RequestVerificationToken: token },
            success: function (response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    showToast(response.message, true);
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
});