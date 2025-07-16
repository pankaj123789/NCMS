define(['services/util'], function (util) {
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

        util.hideLoadingIndicator();
        // cancel ajax requests
        if (textStatus === 'abort') {
            return;
        }
        var responseText = XMLHttpRequest.responseText || XMLHttpRequest.statusText || 'Server communication failed.';
        if (XMLHttpRequest.status == 403) {
            /*jshint -W020 */
            isSessionInvalid = true;
            /*jshint +W020 */
            window.location.href = 'login?ReturnUrl=' + window.location.hash.substring(1);
        }
        // and sometimes the response is an HTML error page from IIS. don't want 
        // to show these in a toast, that's just embarrassing. 
        else if (XMLHttpRequest.status == 500 || responseText.indexOf('<!DOCTYPE') != -1) {
            toastr.error('An error has occurred. Please contact Support.');
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
                toastr.error(responseText);
            }

            if (customFailFunc) {
                customFailFunc(XMLHttpRequest, textStatus, errorThrown);
            }
        }
    }
})