/* jshint boss:true*/(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'datatable';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = $.extend({
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], ['10', '25', '50', '100', ko.Localization('Naati.Resources.Shared.resources.All')]],
            dom: "<'row'<'col-sm-6'B><'col-sm-6'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'l><'col-sm-2'i><'col-sm-5'p>>",
            oSearch: { sSearch: " " },
            autoWidth: false,
            destroy: true
        }, ko.unwrap(valueAccessor()));

        var $element = $(element);
        var subscribers = [];
        var itemsSource = getSource();

        function createTable() {
            var uw = $.extend({ buttons: [] }, unwrap);
            var isArray = $.isArray(uw.buttons);
            delete uw.buttons;

            uw.buttons = isArray ? [] : {};

            if (unwrap.buttons) {
                if (isArray) {
                    for (var i = 0; i < unwrap.buttons.length; i++) {
                        uw.buttons.push($.extend({}, unwrap.buttons[i]));
                    }
                } else {
                    $.extend(true, uw.buttons, unwrap.buttons);
                }
            }

            var table = $element.DataTable(uw);

            if (uw.events) {
                for (var e in uw.events) {
                    table.on(e, uw.events[e]);
                }
            }

            $element.find('.dataTables_empty').attr('colspan', '100%');

            if (!itemsSource) {
                return;
            }

            var items = ko.toJS(itemsSource);
            var simplifyTable = !items || !items.length || items.length <= 10;
            var display = simplifyTable ? 'none' : 'block';
            var $container = $element.closest('.dataTables_wrapper');

            $container.find('.dataTables_length').css('display', display);
            $container.find('.dataTables_paginate').css('display', display);
            if (simplifyTable) {
                $container.find('.dataTables_info')
                    .text('Showing all entries')
                    .addClass('m-b-sm');
            }
        }

        function getSource() {
            var source = null;
            var $tbody = $element.find('tbody');
            if ($tbody.length === 0) return;

            var bindings = ko.bindingProvider.instance.getBindings($tbody[0], bindingContext);

            if (bindings) {
                if (bindings.foreach) {
                    source = bindings.foreach.data;

                    if (!source) {
                        source = bindings.foreach;
                    }
                } else if (bindings.template && bindings.template.foreach) {
                    source = bindings.template.foreach.data;

                    if (!source) {
                        source = bindings.template.foreach;
                    }
                }
            }
            return source;
        }

        function addSubscriber(source) {
            if (!source || !ko.isObservable(source)) {
                return;
            }

            var subs = source.subscribe(function (oldValue) {
                if ($.fn.DataTable.isDataTable($element)) {
                    try {
                        $element.DataTable().clear();
                        $element.DataTable().destroy();
                    }
                    catch (err) {

                    }
                }
            }, null, 'beforeChange');

            subscribers.push(subs);

            var interval = null;
            subs = source.subscribe(function (newValue) {
                if (interval != null) {
                    clearInterval(interval);
                }

                interval = setInterval(function () {
                    if (!$.fn.DataTable.isDataTable($element)) {
                        clearInterval(interval);
                        interval = null;
                        setTimeout(function () {
                            createTable();
                        }, 100);
                    }
                }, 100);
            });

            subscribers.push(subs);
        }

        addSubscriber(itemsSource);
        setTimeout(function () {
            createTable();
        }, 100);

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            if ($.fn.DataTable.isDataTable($element)) {
                $element.DataTable().destroy();
            }

            while (subscribers.length > 0) {
                subscribers.pop().dispose();
            }

            $element.remove();
        });
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});