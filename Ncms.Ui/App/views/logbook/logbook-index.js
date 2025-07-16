define([
    'services/person-data-service',
    'plugins/router'
],
    function (personDataService, router) {

        var serverModel = {
            GivenName: ko.observable(),
            Surname: ko.observable(),
            NaatiNumber: ko.observable(),
            DisplayName: ko.pureComputed(function() {
                return serverModel.GivenName() + ' ' + serverModel.Surname();
            }),
            PractitionerNumber: ko.observable(),
            Credentials: ko.observableArray(),
        };

        var vm = {
            activate: activate,
            logbook: serverModel,
            close: close,
            showAll: ko.observable(false),
            naatiNumber: ko.observable()
        };
        vm.showAll.subscribe(function() {
            personDataService.getFluid(vm.naatiNumber() + '/logbook/' + vm.showAll()).then(function(data) {
                if (data) {
                    ko.viewmodel.updateFromModel(vm.logbook, data);
                }
            });
        });
        var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonLogbookHeadingFormat');

        $.extend(vm,
            {
                windowTitle: ko.pureComputed(function () {
                    return personDetailsHeadingFormat.format(vm.logbook.NaatiNumber(),
                        vm.logbook.DisplayName(), vm.logbook.PractitionerNumber(),"Logbook");
                })
            });

        function activate(naatiNumber) {
            vm.showAll(false);

            vm.naatiNumber(naatiNumber);
            loadLogbook(naatiNumber);
        }

        function loadLogbook(naatiNumber) {
            vm.validation = ko.validatedObservable(vm.logbook);
            return personDataService.getFluid(naatiNumber + '/logbook/' + vm.showAll()).then(function (data) {
                if (data) {
                    ko.viewmodel.updateFromModel(vm.logbook, data);

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
