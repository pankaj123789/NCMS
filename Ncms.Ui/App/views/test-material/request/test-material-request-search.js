define([
    'plugins/router',
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/material-request-data-service',
    'services/util'
],
    function (router, shell, common, enums, materialRequestService, util) {
        var vm = {
            title: shell.titleWithSmall,
            filters: [
                { id: 'Title' },
                { id: 'TestMaterialRequestPanel' },
                { id: 'TestMaterialType' },
                { id: 'TestTaskType' },
                { id: 'Language' },
                { id: 'TestMaterialRequestStatus' },
                { id: 'RoundStatus' },
                { id: 'DueDate' },
                { id: 'SourceTestMaterialId' },
                { id: 'MaterialId' },
                { id: 'OwnerNaatiUser' },
                { id: 'Overdue' },
                { id: 'TestMaterialCredentialDomain' },
            ],
            tableDefinition: {
                headerTemplate: 'test-material-request-header-template',
                rowTemplate: 'test-material-request-row-template'
            },
            searchType: enums.SearchTypes.TestMaterialRequest,
            searchTerm: ko.observable({}),
            testMaterialRequests: ko.observableArray([]),
            idSearchOptions: {
                search: function(value) {
                    var filter = JSON.stringify({ MaterialRequestIdIntList: [value] });
                    materialRequestService
                        .get({ request: { Skip: null, Filter: filter }, supressResponseMessages: true })
                        .then(function(data) {
                            if (!data || !data.length) {
                                return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Test material request', value));
                            }
                            router.navigate('material-request/' + value);
                        });
                }
            },
            canCreateTestMaterial: ko.observable(),
            createNewTestMaterial: function () {
                router.navigate('material-request-wizard/' + enums.MaterialRequestWizardActions.CreateMaterialRequest + '/0');
            }
        };

        vm.parsedSearchTerm = ko.pureComputed(function () {
            var searchTerm = vm.searchTerm();

            var parsedSearchTerm = {};
            
            if (searchTerm.Title) {
                $.extend(parsedSearchTerm, {
                    Title: searchTerm.Title.Data.value
                });
            }

            if (searchTerm.TestMaterialRequestPanel) {
                $.extend(parsedSearchTerm, {
                    TestMaterialRequestPanel: searchTerm.TestMaterialRequestPanel.Data.selectedOptions
                });
            }

            if (searchTerm.TestMaterialType) {
                $.extend(parsedSearchTerm, {
                    TestMaterialType: searchTerm.TestMaterialType.Data.selectedOptions
                });
            }

            if (searchTerm.TestTaskType) {
                $.extend(parsedSearchTerm, {
                    TestTaskType: searchTerm.TestTaskType.Data.selectedOptions
                });
            }

            if (searchTerm.Language) {
                $.extend(parsedSearchTerm, {
                    Language: searchTerm.Language.Data.selectedOptions
                });
            }

            if (searchTerm.TestMaterialRequestStatus) {
                $.extend(parsedSearchTerm, {
                    RequestStatus: searchTerm.RequestStatus.Data.selectedOptions
                });
            }

            if (searchTerm.RoundStatus) {
                $.extend(parsedSearchTerm, {
                    RoundStatus: searchTerm.RoundStatus.Data.selectedOptions
                });
            }

            return parsedSearchTerm;
        });

        vm.testMaterialRequestSearchRequest = ko.pureComputed(function () {
            return $.extend(vm.parsedSearchTerm(),
                {
                    useLess: false
                });
        });
        
        vm.tableDefinition.dataTable = {
            source: vm.testMaterialRequests,
            columnDefs: [
                {
                    targets: -1,
                    orderable: false
                },
                { targets: [1, 8], render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat) }
            ]
        };

        currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Manage).then(function (data) {
            vm.canCreateTestMaterial(data);
        });

        vm.additionalButtons = [
            {
                'class': 'btn btn-success',
                click: vm.createNewTestMaterial,
                icon: 'glyphicon glyphicon-plus',
                resourceName: 'Naati.Resources.Shared.resources.NewTestMaterial',
                show: vm.canCreateTestMaterial()
            }
        ];

        vm.activate = function () {
        };
        
        function parseSearchTerm(searchTerm) {
            var json = {
                TitleString: searchTerm.Title ? searchTerm.Title.Data.value : null,
                PanelIntList: searchTerm.TestMaterialRequestPanel ? searchTerm.TestMaterialRequestPanel.Data.selectedOptions : null,
                TestMaterialTypeIntList: searchTerm.TestMaterialType ? searchTerm.TestMaterialType.Data.selectedOptions : null,
                CredentialTypeIntList: searchTerm.TestMaterialCredentialDomain ? searchTerm.TestMaterialCredentialDomain.Data.Credential.Data.selectedOptions : null,
                TestMaterialDomainIntList: searchTerm.TestMaterialCredentialDomain ? searchTerm.TestMaterialCredentialDomain.Data.Domain.Data.selectedOptions : null,
                TestTaskTypeIntList: searchTerm.TestTaskType ? searchTerm.TestTaskType.Data.selectedOptions : null,
                LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
                TestMaterialRequestStatusIntList: searchTerm.TestMaterialRequestStatus ? searchTerm.TestMaterialRequestStatus.Data.selectedOptions : null,
                RoundStatusIntList: searchTerm.RoundStatus ? searchTerm.RoundStatus.Data.selectedOptions : null,
                DueDateFromString: searchTerm.DueDate ? searchTerm.DueDate.Data.From : null,
                DueDateToString: searchTerm.DueDate ? searchTerm.DueDate.Data.To : null,
                OutputTestMaterialIdIntList: searchTerm.MaterialId ? searchTerm.MaterialId.Data.valueAsArray : null,
                SourceTestMaterialIdIntList: searchTerm.SourceTestMaterialId ? searchTerm.SourceTestMaterialId.Data.valueAsArray : null,
                OwnerUserIntList: searchTerm.OwnerNaatiUser ? searchTerm.OwnerNaatiUser.Data.selectedOptions : null,
                OverdueBoolean: searchTerm.Overdue ? searchTerm.Overdue.Data.checked : null,
            };

            return JSON.stringify(json);
        }

        vm.searchCallback = function () {
            materialRequestService.get({ request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } })
                .then(function (data) {

                    ko.utils.arrayForEach(data,
                        function (item) {
                            item.RequestBadgeColor = util.getMaterialRequestStatusCss(item.RequestStatusTypeId);
                            item.RoundBadgeColor = util.getMaterialRequestRoundStatusCss(item.RoundStatusTypeId);

                        });

                    vm.testMaterialRequests(data);
                });
        };

        return vm;
    });
