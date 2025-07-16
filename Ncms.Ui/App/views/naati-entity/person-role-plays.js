define([
    'modules/enums',
    'services/person-data-service'
],
    function (enums, personService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                isPerson: ko.observable(false),
				naatiNumber: ko.observable(),
				person: {}
            };

            $.extend(defaultParams, params);

            var vm = {
				tableDefinition: {
                    id: 'rolePlaysTable',
                    headerTemplate: 'person-role-plays-header-template',
					rowTemplate: 'person-role-plays-row-template'
                },
				isPerson: defaultParams.isPerson,
				person: defaultParams.person,
                naatiNumber: ko.observable(),
				rolePlays: ko.observableArray([]),
                reload: ko.observable()
			}; 

            vm.tableDefinition.dataTable = {
                source: vm.rolePlays,
                columnDefs: [
                    {
                        targets: [2, 4],
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                    }
                ],
                order: [
                    [0, "desc"],
                    [1, "desc"],
                    [2, "desc"]
                ]
            };

            vm.load = function (naatiNumber) {
                vm.naatiNumber(naatiNumber);
                vm.rolePlays([]);
                personService.getFluid('allRolePlayRequests', { naatiNumber: vm.naatiNumber() })
                    .then(function (data) {
                        data.forEach(function (rolePlay) {

                            var informationString = '';

                            rolePlay.Details.forEach(function(info) {
                                informationString += '- ' +
                                    info.TestComponentName +
                                    ' (' +
                                    info.RolePlayerRoleTypeName +
                                    ',' +
                                    info.SkillName +
                                    ')\n';
                            });

                            rolePlay.RolePlayInformation = informationString;
                            vm.rolePlays.push(ko.viewmodel.fromModel(rolePlay));

                        });
                    });
            };

            return vm;
        }
    });
