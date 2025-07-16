/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko) {
    'use strict';

    ko.bindingHandlers.overlayScrollbars = {
        init: function init(element) {
            OverlayScrollbars(element, {});
        }
    };
});