define([
], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            label: '',
            percent: 0,
            value: 0,
            textCss: '',
            barCss: ''
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.label = defaultParams.label;
        self.total = defaultParams.total;
        self.value = defaultParams.value;
        self.barCss = defaultParams.barCss;
        self.textCss = defaultParams.textCss;

        self.percent = ko.computed(function () {
            return ko.unwrap(defaultParams.percent) + '%';
        });

        self.style = ko.computed(function () {
            return 'width: ' + self.percent();
        });
    }

    return ViewModel;
});
