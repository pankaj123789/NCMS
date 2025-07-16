define([
    'services/util',
    'services/person-data-service',
    'plugins/router',
    'modules/shared-filters',
    'services/screen/date-service',
    'services/screen/message-service'
],
    function (util, personDataService, router, sharedFilters, dateService,message) {

    var serverModel = {
        ReleaseDetails: ko.observable(),
        UseEmail: ko.observable(),
        Deceased: ko.observable(),
        AllowVerifyOnline: ko.observable(),
        RevalidationScheme: ko.observable(),
        DoNotInviteToDirectory: ko.observable(),
        ShowPhotoOnline: ko.observable(),
        EthicalCompetency: ko.observable(),
        InterculturalCompetency: ko.observable(),
        KnowledgeTest: ko.observable(),
        GstApplies: ko.observable(),
        Abn: ko.observable(),
        GivenName: ko.observable(),
        Surname: ko.observable(),
        NaatiNumber: ko.observable(),
        PractitionerNumber: ko.observable(),
        PersonId: ko.observable(),
        EntityId: ko.observable(),
        DisplayName: ko.pureComputed(function() {
            return serverModel.GivenName() + ' ' + serverModel.Surname();
        }),
        AccountNumber: ko.observable().extend({ maxLength: 50 }),
        ExaminerTrackingCategory: ko.observable(),
        Gender: ko.observable(),
        BirthCountryId: ko.observable(),
        BirthDate: ko.observable().extend({
            dateLessThan: moment().format('l'),
            dateGreaterThan: moment('1/1/1753').format('l'),
        }),
        DoNotSendCorrespondence: ko.observable(),
        SendCorrespondence: ko.observable(),
        IsRolePlayer : ko.observable(),
        RolePlayLocations: ko.observableArray([]),
        ShowAllowAutoRecertification: ko.observable(),
        AllowAutoRecertification: ko.observable(),
        AccessDisabledByNcms: ko.observable(),
        
        Senior: ko.observable(),
        Rating: ko.observable().extend({ max: 10, min:0, numeric: 1 }),
        MaximumRolePlaySessions: ko.observable().extend({
            max: 40,
            min: 0,
            pattern: {
                params: '^[0-9]*$',
                message: 'Please enter a whole number between 0 and 40.'
            }
        }),


    };
   
        var vm = {
            canActivate: canActivate,
            settings: serverModel,
            save: save,
            dirtyFlag: new ko.DirtyFlag([serverModel], false),
            close: close,
        }

        serverModel.Deceased.subscribe(function (deceased) {
            if (deceased && !preventDeregisterModal) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Person.resources.DeregisterMyNaatiAccount'),
                    content: ko.Localization('Naati.Resources.Person.resources.DeregisterConfirmation')
                        .format('this person')
                })
                    .then(
                        function (answer) {
                            if (answer === 'no') {
                                preventDeregisterModal = true;
                                serverModel.Deceased(false);
                                preventDeregisterModal = false;
                            }
                        });
            } else {
                preventDeregisterModal = false;
            }
        });

    $.extend(vm,
        {
            windowTitle: ko.pureComputed(function () {
                return util.getPersonSubScreenTitle(serverModel.NaatiNumber(),
                    serverModel.GivenName(), serverModel.Surname(),
                    serverModel.PractitionerNumber(),
                    'Settings');
            }),
            abnOptions: {
                value: vm.settings.Abn
            },
            accountNumberOptions: {
                value: vm.settings.AccountNumber
            },
            trackingCategoryOptions: {
                value: vm.settings.ExaminerTrackingCategory
            },

            rolePlayerRatingOptions: {
                value: vm.settings.Rating
            },

            rolePlayerSessionOptions: {
                value: vm.settings.MaximumRolePlaySessions,
                resattr: {
                    title: 'Naati.Resources.Person.resources.MaxRolePlaySessionsToolTip'
                },
                tooltip: {
                    trigger: 'hover',
                    container: 'body'
                }
            },

            genderOptions:
                $.extend({
                    value: vm.settings.Gender,
                    multiple: false,
                    initialise: true,
                    selectedOptions: vm.settings.Gender
                }, sharedFilters.getFilter('Gender').componentOptions),
            rolePlayLocationOptions:
                $.extend({
                    multiple: true,
                    initialise: true,
                    selectedOptions: vm.settings.RolePlayLocations,
                    enableWithPermission: 'RolePlayer.Configure'
                }, sharedFilters.getFilter('PreferredTestLocation').componentOptions),
            countryOfBirthOptions: {
                value: vm.settings.BirthCountryId,
                entity: 'Country',
                valueField: ko.observable('CountryId'),
                textField: ko.observable('Name'),
                dataCallback: function (data) {
                    return data.sort(util.sortBy('Name'));
                },
                multiple: false
            },
            dateOfBirthOptions: {
                value: vm.settings.BirthDate
            }
    });

    function canActivate(naatiNumber) {
        return loadSettings(naatiNumber);
        }


    function loadSettings(naatiNumber) {
        vm.validation = ko.validatedObservable(vm.settings);
        return personDataService.getFluid(naatiNumber + '/settings/').then(function(data) {
            if (data) {

                vm.settings.Gender('');

                preventDeregisterModal = true
                ko.viewmodel.updateFromModel(vm.settings, data);
                if (vm.settings.BirthDate()) {
                    vm.settings.BirthDate(moment(vm.settings.BirthDate()).format(CONST.settings.shortDateDisplayFormat));
                }
                preventDeregisterModal = false

                vm.settings.SendCorrespondence(!vm.settings.DoNotSendCorrespondence());
                vm.dirtyFlag().reset();

                return true;
            }

            return false;
        },
        function() {
            return false;
        });
    }

    function save() {
        if (!vm.validation.isValid()) {
            vm.validation.errors.showAllMessages();
            return;
        }
        var json = ko.toJS(vm.settings);

        json.Gender = vm.settings.Gender != undefined && vm.settings.Gender() && vm.settings.Gender().length > 0?vm.settings.Gender()[0]:undefined; // select-component returns an array
        json.BirthDate = dateService.toPostDate(json.BirthDate);
        json.DoNotSendCorrespondence = !json.SendCorrespondence;

        personDataService.post(json, 'settings').then(function() {
            toastr.success('Settings saved');
            close();
        });
    }

    function close() {
        router.navigateBack();
    }

        vm.rolePlayerRatingOptions.value.extend({ max: 10 });
    return vm;
});
