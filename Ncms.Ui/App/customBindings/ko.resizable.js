/* jshint boss:true*/(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'resizable';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = ko.unwrap(valueAccessor());

        var $element = $(element);
        
        if (unwrap.events) {
            for (var e in unwrap.events) {
                $element.on(e, unwrap.events[e]);
            }
        }

        $element.resizable(unwrap);
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});
