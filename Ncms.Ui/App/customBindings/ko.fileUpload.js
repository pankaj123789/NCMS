/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'fileupload';

    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = ko.unwrap(valueAccessor()) || {};
        var $element = $(element);
        var $dropzone = $element.closest(".dropzone");

        var fileuploadOptions = {
            url: "",
            dropZone: $dropzone,
            dataType: 'json'
        };

        $.extend(true, fileuploadOptions, unwrap.fileUploadOptions);

        $(document)
            .bind('dragover', function (e) {
                var foundDropzone,
                    timeout = window.dropZoneTimeout;
                if (!timeout) {
                    $dropzone.addClass('in');
                }
                else {
                    clearTimeout(timeout);
                }

                var found = false,
                node = e.target;

                do {

                    if (node === $dropzone[0]) {
                        found = true;
                        foundDropzone = $(node);
                        break;
                    }

                    node = node.parentNode;

                } while (node != null);

                $dropzone.removeClass('in active');

                if (found) {
                    foundDropzone.addClass('active');
                }

                window.dropZoneTimeout = setTimeout(function () {
                    window.dropZoneTimeout = null;
                    $dropzone.removeClass('in active');
                }, 100);
            })
            .bind('drop dragover', function (e) {
                e.preventDefault();
            })
            .bind('drop', function (e) {
                $dropzone.removeClass('in active');
            });

        $element.fileupload(fileuploadOptions);
        if (unwrap.events) {
            for (var e in unwrap.events) {
                $element.on(e, unwrap.events[e]);
            }
        }
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});