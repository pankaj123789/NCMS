define([
        'plugins/router',
        'modules/enums',
        'modules/common',
        'services/util',
        'services/application-data-service',
        'services/screen/message-service',
],
    function(router, enums, common, util, applicationService, messageService) {
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
                naatiNumber: ko.observable().extend({ required: true }),
                content: ko.observable(),
                institutionId: ko.observable(false),
            };

            vm.naatiNumberOptions = {
                source: common.functions().naatiNumberSearch,
                multiple: false,
                valueProp: 'NaatiNumber',
                labelProp: 'Name',
                resattr: {
                    placeholder: 'Naati.Resources.Shared.resources.NAATINumber'
                },
                select: function (e, data) {
                    vm.naatiNumber(data.item.data ? data.item.data.NaatiNumber : null);
                },
                disable: ko.observable(false),
                textValue: ko.observable()
            };

            var validation = ko.validatedObservable([vm.newApplicationType, vm.naatiNumber]);
            vm.applicationTypesParams = {
                multiple: false,
                value: vm.newApplicationType,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                options: ko.observableArray()
            };

            vm.show = function (naatiNumber, institutionId) {
                var data = vm.applicationTypesParams.options();
                if (data.length === 1) {
                    vm.newApplicationType(data[0].Id);
                }
                else {
                    vm.newApplicationType(null);
                }

                vm.institutionId(institutionId);
                vm.naatiNumber(naatiNumber);
                vm.naatiNumberOptions.textValue(naatiNumber);
                vm.naatiNumberOptions.disable(naatiNumber);

                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }

                $('#newApplicationModal').modal('show');
            };

            vm.createNewApplication = function() {
                if (!validation.isValid()) {
                    return validation.errors.showAllMessages();
                }
                applicationService.post(
                    {
                        ApplicationTypeId: vm.newApplicationType(),
                        NaatiNumber: vm.naatiNumber()
                    }, 'duplicatedApplication').then(function(isWarned) {
                        if (isWarned) {
                            $.each(vm.applicationTypesParams.options(), function(i, item) {
                                    if (item.Id === vm.newApplicationType()) {
                                        vm.newApplicationTypeText(item.DisplayName);
                                        return;
                                    }
                                });

                            vm.content(ko.Localization('Naati.Resources.Application.resources.DuplicatedApplication')
                                .replace('{0}', vm.newApplicationTypeText()));

                            messageService.confirm({
                                title: ko.Localization('Naati.Resources.Application.resources.NewApplication'),
                                content: vm.content()
                            }).then(function(answer) {
                                if (answer == 'yes') {
                                    createApplication();
                                }
                            });
                        } else {
                            createApplication();
                        }
                    });
            };

            vm.activate = function() {
                common.functions().getLookup('CredentialApplicationTypeBackend').then(function (data) {
                    vm.applicationTypesParams.options(data.sort(util.sortBy('DisplayName')));
                });
            }

            function createApplication() {
                if (!validation.isValid()) {
                    return validation.errors.showAllMessages();
                }
                applicationService.post(
                    {
                        ApplicationInfo: {
                            ApplicationId: 0,
                            ApplicationTypeId: vm.newApplicationType(),
                            NaatiNumber: vm.naatiNumber(),
                            SponsorInstitutionId: vm.institutionId()
                        },
                        Sections: [],
                        CredentialRequests: [],
                        Notes: []
                    },
                    'application').then(
                    function(data) {
                        if (data && data.CredentialApplicationId) {
                            $('#newApplicationModal').modal('hide');
                            var url = 'application/' + data.CredentialApplicationId;
                            router.navigate(url);
                        }
                    });
            }

            return vm;
        }
    });
