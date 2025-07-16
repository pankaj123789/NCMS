define([
    'modules/common',
    'modules/enums',
    'services/application-data-service',
],
    function (common, enums, applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var serverModel = {
                NaatiNumber: ko.observable().extend({ required: true }),
                FamilyName: ko.observable(),
                GivenName: ko.observable(),
                OtherNames: ko.observable(),
                PrimaryEmail: ko.observable(),
                PrimaryContactNumber: ko.observable(),
                AddressLine1: ko.observable(),
                AddressLine2: ko.observable(),
                PrimaryAddress: ko.observable(),
                CountryOfBirth: ko.observable(),
                DateOfBirth: ko.observable(),
                Nationality: ko.observable(),
                Gender: ko.observable(),
                EthicalCompetency: ko.observable(),
                InterculturalCompetency: ko.observable(),
                KnowledgeTest: ko.observable()
            }

            serverModel.Address = ko.computed({
                write: function () { },
                read: function () {
                    return '{0}{1} {2}'.format(
                        serverModel.AddressLine1() || '',
                        serverModel.AddressLine1() ? ',' : '',
                        serverModel.AddressLine2() || '');
                }
            });

            var clearModel = ko.toJS(serverModel);

            serverModel.GenderText = ko.pureComputed(function () {
                var gender = null;
                for (var g in enums.Genders) {
                    if (enums.Genders[g] === serverModel.Gender()) {
                        gender = g;
                        break;
                    }
                }
                if (gender == null) return"";
                return ko.Localization('Naati.Resources.Shared.resources.' + gender);
            });

            serverModel.PrimaryContactNumberFormatted = ko.pureComputed(function () {
                return common.functions().formatPhone(serverModel.PrimaryContactNumber());
            });

            var vm = {
                isAdding: ko.observable(false),
                showPersonDetails: ko.observable(false),
                naatiNumber: ko.observable(),
                applicant: serverModel,
                isValid: ko.observable(true),
                dirtyFlag: new ko.DirtyFlag([serverModel], false)
            };

            var validation = ko.validatedObservable(serverModel);

            vm.dateOfBirthTooltip = {
                container: 'body',
                title: function () {
                    if (!serverModel.CountryOfBirth()) {
                        return;
                    }

                    return '{0}: {1}'.format(ko.Localization('Naati.Resources.Application.resources.CountryOfBirth'), serverModel.CountryOfBirth());
                }
            };

            vm.add = function (naatiNumber) {
                bindApplicant(clearModel);
                vm.isAdding(true);

                if (naatiNumber) {
                    vm.naatiNumber(naatiNumber);
                    loadApplicant(naatiNumber);
                }
            };

            vm.load = function (naatiNumber) {
                vm.naatiNumber(naatiNumber);
                loadApplicant(naatiNumber);
                vm.isAdding(false);
            };

            vm.validate = function () {
                if (!vm.dirtyFlag().isDirty()) {
                    return true;
                }

                vm.isValid(validation.isValid());

                if (!vm.isValid()) {
                    validation.errors.showAllMessages();
                }

                return vm.isValid();
            };

            vm.clearValidation = function () {
                vm.isValid(true);
                validation.errors.showAllMessages(false);
                validationAccreditation.errors.showAllMessages(false);
            };

            vm.save = function () {
                var defer = Q.defer();
                var promise = defer.promise;

                if (!vm.dirtyFlag().isDirty()) {
                    defer.resolve('Leave');
                    return promise;
                }

                if (!vm.validate()) {
                    defer.resolve('Invalid');
                    return promise;
                }

                defer.resolve();
                return promise;
            };

            vm.isDirty = function () {
                return vm.dirtyFlag().isDirty();
            };

            vm.refresh = function () {
                loadApplicant(vm.naatiNumber());
            };

            return vm;

            function loadApplicant(naatiNumber) {
                if (!naatiNumber) {
                    return bindApplicant(clearModel);
                }

                applicationService.getFluid('applicant/' + naatiNumber).then(bindApplicant);
            }

            function bindApplicant(applicant) {
                ko.viewmodel.updateFromModel(serverModel, applicant);
                vm.dirtyFlag().reset();
            }
        }
    });
