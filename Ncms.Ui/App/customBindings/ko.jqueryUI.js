/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'jqueryui';

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        $.applyjQueryUI($(element));
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});