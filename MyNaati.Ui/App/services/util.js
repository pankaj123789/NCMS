define(['durandal/system', 'durandal/app'], function (system, app) {
    var stringEncodings;
    var isPageLoading = ko.observable(false);

    function init() {
        return urlEncodings.done(function (data) {
            stringEncodings = data;
        });
    }

    function showLoadingIndicator() {
        var $body = $('body');

        if ($body.hasClass('modal-open')) {
            var $modal = $('.modal:visible .modal-body');
            if (!$('.butterbar', $modal).length) {
                $modal.prepend($('<div ui-butterbar class="butterbar hide"><span class="bar"></span></div>'));
            }
        }

        var $loadingBar = $('.butterbar');

        $loadingBar.removeClass('hide');
        $loadingBar.addClass('active');

        isPageLoading(true);

        app.trigger(CONST.eventNames.loadingIndicatorStatusChanged, true);
    }

    function hideLoadingIndicator() {
        var $loadingBar = $('.butterbar');

        $loadingBar.removeClass('active');
        $loadingBar.addClass('hide');

        isPageLoading(false);

        app.trigger(CONST.eventNames.loadingIndicatorStatusChanged, false);
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    function downloadFile(url, successMessage, errorMessage, callback) {
        if (jQuery.type(url) === 'function') {
            url = url();
        }

        if (!url) {
            return;
        }

        successMessage = 'Download successful.';
        errorMessage = 'Download failed.';

        app.trigger(CONST.eventNames.showLoadingIndicator);

        $.fileDownload(url, {
            successCallback: function (url, message) {
                if (successMessage) {
                    toastr.success(successMessage);
                } else {
                    if (message)
                        toastr.success(message);
                }

                app.trigger(CONST.eventNames.cancelLoadingIndicator);

                if (callback)
                    callback('Ok');
            },
            failCallback: function (error, url) {
                var message = errorMessage;

                if (isJson(error)) {
                    var obj = JSON.parse(error.replace(/(<[^>]+>)+/g, ""));

                    if (obj && obj.error) {
                        message = obj.error;
                    }
                }

                toastr.error(message || 'Download failed');

                app.trigger(CONST.eventNames.cancelLoadingIndicator);

                if (callback)
                    callback('Error', error);
            }
        });
    }

    function isJson(str) {
        try {
            return (JSON.parse(str) && !!str);
        } catch (e) {
            return false;
        }
    }

    app.on(CONST.eventNames.showLoadingIndicator).then(showLoadingIndicator);
    app.on(CONST.eventNames.cancelLoadingIndicator).then(hideLoadingIndicator);

    $(document).on({
        ajaxStart: function () {
            app.trigger(CONST.eventNames.showLoadingIndicator);
            $('body').addClass('ajax-start');
        },
        ajaxStop: function () {
            app.trigger(CONST.eventNames.cancelLoadingIndicator);
            $('body').removeClass('ajax-start');
        }
    });

    function resetModel(model) {
        for (var prop in model) {
            if (model.hasOwnProperty(prop)) {
                if (ko.isObservable(model[prop]) && !ko.isComputed(model[prop])) {
                    if ($.isArray(model[prop]())) {
                        model[prop]([]);
                    }
                    else {
                        if (typeof model[prop]() === 'boolean') {
                            model[prop](false);
                        } else {
                            model[prop]('');
                        }
                    }
                }
            }
        }
    }

    function addressToString(address) {
        var string = address.StreetDetails();
        if (address.Suburb()) {
            string += ', ' + address.Suburb();
        }
        if (address.CountryName() !== 'Australia') {
            string += ', ' + address.CountryName();
        }
        return string;
    }

    function humanizeDate(date) {
        var delta = Math.round((+new Date - date) / 1000);

        var minute = 60,
            hour = minute * 60,
            day = hour * 24,
            week = day * 7;

        var fuzzy;

        if (delta < 30) {
            fuzzy = 'just then';
        } else if (delta < minute) {
            fuzzy = delta + ' seconds ago';
        } else if (delta < 2 * minute) {
            fuzzy = 'a minute ago'
        } else if (delta < hour) {
            fuzzy = Math.floor(delta / minute) + ' minutes ago';
        } else if (Math.floor(delta / hour) == 1) {
            fuzzy = '1 hour ago.'
        } else if (delta < day) {
            fuzzy = Math.floor(delta / hour) + ' hours ago';
        } else if (delta < day * 2) {
            fuzzy = 'yesterday';
        }

        return fuzzy;
    }

    return {
        isPageLoading: function () { return isPageLoading() },
        showLoadingIndicator: showLoadingIndicator,
        hideLoadingIndicator: hideLoadingIndicator,
        init: init,
        downloadFile: downloadFile,
        guid: guid,
        resetModel: resetModel,
        addressToString: addressToString,
        humanizeDate: humanizeDate
    };
});
