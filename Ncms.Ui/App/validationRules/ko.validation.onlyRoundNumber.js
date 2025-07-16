ko.validation.rules['roundNumber'] = {
    validator: function (value, otherValue) {

        if (!otherValue && !value) {
            return true;
        }
        if (!value) {
            return true;
        }
        
        var isValid = !(value.indexOf('.') > -1);

        if (!isValid) {
            return false;
        }

        return true;
    },
    message: 'Please enter round number'
};