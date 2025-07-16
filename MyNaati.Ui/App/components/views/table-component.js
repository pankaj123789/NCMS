define(['services/util'], function (util) {
    function ViewModel(params) {
        var self = this;

        params.component = self;
        self.id = params.id || util.guid();
        self.dataTable = params.dataTable;
        self.headerTemplate = params.headerTemplate;
        self.rowTemplate = params.rowTemplate;

        self.source = ko.observableArray(self.dataTable.source());

        self.dataTable.source.subscribe(function () {
            self.source([]);
            self.source(self.dataTable.source());
        });
    }

    return ViewModel;
});
