/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'daterangepicker';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    var dataBindingName = bindingName + 'Data';

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var $element = $(element);
        var unwrap = ko.unwrap(valueAccessor());
        $element
            .daterangepicker(unwrap)
            .on('cancel.daterangepicker', function(ev, picker) {
                //do something, like clearing an input
                $(this).val('');
            });

        allBindingsAccessor().value.subscribe(function (newValue) {
            var startAndEndDate = getStartAndEndDate(newValue);
            $element.data('daterangepicker').setStartDate(startAndEndDate.start);
            $element.data('daterangepicker').setEndDate(startAndEndDate.end);
        });
    }

    function getStartAndEndDate(date) {
        var start = undefined;
        var end = undefined;

        if (date) {
            var splitted = date.split('-');
            start = moment($.trim(splitted[0]), "DD/MM/YYYY");
            end = moment($.trim(splitted[1]), "DD/MM/YYYY");
        }

        return {
            start: start,
            end: end
        };
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});