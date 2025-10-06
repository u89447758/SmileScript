$(function () {

    var authModalTitle = $('#authModalLabel');
    var authModalBody = $('#authModalBody');
    var loadingSpinner = '<div class="text-center"><div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div></div>';

    /**
     * A reusable function to load content into the modal via AJAX using Bootstrap 4 syntax.
     * @param {string} url The URL to fetch the modal content from.
     * @param {string} title The title to display in the modal header.
     */
    function loadAuthModal(url, title) {
        authModalTitle.text(title);
        authModalBody.html(loadingSpinner);
        $('#authModal').modal('show');

        $.get(url, function (data) {
            authModalBody.html(data);
        }).fail(function () {
            authModalBody.html('<p class="text-danger">Sorry, an error occurred while loading the content.</p>');
        });
    }

    // Event listeners for triggering the modals.
    $(document).on('click', '.login-modal-trigger', function (e) {
        e.preventDefault();
        loadAuthModal('/Auth/GetLoginModal', 'Log In');
    });

    $(document).on('click', '.register-modal-trigger', function (e) {
        e.preventDefault();
        loadAuthModal('/Auth/GetRegisterModal', 'Create Account');
    });

    /**
     * Handle form submission for any form loaded into the modal body.
     */
    authModalBody.on('submit', 'form', function (e) {
        e.preventDefault();
        var form = $(this);
        var submitButton = form.find('button[type="submit"]');
        var originalButtonText = submitButton.html();
        submitButton.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    showToast(response.message || 'Success!', true);
                    $('#authModal').modal('hide');
                    setTimeout(function () {
                        window.location.href = response.redirectUrl || window.location.pathname;
                    }, 1500);
                } else {
                    showToast(response.message || 'An error occurred.', false);
                    submitButton.prop('disabled', false).html(originalButtonText);
                }
            },
            error: function () {
                showToast('An unexpected network error occurred. Please try again.', false);
                submitButton.prop('disabled', false).html(originalButtonText);
            }
        });
    });

    // Handle the logout confirmation using AJAX.
    $('#confirmLogoutButton').on('click', function () {
        var logoutForm = $('#logoutForm');
        $.ajax({
            url: logoutForm.attr('action'),
            type: 'POST',
            data: logoutForm.serialize(),
            success: function (response) {
                if (response.success) {
                    showToast(response.message, true);
                    $('#logoutConfirmModal').modal('hide');
                    setTimeout(function () {
                        window.location.href = response.redirectUrl;
                    }, 1500);
                } else {
                    showToast('Logout failed. Please try again.', false);
                }
            },
            error: function () {
                showToast('An unexpected network error occurred during logout.', false);
            }
        });
    });

    // --- FINAL ACCESSIBILITY FIX ---
    // This event fires just as the modal starts to close.
    $('.modal').on('hide.bs.modal', function () {
        // Step 1: Find the currently active element within the document and force it to lose focus.
        // This is the most direct way to tell the browser that nothing inside the modal is active anymore.
        if (document.activeElement) {
            $(document.activeElement).trigger('blur');
        }

        // Step 2: As a safe fallback, explicitly set the focus back to the main body of the page.
        // This ensures focus is not lost or trapped somewhere inaccessible.
        $('body')[0].focus();
    });

});