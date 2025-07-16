/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'loaderIndicator';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var $element = $(element);
        var unwrap = ko.unwrap(valueAccessor());

        unwrap = $.extend({}, { indicator: false, autoDisable: true, loadTemplate: 'Loading...' }, unwrap);

        var oldHtml = $element.html();
        var disable = $element.attr("disabled");

        function showLoading(newValue) {
            if (newValue) {
                oldHtml = $element.html();
                disable = $element.attr("disabled");
                $element.html(unwrap.loadTemplate);

                if (unwrap.autoDisable)
                    $element.attr("disabled", "disabled");
            }
            else {
                $element.html(oldHtml);
                $element.attr("disabled", disable || null);
            }
        }

        if (ko.isObservable(unwrap.indicator)) {
            unwrap.indicator.subscribe(function (newValue) {
                showLoading(newValue);
            });
        }

        showLoading(ko.unwrap(unwrap.indicator));
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});