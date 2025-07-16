define([
    'services/util',
    'services/application-data-service',
], function (util, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null,
            nextStepAction: null,
        };

        $.extend(defaultParams, params);

        var vm = {
            searchCriteria: ko.observable(),
            materialRequest: defaultParams.materialRequest,
            testMaterialSources: ko.observableArray(),
            showStep: ko.observable(true),
            nextStepAction: defaultParams.nextStepAction,
            wizardService: defaultParams.wizardService,
            testMaterialSourceId: defaultParams.materialRequest.SourceTestMaterialId,
            action: defaultParams.action,
            outputMaterial: defaultParams.outputMaterial,
        };

        var validation = ko.validatedObservable([vm.testMaterialSourceId]);

        vm.tableDefinition = {
            id: 'materialSourceTable',
            headerTemplate: 'materialsource-header-template',
            rowTemplate: 'materialsource-row-template'
        };

        vm.tableDefinition.dataTable = {
            source: vm.testMaterialSources,
            select: {
                style: 'single',
                info: false
            },
            columnDefs: [
                { orderable: false, className: 'select-checkbox', targets: 0 },
                { targets: -1, orderable: false },
                { orderData: 1, targets: 1 }
            ],
            events: {
                select: selectTable,
                deselect: selectTable
            },
            initComplete: selectTableIfSingle
        };

        vm.testMaterialSourceTableOptions = {
            name: 'table-component',
            params: vm.tableDefinition
        };

        vm.find = function () {
            defaultParams.wizardService.getFluid('wizard/testmaterialsource/' + vm.searchCriteria()).then(function (data) {
                ko.utils.arrayForEach(data, function (tm) {
                    tm.BadgeTooltip = util.getTestMaterialStatusToolTip(tm.StatusId, tm.LastUsedDate);
                    tm.BadgeColor = util.getTestMaterialStatusColor(tm.StatusId);
                    tm.BadgeText = util.getTestMaterialStatusText(tm.StatusId);
                    tm.AvailableMessage = tm.Available ? ko.Localization('Naati.Resources.TestMaterial.resources.Available') : ko.Localization('Naati.Resources.TestMaterial.resources.NotAvailable');
                });
                if (!data.length) {
                    vm.materialRequest.SourceTestMaterialId(0);
                }
                vm.testMaterialSources(data);
            });
        };

     

        vm.load = function () {

            vm.searchCriteria(null);

            vm.testMaterialSources([]);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
            vm.wizardService.getFluid('wizard/sourceTestMaterial/show/' + vm.action().Id + '/' + vm.outputMaterial.TestMaterialTypeId()).then(function (data) {
                vm.showStep(data);
                if (vm.showStep()) {
                    vm.searchCriteria(vm.materialRequest.SourceTestMaterialId());
                    vm.find();
                } else {
                    vm.materialRequest.SourceTestMaterialId(0);
                    vm.nextStepAction();
                }
            });
        }

        vm.postData = function () {
            return ko.toJS(vm.outputMaterial);
        };

        vm.isValid = function () {
            var isValid = !vm.showStep() || validation.isValid();

            if (!isValid) {
                validation.errors.showAllMessages();
            }

            return isValid;
        };

        vm.postData = function () {
            return { TestMaterialId: vm.testMaterialSourceId() || 0 };
        };

        function selectTableIfSingle() {
            if (vm.testMaterialSources().length != 1) {
                return;
            }

            var $table = $('#' + vm.tableDefinition.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(selectTableIfSingle, 100);
                return;
            }

            $table.DataTable().rows().select();
            vm.testMaterialSourceId(vm.testMaterialSources()[0].Id);
        }

        function selectTable(e, dt) {
            vm.testMaterialSourceId(null);

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            var id = vm.testMaterialSources()[indexes[0]].Id;
            vm.testMaterialSourceId(id);
        }

        return vm;
    }
});