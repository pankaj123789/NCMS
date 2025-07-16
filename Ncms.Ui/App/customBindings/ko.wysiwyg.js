/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    var bindingName = 'wysiwyg';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var unwrap = ko.unwrap(valueAccessor());

        function loadEditor(editor) {
            if ('value' in unwrap) {
                $(element).val(ko.toJS(unwrap.value));

                if (ko.isObservable(unwrap.value)) {
                    var preventChange = false;
                    ko.computed(function () {
                        if (editor.getData() != unwrap.value()) {
                            preventChange = true;
                            editor.setData(unwrap.value() || '');
                        }
                    });

                    // bit of a hack for the email-compose wizard step. force the editor to sanitize the initial value, in case 
                    // the user makes no changes before we post the content. (posting unsanitized html can trigger firewall rules)
                    var initialLoadComputed = ko.computed(function () {
                        const value = unwrap.value();
                        if (value) {
                            preventChange = true;
                            const sanitized = editor.getData();
                            if (sanitized !== value) {
                                unwrap.value(sanitized);
                            }
                            // we only want this to happen once
                            initialLoadComputed.dispose();
                        }
                    });

                    editor.model.document.on('change:data', function () {

                        if (preventChange) {
                            return preventChange = false;
                        }
                        unwrap.value(editor.getData());
                    });
                }
            }

            if (unwrap.events) {
                for (var e in unwrap.events) {
                    editor.on(e, unwrap.events[e]);
                }
            }
        }

        ClassicEditor.create(element, {
            toolbar: {
                removeItems: ['uploadImage', 'mediaEmbed']
            }
        } ).then(loadEditor);
    }

    return ko.bindingHandlers[bindingName] = {
        init: init
    };
});