define([
    'services/credentialrequest-data-service'
],
    function (credentialrequestService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null,
            };

            $.extend(defaultParams, params);

            var vm = {
                summary: defaultParams.summary,
                applicants: ko.observableArray(),
                tableDefinition: {
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' },
                    columnDefs: [
                        {
                            targets: 0,
                            render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
                        }
                    ],
                    order: [
                        [0, "asc"]
                    ]
                },
            };

            vm.load = function () {
                credentialrequestService.getFluid('existingapplicants', vm.summary.Request()).then(function(data) {
                    var activeApplicants = data.filter(function(a) { return !a.Rejected; } );
                    vm.applicants(activeApplicants);
                });
            };

            return vm;
        }
    });