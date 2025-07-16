define([
    'services/util',
    'services/person-data-service',
], function (util,personService) {

    return {
        getInstance: getInstance
    };

    function getInstance(person) {

        var self = this;

        var vm = {
            person: person,
            modalId: util.guid(),
            qrCodeLog: ko.observableArray([]),
            tableDefinition: {
                headerTemplate: 'auditLog-header-template',
                rowTemplate: 'auditLog-row-template'
            }
        };

        self.qrCodeLog = vm.qrCodeLog;

        vm.tableDefinition.dataTable = {
            source: self.qrCodeLog,
            searching: false,
            paging: false,
            info: false,
            columnDefs: [
                {
                    targets: [2,3],
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ]
        };

        vm.show = function () {

            defer = Q.defer();

            $('#' + vm.modalId).modal('show');

            personService.getFluid('qrCodeSummary', { PersonId: vm.person.PersonId() }).then(function (data) {
                vm.qrCodeLog(data);

            });

            return defer.promise;
        };

        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
        };
        return vm;
    }
});