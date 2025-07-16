define([
    'plugins/router',
    'services/person-data-service',
    'modules/common',
    'modules/enums',
    'services/util',
    'services/screen/date-service'
], function (router, personService, common, enums, util,dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var serverModel = {
            GivenName: ko.observable().extend({ required: true }),
            OtherNames: ko.observable().extend({ required: false }),
            FamilyName: ko.observable().extend({ required: true }),
            PrimaryEmail: ko.observable().extend({
                required: true,
                pattern:
                    /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            }),
            BirthDate: ko.observable().extend({
                required: true,
                dateLessThan: moment().format('l'),
                dateGreaterThan: moment('1/1/1900').format('l') 
            }),
            CountryOfBirth: ko.observable().extend({required: true}),
            Gender: ko.observable().extend({ required: true })
        };
  
        var cleanModel = ko.toJS(serverModel);
        var vm = {
            modalId: 'createPersonModal',
            details: serverModel       
        };

        $.extend(vm,
            {
                countryOfBirthOptions: {
                    value: vm.details.CountryOfBirth,
                    entity: 'Country',
                    valueField: ko.observable('CountryId'),
                    textField: ko.observable('Name'),
                    dataCallback: function (data) {
                        return data.sort(util.sortBy('Name'));
                    },
                    multiple: false
                },
                genderOptions: {
                    value: vm.details.Gender,
                    multiple: false
                   
                }
            });
        
        var validation = ko.validatedObservable(vm.details);
        vm.dirtyFlag = new ko.DirtyFlag(vm.details, false);
        vm.canSave = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.create = function () {
            if (!validation.isValid()) {
                return validation.errors.showAllMessages();
            }
            vm.details.DateOfBirth = dateService.toPostDate(vm.details.BirthDate());
            personService.post(ko.toJS(vm.details), 'create').then(function (data) {
                $('#' + vm.modalId).modal('hide');
                toastr.success(ko.Localization('Naati.Resources.Person.resources.PersonCreated'));
                router.navigate('person/' + data);
            });
        };

        vm.show = function () {
            ko.viewmodel.updateFromModel(serverModel, cleanModel);
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            $('#' + vm.modalId).modal('show');
        };

        return vm;
    }
});