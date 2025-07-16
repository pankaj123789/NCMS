/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'modal';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = ko.unwrap(valueAccessor());
        var $element = $(element);

        if (ko.unwrap(unwrap.show)) {
            $element.modal('show');
        }
        if (unwrap.events) {
            for (var e in unwrap.events) {
                $element.on(e, unwrap.events[e]);
            }
        }
        if (ko.isObservable(unwrap.show)) {
            $element.on('hidden.bs.modal.cb', function () {
                unwrap.show(false);
            });

            unwrap.show.subscribe(function (newValue) {
                if (newValue) {
                    $element.modal('show');
                }
                else {
                    $element.modal('hide');
                }
            });
        }
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});