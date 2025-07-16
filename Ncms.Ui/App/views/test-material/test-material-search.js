define([
    'plugins/router',
    'services/test-material-data-service',
    'views/shell',
    'views/test-material/test-material-add',
    'modules/enums',
    'services/util'
], function (router, testMaterialService, shell, testMaterialAdd, enums, util) {
    var testMaterialAddInstance = testMaterialAdd.getInstance();
    var vm = {
        isWriter: ko.observable(false),
        title: shell.titleWithSmall,
        filters: [
            { id: 'MaterialId'},
            { id: 'Language' },
            { id: 'CredentialType' },
            { id: 'TestTaskType' },
            { id: 'Availability' },
            { id: 'Title' },
            { id: 'TestMaterialStatus' },
            { id: 'TestSpecification' },
            { id: 'TestMaterialType' },
            { id: 'TestMaterialDomain' },
        ],
        tableDefinition: {
            id: 'testMaterialTable',
            headerTemplate: 'testmaterialsearch-header-template',
            rowTemplate: 'testmaterialsearch-row-template'
        },
        additionalButtons: [{
            'class': 'btn btn-success',
            click: add,
            icon: 'glyphicon glyphicon-plus',
            resourceName: 'Naati.Resources.TestMaterial.resources.NewTestMaterial',
            enableWithPermission: 'TestMaterial.Create'
        }],
        searchType: enums.SearchTypes.TestMaterial,
        searchTerm: ko.observable({}),
        testMaterials: ko.observableArray([]),
        idSearchOptions: {
            search: function (value) {
                var filter = JSON.stringify({ MaterialIdIntList: [value] });
                testMaterialService.getFluid('searchTestMaterials', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                    .then(function (data) {
                        if (!data || !data.length) {
                            return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Test Material', value));
                        }
                        router.navigate('test-material/' + value);
                    });
            },
            prefix: '#'
        }
    };

    vm.tableDefinition.dataTable = {
        source: vm.testMaterials,
        columnDefs: [
            { targets: -1, orderable: false },
            { orderData: 1, targets: 0 },
            { targets: 1, visible: false }
        ]
    };


    vm.parsedSearchTerm = ko.pureComputed(function () {
        var searchTerm = vm.searchTerm();

        var parsedSearchTerm = {};

        if (searchTerm.EmailTemplateName) {
            //$.extend(parsedSearchTerm, {
            //    NaatiNumber: searchTerm.EmailTemplateName.Data.EmailTemplateName
            //});
        }

        return parsedSearchTerm;
    });

    function parseSearchTerm(searchTerm) {
        var json = {
            MaterialIdIntList: searchTerm.MaterialId ? searchTerm.MaterialId.Data.valueAsArray : null,
            LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
            CredentialTypeIntList: searchTerm.CredentialType ? searchTerm.CredentialType.Data.selectedOptions : null,
            TaskTypeIntList: searchTerm.TestTaskType ? searchTerm.TestTaskType.Data.selectedOptions : null,
            AvailabilityBoolean: searchTerm.Availability ? searchTerm.Availability.Data.checked : null,
            TitleString: searchTerm.Title ? searchTerm.Title.Data.value : null,
            TestMaterialStatusIntList: searchTerm.TestMaterialStatus ? searchTerm.TestMaterialStatus.Data.selectedOptions : null,
            TestMaterialTypeIntList: searchTerm.TestMaterialType ? searchTerm.TestMaterialType.Data.selectedOptions : null,
            TestSpecificationIntList: searchTerm.TestSpecification ? searchTerm.TestSpecification.Data.selectedOptions : null,
            TestMaterialDomainIntList: searchTerm.TestMaterialDomain ? searchTerm.TestMaterialDomain.Data.selectedOptions : null,
        };

        return JSON.stringify(json);
    }

    vm.activate = function () {
        currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Update).then(vm.isWriter);
    }

    vm.searchCallback = function () {
        testMaterialService.getFluid('searchTestMaterials', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } })
            .then(function (data) {
                ko.utils.arrayForEach(data, function (tm) {

                    tm.BadgeTooltip = util.getTestMaterialStatusToolTip(tm.StatusId, tm.LastUsedDate);
                    tm.BadgeColor = util.getTestMaterialStatusColor(tm.StatusId);
                    tm.BadgeText = util.getTestMaterialStatusText(tm.StatusId);
                });

                vm.testMaterials(data);
            });
    };

    vm.testMaterialAddOptions = {
        view: 'views/test-material/test-material-add',
        model: testMaterialAddInstance
    };
  

    function add() {
        testMaterialAddInstance.show().then(function (data) {
            edit(data.Id);
        });
    };

    vm.edit = function (testMaterial) {
        edit(testMaterial.Id);
    };

    vm.availableMessage = function (testMaterial) {
        if (testMaterial.Available) {
            return ko.Localization('Naati.Resources.TestMaterial.resources.Available');
        }

        return ko.Localization('Naati.Resources.TestMaterial.resources.NotAvailable');
    };

    return vm;

    function edit(id) {
        router.navigate('test-material/' + id);
    }
});
