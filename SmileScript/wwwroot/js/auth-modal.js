$(function () {

    var authModalTitle = $('#authModalLabel');
    var authModalBody = $('#authModalBody');
    var loadingSpinner = '<div class="text-center"><div class="spinner-border" role="status"><span class="sr-only">Loading...</span></div></div>';

    // --- START: New code for Show/Hide Password ---
    // Use event delegation on the document to handle clicks for elements
    // that are loaded dynamically into the auth modal.
    $(document).on('click', '.password-toggle', function () {
        // Find the input field that is a direct sibling before this button.
        var input = $(this).siblings('input');
        // Find the icon element inside this button.
        var icon = $(this).find('i');

        // Check the current type of the input field.
        if (input.attr('type') === 'password') {
            // If it's a password, change it to text to show it.
            input.attr('type', 'text');
            // Change the icon to the "slashed" version.
            icon.removeClass('bi-eye').addClass('bi-eye-slash');
        } else {
            // If it's text, change it back to password to hide it.
            input.attr('type', 'password');
            // Change the icon back to the "open" version.
            icon.removeClass('bi-eye-slash').addClass('bi-eye');
        }
    });
    // --- END: New code for Show/Hide Password ---

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

    $(document).on('click', '.login-modal-trigger', function (e) {
        e.preventDefault();
        loadAuthModal('/Auth/GetLoginModal', 'Log In');
    });

    $(document).on('click', '.register-modal-trigger', function (e) {
        e.preventDefault();
        loadAuthModal('/Auth/GetRegisterModal', 'Create Account');
    });

    // *** NEW EVENT LISTENER ADDED HERE ***
    $(document).on('click', '.forgot-password-modal-trigger', function (e) {
        e.preventDefault();
        loadAuthModal('/Auth/GetForgotPasswordModal', 'Reset Password');
    });

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

                    // *** MODIFIED LOGIC HERE ***
                    // Only redirect if the server sends a redirectUrl (for login/register).
                    // For "Forgot Password", it will not redirect, just show the toast.
                    if (response.redirectUrl) {
                        setTimeout(function () {
                            window.location.href = response.redirectUrl;
                        }, 1500);
                    }
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

    $('.modal').on('hide.bs.modal', function () {
        if (document.activeElement) {
            $(document.activeElement).trigger('blur');
        }
        $('body')[0].focus();
    });

});