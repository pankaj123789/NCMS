define([
    'plugins/router',
    'views/shell',
    'modules/enums',
    'views/system/language-create',
    'services/setup-data-service'
], function (router, shell, enums, languageCreate, setupService) {
    var searchType = enums.SearchTypes.Language;
    var searchTerm = ko.observable({});
    var tableId = 'languageTable';
    var tableDefinition = {
        id: tableId,
        headerTemplate: 'languages-header-template',
        rowTemplate: 'languages-row-template'
    };

    var vm = {
        searchOptions: {
            name: 'search-component',
            params: {
                title: shell.titleWithSmall,
                filters: [
                    { id: 'Name' },
                    { id: 'LanguageGroup' },
                ],
                searchType: searchType,
                searchTerm: searchTerm,
                searchCallback: searchCallback,
                tableDefinition: tableDefinition,
                additionalButtons: [{
                    'class': 'btn btn-success',
                    icon: 'glyphicon glyphicon-plus',
                    resourceName: 'Naati.Resources.Language.resources.NewLanguage',
                    click: create,
                    disable: !currentUser.hasPermissionSync(enums.SecNoun.Language, enums.SecVerb.Create) 
                }],
                idSearchOptions: {
                    search: function (value) {
                        var filter = JSON.stringify({ LanguageIdIntList: [value] });
                        setupService.getFluid('language', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                            .then(function (data) {
                                if (!data || !data.length) {
                                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Language', value));
                                }
                                router.navigate('system/language/' + value);
                            });
                    }
                }
            }
        },
        languages: ko.observableArray([])
    };

    var languageCreateInstance = languageCreate.getInstance();

    vm.languageCreateOptions = {
        view: 'views/system/language-create.html',
        model: languageCreateInstance
    };

    tableDefinition.dataTable = {
        source: vm.languages,
        order: [
            [0, "asc"],
        ]
    };

    function create() {
        languageCreateInstance.show().then(function (data) {
            languageCreateInstance.hide();
            router.navigate('system/language/' + data.Id);
        });
    }

    function parseSearchTerm(searchTerm) {
        var json = {
            NameString: searchTerm.Name ? searchTerm.Name.Data.value : null,
            GroupIntList: searchTerm.LanguageGroup ? searchTerm.LanguageGroup.Data.selectedOptions : null,
        };

        return JSON.stringify(json);
    }

    function searchCallback() {
        setupService.getFluid('language', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(searchTerm()) } }).then(vm.languages);
    };

    vm.activate = function () {
        $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
    };

    return vm;
});