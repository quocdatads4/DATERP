// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    // Add overlay HTML to body if not exists
    if ($('#global-loading-overlay').length === 0) {
        $('body').append(
            '<div id="global-loading-overlay">' +
            '<div class="loading-content">' +
            '<div class="spinner"></div>' +
            '<div class="loading-text"><i class="fas fa-hourglass-half"></i> Đang tải...</div>' +
            '</div>' +
            '</div>'
        );
    }

    // Show overlay on link clicks (excluding target="_blank", # links, etc.)
    $(document).on('click', 'a', function (e) {
        var href = $(this).attr('href');
        var target = $(this).attr('target');

        if (href &&
            !href.startsWith('#') &&
            !href.startsWith('javascript:') &&
            target !== '_blank' &&
            !e.ctrlKey && !e.shiftKey && !e.metaKey && // Don't block new tab/window
            !$(this).hasClass('no-loading') && // Allow opt-out
            !$(this).closest('.lpex-menu-item-link').length > 0 // avoid conflict with some menu items if needed, though general rule is good
        ) {
            $('#global-loading-overlay').css('display', 'flex');
        }
    });

    // Show overlay on form submit
    $(document).on('submit', 'form', function () {
        if (!$(this).hasClass('no-loading')) {
            $('#global-loading-overlay').css('display', 'flex');
        }
    });

    // Hide overlay when page is restored from bfcache (back button)
    window.addEventListener('pageshow', function (event) {
        if (event.persisted) {
            $('#global-loading-overlay').hide();
        }
    });
});
