(function () {
    ko.bindingHandlers.collapser = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            var collapserObservable = ko.observable();

            if (ko.isObservable(value)) {
                collapserObservable = value;
            }
            else {
                collapserObservable(value);
            }

            console.log(bindingContext);
            var newBindingContext = bindingContext.extend({ $expanded: collapserObservable });
            ko.applyBindingsToDescendants(newBindingContext, element);

            return { controlsDescendantBindings: true };
        }
    };

    ko.bindingHandlers.collapserHeader = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            if (!bindingContext.$expanded) {
                return;
            }

            $(element).off('click.collapser').on('click.collapser', function (e) {
                bindingContext.$expanded(!bindingContext.$expanded());
            });
        }
    };

    ko.bindingHandlers.collapserContent = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            if (!bindingContext.$expanded) {
                return;
            }

            ko.applyBindingsToNode(element, { if: bindingContext.$expanded }, bindingContext);
            return { controlsDescendantBindings: true };
        }
    };

    ko.virtualElements.allowedBindings.collapser = true;
})();
