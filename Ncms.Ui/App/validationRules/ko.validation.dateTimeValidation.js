ko.validation.rules['timeValid'] = {
    validator: function (value, otherValue) {

        if (!otherValue && !value) {
            return true;
        }
        if (!value) {
            return true;
        }

        var isValid = moment("2020-01-01 " + value, "YYYY-MM-DD HH:mm", true).isValid();
        
        if (!isValid) {
            return false;
        }

        return true;
    },
    message: ko.Localization("Naati.Resources.Validation.resources.TimeValid")
};