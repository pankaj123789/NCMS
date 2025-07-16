define([
    'modules/enums',
    'services/util',
    'services/person-data-service',
    'views/naati-entity/credential-edit',
    'views/naati-entity/set-credential-terminate-date',
    'views/naati-entity/move-credential'
],
    function (enums, util, personService, credentialEdit, setCredentialTerminateDate, moveCredential) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                isPerson: ko.observable(false),
                naatiNumber: ko.observable(),
                person: {}
            };

            $.extend(defaultParams, params);

            var vm = {
                tableDefinition: {
                    id: 'credentialsTable',
                    headerTemplate: 'person-credentials-header-template',
                    rowTemplate: 'person-credentials-row-template',
                },
                credentialEditOptions: {
                    view: 'views/naati-entity/credential-edit',
                    model: credentialEdit.getInstance()
                },
                isPerson: defaultParams.isPerson,
                person: defaultParams.person,
                naatiNumber: ko.observable(),
                credentialRequests: ko.observableArray([]),
                credentials: ko.observableArray([]),
                applications: applications,
                applicationsModalInstance: defaultParams.credentialApplicationsInstance,
                canManageCredentials: ko.observable(false),
                canTerminateCredentials: ko.observable(false),
                reload: ko.observable()
            };

            vm.setCredentialTerminateDateOptions = {
                view: 'views/naati-entity/set-credential-terminate-date',
                model: setCredentialTerminateDate.getInstance(vm.credentials)
            };

            vm.moveCredentialOptions = {
                view: 'views/naati-entity/move-credential',
                model: moveCredential.getInstance({ person: vm.person })
            };

            vm.tableDefinition.dataTable = {
                source: vm.credentials,
                columnDefs: [
                    {
                        targets: [2, 3, 4],
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                    },
                    {
                        targets: -1,
                        orderable: false
                    }
                ],
                order: [
                    [0, "desc"],
                    [1, "desc"],
                    [2, "desc"]
                ]
            };

            vm.editCredential = function (credential) {
                vm.credentialEditOptions.model.show(credential);
            };

            vm.setTerminationDate = function (credential) {
                vm.setCredentialTerminateDateOptions.model.show(ko.toJS(credential)).then(function () {
                    vm.reload.valueHasMutated();
                });
            };

            vm.moveCredential = function (credential) {
                vm.moveCredentialOptions.model.show(ko.toJS(credential)).then(function () {
                    vm.reload.valueHasMutated();
                });
            };

            function applications(id) {
                vm.applicationsModalInstance.show(id());
            };

            vm.load = function (naatiNumber) {
                vm.naatiNumber(naatiNumber);
                var filter = JSON.stringify({ NaatiNumberIntList: [vm.naatiNumber()] });
                vm.credentials([]);
                personService.getFluid('allcredentialrequests', { naatiNumber: vm.naatiNumber() })
                    .then(function (data) {
                        data.forEach(function (credential) {

                            var certificationPeriod = credential.CertificationPeriod || {};

                            credential.CredentialType = credential.CredentialTypeInternalName;
                            credential.Certification = credential.CertificationPeriod;
                            credential.CertificationPeriodId = (credential.CertificationPeriod || {}).Id;
                            credential.Direction = credential.SkillDisplayName;

                            if (credential.Certification) {
                                credential.ExpiryDate = certificationPeriod.EndDate;
                            }
                            if (vm.person.Deceased()) {
                                credential.StatusId = enums.CredentialStatus.Teminated;
                                credential.ShowInOnlineDirectory = false; 
                                credential.TerminationDate = moment().format();
                            }
                            vm.interpretCredentialRecertificationStatus(credential);
                            credential = util.updateCredentialStatus(credential);
                            vm.credentials.push(ko.viewmodel.fromModel(credential));

                        });
                    });

                currentUser.hasPermission(enums.SecNoun.Credential, enums.SecVerb.Configure).then(vm.canManageCredentials);
                currentUser.hasPermission(enums.SecNoun.Credential, enums.SecVerb.Configure).then(vm.canTerminateCredentials);
            };

            vm.canSetTerminationDate = function (credential) {

                return credential.StatusId() !== enums.CredentialStatus.Expired;
            };

            vm.showEdit = function (credential) {

                return credential.Certification && currentUser.hasPermissionSync(enums.SecNoun.Credential, enums.SecVerb.Configure);
            }

            vm.showMove = function (credential) {

                return credential.Certification;
            }

            vm.interpretCredentialRecertificationStatus = function (credential) {
                switch (credential.RecertificationStatus) {
                    case enums.RecertificationStatusType.EligibleForNew:
                        credential.recertificationStatusClass = 'bg-success';
                        credential.recertificationStatusDisplay = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusEligibleForNew');
                        credential.recertificationStatusTooltip = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusEligibleForNewTooltip');
                        break;
                    case enums.RecertificationStatusType.EligibleForExisting:
                        credential.recertificationStatusClass = 'bg-success';
                        credential.recertificationStatusDisplay = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusEligibleForExisting');
                        credential.recertificationStatusTooltip = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusEligibleForExistingTooltip');
                        break;
                    case enums.RecertificationStatusType.BeingAssessed:
                        credential.recertificationStatusClass = 'bg-darkblue';
                        credential.recertificationStatusDisplay = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusBeingAssessed');
                        credential.recertificationStatusTooltip = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusBeingAssessedTooltip');
                        break;
                    case enums.RecertificationStatusType.Failed:
                        credential.recertificationStatusClass = 'bg-danger';
                        credential.recertificationStatusDisplay = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusFailed');
                        credential.recertificationStatusTooltip = ko.Localization('Naati.Resources.Application.resources.RecertificationStatusFailedTooltip');
                        break;
                    default:
                        credential.recertificationStatusClass = '';
                        credential.recertificationStatusDisplay = '';
                        credential.recertificationStatusTooltip = '';
                        break;
                }
            }

            return vm;
        }
    });
