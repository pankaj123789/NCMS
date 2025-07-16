define([
    'plugins/router',
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/application-data-service'
],
    function (router, shell, common, enums, applicationService) {
        var vm = {
            title: shell.titleWithSmall,
            filters: [
                { id: 'NAATINumber' },
                { id: 'ApplicationReference' },
                { id: 'ApplicationOwner' },
                { id: 'PersonName' },
                { id: 'ContactNumber' },
                { id: 'ActiveApplication' },
                { id: 'AutoCreatedApplication' },
                { id: 'AutoCreatedCredentialRequest' },
                { id: 'ApplicationType' },
                { id: 'CredentialRequestType' },
                { id: 'ApplicationStatusType' },
                { id: 'CredentialRequestStatusType' },
                { id: 'EnteredOffice' },
                { id: 'Language' },
                { id: 'PreferredTestLocation' },
                { id: 'TestDateFromAndTo', name: 'Naati.Resources.Shared.resources.EnteredDate' },
                { id: 'TestDateFromAndTo2', name: 'Naati.Resources.Shared.resources.StatusModifiedDate' },
                { id: 'Sponsor' },
                { id: 'EndorsedQualifications' }
            ],
            tableDefinition: {
                id: 'applicationsTable',
                headerTemplate: 'application-header-template',
                rowTemplate: 'application-row-template'
            },
            searchType: enums.SearchTypes.Application,
            searchTerm: ko.observable({}),
            applications: ko.observableArray([]),
            newApplicationType: ko.observable().extend({ required: true }),
            idSearchOptions: {
                search: function (value) {
                    var filter = JSON.stringify({ ApplicationReferenceString: 'APP' + value });
                    applicationService.post({ Skip: null, Take: 2, Filter: filter }, 'search')
                        .then(function (data) {
                            if (!data || !data.length) {
                                return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Application', value));
                            }
                            router.navigate('application/' + value);
                        });
                },
                prefix: 'APP'
            }
        };

        vm.parsedSearchTerm = ko.pureComputed(function () {
            var searchTerm = vm.searchTerm();

            var parsedSearchTerm = {};

            if (searchTerm.NAATINumber) {
                $.extend(parsedSearchTerm, {
                    NaatiNumber: searchTerm.NAATINumber.Data.NAATINumber
                });
            }

            if (searchTerm.ApplicationReference) {
                $.extend(parsedSearchTerm, {
                    ApplicationReference: searchTerm.ApplicationReference.Data.value
                });
            }

            if (searchTerm.PersonName) {
                $.extend(parsedSearchTerm, {
                    GivenNames: searchTerm.PersonName.Data.value
                });
            }

            if (searchTerm.ContactNumber) {
                $.extend(parsedSearchTerm, {
                    ContactNumber: searchTerm.ContactNumber.Data.value
                });
            }

            if (searchTerm.ActiveApplication) {
                $.extend(parsedSearchTerm, {
                    ActiveApplication: searchTerm.ActiveApplication.Data.checked
                });
            }

            if (searchTerm.AutoCreatedApplication) {
                $.extend(parsedSearchTerm, {
                    AutoCreatedApplication: searchTerm.AutoCreatedApplication.Data.checked
                });
            }

            if (searchTerm.ApplicationType) {
                $.extend(parsedSearchTerm, {
                    ApplicationType: searchTerm.ApplicationType.Data.selectedOptions
                });
            }

            if (searchTerm.CredentialRequestType) {
                $.extend(parsedSearchTerm, {
                    CredentialRequestType: searchTerm.CredentialRequestType.Data.selectedOptions
                });
            }

            if (searchTerm.ApplicationStatusType) {
                $.extend(parsedSearchTerm, {
                    ApplicationStatus: searchTerm.ApplicationStatusType.Data.selectedOptions
                });
            }

            if (searchTerm.CredentialRequestStatusType) {
                $.extend(parsedSearchTerm, {
                    CredentialRequestStatus: searchTerm.CredentialRequestStatusType.Data.selectedOptions
                });
            }

            if (searchTerm.ApplicationOwner) {
                $.extend(parsedSearchTerm, {
                    ApplicationOwner: searchTerm.ApplicationOwner.Data.selectedOptions
                });
            }

            if (searchTerm.Language) {
                $.extend(parsedSearchTerm, {
                    Language: searchTerm.Language.Data.Options
                });
            }

            return parsedSearchTerm;
        });

        vm.applicationSearchRequest = ko.pureComputed(function () {
            return $.extend(vm.parsedSearchTerm(),
                {
                    useLess: false
                });
        });

        vm.export = function () {
            return applicationService.url() + '/exportApplications/?' + $.param({ Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) });
        };

        vm.additionalButtons = [{
            'class': 'btn btn-success',
            downloadFile: { url: vm.export },
            icon: 'glyphicon glyphicon-plus',
            resourceName: 'Naati.Resources.Shared.resources.ExportToExcel',
            disable: !currentUser.hasPermissionSync(enums.SecNoun.Application, enums.SecVerb.Download)
        }];

        var validation = ko.validatedObservable([vm.newApplicationType]);
        vm.applicationTypesParams = {
            multiple: false,
            value: vm.newApplicationType,
            options: common.functions().optionsNameFactory(enums.ApplicationTypes, 'ApplicationTypes', true)
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
            source: vm.applications,
            columnDefs: [
                {
                    targets: 3,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                },
                {
                    targets: 0,
                    type: 'natural'
                },
                {
                    targets: -1,
                    orderable: false
                },
            ],
            oLanguage: { sInfoEmpty: '' },
            order: [
                [3, "asc"],
                [0, "desc"],
                [1, "desc"],
                [2, "desc"]
            ]
        };

        function showLines(application, row) {
            applicationService.getFluid('{0}/credentialrequests'.format(application.Id)).then(function (data) {
                var detailsTemplate = ko.generateTemplate('application-search-detail-template', {
                    tableDefinition: {
                        searching: false,
                        paging: false,
                        order: [
                            [0, "desc"],
                            [1, "desc"]
                        ],
                        oLanguage: { sInfoEmpty: '', sInfo: '' }
                    },
                    data: data
                });
                row.child(detailsTemplate).show();
            });
        }

        vm.activate = function () {
        };

        vm.formatNumber = function (phoneNumber) {
            return common.functions().formatPhone(phoneNumber);
        };

        function parseSearchTerm(searchTerm) {
            var json = {
                NaatiNumberIntList: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
                ApplicationReferenceString: searchTerm.ApplicationReference ? searchTerm.ApplicationReference.Data.value : null,
                PersonNameString: searchTerm.PersonName ? searchTerm.PersonName.Data.value : null,
                PhoneNumberString: searchTerm.ContactNumber ? searchTerm.ContactNumber.Data.value : null,
                ActiveApplicationBoolean: searchTerm.ActiveApplication ? searchTerm.ActiveApplication.Data.checked : null,
                AutoCreatedApplicationBoolean: searchTerm.AutoCreatedApplication ? searchTerm.AutoCreatedApplication.Data.checked : null,
                AutoCreatedCredentialRequestBoolean: searchTerm.AutoCreatedCredentialRequest ? searchTerm.AutoCreatedCredentialRequest.Data.checked : null,
                ApplicationTypeIntList: searchTerm.ApplicationType ? searchTerm.ApplicationType.Data.selectedOptions : null,
                EnteredOfficeIntList: searchTerm.EnteredOffice ? searchTerm.EnteredOffice.Data.selectedOptions : null,
                CredentialRequestTypeIntList: searchTerm.CredentialRequestType ? searchTerm.CredentialRequestType.Data.selectedOptions : null,
                ApplicationStatusIntList: searchTerm.ApplicationStatusType ? searchTerm.ApplicationStatusType.Data.selectedOptions : null,
                CredentialRequestStatusIntList: searchTerm.CredentialRequestStatusType ? searchTerm.CredentialRequestStatusType.Data.selectedOptions : null,
                ApplicationOwnerIntList: searchTerm.ApplicationOwner ? searchTerm.ApplicationOwner.Data.selectedOptions : null,
                LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
                PreferredTestLocationIntList: searchTerm.PreferredTestLocation ? searchTerm.PreferredTestLocation.Data.selectedOptions : null,
                EnteredDateFrom: searchTerm.TestDateFromAndTo && searchTerm.TestDateFromAndTo.Data.From ? searchTerm.TestDateFromAndTo.Data.From : null,
                EnteredDateTo: searchTerm.TestDateFromAndTo && searchTerm.TestDateFromAndTo.Data.To ? searchTerm.TestDateFromAndTo.Data.To : null,
                StatusModifiedDateFrom: searchTerm.TestDateFromAndTo2 && searchTerm.TestDateFromAndTo2.Data.From ? searchTerm.TestDateFromAndTo2.Data.From : null,
                StatusModifiedDateTo: searchTerm.TestDateFromAndTo2 && searchTerm.TestDateFromAndTo2.Data.To ? searchTerm.TestDateFromAndTo2.Data.To : null,
                SponsorIntList: searchTerm.Sponsor ? searchTerm.Sponsor.Data.selectedOptions : null,

                EndorsedQualificationIdsIntList: searchTerm.EndorsedQualifications ? searchTerm.EndorsedQualifications.Data.EndorsementQualificationIds : null,
                EndorsementQualificationDateFrom: searchTerm.EndorsedQualifications && searchTerm.EndorsedQualifications.Data.EndorsementPeriod.From ? searchTerm.EndorsedQualifications.Data.EndorsementPeriod.From : null,
                EndorsementQualificationDateTo: searchTerm.EndorsedQualifications && searchTerm.EndorsedQualifications.Data.EndorsementPeriod.To ? searchTerm.EndorsedQualifications.Data.EndorsementPeriod.To : null
            };

            $.extend(json, searchTerm.ApplicationStatus);

            return JSON.stringify(json);
        }

        vm.searchCallback = function () {
            applicationService.post({ Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) }, 'search')
                .then(function (data) {
                    vm.applications(data);
                });
        };

        return vm;
    });
