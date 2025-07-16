define([
    'services/application-data-service'
],
    function (applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                naatiNumber: ko.observable(0)
            };
            $.extend(defaultParams, params);

            var vm = {
                newApplicationType: ko.observable().extend({ required: true }),
                newApplicationTypeText: ko.observable(),
                close: close,
                naatiNumber: ko.observable(),
                content: ko.observableArray([]),
                tableDefinition: {
                    headerTemplate: 'person-exemptions-header-template',
                    rowTemplate: 'person-exemptions-row-template',
                }
            };
            if (vm.tableDefinition.dataTable == undefined) {
                vm.tableDefinition.dataTable = {
                    source: vm.content,
                    columnDefs: [
                        {
                            targets: [2, 3],
                            render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                        }
                    ]
                };
            };


            vm.load = function (naatiNumber) {
                vm.content.removeAll();
                vm.naatiNumber(naatiNumber);
                applicationService.getFluid('exemptions', { naatiNumber: vm.naatiNumber() })
                    .then(function (data) {
                        data.forEach(function (element) {
                            var exemption = {
                                CredentialType: element.ExemptedCredentialName,
                                Skill: element.ExemptedCredentialSkill,
                                ExemptDate: element.ExemptionStartDate,
                                TerminationDate: element.ExemptionEndDate,
                                ExemptBy: element.ExemptionGrantedByUser
                            };
                            vm.content.push(ko.viewmodel.fromModel(exemption));                       
                        });
                    });
            };

            return vm;
        }
    });