/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'select2';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var defaultOptions = { width: '100%' };
        var obj = $.extend({}, defaultOptions, valueAccessor()),
          allBindings = allBindingsAccessor(),
          lookupKey = allBindings.lookupKey;

        var $element = $(element);
        $element.select2(obj);

        if (lookupKey) {
            var value = ko.utils.unwrapObservable(allBindings.value);
            $element.select2('data', ko.utils.arrayFirst(obj.data.results, function (item) {
                return item[lookupKey] === value;
            }));
        }

        var subscribers = [];

        if (obj.valueObject && ko.isObservable(obj.valueObject)) {
            var valueObject = obj.valueObject();
            if (valueObject) {
                updateValue($element, valueObject);
            }

            var subscriber = obj.valueObject.subscribe(function (newValue) {
                updateValue($element, newValue);
            });

            subscribers.push(subscriber);
        }

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $element.select2('destroy');
            while (subscribers.length > 0) {
                subscribers.pop().dispose();
            }
        });
    }

    function update(element, valueAccessor, allBindingsAccessor) {
        var value = ko.utils.unwrapObservable(allBindingsAccessor().value || allBindingsAccessor().selectedOptions);
        if (value) $(element).select2('val', value);
    }

    function updateValue($element, valueObject) {
        $element.find('option').remove();

        var $option = $('<option selected></option>');
        $element.append($option);
        $option.text(valueObject.text).val(valueObject.id);
        $option.removeData();
        $element.trigger('change');
    }

    return ko.bindingHandlers[bindingName] = {
        init: init,
        update: update
    };
});
