/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';
    ko.bindingHandlers.disableOnAjaxStart = {
        init: function (element, valueAcessor, allBindingsAccessor) {
            return ko.bindingHandlers.ajaxEvents.init(element, function () {
                return {
                    start: function () {
                        ko.bindingHandlers.disable.update(element,  ko.observable(true));
                    },
                    end: function () {
                        if (allBindingsAccessor.has('disable')) {
                            return ko.bindingHandlers.disable.update(element, allBindingsAccessor.get('disable'));
                        }
                        if (allBindingsAccessor.has('enable')) {
                            return ko.bindingHandlers.enable.update(element, allBindingsAccessor.get('enable'));
                        }
                        ko.bindingHandlers.disable.update(element, ko.observable(false));
                    }
                };
            });
        }
    };
});