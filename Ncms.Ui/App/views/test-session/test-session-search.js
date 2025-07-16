define([
    'plugins/router',
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/util',
    'services/entity-data-service',
    'services/testsession-data-service',
    'services/screen/date-service'
], function (router, shell, common, enums, util, entityService, testSessionService, dateService) {
    var searchType = enums.SearchTypes.TestSession;
    var tableId = 'testsessionTable';

    var vm = {
        title: shell.titleWithSmall,
        filters: [
            { id: 'TestDateFromAndTo' },
            { id: 'CredentialTestSession' },
            { id: 'TestLocation' },
            { id: 'SessionName' },
            { id: 'IncludeCompletedSessions' },
            { id: 'TestSpecification' },
            { id: 'NewCandidatesOnly' },
            { id: 'IsActive' }
        ],
        tableDefinition: {
            id: tableId,
            headerTemplate: 'testsession-header-template',
            rowTemplate: 'testsession-row-template'
        },
        searchType: searchType,
        searchTerm: ko.observable({}),
        testsessions: ko.observableArray([]),
        selectedTestIndices: ko.observableArray([]),
        additionalButtons: [{
            'class': 'btn btn-success',
            click: createTestSession,
            icon: 'glyphicon glyphicon-plus',
            resourceName: 'Naati.Resources.TestSession.resources.NewTestSession',
            showWithPermission: 'TestSession.Create'
        },
        {
            'class': 'btn btn-success',
            click: assignTestMaterial,
            icon: 'fa fa-check',
            resourceName: 'Naati.Resources.TestSession.resources.AssignTestMaterial',
            disable: ko.pureComputed(function () { return !vm.selectedTestIndices().length; })
            }],
        testSessionsCookieId: ko.observable(),
        idSearchOptions: {
            search: function (value) {
                var filter = JSON.stringify({ TestSessionIntList: [value] });
                testSessionService.get({ request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                    .then(function (data) {
                        if (!data || !data.length) {
                            return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Test Session', value));
                        }
                        router.navigate('test-session/' + value);
                    });
            },
            prefix: 'TS'
        }
    };

    vm.selectedTestSessions = ko.pureComputed(function () {
        return ko.utils.arrayMap($('#' + tableId).DataTable().rows(vm.selectedTestIndices()).data(), function (data) {
            var testSessionId = data[1];
            return testSessionId;
        });
    });

    var commonFunctions = common.functions();
    var topics = common.topics();

    vm.tableDefinition.dataTable = {
        source: vm.testsessions,
        columnDefs: [
            {
                targets: [2],
                orderData: [1, 1]
            },
            {
                targets: 0,
                render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
            }
        ],
        select: {
            style: 'multi+shift'
        },
        events: {
            select: function (e, dt, type, indices) {
                vm.selectedTestIndices(vm.selectedTestIndices().concat(ko.utils.arrayFilter(indices, function (index) {
                    return vm.selectedTestIndices.indexOf(index) === -1;
                })));
            },
            deselect: function (e, dt, type, indices) {
                vm.selectedTestIndices.remove(function (index) {
                    return indices.indexOf(index) !== -1;
                });
            }
        },
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
                    text: '<i class="fa fa-check-square"></i><span> ' + ko.Localization('Naati.Resources.Shared.resources.SelectAll') + '</span>',
                    className: 'btn btn-default ml-1',
                    enabled: ko.pure,
                    action: selectAll
                },
                {
                    text: '<i class="far fa-square"></i><span> ' + ko.Localization('Naati.Resources.Shared.resources.DeselectAll') + '</span>',
                    className: 'btn btn-default ml-1',
                    enabled: true,
                    action: deselectAll
                },

            ]
        },
    };

    function parseSearchTerm(searchTerm) {
        var json = {
            TestDateFromString: searchTerm.TestDateFromAndTo ? searchTerm.TestDateFromAndTo.Data.From : null,
            TestDateToString: searchTerm.TestDateFromAndTo ? searchTerm.TestDateFromAndTo.Data.To : null,
            CredentialIntList: searchTerm.CredentialTestSession ? searchTerm.CredentialTestSession.Data.Credential.Data.selectedOptions : null,
            CredentialSkillIntList: searchTerm.CredentialTestSession ? searchTerm.CredentialTestSession.Data.CredentialSkill.Data.selectedOptions : null,
            TestSpecificationIntList: searchTerm.TestSpecification ? searchTerm.TestSpecification.Data.selectedOptions : null,
            TestLocationIntList: searchTerm.TestLocation ? searchTerm.TestLocation.Data.PreferredTestLocation.Data.selectedOptions : null,
            TestVenueIntList: searchTerm.TestLocation ? searchTerm.TestLocation.Data.TestVenue.Data.selectedOptions : null,
            SessionNameString: searchTerm.SessionName ? searchTerm.SessionName.Data.value : null,
            IncludeCompletedSessionsBoolean: searchTerm.IncludeCompletedSessions ? searchTerm.IncludeCompletedSessions.Data.checked : null,
            NewCandidatesOnlyBoolean: searchTerm.NewCandidatesOnly ? searchTerm.NewCandidatesOnly.Data.checked : null,
            IsActiveBoolean: searchTerm.IsActive ? searchTerm.IsActive.Data.checked : null
        };

        return JSON.stringify(json);
    }

    vm.searchCallback = function () {

        vm.selectedTestIndices([]);

        testSessionService.get({ request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } }).then(function (data) {

            var arrayList = null;
            
            ko.utils.arrayForEach(data, function (item) {
                item.StrTestSessionId = 'TS' + item.TestSessionId;
                item.StrTestLocation = item.TestLocationName + ', ' + item.TestLocationStateName;
            });

            arrayList = data;
            vm.testsessions(arrayList);
        });
    };
   
    vm.activate = function () {
        $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
        if (vm.testSessionsCookieId()) {
            Cookies.remove(vm.testSessionsCookieId());
        }
    };

    function createTestSession() {
        router.navigate('test-session-wizard');
    }

	function assignTestMaterial() {
		var ids = ko.utils.arrayMap(vm.selectedTestSessions(), function (item) {
			return item;
		});


        var guid = util.guid();
        vm.testSessionsCookieId(guid);
	    var oneMinute = new Date(new Date().getTime() + 1 * 60 * 1000);

        Cookies.set(vm.testSessionsCookieId(), ids, { expires: oneMinute});
        router.navigate('test-material-wizard/' + vm.testSessionsCookieId());
    }

    function selectAll() {
        $('#' + tableId).DataTable().rows().select();
    }
    function deselectAll() {
        $('#' + tableId).DataTable().rows().deselect();
    }



    return vm;
});
