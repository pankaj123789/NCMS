/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'popover';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var params = ko.unwrap(valueAccessor());
        var value = $.extend({}, params);
        var $element = $(element);
        if (params) {
            params.$element = $element;
        }
        $element.popover(value);
    }

    return ko.bindingHandlers[bindingName] = {
        after: ['resattr', 'attr'],
        init: init
    };
});