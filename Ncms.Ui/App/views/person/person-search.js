define([
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/person-data-service',
    'views/naati-entity/create-person'
],
    function (shell, common, enums, personService, createNewPerson) {
        var vm = {
            title: shell.titleWithSmall,           
            filters: [
                { id: 'NAATINumber' },
                { id: 'PractitionerNumber' },
                { id: 'GivenName' },
                { id: 'FamilyName' },
                { id: 'ContactNumber' },
                { id: 'Email' },
                { id: 'DateOfBirth' },
                { id: 'Gender' },
                { id: 'ActiveApplicationType' },
                { id: 'Credential'},
                { id: 'CredentialStatusType' },
                { id: 'PersonType' },
                { id: 'State' },
                { id: 'RolePlayer' },
                { id: 'PersonExaminer' },
                { id: 'Deceased' }

            ],
            tableDefinition: {
                id: 'personTable',
                headerTemplate: 'person-header-template',
                rowTemplate: 'person-row-template'
            },
            searchType: enums.SearchTypes.Person,
            searchTerm: ko.observable({}),
            entities: ko.observableArray([]),
            additionalButtons: [{
                'class': 'btn btn-success',
                click: createPerson,
                icon: 'fa fa-plus',
                resourceName: 'Naati.Resources.Person.resources.CreateNewPerson',
                enableWithPermission: 'Person.Create'
            }]
        };

        var createPersonInstance = createNewPerson.getInstance();
        vm.createPersonOptions = {
            view: 'views/naati-entity/create-person',
            model: createPersonInstance
        },
        
        vm.formatPhone = function(phone) {
            return common.functions().formatPhone(phone);
        };

        vm.showDetails = function (i, e) {
            var target = $(e.target);
            var tr = target.closest('tr');
            var dt = tr.closest('#' + vm.tableDefinition.id).DataTable();
            var row = dt.row(tr);

            if (row.child.isShown()) {
                target.removeClass('fa-chevron-down').addClass('fa-chevron-right');
                tr.removeClass('details');
                row.child.hide();
            }
            else {
                target.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                tr.addClass('details');
                showLines(i, row);
            }
        };

        vm.tableDefinition.dataTable = {
            source: vm.entities,
            columnDefs: [
                { targets: -1, orderable: false },
                {
                    targets:4,
                    render: function (data) {
                        var separator = ', ';
                        if (data) {
                            return data.split(separator).map(function (value) {
                                return ko.Localization('Naati.Resources.PersonType.resources.' + value);
                            }).join(separator);
                        }

                        return '';
                    } 
                }
            ],
            oLanguage: {
                sInfoEmpty: ''
            },
         
        };
        
        vm.activate = function() {
        };

        function showLines(application, row) {
            personService.getFluid('{0}/detail'.format(application.Id)).then(function (data) {
                var detailsTemplate = ko.generateTemplate('persondetail-template', {
                    tableDefinition: {
                        searching: false,
                        paging: false,
                        oLanguage: { sInfoEmpty: '', sInfo: '' }
                    },
                    data: data
                });
                row.child(detailsTemplate).show();
            });
        }

        function parseSearchTerm(searchTerm) {
            var json = {
                NaatiNumberIntList: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
                PractitionerNumberString: searchTerm.PractitionerNumber ? searchTerm.PractitionerNumber.Data.value : null,
                GivenNamesString: searchTerm.GivenName ? searchTerm.GivenName.Data.value : null,
                FamilyNameString: searchTerm.FamilyName ? searchTerm.FamilyName.Data.value : null,
                PhoneNumberString: searchTerm.ContactNumber ? searchTerm.ContactNumber.Data.value : null,
                EmailString: searchTerm.Email ? searchTerm.Email.Data.value : null,
                DateOfBirthFromString: searchTerm.DateOfBirth ? searchTerm.DateOfBirth.Data.From : null,
                DateOfBirthToString: searchTerm.DateOfBirth ? searchTerm.DateOfBirth.Data.To : null,
                GenderStringList: searchTerm.Gender ? searchTerm.Gender.Data.selectedOptions : null,
                CredentialTypeIntList: searchTerm.CredentialRequestType ? searchTerm.CredentialRequestType.Data.selectedOptions : null,
                ActiveApplicationTypeIntList: searchTerm.ActiveApplicationType ? searchTerm.ActiveApplicationType.Data.selectedOptions : null,
                CredentialIntList: searchTerm.Credential ? searchTerm.Credential.Data.Credential.Data.selectedOptions : null,
                CredentialSkillIntList: searchTerm.Credential ? searchTerm.Credential.Data.CredentialSkill.Data.selectedOptions : null,
                CredentialStatusTypeIntList: searchTerm.CredentialStatusType ? searchTerm.CredentialStatusType.Data.selectedOptions : null,
                PersonTypeIntList: searchTerm.PersonType ? searchTerm.PersonType.Data.selectedOptions : null,
                AddressStateIntList: searchTerm.State ? searchTerm.State.Data.selectedOptions : null,
                RolePlayerCredentialTypeIntList: searchTerm.RolePlayer && searchTerm.RolePlayer.Data.CredentialType.Data.selectedOptions.length ? searchTerm.RolePlayer.Data.CredentialType.Data.selectedOptions : null,
                RolePlayerPanelIntList: searchTerm.RolePlayer && searchTerm.RolePlayer.Data.Panel.Data.selectedOptions.length ? searchTerm.RolePlayer.Data.Panel.Data.selectedOptions : null,
                ExaminerCredentialTypeIntList: searchTerm.PersonExaminer && searchTerm.PersonExaminer.Data.CredentialType.Data.selectedOptions.length ? searchTerm.PersonExaminer.Data.CredentialType.Data.selectedOptions : null,
                ExaminerPanelIntList: searchTerm.PersonExaminer && searchTerm.PersonExaminer.Data.Panel.Data.selectedOptions.length ? searchTerm.PersonExaminer.Data.Panel.Data.selectedOptions : null,
                DeceasedBoolean: searchTerm.Deceased && searchTerm.Deceased.Data ? searchTerm.Deceased.Data.checked : null
            };

            return JSON.stringify(json);
        }

        vm.searchCallback = function () {
            personService.getFluid('list', { request: { Take: 500, Skip: null, Filter: parseSearchTerm(vm.searchTerm()) } })
                .then(function (data) {
                    vm.entities(data);
                });
        };

        function createPerson() {

            createPersonInstance.show();
        }

        return vm;
    });
