define(['services/test-material-data-service', 'services/screen/message-service'],
function (testmaterialService, messageService) {

    function getInstance() {
        var defer = null;

        var vm = {
            selectedIds: ko.observableArray(),
            testSittingIds: ko.observableArray(),
            testApplicantTestMaterials: ko.observableArray(),
            credentialType: ko.observable(),
            credentialTypeId: ko.observable(),
            skill: ko.observable(),
            skillId: ko.observable(),
            showAllValue: ko.observable(false),
            showIncludedValue: ko.observable(false),
            testTasks: ko.observableArray(),
            testComponentId: ko.observable(),
            showIncludedMaterialLabel: ko.observable(),
            show: function (params) {
                defer = Q.defer();
                vm.testComponentId(null);
                drawmaterial(params);
                return defer.promise;
            },

            assign: function () {
                var request = {
                    TestSittingIds: vm.testSittingIds(),
                    TestComponentIds: {}
                };

                request.TestComponentIds[vm.testComponentId()] = vm.selectedIds();

                testmaterialService.post(request, 'assignMaterial').then(function (data) {
                    $('#drawMaterialModal').modal('hide');
                    toastr.success('Details saved');
                    defer.resolve(request);
                });

                return false;
            },

            removeTestMaterials: function (param) {

                var request = {
                    TestSittingIds: param
                };

                defer = Q.defer();
                testmaterialService.post(request, 'removeTestMaterials').then(function () {
                    defer.resolve(request);
                });
                return defer.promise;
            },

            tableDefinition: {
                id: 'testsitting-material-table',
                headerTemplate: 'testsittingmaterial-header-template',
                rowTemplate: 'testsettingmaterial-row-template'
            }
        };

        vm.activate = function () {
            testmaterialService.getFluid('getIncludeSystemValueSkillNames').then(function (data) {
                if (data) {
                    vm.showIncludedMaterialLabel('Include source material: ' + data[0]);
                }
                else {
                    vm.showIncludedMaterialLabel('Include source material: (none configured)' );
                }
            });
        }

        vm.tableDefinition.dataTable = {
            source: vm.testApplicantTestMaterials,
            columnDefs: [
                {
                    targets: -1,
                    orderable: false
                }
            ],
            order: [
                [0, "asc"]
            ],
            select: {
                style: 'multi+shift'
            },
            events: {
                select: selectTable,
                deselect: selectTable,
            }
        }

        vm.canAssign = ko.pureComputed(function () {
            return vm.testComponentId() && vm.selectedIds().length > 0;
        });

        vm.testTasksOptions = {
            value: vm.testComponentId,
            multiple: false,
            options: vm.testTasks,
            optionsValue: 'TestComponentId',
            optionsText: 'OptionText'
        };

        vm.title = ko.pureComputed(function () {
            var text = ko.Localization('Naati.Resources.TestMaterial.resources.AssignTestMaterial');
            if (vm.credentialType()) {
                text += ' - ' + vm.credentialType();
            }
            if (vm.skill()) {
                text += ' ' + vm.skill();
            }
            return text;
        });

        ko.computed(function () {
            vm.selectedIds([]);

            var testComponentId = vm.testComponentId();
            var showAllValue = vm.showAllValue();
            var showIncludedValue = vm.showIncludedValue();

            if (!testComponentId) {
                return vm.testApplicantTestMaterials([]);
            }

            var request = {
                TestComponentId: testComponentId,
                SkillId: showAllValue ? null : vm.skillId(),
                IncludeSystemValueSkillTypes: showIncludedValue ? true : false
            };

            testmaterialService.getFluid('fromtesttask', request).then(function (materials) {
                vm.testApplicantTestMaterials(materials);
            });
        });

        function selectTable(e, dt) {
            vm.selectedIds([]);

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                vm.selectedIds.push(vm.testApplicantTestMaterials()[indexes[i]].Id);
            }
        }

        function drawmaterial(params) {
            var options = {
                TestSittingIds: vm.testSittingIds(),
                ShowAllMaterials: vm.showAllValue(),
                CredentialTypeId: vm.credentialTypeId(),
                CredentialType: vm.credentialType(),
                SkillId: vm.skillId(),
                Skill: vm.skill()
            };

            $.extend(options, params);

            vm.testSittingIds(options.TestSittingIds);
            vm.showAllValue(options.ShowAllMaterials);
            vm.credentialType(options.CredentialType);
            vm.credentialTypeId(options.CredentialTypeId);
            vm.skill(options.Skill);
            vm.skillId(options.SkillId);

            var array = options.TestTasks();
            var optionArray = Array.from(new Set(array.map(s => s.TestComponentId())))
                .map(testComponentId => {
                    return {
                        TestComponentId: testComponentId,
                        TestComponentTypeLabel: array.find(s => s.TestComponentId() === testComponentId).TestComponentTypeLabel(),
                        Label: array.find(s => s.TestComponentId() === testComponentId).Label(),
                        TestComponentName: array.find(s => s.TestComponentId() === testComponentId).TestComponentName()
                    }
                });

            ko.utils.arrayForEach(optionArray,
                function(t) {
                    t.OptionText = ko.computed(function() {
                        return t.TestComponentTypeLabel + t.Label;
                    });
                });

            vm.testTasks(optionArray);

            //testmaterialService.getFluid('testtasks/' + options.CredentialTypeId).then(function (data) {
            //    ko.utils.arrayForEach(data, function (d) {
            //        d.Label = d.TestComponentTypeLabel + d.Label;
            //    });
            //    vm.testTasks(data);
            //});

            $('#drawMaterialModal').modal('show');
        }

        return vm;
    }

    return {
        getInstance: getInstance
    };
});
