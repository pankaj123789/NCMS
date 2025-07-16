define([
    'modules/enums',
    'services/util',
    'services/person-data-service',
  'services/screen/date-service',
], function (enums, util, personService, dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance(person) {
        var serverModel = {
            Id: ko.observable(),
            StartDate: ko.observable(),
            EndDate: ko.observable(),
            OriginalEndDate: ko.observable(),
            NewEndDate: ko.observable(new Date()).extend({
                required: true
            }),
            Notes: ko.observable().extend({
                required: true
            }),
        };

        serverModel.NewEndDate.extend({
            dateLessThan: ko.pureComputed(function() {
                return moment(serverModel.EndDate()).add({ months: 12 }).format('l');
            }),
        });

        serverModel.NewEndDate.extend({
            certificationPeriod: ko.pureComputed(function () {
                var endDate = moment(serverModel.EndDate()).add({ months: -12 }).format('l');
                var originalEndDate = moment(serverModel.OriginalEndDate()).format('l');
                return { originalEndDate: originalEndDate, endDate: endDate };
            })
        });

        var emptyModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        serverModel.Status = ko.pureComputed(function () {
            return status(serverModel.StartDate(), serverModel.EndDate());
        });

        var vm = {
            person: person,
            modalId: util.guid(),
            periods: ko.observableArray(),
            period: serverModel
        };

        var defer;
        vm.show = function () {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            defer = Q.defer();

            $('#' + vm.modalId).modal('show');

            var periodsRequest = {
                PersonId: vm.person.PersonId(),
                CertificationPeriodStatus: [
                    enums.CertificationPeriodStatus.Expired,
                    enums.CertificationPeriodStatus.Current,
                    enums.CertificationPeriodStatus.Future
                ]
            };

            personService.getFluid('certificationperiods', periodsRequest).then(function (data) {
                vm.periods(data);
                if (data.length === 1) {
                    vm.selectPeriod(data[0]);
                }
            });

            return defer.promise;
        };

        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
        };

        vm.selectPeriod = function (p) {
            ko.viewmodel.updateFromModel(serverModel, p);
            serverModel.NewEndDate(moment(p.EndDate).toDate());
        };

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var request = ko.toJS(serverModel);
            request.NewEndDate = dateService.toPostDate(request.NewEndDate);

            personService.post(request, 'extendcertification').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                vm.close();
                defer.resolve(data);
            });
        };

        vm.status = status;

        function status(startDate, endDate) {
            if (moment(endDate).toDate() < new Date()) {
                return ko.Localization('Naati.Resources.Person.resources.Expired');
            }
            if (moment(startDate).toDate() > new Date()) {
                return ko.Localization('Naati.Resources.Person.resources.Future');
            }

            return ko.Localization('Naati.Resources.Person.resources.Current');
        };

        return vm;
    }
});