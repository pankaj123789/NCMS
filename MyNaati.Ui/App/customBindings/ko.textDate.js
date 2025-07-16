/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'textDate';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        return ko.bindingHandlers.text.update(element, function () {
            var value = valueAccessor();
            return textDate(value);
        });
    }

    function textDate(value) {
        if (!value) {
            return '';
        }

        var options = { 
            inputFormat: '', 
            outputFormat: CONST.settings.shortDateDisplayFormat 
        };

        var settings = value;

        if (typeof value === "function") {
            value = value();
        }

        if (typeof value === "string") {
            settings = { value: value, inputFormat: moment.ISO_8601 };
        }

        $.extend(options, settings);

        var jsValue = ko.toJS(options.value);
        if (!jsValue) {
            return '';
        }

        return moment(jsValue, options.inputFormat).format(options.outputFormat);
    }

    return ko.bindingHandlers[bindingName] = {
        update: update
    };
});