define([
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/institution-data-service',
    'views/naati-entity/create-institution'
    ],
    function (shell, common, enums, institutionService, createInstitution) {
        var vm = {
            title: shell.titleWithSmall,
            filters: [
                { id: 'NAATINumber' },
                { id: 'OrganisationName' },
                { id: 'ContactNumber' },
                { id: 'Email' },
                { id: 'ContactName' }
            ],
            tableDefinition: {
                id: 'organisationTable',
                headerTemplate: 'organisation-header-template',
                rowTemplate: 'organisation-row-template'
            },
            searchType: enums.SearchTypes.Organisation,
            searchTerm: ko.observable({}),
            entities: ko.observableArray([]),
            additionalButtons: [{
                'class': 'btn btn-success',
                click: showCreateOrganisation,
                icon: 'fa fa-plus',
                resourceName: 'Naati.Resources.Institution.resources.NewInstitution',
                enableWithPermission: 'Organisation.Create'
            }]
        };

        var createInstitutionInstance = createInstitution.getInstance();
        vm.createInstitutionOptions = {
            view: 'views/naati-entity/create-institution',
            model: createInstitutionInstance
        },

        vm.formatPhone = function (phone) {
            return common.functions().formatPhone(phone);
        };
        

        vm.tableDefinition.dataTable = {
            source: vm.entities,
            columnDefs: [
                { targets: -1, orderable: false }
            ],
            oLanguage: {
                sInfoEmpty: ''
            }
        };

        function organisationSearchTerm(searchTerm) {
            var json = {
                NaatiNumberIntList: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
                NameString: searchTerm.OrganisationName ? searchTerm.OrganisationName.Data.value : null,
                ContactNumberString: searchTerm.ContactNumber ? searchTerm.ContactNumber.Data.value : null,
                EmailString: searchTerm.Email ? searchTerm.Email.Data.value : null,
                ContactNameString: searchTerm.ContactName ? searchTerm.ContactName.Data.value : null
            };

            return JSON.stringify(json);
        }

        vm.searchCallback = function () {
            institutionService.getFluid('list', { request: { Take: 500, Skip: null, Filter: organisationSearchTerm(vm.searchTerm()) } })
                .then(function (data) {
                    vm.entities(data);
                });
        };

        function showCreateOrganisation() {
            createInstitutionInstance.show();
        }

        return vm;
    });
