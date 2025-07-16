define([
        'plugins/router',
        'modules/enums',
        'modules/common',
        'services/application-data-service',
        'services/screen/message-service'
],
    function(router, enums, common, applicationService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                naatiNumber: ko.observable(0)
            };
            $.extend(defaultParams, params);

            var vm = {
                newApplicationType: ko.observable().extend({ required: true }),
                newApplicationTypeText:ko.observable(),
                close: close,
                content: ko.observableArray([])
            };

            vm.show = function (id) {
                applicationService.getFluid('applicationsforcredential/{0}'.format(id)).then(function (data) {
                    vm.content.removeAll();
                    data.forEach(function(credentialApplication) {
                        vm.content.push({
                            Id: credentialApplication.ApplicationId,
                            ApplicationType: credentialApplication.ApplicationTypeName,
                            EnteredDate: formatDate(credentialApplication.EnteredDate)
                        });
                    });
                });

                $('#credentialApplicationsModal').modal('show');
            };

            function formatDate(data) {
                return moment(data).isValid()
                    ? moment(data).format(CONST.settings.shortDateDisplayFormat)
                    : "";
            }

            function close() {
                $('#credentialApplicationsModal').modal('hide');
            }

            return vm;
        }
    });
