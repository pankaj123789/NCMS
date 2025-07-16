/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'timepicker';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allBindings = allBindingsAccessor();
        var $element = $(element);
        var unwrap = ko.unwrap(valueAccessor());
        $element.timepicker(unwrap);

        if (allBindings.value && ko.isObservable(allBindings.value)) {
            allBindings.value.subscribe(function (newValue) {
                if (!newValue) {
                    newValue = "00:00";
                }
                $element.timepicker('setTime', newValue);
            });
        }
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});