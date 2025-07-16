ko.validation.rules['milestoneDateWithinProjectDates'] = {
    validator: function (val, predicate) {
        if (!val)
            return true;

        var isValid = false;
        $.ajax({
            url: 'api/Milestone/ValidateMilestoneDateWithinProjectApplication',
            async: false,
            cache: false,
            type: "GET",
            data: {
                projectId: predicate.projectId,
                milestoneId: predicate.milestoneId,
                date: val
            },
            success: function () {
                isValid = true;
            },
            error: function (xmlHttpRequest) {
                isValid = false;
            }
        });

        return isValid;
    },
    message: 'The date must be within Project Start Date and End Date'
};