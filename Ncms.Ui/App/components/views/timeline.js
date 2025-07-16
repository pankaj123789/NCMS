define([], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            items: ko.observableArray()
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.items = defaultParams.items;
        self.regenerateEmailPreview= function() {
            this.compose.model.load();
        }
    }

    return ViewModel;
});
