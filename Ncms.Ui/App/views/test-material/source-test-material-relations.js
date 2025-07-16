define([
    'services/test-material-data-service',
        'services/util'
],
    function (testMaterialService, util) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                isWriter: ko.observable(false),
                sourceRelations: ko.observableArray(),
                tableDefinition: {
                    headerTemplate: 'source-material-relations-header-template',
                    rowTemplate: 'source-material-relations-row-template'
                }
            };

            vm.tableDefinition.dataTable = {
                source: vm.sourceRelations,
                columnDefs: [
                ],
                order: [
                    [0, "desc"]
                ]
            };

            vm.load = function (sourceTestMaterialId, Id) {
                vm.sourceRelations([]);

                if (sourceTestMaterialId !== undefined && sourceTestMaterialId !== null) {
                    testMaterialService.getFluid('searchTestMaterials',
                            {
                                request: {
                                    Skip: null,
                                    Take: null,
                                    Filter: '{SourceTestMaterialIdIntList:[' + sourceTestMaterialId + ']}'
                                },
                                supressResponseMessages: true
                            })
                        .then(function (data) {
                            data = data.filter(function (e) {
                                return e.Id !== Id;
                            });

                            ko.utils.arrayForEach(data,
                                function (tm) {
                                    tm.sourceId = sourceTestMaterialId;
                                    tm.BadgeTooltip = util.getTestMaterialStatusToolTip(tm.StatusId, tm.LastUsedDate);
                                    tm.BadgeColor = util.getTestMaterialStatusColor(tm.StatusId);
                                    tm.BadgeText = util.getTestMaterialStatusText(tm.StatusId);
                                });

                            vm.sourceRelations(data);
                        });
                }
            };

            vm.edit = function (testMaterial) {
                edit(testMaterial.Id);
            };

            vm.availableMessage = function (testMaterial) {
                if (testMaterial.Available) {
                    return ko.Localization('Naati.Resources.TestMaterial.resources.Available');
                }

                return ko.Localization('Naati.Resources.TestMaterial.resources.NotAvailable');
            };

            return vm;
        }

        function edit(id) {
            router.navigate('test-material/' + id);
        }
    });
