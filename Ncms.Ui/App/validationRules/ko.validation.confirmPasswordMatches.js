ko.validation.rules['confirmPasswordMatches'] = {
    message: 'Passwords do not match.',
    validator: function (val, params) {
        var otherValue = params;
        return val === ko.validation.utils.getValue(otherValue);
    }
};