ko.validation.rules['emailAddress'] = {
    validator: function (val, validate) {
        if (!validate) { return true; }

        if (!ko.validation.utils.isEmptyVal(val)) {
            // use the required: true property if you don't want to accept empty values

            var email = '';
            // extract email address from angle brackets if present
            if (val.indexOf('<') != -1) {
                var matches = val.match(/<([^>]+)>/);
                if (matches) {
                    email = matches.pop();
                }
            } else {
                email = val;
            }
            return ko.validation.rules['email'].validator($.trim(email), validate);
        }

        return true;
    },
    message: ko.Localization('EmailAddressesInvalid')
};