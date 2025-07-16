define([
    'services/screen/date-service',
    'plugins/router',
    'services/institution-data-service'
],
function(dateService, router, institutionService) {

    var serverModel = {
        NaatiNumber: ko.observable(),
        InstitutionId: ko.observable(),
        EntityId: ko.observable(),
        Name: ko.observable(),
        TrustedPayer: ko.observable()
    }
   
    var vm = {
        canActivate: canActivate,
        settings: serverModel,
        save: save,
        close: close
    };

    $.extend(vm,
        {
            windowTitle: ko.pureComputed(function() {
                return 'Organisation Settings: ' +
                    vm.settings.NaatiNumber() + " - " + vm.settings.Name();
                
            }),
            abnOptions: {
                value: vm.settings.Abn
            },
            accountNumberOptions: {
                value: vm.settings.AccountNumber
            },
        trackingCategoryOptions: {
            value: vm.settings.ExaminerTrackingCategory
        }
    });

    function canActivate(naatiNumber) {
        return loadSettings(naatiNumber);
    }

    function loadSettings(naatiNumber) {
        vm.validation = ko.validatedObservable(vm.settings);
        return institutionService.getFluid(naatiNumber).then(function(data) {
            if (data) {
                ko.viewmodel.updateFromModel(vm.settings, data);

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

        var json = {
            NaatiNumber: vm.settings.NaatiNumber(),
            InstitutionId: vm.settings.InstitutionId(),
            TrustedPayer: vm.settings.TrustedPayer()
        };

        institutionService.post(json, 'settings').then(function() {
            toastr.success('Settings saved');
            close();
        });
    }

    function close() {
        router.navigateBack();
    }

    return vm;
});
