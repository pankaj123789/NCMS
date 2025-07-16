
function getExternalDocGroupData() {
    var list = [];
    $.ajax({
        url: 'api/ExternalDocumentTypeGroup/GetGroupPriorities',
        async: false,
        cache: false,
        type: "GET",
        success: function (data) {
            list = data.data;
        }
    });
    return list;
}

ko.validation.rules['externalDocGroupPriority'] = {
    validator: function (val, predicate) {

        if (!predicate)
            predicate = 0;

        var isValid = true;
        var list = getExternalDocGroupData();

        $.each(list, function (i, v) {
            if (v.Id !== predicate) {

                if (v.Priority === parseInt(val)) {
                    ko.validation.rules['externalDocGroupPriority'].message =
                        ko.Format(ko.Localization('TheDocumentGroupXIsUsingThisPriorityNumber'), v.GroupName);
                    isValid = false;
                }
            }
        });

        return isValid;

    },
    message: 'Error'
};



ko.validation.rules['externalDocGroupName'] = {
    validator: function (val, predicate) {

        if (!predicate)
            predicate = 0;

        var isValid = true;

        var list = getExternalDocGroupData();

        $.each(list, function (i, v) {
            if (v.Id !== predicate) {
                if (v.GroupName.toLowerCase() === val.toLowerCase()) {
                    ko.validation.rules['externalDocGroupName'].message =
                        ko.Format(ko.Localization('ThereIsADocumentGroupWithThisXalready'), 'name');
                    isValid = false;
                }
            }
        });

        return isValid;
    },
    message: 'Error'
};