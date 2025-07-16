define([
    'services/util',
    'services/person-data-service',
    'services/screen/date-service',
    'modules/enums',
], function (util, personService, dateService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(credentials) {
        var serverModel = {
            Certification: ko.observable(),
            CertificationPeriod: ko.observable(),
            Id: ko.observable(),
            TerminationDate: ko.observable(),
            StartDate: ko.observable(),
            ExpiryDate: ko.observable(),
            NewTerminationDate: ko.observable(),
            Notes: ko.observable('').extend({ required: true }),
            StatusId: ko.observable()
        };

        var emptyModel = ko.toJS(serverModel);

        serverModel.ComputedExpiryDate = ko.pureComputed(function () {
            var expiryDate = serverModel.ExpiryDate();
            if (!expiryDate) {
                return moment().add({ years: 10 }).format();
            }
            return expiryDate;
        });

        serverModel.NewTerminationDate.extend({
            required: {
                onlyIf: ko.pureComputed(function () {
                    return !serverModel.TerminationDate();
                })
            },
            dateLessThan: ko.pureComputed(function () {
                if (serverModel.Certification()) {
                    return serverModel.CertificationPeriod() ? moment(serverModel.CertificationPeriod().EndDate).format('l') : moment().format('l');
                }
                return moment(serverModel.ComputedExpiryDate()).add({ days: -1 }).format('l');
            }),
            dateGreaterThan: ko.pureComputed(function () {
                return moment(serverModel.StartDate()).format('l');
            }),
        });


        var validation = ko.validatedObservable(serverModel);

        var vm = {
            modalId: util.guid(),
            credentials: credentials,
            credential: serverModel,
            visible: ko.observable(false)
    };

        var defer;
        vm.show = function(credential) {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);
            ko.viewmodel.updateFromModel(serverModel, $.extend(true, {}, emptyModel, credential));

            if (credential.TerminationDate) {
                serverModel.NewTerminationDate(moment(credential.TerminationDate).toDate());
            }
            else {
                serverModel.NewTerminationDate(moment().toDate());
            }

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            defer = Q.defer();

            $('#' + vm.modalId).modal('show');
            vm.visible (true);
            return defer.promise;
        };

        vm.close = function () {
            vm.visible(false);
            $('#' + vm.modalId).modal('hide');
        };

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var request = ko.toJS(serverModel);

            var periodEnd = getNewCertificationPeriodEnd();
            request.CertificationPeriodEndDate = dateService.toPostDate(moment(periodEnd).toDate());
            request.NewTerminationDate = dateService.toPostDate(serverModel.NewTerminationDate());
            personService.post(request, 'credentialterminatedate').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                vm.close();
                defer.resolve(data);
            });
        };

        function getNewCertificationPeriodEnd() {
            if (!serverModel.Certification()) {
                return null;
            }
            var lastTerminationDate = getLastTerminationDateForCredentialsInCertificationPeriod();
            var lastCredential = isLastCredentialNotTerminatedOrExpiredInCertificationPeriod();
            if (lastCredential && lastTerminationDate) {

                return lastTerminationDate;
            }

            return serverModel.CertificationPeriod().EndDate;
        }

        function isLastCredentialNotTerminatedOrExpiredInCertificationPeriod() {
            if (!serverModel.Certification()) {
                return false;
            }

            var isActive = isCredentialActive(serverModel);
            if (!isActive) {
                return false;
            }

            var credentials = vm.credentials();
            for (var i = 0; i < credentials.length; i++) {
                var c = credentials[i];
                if (c.Id() === serverModel.Id()
                    || !c.Certification || (c.Certification instanceof Function && !c.Certification())
                    || c.CertificationPeriod.Id() !== serverModel.CertificationPeriod().Id) {
                    continue;
                }

                if (!c.TerminationDate()) {
                    return false;
                }
            }

            return true;
        };

        function getLastTerminationDateForCredentialsInCertificationPeriod() {

            var terminationDate = undefined;
            var credentials = vm.credentials();
            for (var i = 0; i < credentials.length; i++) {
                var c = credentials[i];
                if (!c.Certification || (c.Certification instanceof Function && !c.Certification())
                    || c.CertificationPeriod.Id() !== serverModel.CertificationPeriod().Id || !c.TerminationDate()) {
                    continue;
                }

                var dateToCompare = c.Id() === serverModel.Id() ?  dateService.toPostDate(serverModel.NewTerminationDate()) : c.TerminationDate();

                terminationDate = dateToCompare > terminationDate
                    ? dateToCompare
                    : (terminationDate || dateToCompare);
            }

            if (credentials.length == 1 && !credentials[0].TerminationDate()) {
                terminationDate = serverModel.NewTerminationDate();
            }
            return terminationDate;

        };

        vm.showWarningMessage = ko.pureComputed(function () {
            return isLastCredentialNotTerminatedOrExpiredInCertificationPeriod() && vm.visible();
        });

        vm.isCredentialTerminated = ko.pureComputed(isCredentialTerminated);

        function isCredentialTerminated() {
            if (!serverModel.TerminationDate()) {
                return false;
            }

            var terminationDate = moment(serverModel.TerminationDate()).toDate();
            return terminationDate <= new Date();
        }

        function isCredentialActive(credential) {
            return credential.StatusId() === enums.CredentialStatus.Active;
        }

        return vm;
    }
});