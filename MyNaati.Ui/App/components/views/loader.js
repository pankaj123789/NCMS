define([], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            show: ko.observable(true),
            text: ko.observable('Loading...')
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.show = defaultParams.show;
        self.text = defaultParams.text;
    }

    return ViewModel;
});
