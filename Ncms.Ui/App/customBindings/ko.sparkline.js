/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'sparkline';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var accessor = valueAccessor();
        var subscribers = [];

        function createChart() {
            var values = ko.unwrap(accessor.values);
            var options = ko.unwrap(accessor.options);
            $(element).sparkline(values, options);
        }

        if (ko.isObservable(accessor.values)) {
            subscribers.push(accessor.values.subscribe(createChart));
        }

        if (ko.isObservable(accessor.options)) {
            subscribers.push(accessor.options.subscribe(createChart));
        }

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            while (subscribers.length > 0) {
                subscribers.pop().dispose();
            }
        });

        createChart();
    }

    function update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    }

    return ko.bindingHandlers[bindingName] = {
        init: init,
        update: update
    };
});