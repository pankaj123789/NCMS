var loadingOverlay = function () {
    var selector = '.loading-overlay';

    return {
        show: function () {
            $(selector).fadeIn();
        },
        hide: function () {
            $(selector).fadeOut();
        }
    }
}();
