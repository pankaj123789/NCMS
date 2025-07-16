/* jshint boss:true*/(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'tableedit';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = ko.unwrap(valueAccessor());
        var $element = $(element);
        setTimeout(function () {
            $element.Tabledit(unwrap);
        }, 200);
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});