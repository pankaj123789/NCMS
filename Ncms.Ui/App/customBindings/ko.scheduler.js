/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'scheduler';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allBindings = allBindingsAccessor();
        var schedulerBinding = allBindings[bindingName];
        var config = schedulerBinding.config();
        var resources = {
            resources: function (callback) {
                callback(schedulerBinding.resources());
                schedulerBinding.resources.subscribe(function (newValue) {
                    callback(newValue);
                    $(element).fullCalendar('refetchResources');
                });
            }
        };

        var events = {
            events: ko.unwrap(schedulerBinding.events)
        };
        var license = { schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives' };

        config = $.extend({}, config, license, resources, events);

        $(element).fullCalendar(config);
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});