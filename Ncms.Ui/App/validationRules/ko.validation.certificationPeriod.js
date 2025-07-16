ko.validation.rules['certificationPeriod'] = {
    validator: function (value, otherValue) {
        if (otherValue.endDate && otherValue.originalEndDate) {
            var newDate = moment(value, 'l').toDate();
            var endDate = moment(ko.toJS(otherValue.endDate), 'l').toDate();
            var originalEndDate = moment(ko.toJS(otherValue.originalEndDate), 'l').toDate();

            if (newDate < endDate || newDate < originalEndDate)
                return false;
        }
        return true;
    },
    message: function (value, otherValue) {
        if (value.endDate && value.originalEndDate) {
            var newDate = otherValue();
            var endDate = moment(ko.toJS(value.endDate), 'l').toDate();

            if (newDate < endDate )
                return ko.Localization("Naati.Resources.Validation.resources.GreaterThanOrEqual").format(value.endDate);
            return ko.Localization("Naati.Resources.Validation.resources.GreaterThanOrEqual").format(value.originalEndDate);
        }

        return ko.Localization("Naati.Resources.Validation.resources.GreaterThanOrEqual").format(value);
    }
};