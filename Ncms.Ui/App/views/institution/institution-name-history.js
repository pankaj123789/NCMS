define([
        'services/screen/date-service',
        'services/institution-data-service',
        'plugins/router'
],
    function (dateService, institutionService, router) {

        var serverModel = {
            AbbreviatedName: ko.observable(),
            NaatiNumber: ko.observable(),
            InstitutionId: ko.observable(),
            EntityId: ko.observable(),
            NameHistory: ko.observableArray(),
            Name: ko.observable()
        }

        var vm = {
            dateService: dateService,
            canActivate: canActivate,
            institution: serverModel,
            close: close
        };

        var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat');

        $.extend(vm,
        {
            windowTitle: ko.pureComputed(
                function () {
                    return 'Organisation Name History: ' +
                        personDetailsHeadingFormat.format(vm.institution.NaatiNumber(),
                            vm.institution.Name());
                })
        });

        function canActivate(naatiNumber) {
            return loadInstitution(naatiNumber);
        }

        function loadInstitution(naatiNumber) {
            return institutionService.getFluid(naatiNumber)
                .then(
                    function (data) {
                        if (data) {
                            ko.viewmodel.updateFromModel(vm.institution, data, true).onComplete(function() {
                                var nameHistory = vm.institution.NameHistory();

                                nameHistory.sort(function (a, b) {
                                    return moment(b.EffectiveDate()).toDate() - moment(a.EffectiveDate()).toDate();
                                });

                                vm.institution.NameHistory(nameHistory);
                            });
                            return true;
                        }
                        return false;
                    },
                    function () {
                        return false;
                    });
        }

        function close() {
            router.navigateBack();
        }

        return vm;
    });
