/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    ko.bindingHandlers.ajaxEvents = {
        init: function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var value = ko.unwrap(valueAccessor());

            function startAjax() {
                if (value.start) {
                    value.start.call(element);
                }
            }

            function endAjax() {
                if (value.end) {
                    value.end.call(element);
                }
            }

            require(['durandal/app'], function (app) {
                app.on(CONST.eventNames.showLoadingIndicator).then(startAjax);
                app.on(CONST.eventNames.cancelLoadingIndicator).then(endAjax);
            });
        }
    };
});