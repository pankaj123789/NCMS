/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    ko.bindingHandlers.clockpicker = {
        init: function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var value = valueAccessor();

            var defaultValues = {
                autoclose: true,
                applyMask: true
            };

            $.extend(defaultValues, ko.toJS(value));

            var $element = $(element);
            $element.clockpicker(defaultValues);
            
            if (value && value.init) {
                value.init(element);
            }

            if (defaultValues.applyMask) {
                $element.inputmask("99:99");
            }
        }
    };
});