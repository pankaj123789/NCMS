define([
    'services/person-data-service'
],
    function (personService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                naatiNumber: ko.observable(0)
            };
            $.extend(defaultParams, params);

            var vm = {
                close: close,
                naatiNumber: ko.observable(),
                content: ko.observableArray([]),
                tableDefinition: {
                    headerTemplate: 'person-qrcodes-header-template',
                    rowTemplate: 'person-qrcodes-row-template',
                },
                toggleQrCode: toggleQrCode,

            };
            if (vm.tableDefinition.dataTable == undefined) {
                vm.tableDefinition.dataTable = {
                    source: vm.content,
                    columnDefs: [
                        {
                            targets: [1, 3],
                            render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                        }
                    ],
                    ordering: true,
                    order: [1, 'asc']
                };
            };

            function toggleQrCode(data) {
                personService.getFluid('toggleQrCode', { naatiNumber: vm.naatiNumber(), qrCode: data.QrCode() })
                    .then(function (data) {
                        vm.content.removeAll();
                        data.forEach(function (element) {
                            var qrCode = {
                                PractitionerVerificationUrl: element.PractitionerVerificationUrl,
                                QrCode: element.QrCode,
                                GeneratedDate: element.GeneratedDate,
                                Status: element.InactiveDate ? "InActive" : "Active",
                                ModifiedDate: element.ModifiedDate,
                                Checked: element.InactiveDate ? true : false
                            };
                            vm.content.push(ko.viewmodel.fromModel(qrCode));
                        });
                    });
            }

            vm.load = function (naatiNumber) {
                vm.content.removeAll();
                vm.naatiNumber(naatiNumber);
                personService.getFluid('qrCodes', { naatiNumber: vm.naatiNumber() })
                    .then(function (data) {
                        data.forEach(function (element) {
                            var qrCode = {
                                PractitionerVerificationUrl: element.PractitionerVerificationUrl,
                                QrCode: element.QrCode,
                                GeneratedDate: element.GeneratedDate,
                                Status: element.InactiveDate ? "InActive" : "Active",
                                ModifiedDate: element.ModifiedDate,
                                Checked: element.InactiveDate ? true : false
                            };
                            vm.content.push(ko.viewmodel.fromModel(qrCode));
                        });
                    });
            };

            return vm;
        }
    });