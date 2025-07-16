ko.validation.rules['isUrl'] = {
    validator: function (val, validate) {
        if (!validate) { return true; }

        if (!ko.validation.utils.isEmptyVal(val)) {
            // use the required: true property if you don't want to accept empty values
            return validateURL(val);
        }

        return true;
    },
    message: ko.Localization('InvalidLinkUrl')
};
