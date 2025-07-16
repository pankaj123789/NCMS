!(function () {
    "use strict";

    ko.bindingHandlers.showWithPermission = {
        init: function (element, valueAccessor) {

            var value = ko.unwrap(valueAccessor());
            if (!value) {
                $(element).show();
                return true;
            }

            var parts = value.split('.');
            var noun = parts[0];
            var verb = parts.length > 1 ? parts[1] : 2;

            $(element).hide();
            
            currentUser.hasPermission(noun, verb).then(function (permission) {
                if (!permission) {
                    return hideElement(element);
                }
                $(element).show();
            });
        }
    };

    ko.bindingHandlers.enableWithPermission = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            return ko.bindingHandlers.enable.update(element, function () {
                var value = ko.unwrap(valueAccessor());
                var bindings = ko.unwrap(allBindingsAccessor());
                var parts = value.split('.');
                var noun = parts[0];
                var verb = parts.length > 1 ? parts[1] : 2;
                updateReadOnlyProperty();
                ko.computed(function () {
                    ko.toJS(bindings.attr);
                    updateReadOnlyProperty();
                });
                function updateReadOnlyProperty() {
                    currentUser.hasPermission(noun, verb).then(function (hasPermission) {
                        setReadonly(element, !hasPermission);
                        setChildrenReadonly(element, !hasPermission);
                   });
                }
            });
        }
    };

    function hideElement(element) {
        $(element).hide();
        // time issue with ckeditor
        setTimeout(function () {
            $(element).remove();
        },
            200);
    }

    function setReadonly(element, set) {
        var $element = $(element);
        if (!set) {
            if ($element.is('a')) {
                if ($element.data('old_href')) {
                    $element.prop('href', $element.data('old_href'));
                }
                $element.removeAttr('type');
            }

            $element.removeAttr('readonly');
            $element.removeAttr('disabled');
        } else {
            if ($element.is('a')) {
                $element.data('old_href', $element.prop('href'));
                $element.removeAttr('href');
                $element.attr('type', 'button');
                $element.off('click');
            }

            $element.attr('readonly', 'readonly');
            $element.attr('disabled', 'true');
        }
    }

    function setChildrenReadonly(element, makeReadonly) {
        var inputs = $(element).find('input, textarea');
        var checkboxes = $(element).find('input[type=checkbox]');
        var radios = $(element).find('input[type=radio]');
        var selects = $(element).find('select, button');

        if (makeReadonly) {
            inputs.attr('readonly', 'readonly');
            checkboxes.attr('disabled', 'true');
            radios.attr('disabled', 'true');
            selects.attr('disabled', 'true');
        } else {
            inputs.removeAttr('readonly');
            checkboxes.removeAttr('disabled');
            radios.removeAttr('disabled');
            selects.removeAttr('disabled');
        }
    }

})();