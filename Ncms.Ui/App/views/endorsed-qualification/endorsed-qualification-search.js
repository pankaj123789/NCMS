define([
    'plugins/router',
    'services/institution-data-service',
    'views/shell',
    'views/endorsed-qualification/endorsed-qualification-add',
    'modules/enums',
    'services/util'
], function (router, institutionService, shell, endorsedQualifiationAdd, enums, util) {
    var endorsedQualificationAddInstance = endorsedQualifiationAdd.getInstance();
    var vm = {
        title: shell.titleWithSmall,
        filters: [
            { id: 'Organisation' },
            { id: 'Location' },
            { id: 'Qualification' },
            { id: 'CredentialType' },
            { id: 'EndorsementDatePeriod' }
        ],
        tableDefinition: {
            id: 'endorsedQualificationTable',
            headerTemplate: 'endorsed-qualification-header-template',
            rowTemplate: 'endorsed-qualification-row-template'
        },
        additionalButtons: [{
            'class': 'btn btn-success',
            click: add,
            icon: 'glyphicon glyphicon-plus',
            resourceName: 'Naati.Resources.Shared.resources.NewEndorsedQualification',
            enableWithPermission: 'EndorsedQualification.Create',
        }],
        searchType: enums.SearchTypes.EndorsedQualification,
        searchTerm: ko.observable({}),
        endorsedQualifications: ko.observableArray([]),
        idSearchOptions: {
            search: function (value) {
                var filter = JSON.stringify({ EndorsedQualificationIdIntList: [value] });
                institutionService.getFluid('searchEndorsedQualifications', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                    .then(function (data) {
                        if (!data || !data.length) {
                            return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Endorsed Qualification', value));
                        }
                        router.navigate('endorsed-qualification/' + value);
                    });
            },
            prefix: '#'
        }
    };

    vm.tableDefinition.dataTable = {
        source: vm.endorsedQualifications,
        order: [
            [1, "asc"],
            [2, "asc"]
        ]
    };


    vm.parsedSearchTerm = ko.pureComputed(function () {
        var searchTerm = vm.searchTerm();

        var parsedSearchTerm = {};

        return parsedSearchTerm;
    });

    function parseSearchTerm(searchTerm) {
        var json = {
            InstitutionNaatiNumberIntList: searchTerm.Organisation ? searchTerm.Organisation.Data.selectedOptions : null,
            LocationString: searchTerm.Location ? searchTerm.Location.Data.value : null,
            QualificationString: searchTerm.Qualification ? searchTerm.Qualification.Data.value : null,
            CredentialTypeIntList: searchTerm.CredentialType ? searchTerm.CredentialType.Data.selectedOptions : null,
            EndorsementFromString: searchTerm.EndorsementDatePeriod ? searchTerm.EndorsementDatePeriod.Data.From : null,
            EndorsementToString: searchTerm.EndorsementDatePeriod ? searchTerm.EndorsementDatePeriod.Data.To : null,
        };

        return JSON.stringify(json);
    }

    vm.searchCallback = function () {
        institutionService.getFluid('searchEndorsedQualifications', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } })
            .then(function (data) {
                ko.utils.arrayForEach(data, function (eq) {
                    var startDate = moment(eq.EndorsementPeriodFrom).format(CONST.settings.shortDateDisplayFormat);
                    var enDate = moment(eq.EndorsementPeriodTo).format(CONST.settings.shortDateDisplayFormat);
                    eq.FormattedPeriod = ko.Localization('Naati.Resources.Institution.resources.EndorsementPeriodRange').format(startDate, enDate);
                });

                vm.endorsedQualifications(data);
            });
    };

    vm.endorsedQualificationOptions = {
        view: 'views/endorsed-qualification/endorsed-qualification-add',
        model: endorsedQualificationAddInstance
    };

    function add() {
        endorsedQualificationAddInstance.show().then(function (data) {
            edit(data.EndorsedQualificationId);
        });
    };

    vm.edit = function (endorsedQualification) {
        edit(endorsedQualification.EndorsedQualificationId);
    };

    vm.ActiveMessage = function (endorsedQualification) {
        if (endorsedQualification.Active) {
            return ko.Localization('Naati.Resources.Shared.resources.Active');
        }

        return ko.Localization('Naati.Resources.Shared.resources.Inactive');
    };

    return vm;

    function edit(id) {
        router.navigate('endorsed-qualification/' + id);
    }
});
