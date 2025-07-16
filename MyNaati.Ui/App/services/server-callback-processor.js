define([], function () {
    return {
        showMessages: showMessages,
        showError: showError
    };

    function showMessages(info, warning, error) {
        var options = {
            'timeOut': getToastTimeout(info) + getToastTimeout(warning)
        };

        showMessage('info', info, options);
        showMessage('warning', warning, options);
        showMessage('error', error, options);
    }

    function showMessage(messageType, message, options) {
        if ($.type(message) === 'string') {
            toastr[messageType](message);
        } else if ($.isArray(message)) {
            for (var i = 0; i < message.length; i++) {
                toastr[messageType](message[i], null, options);
            }
        }
    }

    function getToastTimeout(messages) {
        var baseTimeOut = 5000;
        if ($.isArray(messages)) {
            return baseTimeOut * messages.length;
        } else {
            return baseTimeOut;
        }
    }

    function showError(XMLHttpRequest, textStatus, errorThrown, customFailFunc) {
        // cancel ajax requests
        if (textStatus === 'abort') {
            return;
        }
        var responseText = XMLHttpRequest.responseText || XMLHttpRequest.statusText || 'Server communication failed.';
        // 401 probably means the user has been signed out. go to the login page.
        if (XMLHttpRequest.status == 401) {
            /*jshint -W020 */
            isSessionInvalid = true;
            /*jshint +W020 */
            window.location.href = window.baseUrl + 'Errors/Error';
        }
            // and sometimes the response is an HTML error page (e.g. 403) from IIS. don't want 
            // to show these in a toast, that's just embarrassing. 
        else if (XMLHttpRequest.status == 500 || responseText.indexOf('<!DOCTYPE') != -1) {
            window.location.href = window.baseUrl + 'Errors/Error';
        }
        else {
            try {
                var errors = jQuery.parseJSON(responseText);
                var options = {
                    'timeOut': getToastTimeout(errors)
                };
                if ($.type(errors) === 'string') {
                    toastr.error(errors, null, options);
                } else if ($.isArray(errors)) {
                    for (var i = 0; i < errors.length; i++) {
                        toastr.error(errors[i], null, options);
                    }
                } else {
                    toastr.error(errors.ExceptionMessage || errors.Message, null, options);
                }
            } catch (e) {
                if (responseText.toLower() !== 'ok') {
                    toastr.error(responseText);
                }
            }

            if (customFailFunc) {
                customFailFunc(XMLHttpRequest, textStatus, errorThrown);
            }
        }
    }
})