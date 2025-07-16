ko.validation.rules['dateGreaterThan'] = {
    validator: function (value, otherValue) {
        if (!otherValue && !value) {
            return true;
        }
        if (!value) {
            return true;
        }

        var date = moment(value, 'l').toDate();
        var jsDate = ko.toJS(otherValue);
        if (date < moment(jsDate, 'l').toDate()) {
            return false;
        }

        return true;
    },
    message: ko.Localization("Naati.Resources.Validation.resources.GreaterThanOrEqual")
};