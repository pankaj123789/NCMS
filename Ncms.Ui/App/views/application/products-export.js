define([
        'views/shell',
        'modules/common',
        'modules/enums',
        'services/application-data-service',
        'services/util'
    ],
    function(shell, common, enums, applicationService, util) {
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
                { id: 'Credential' },
                { id: 'CredentialStatusType' },
                { id: 'State' },
                { id: 'CredentialIssueDate' },
                { id: 'CredentialStartDate' },
                { id: 'CredentialEndDate' },
                { id: 'ShowInOnlineDirectory' },
                { id: 'ProductCardClaim' },
                { id: 'CertificationCredentialType' }
            ],
            tableDefinition: {
                headerTemplate: 'product-export-header-template',
                rowTemplate: 'product-export-row-template'
            },
            searchType: enums.SearchTypes.Person,
            searchTerm: ko.observable({}),
            entities: ko.observableArray([])
        };


        vm.formatPhone = function(phone) {
            return common.functions().formatPhone(phone);
        };

        vm.allowExport = ko.computed(function () {
            return vm.entities().length> 0;
        });

        vm.tableDefinition.dataTable = {
            source: vm.entities,
            buttons: {
                dom: {
                    button: {
                        tag: 'label',
                        className: ''
                    },
                    buttonLiner: {
                        tag: null
                    }
                },
                buttons: [
                    {
                        text: '<span class="glyphicon glyphicon-camera"></span><span>' +
                            ko.Localization('Naati.Resources.Shared.resources.ExportPhotos') +
                            '</span>',
                        className: 'btn btn-default',
                        enabled: currentUser.hasPermissionSync(enums.SecNoun.Credential, enums.SecVerb.Download),
                        action: downloadPhotosAndExcel
                        
                    }
                ]
            },
            columnDefs: [
                { targets: -1, orderable: false },
                {
                    targets: [5, 6, 8],
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
            oLanguage: {
                sInfoEmpty: ''
            },
            initComplete: enableDisableDataTableButtons,
        };

        vm.activate = function() {
        };

        function downloadPhotosAndExcel() {
            if (vm.allowExport()) {
                applicationService.getFluid('credentialPhotoExcel',
                        { request: { Take: 500, Skip: null, Filter: parseSearchTerm(vm.searchTerm()) } })
                    .then(function(data) {
                        window.location.replace(data);
                    });
            }
        }

        function enableDisableDataTableButtons() {
            var $table = $('#' + vm.tableDefinition.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var buttons = $table.DataTable().buttons('*');

            if (!buttons.length) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var disable = vm.entities().length === 0;
            if (disable) {
                buttons.disable();
            }
            else {
                buttons.enable();
            }
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
                ApplicationTypeIntList: searchTerm.ActiveApplicationType ? searchTerm.ActiveApplicationType.Data.selectedOptions : null,
                CredentialStartDateFromString: searchTerm.CredentialStartDate ? searchTerm.CredentialStartDate.Data.From : null,
                CredentialStartDateToString: searchTerm.CredentialStartDate ? searchTerm.CredentialStartDate.Data.To : null,
                CredentialEndDateFromString: searchTerm.CredentialEndDate ? searchTerm.CredentialEndDate.Data.From : null,
                CredentialEndDateToString: searchTerm.CredentialEndDate ? searchTerm.CredentialEndDate.Data.To : null,
                CredentialTypeIntList: searchTerm.Credential ? searchTerm.Credential.Data.Credential.Data.selectedOptions : null,
                SkillIntList: searchTerm.Credential ? searchTerm.Credential.Data.CredentialSkill.Data.selectedOptions : null,
                CredentialStatusTypeIntList: searchTerm.CredentialStatusType ? searchTerm.CredentialStatusType.Data.selectedOptions : null,
                ShowInOnlineDirectoryBoolean: searchTerm.ShowInOnlineDirectory ? searchTerm.ShowInOnlineDirectory.Data.checked : null,
                CredentialIssueDateFromString: searchTerm.CredentialIssueDate ? searchTerm.CredentialIssueDate.Data.From : null,
                CredentialIssueDateToString: searchTerm.CredentialIssueDate ? searchTerm.CredentialIssueDate.Data.To : null,
                ProductCardClaimBoolean: searchTerm.ProductCardClaim ? searchTerm.ProductCardClaim.Data.checked : null,
                CertificationCredentialTypeBoolean: searchTerm.CertificationCredentialType ? searchTerm.CertificationCredentialType.Data.checked : null,
                StateIntList: searchTerm.State ? searchTerm.State.Data.selectedOptions : null
            };

            return JSON.stringify(json);
        }

        vm.searchCallback = function () {
            applicationService.getFluid('credentialSearch', { request: { Take: 500, Skip: null, Filter: parseSearchTerm(vm.searchTerm()) } })
                .then(function (data) {
                    ko.utils.arrayForEach(data, function (c) {
                        c = util.updateCredentialStatus(c);
                    });
                    vm.entities(data);
                });
            //.then(function (data) {
            //    ko.utils.arrayForEach(data, function (tm) {

            //        tm.BadgeTooltip = util.getTestMaterialStatusToolTip(tm.StatusId, tm.LastUsedDate);
            //        tm.BadgeColor = util.getTestMaterialStatusColor(tm.StatusId);
            //        tm.BadgeText = util.getTestMaterialStatusText(tm.StatusId);
            //    });

            //    vm.testMaterials(data);
            //});
        };

        return vm;
    });
