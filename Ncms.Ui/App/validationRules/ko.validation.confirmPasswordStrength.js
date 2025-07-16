ko.validation.rules['confirmPasswordStrength'] = {
    message: 'The password strength requirements have not been met.',
    validator: function (val, inputId) {
        var verdictText = $.fn.pwstrength('getVerdictText', $('#' + inputId));
        return verdictText == 'Very Strong';
    }
};