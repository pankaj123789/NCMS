/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'selectIfSingle';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allBindings = allBindingsAccessor();
        var options = allBindings.options;
        var value = allBindings.value;
        var bindingValue = ko.unwrap(valueAccessor());
        if (bindingValue === false) {
            return false;
        }

        if (!options || !value) {
            return;
        }

        ko.computed(selectIfSingle);

        selectIfSingle();

        function selectIfSingle() {
            var opts = ko.toJS(options);
            var optionsValue = allBindings.optionsValue;
            if (!optionsValue) {
                optionsValue = 'value';
            }

            var isSingle =
                (opts.length === 1 && opts[0][optionsValue] !== '') ||
                (opts.length === 2 && opts[0][optionsValue] === '');

            if (!isSingle) {
                return;
            }

            var index = opts[0][optionsValue] !== '' ? 0 : 1;
            var val = opts[index][optionsValue];
            if (ko.isObservable(value)) {
                value(val);
            }
            else {
                value = val;
            }
        }
    }

    return ko.bindingHandlers[bindingName] = {
        init: init,
    };
});