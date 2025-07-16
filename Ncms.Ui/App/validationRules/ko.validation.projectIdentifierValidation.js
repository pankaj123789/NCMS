ko.validation.rules['projectIdentifierValidation'] = {
    validator: function (val, predicate) {
        if (!val)
            val = '';
        if (val.length < 8)
            return true;
        if (predicate == true) {
            var isValid = false;
            $.ajax({
                url: 'api/Project/ValidateProjectIdentifier',
                async: false,
                cache: false,
                type: "GET",
                data: { projectIdentifier: val.toUpperCase() },
                success: function () {
                    isValid = true;
                },
                error: function (xmlHttpRequest) {
                    var errors = jQuery.parseJSON(xmlHttpRequest.responseText);
                    ko.validation.rules['projectIdentifierValidation'].message = errors[0];
                    isValid = false;
                }
            });
            return isValid;
        }
        return true;
    },
    message: 'The project identifier is not valid'
};