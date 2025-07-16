/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    ko.bindingHandlers.dropdown = {
        init: function init(element) {
            var $element = $(element);
            $element.click(function (e) {
                $.fn.dropdown.Constructor.prototype.toggle.call(element, [e]);
                return false;
            });
            
            $element.parent().on('click', '.dropdown-menu li a', function (e) {
                $.fn.dropdown.Constructor.prototype.clearMenus.call(element, [e]);
                return false;
            });
        }
    };
});