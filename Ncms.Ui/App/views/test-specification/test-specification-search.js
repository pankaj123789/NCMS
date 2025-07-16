define([
    'plugins/router',
    'services/testspecification-data-service',
    'views/shell',
    'views/test-specification/test-specification-add',
    'services/util'
], function (router, testSpecificationService, shell, testSpecificationAdd, util) {
    var testSpecificationAddInstance = testSpecificationAdd.getInstance();
    var vm = {
        title: shell.titleWithSmall,
        tableDefinition: {
            id: 'testspecificationTable',
            headerTemplate: 'testspecification-header-template',
            rowTemplate: 'testspecification-row-template'
        },
        testSpecifications: ko.observableArray([]),
        idSearchValue: ko.observable()
    };

    vm.tableDefinition.dataTable = {
        source: vm.testSpecifications,
        columnDefs: [
            {
                targets: [1],
                orderData: [0, 0]
            }
        ],
        initComplete: function () {
            $(".dataTables_filter [type=search]")
                .prop('autocomplete', 'new-password');
        }
    };

    vm.activate = function () {
        testSpecificationService.get().then(vm.testSpecifications);
    };

    vm.availableMessage = function (testspecification) {
        if (testspecification.Active) {
            return ko.Localization('Naati.Resources.Shared.resources.Active');
        }

        return ko.Localization('Naati.Resources.Shared.resources.NotAvailable');
    };

    vm.idSearchClick = function () {
        testSpecificationService.getFluid(vm.idSearchValue(), { supressResponseMessages: true })
            .then(function (data) {
                if (!data || !data.length) {
                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Test Specification', vm.idSearchValue()));
                }
                router.navigate('test-specification/' + vm.idSearchValue());
            });
    };

    vm.checkEnterPressed = function (data, e) {
        var keyCode = e.which || e.keyCode;
        if (keyCode == 13) {
            vm.idSearchClick();
            return false;
        }
        return true;
    };

    vm.testSpecificationAddOptions = {
        view: 'views/test-specification/test-specification-add',
        model: testSpecificationAddInstance
    };

    vm.add = function() {
        testSpecificationAddInstance.show().then(function (data) {
            router.navigate('test-specification/' + data.Id);
        });
    };

    return vm;
});
