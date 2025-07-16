define([
    'services/application-data-service'
],
    function (applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                isPerson: ko.observable(false),
                naatiNumber: ko.observable()
            };

            $.extend(defaultParams, params);

            var vm = {
                tableDefinition: {
                    id: 'personApplicationTable',
                    headerTemplate: 'person-application-header-template',
                    rowTemplate: 'person-application-row-template'
                },
                isPerson: defaultParams.isPerson,
                naatiNumber: ko.observable(),
                applications: ko.observableArray([])
            };

            vm.showDetails = function (i, e) {
                var target = $(e.target);
                var tr = target.closest('tr');
                var dt = tr.closest('#' + vm.tableDefinition.id).DataTable();
                var row = dt.row(tr);

                if (row.child.isShown()) {
                    target.removeClass('fa-chevron-down').addClass('fa-chevron-right');
                    tr.removeClass('details');
                    row.child.hide();
                }
                else {
                    target.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                    tr.addClass('details');
                    showLines(i, row);
                }
            };

            vm.tableDefinition.dataTable = {
                source: vm.applications,
                columnDefs: [
                    {
                        targets: 3,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                    },
                    {
                        targets: -1,
                        orderable: false
                    },
                ],
                order: [
                    [3, "desc"],
                    [1, "asc"]
                ]
            };

            vm.load = function (naatiNumber) {
                vm.naatiNumber(naatiNumber);
                var filter = JSON.stringify({ NaatiNumberIntList: [vm.naatiNumber()] });
                applicationService.post({
                        Skip: null,
                        Take: 500,
                        Filter: filter
                    } , 'search')
                    .then(function (data) {
                        vm.applications(data);
                    });
                return true;
            };

            vm.newApplication = function () {};

            function showLines(application, row) {
                applicationService.getFluid('{0}/credentialrequests'.format(application.Id)).then(function (data) {
                    var detailsTemplate = ko.generateTemplate('person-application-detail-template', {
                        tableDefinition: {
                            searching: false,
                            paging: false,
                            order: [
                                [0, "desc"],
                                [1, "desc"]
                            ],
                            oLanguage: { sInfoEmpty: '', sInfo: '' }
                        },
                        data: data
                    });
                    row.child(detailsTemplate).show();
                });
            }

            return vm;
        }
    });
