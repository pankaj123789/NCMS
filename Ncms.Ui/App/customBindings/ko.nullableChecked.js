(function () {
    ko.bindingHandlers.nullableChecked = {
        init: function (element, valueAccessor) {
            ko.bindingHandlers.checked.init(element, valueAccessor);
        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value == null) {
                element.indeterminate = true;
            }
            else {
                element.indeterminate = false;
            }
        }
    };
})();
