define([], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            value: ko.observable(''),
            valueUpdate: 'afterkeyup',
            disable: false,
            attr: {},
            css: 'form-control'
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.value = ko.isObservable(defaultParams.value)
            ? defaultParams.value
            : ko.observable(defaultParams.value);
        self.valueUpdate = defaultParams.valueUpdate;
        self.disable = defaultParams.disable;
        self.attr = defaultParams.attr;
        self.css = defaultParams.css;
        self.resattr = defaultParams.resattr;
        self.tooltip = defaultParams.tooltip;

        self.getJson = function () {
            return ko.toJS({ value: self.value() });
        }
    }

    return ViewModel;
});
