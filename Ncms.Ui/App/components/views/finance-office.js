define([
    'services/finance-data-service',
], function (financeService) {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        self.office = params.Office || {};
        self.office.options = ko.observableArray();
        self.office.selectedOptions = self.office.selectedOptions || [];
        self.office.selectedOptions = ko.observableArray(self.office.selectedOptions);

        self.eftMachine = params.EFTMachine || {};
        self.eftMachine.options = ko.observableArray();
        self.eftMachine.selectedOptions = self.eftMachine.selectedOptions || [];
        self.eftMachine.selectedOptions = ko.observableArray(self.eftMachine.selectedOptions);

        self.loadOptions = function () {
            financeService.getFluid('offices').then(function (data) {
                var offices = $.map(data,
                    function (e) {
                        return {
                            EntityId: e.Id,
                            Name: e.Name
                        };
                });

                self.office.options(offices);
                self.loadMachines();
            });
        };

        self.loadMachines = function () {
            var offices = self.office.selectedOptions();
            if (!offices.length) {
                offices = $.map(self.office.options(), function (e) { return e.EntityId; });
            }

            financeService.getFluid('eftMachines').then(function (data) {
                var efts = $.map(data,
                    function (e) {
                        if (offices.indexOf(e.OfficeId) !== -1) {
                            return {
                                EntityId: e.Id,
                                Name: e.TerminalNumber
                            };
                        }
                    });
                self.eftMachine.options(efts);
            });
        };
        self.office.selectedOptions.subscribe(self.loadMachines);

        self.getJson = function () {
            return {
                Office: self.office.component.getJson(),
                EFTMachine: self.eftMachine.component.getJson()
            };
        };

        self.loadOptions();
    }

    return ViewModel;
});