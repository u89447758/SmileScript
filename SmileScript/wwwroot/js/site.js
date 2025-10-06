// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Global function to show a toast message
function showToast(message, isSuccess) {
    var toastHtml = `
        <div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">
            <div class="toast-header">
                <strong class="mr-auto ${isSuccess ? 'text-success' : 'text-danger'}">
                    ${isSuccess ? '<i class="bi bi-check-circle-fill"></i> Success' : '<i class="bi bi-x-circle-fill"></i> Error'}
                </strong>
                <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>`;
    $('#toastContainer').append(toastHtml);
    // Initialize and show the toast
    // NOTE: For Bootstrap 4 used in AdminLTE, we must use .toast('show')
    // For Bootstrap 5, it would be bootstrap.Toast.getInstance(element).show()
    $('.toast:last').toast('show');

    // Remove the toast from the DOM after it's hidden
    $('.toast').on('hidden.bs.toast', function () {
        $(this).remove();
    });
}