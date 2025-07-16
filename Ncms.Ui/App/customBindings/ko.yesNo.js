ko.bindingHandlers.yesNo = {
    update: function (element, valueAccessor, allBindings) {
        var yesLabel = typeof allBindings().yesLabel === 'undefined' ? ko.Localization('Naati.Resources.Shared.resources.Yes') : ko.Localization(ko.unwrap(allBindings().yesLabel));
        var noLabel = typeof allBindings().noLabel === 'undefined' ? ko.Localization('Naati.Resources.Shared.resources.No') : ko.Localization(ko.unwrap(allBindings().noLabel));

        var value = valueAccessor();
        var unwrap = ko.unwrap(value);
        var yesNo = unwrap ? yesLabel : noLabel;
        ko.applyBindingsToNode(element, { text: yesNo });
    }
};