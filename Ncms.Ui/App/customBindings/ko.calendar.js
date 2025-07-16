/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    ko.bindingHandlers.calendar = {
        init: function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var value = valueAccessor();

            var defaultValues = {
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                defaultView: 'agendaWeek',
                navLinks: true, // can click day/week names to navigate views
                editable: true,
                eventLimit: true, // allow "more" link when too many events
                events: [],
                timeFormat: 'hh:mm A',
                views: {
                    week: {
                        titleFormat: 'D MMM, YYYY',
                        columnFormat: 'ddd, D MMM'
                    }
                }
            };

            $.extend(defaultValues, ko.toJS(value));

            $(element).fullCalendar(defaultValues);

            if (value.init) {
                value.init(element);
            }
        }
    };
});