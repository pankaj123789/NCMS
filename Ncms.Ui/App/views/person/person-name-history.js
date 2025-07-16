define([
        'services/util',
        'services/screen/date-service',
        'services/person-data-service',
        'plugins/router'
],
    function (util, dateService, personDataService, router) {

        var serverModel = {
            GivenName: ko.observable(),
            Surname: ko.observable(),
            BirthDate: ko.observable(),
            NaatiNumber: ko.observable(),
            PractitionerNumber: ko.observable(),
            PersonId: ko.observable(),
            EntityId: ko.observable(),
            NameHistory: ko.observableArray(),
            DisplayName: ko.pureComputed(function() {
                return serverModel.GivenName() + ' ' + serverModel.Surname();
            })
        };

        var vm = {
            dateService: dateService,
            canActivate: canActivate,
            person: serverModel,
            close: close
        };

        $.extend(vm,
        {
            windowTitle: ko.pureComputed(
                function () {
                    return util.getPersonSubScreenTitle(serverModel.NaatiNumber(),
                        serverModel.GivenName(), serverModel.Surname(),
                        serverModel.PractitionerNumber(),
                        'Name History');
                })
        });

        function canActivate(naatiNumber) {
            return loadPerson(naatiNumber);
        }

        function loadPerson(naatiNumber) {
            return personDataService.getFluid(naatiNumber)
                .then(
                    function (data) {
                        if (data) {
                            ko.viewmodel.updateFromModel(vm.person, data, true).onComplete(function () {
                                var nameHistory = vm.person.NameHistory();

                                nameHistory.sort(function (a, b) {
                                    return moment(b.EffectiveDate()).toDate() - moment(a.EffectiveDate()).toDate();
                                });

                                vm.person.NameHistory(nameHistory);
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
