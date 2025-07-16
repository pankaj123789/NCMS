define(['services/util'], function (util) {
    function ViewModel(params) {
        var self = this;

        params.component = self;
        self.id = params.id || util.guid();
        self.dataTable = params.dataTable;
        self.headerTemplate = params.headerTemplate;
        self.rowTemplate = params.rowTemplate;
        self.css = params.css;

        self.source = ko.observableArray(self.dataTable.source());

        self.dataTable.source.subscribe(function (value) {
            var sourceLength = self.source() ? self.source().length : 0;
            var valueLength = value ? value.length : 0;

            if (sourceLength && valueLength && sourceLength === 0) {
                return;
            }

            self.source(value);
        });
    }

    return ViewModel;
});
