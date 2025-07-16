define([
    'plugins/router',
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/credentialrequest-data-service'
],
    function (router, shell, common, enums, credentialrequestService) {
        var vm = {
            title: 'Credential Request Summary',
            filters: [
                { id: 'PreferredTestLocation' },
                { id: 'CredentialRequestStatusTypeForSummary' },
                { id: 'ApplicationType' },
                { id: 'CredentialRequestType' },
                { id: 'Language' },
                // disabled for 1.5 { id: 'IntendedTestSession' }
            ],
            tableDefinition: {
                id: 'credential-request-summary-table',
                headerTemplate: 'credential-request-summary-header-template',
                rowTemplate: 'credential-request-summary-row-template'
            },
            searchType: enums.SearchTypes.CredentialRequestSummary,
            searchTerm: ko.observable({}),
            entities: ko.observableArray([])
        };

        vm.tableDefinition.dataTable = {
            source: vm.entities,
            columnDefs: [
                { targets: -1, orderable: false },
                { "orderData": [0], "targets": [1] }
            ],
            oLanguage: {
                sInfoEmpty: ''
            },
            order: [
                [1, "desc"]
            ]
        };

        vm.getActions = function (summary) {
            credentialrequestService.getFluid('actions', { CredentialRequestStatusTypeId: summary.CredentialRequestStatusTypeId }).then(function (data) {
                if (!data.length) {
                    data = [{
                        Name: ko.Localization('Naati.Resources.CredentialRequestSummary.resources.NoActionsAvailable'),
                        Disabled: true
                    }]
                }

                summary.Actions(data);
            });
        };

        vm.selectAction = function (summary, action) {
            if (action.Disabled) {
                event.preventDefault();
                return false;
            }

            var url = 'credential-request-wizard/{0}/{1}/{2}/{3}/{4}/{5}'.format(
                summary.CredentialApplicationTypeId,
                summary.CredentialTypeId,
                summary.SkillId,
                summary.CredentialRequestStatusTypeId,
                summary.TestLocationId,
                action.Id);

            router.navigate(url);
        };

        function credentialRequestSummarySearchTerm(searchTerm) {
            var json = {
                PreferredTestLocationIntList: searchTerm.PreferredTestLocation ? searchTerm.PreferredTestLocation.Data.selectedOptions : null,
                CredentialRequestStatusIntList: searchTerm.CredentialRequestStatusTypeForSummary ? searchTerm.CredentialRequestStatusTypeForSummary.Data.selectedOptions : null,
                ApplicationTypeIntList: searchTerm.ApplicationType ? searchTerm.ApplicationType.Data.selectedOptions : null,
                CredentialRequestTypeIntList: searchTerm.CredentialRequestType ? searchTerm.CredentialRequestType.Data.selectedOptions : null,
                LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
                // disabled for 1.5 TestSessionLocationIntList: searchTerm.IntendedTestSession ? searchTerm.IntendedTestSession.Data.selectedOptions : null
            };
            return JSON.stringify(json);
        }
        
        vm.searchCallback = function () {
            credentialrequestService.getFluid('summaries', { request: { Take: 500, Skip: null, Filter: credentialRequestSummarySearchTerm(vm.searchTerm()) } })
                .then(function (data) {
                    ko.utils.arrayForEach(data, function (d) {
                        d.Actions = ko.observableArray();
                    });
                    vm.entities(data);
                });
        };

        return vm;
    });