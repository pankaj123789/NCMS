/* The contents of this file are actually pretty deceptive (sorry!).  They override knockout validation's handling 
  of the html5 type="date" input field.

You probably won't see any references to dateISO in our code.

This:
 * Makes validation work with our short date format.
 * Provides (more sensible) validation messages for dates.
 * Extends the min and max validation rules (specifically for handling html5 input type="date" fields)
 * Excludes a couple of edge-cases where js interprets dates differently to how we want them interpreted.
*/
ko.validation.rules['dateISO'] = {
	validator: function (val, validateParams) {
		if (!validateParams) { return true; }
		return ko.validation.utils.isEmptyVal(val) || (validateParams && !/Invalid|NaN/.test(new Date(val)));
	},
	message: ko.Localization('DateInvalidDateEntered')
};

// check if we have imported this set of rules already, only monkeypatch once
if (!ko.validation.rules._min)
{
	ko.validation.rules._min = ko.validation.rules.min;
	ko.validation.rules._max = ko.validation.rules.max;
}

ko.validation.rules['min'] = {
	validator: function (val, validateParams) {
		if (validateParams.typeAttr !== 'date')
		{
			return ko.validation.rules._min.validator(val, validateParams);
		}

		if (!val)
		{
			return true;
		}

		// This is necessary because js' date constructor, while defined for certain formats (ISO 8601 derivative),
		// reiles on browser defined algorithms to try to interpret the selected date from an input string.
		//
		// For better or worse, our storage format is YYYY/MM/DD.
		//
		// The browser heuristics mean that the browser often gets things wrong.  Particularly when a date begins with 000.
		// eg "0001/02/03" gets interpreted as 01/02/2003.  
		// 
		// As we don't support non-gregorian dates, and we need to store these dates in SQL, it is not sensible to accept
		// dates prior to eg 1901.  As such, if a date string starts with "000", we know it's going to fail min validation.
		// 
		// It's also bad when things start with "00" (some browsers think that it's 2015 if you say 0015/01/01).
		// Browser date pickers will show the year 0015 though.  TL;DR: if it starts with 00, and we have a specified min
		// value, then the validation WILL (and should) fail.
		if (val.indexOf("00") === 0)
		{
			return false;
		}

		return new Date(val) >= new Date(validateParams.value);
	},
	message: function(minVal, selectedVal){
		var isDate = false;
		if (selectedVal.rules)
		{
			var match = ko.utils.arrayFirst(selectedVal.rules(), function(rule) {
				return rule.rule == 'dateISO';
			});
			if (match) {
				isDate = true;
			}
		}

		if (!isDate)
		{
			return ko.Format(ko.validation.rules._min.message, minVal, ko.utils.unwrapObservable(selectedVal));
		}

		return ko.Format(
		  ko.Localization('DateMin'), 
		  moment(minVal).format(CONST.settings.shortDateDisplayFormat), 
		  ko.utils.unwrapObservable(selectedVal));
	}
}

ko.validation.rules['max'] = {
	validator: function (val, validateParams) {
		if (validateParams.typeAttr !== 'date')
		{
			return ko.validation.rules._max.validator(val, validateParams);
		}

		if (!val)
		{
			return true;
		}

		return new Date(val) <= new Date(validateParams.value);
	},
	message: function(maxVal, selectedVal){
		var isDate = false;
		if (selectedVal.rules)
		{
			var match = ko.utils.arrayFirst(selectedVal.rules(), function(rule) {
				return rule.rule == 'dateISO';
			});
			if (match) {
				isDate = true;
			}
		}
		
		if (!isDate)
		{
			return ko.Format(ko.validation.rules._max.message, maxVal, ko.utils.unwrapObservable(selectedVal));
		}

		return ko.Format(
			ko.Localization('DateMax'), 
			moment(maxVal).format(CONST.settings.shortDateDisplayFormat), 
			ko.utils.unwrapObservable(selectedVal));
	}
}