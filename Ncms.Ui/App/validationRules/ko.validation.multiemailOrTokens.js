ko.validation.rules['multiemailOrTokens'] = {
	validator: function (val, validate) {
		if (!validate) { return true; }
 
		var isValid = true;
		if (!ko.validation.utils.isEmptyVal(val)) {
			// use the required: true property if you don't want to accept empty values
			var values = val.split(';');
			$(values).each(function (index) {
			    isValid = ko.validation.rules['emailAddressOrToken'].validator($.trim(this), validate);
				return isValid; // short circuit each loop if invalid
			});
		}
		return isValid;
	},
	message: ko.Localization('EmailAddressesInvalidOrToken')
};