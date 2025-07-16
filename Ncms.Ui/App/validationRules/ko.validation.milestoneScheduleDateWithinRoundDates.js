ko.validation.rules['milestoneScheduleDateWithinRoundDates'] = {
    validator: function (val, predicate) {
        if (!val)
            return true;

        var isValid = true;
        var filledDate = moment(val);

        predicate.forEach(function (roundItem) {
            if (moment(roundItem.LastDate).isAfter(filledDate)) {
                isValid = false;
            }
        });

        return isValid;
    },
    message: 'Payment Schedule dates must be after already related Round dates'
};