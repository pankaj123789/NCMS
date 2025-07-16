/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'check';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = ko.unwrap(valueAccessor());
        $(element).addClass('fa');
        ko.applyBindingsToNode(element, { css: { 'text-green': value, 'text-red': !value, 'fa-check': value, 'fa-ban': !value } });
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});