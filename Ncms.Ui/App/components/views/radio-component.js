define([], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            checked: ko.observable(false),
            trueText: ko.Localization('Naati.Resources.Shared.resources.Yes'),
            falseText: ko.Localization('Naati.Resources.Shared.resources.No'),

            options: ko.observableArray(),
            selectedValue: ko.observable()
        };

        params.component = self;

        $.extend(defaultParams, params);

        self.checked = ko.observable(ko.toJS(defaultParams.checked) ? 'true' : 'false');
        self.trueText = defaultParams.trueText;
        self.falseText = defaultParams.falseText;

        //for options
        self.options = defaultParams.options;
        self.selectedValue = defaultParams.selectedValue;
        self.hasOptions = function () {
            return self.options().length > 0;
        };
        if (self.hasOptions()) {
            ko.utils.arrayForEach(self.options(), function (f) {
                f.value = f.FieldOptionId();
            });
        }

        self.getJson = function () {
            if (!self.hasOptions())
                return ko.toJS({ checked: self.checked() === 'true' });

            if (self.hasOptions()) {
                return ko.toJS({ selectedOptions: [self.selectedValue()] });
            }
        };
    }

    return ViewModel;
});
