define([
    'plugins/router',
    'modules/enums',
    'services/util',
    'services/person-data-service',
    'services/screen/date-service',
    'services/screen/message-service',
], function (router, enums, util, personService, dateService, message) {
    var serverModel = {
        Id: ko.observable(),
        StartDate: ko.observable(),
        EndDate: ko.observable(),
        Status: ko.observable(),
        OriginalEndDate: ko.observable(),
        Credentials: ko.observableArray(),
        CredentialApplicationId: ko.observable(),
    };

    var emptyModel = ko.toJS(serverModel);

    var vm = {
        tableId: util.guid(),
        naatiNumber: ko.observable(),
        modalId: util.guid(),
        periods: ko.observableArray(),
        credentials: ko.observableArray(),
        period: serverModel,
        checked: ko.observable(),
        checkResult: ko.observable(),
        notes: ko.observable(),
        tableDefinition: {
            dom: "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
            searching: false,
            paging: false,
            oLanguage: { sInfoEmpty: '', sInfo: '' },
            select: {
                style: 'single',
                info: false
            },
            events: {
                select: select,
                deselect: select,
                'user-select': function (e) {
                    if (vm.disableFields()) {
                        e.preventDefault();
                    }
                },
            },
            columnDefs: [
                {
                    orderable: false,
                    className: 'select-checkbox',
                    targets: 0
                },
                {
                    targets: [1, 2, 3],
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                },
                {
                    targets: -1,
                    orderable: false
                }
            ],
            order: [
                [1],
                [2],
                [3]
            ]
        }
    };

    var validation = ko.validatedObservable([vm.notes]);

    vm.notes.extend({
        required: {
            onlyIf: vm.checkResult
        }
    });

    vm.dirtyFlag = new ko.DirtyFlag([serverModel], false);

    vm.canCheck = ko.computed(function () {
        return vm.dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.CertificationPeriod, enums.SecVerb.Update);
    });

    vm.activate = function (id) {
        ko.viewmodel.updateFromModel(serverModel, emptyModel);
        vm.naatiNumber(id);
        vm.checkResult(null);
        vm.notes(null);

        vm.dirtyFlag().reset();

        loadCertifications();
        loadCredentials();

        if (validation.errors) {
            validation.errors.showAllMessages(false);
        }

        return true;
    };

    vm.selectPeriod = function (p) {
        if (!p) {
            p = emptyModel;
        }

        ko.viewmodel.updateFromModel(serverModel, p);
    };

    vm.check = function () {
        personService.post(request(), 'checkcertificationperiod').then(function (data) {
            vm.checkResult(data);
            vm.dirtyFlag().reset();
        });
    };

    vm.calculateStartDate = function () {
        personService.post(vm.period.Id(), 'RecalculateCertificationPeriodStartDate').then(function (data) {
            if (data !== null) {
                vm.period.StartDate(dateService.toUIDate(data));
            }
        });
    };

    vm.disableFields = ko.computed(function () {
        return vm.checkResult() && !vm.checkResult().Errors.length && !vm.checkResult().Warnings.length;
    });

    vm.showSave = ko.computed(function () {
        return vm.checkResult() && !vm.checkResult().Errors.length && !vm.dirtyFlag().isDirty();
    });

    vm.showRecheck = ko.computed(function () {
        return vm.checkResult() && (vm.checkResult().Errors.length || vm.checkResult().Warnings.length);
    });

    vm.save = function () {
        if (!validation.isValid()) {
            validation.errors.showAllMessages();
            return;
        }

        var req = request();
        req.Notes = vm.notes();
        req.NaatiNumber = vm.naatiNumber();

        personService.post(req, 'certificationperiod').then(function (data) {
            toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            vm.activate(vm.naatiNumber());
            close();
        });
    };

    vm.tryClose = function () {
        if (vm.dirtyFlag().isDirty()) {
            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
            })
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            close();
                        }
                    });
        } else {
            close();
        }
    }

    vm.periodCredentials = function (periodId) {
        return ko.utils.arrayFilter(vm.credentials(), function (c) {
            return c.CertificationPeriod && c.CertificationPeriod.Id === periodId;
        });
    };

    vm.selectIfSingle = function () {
        if (vm.periods().length !== 1) {
            return;
        }

        setTimeout(function () {
            var table = $("#" + vm.tableId).DataTable();
            table.rows([0])
                .nodes()
                .to$()
                .addClass('selected');
        }, 500);

        vm.selectPeriod(vm.periods()[0]);
    }

    function request() {
        var result = ko.toJS(serverModel);
        result.OriginalEndDate = dateService.toPostDate(result.OriginalEndDate);
        result.EndDate = dateService.toPostDate(result.EndDate);
        result.StartDate = dateService.toPostDate(result.StartDate);
        result.Credentials = vm.credentials();
        return result;
    }

    function close() {
        router.navigateBack();
    };

    function loadCertifications() {
        var periodsRequest = {
            NaatiNumber: vm.naatiNumber(),
            CertificationPeriodStatus: [
                enums.CertificationPeriodStatus.Expired,
                enums.CertificationPeriodStatus.Current,
                enums.CertificationPeriodStatus.Future
            ]
        };

        personService.getFluid('certificationperiods', periodsRequest).then(function (data) {
            ko.utils.arrayForEach(data, function (d) {
                d.StartDate = moment(d.StartDate).toDate();
                d.EndDate = moment(d.EndDate).toDate();
                d.OriginalEndDate = moment(d.OriginalEndDate).toDate();

                var status = null;
                for (var cp in enums.CertificationPeriodStatus) {
                    if (enums.CertificationPeriodStatus[cp] === d.CertificationPeriodStatus) {
                        status = cp;
                        break;
                    }
                }

                d.Status = ko.Localization('Naati.Resources.Person.resources.' + status);

            });

            vm.periods(data);
        });
    }

    function loadCredentials() {
        personService.getFluid('allcredentialrequests', { naatiNumber: vm.naatiNumber() }).then(vm.credentials);
    }

    function select(e, dt) {
        var indexes = dt.rows('.selected').indexes();

        if (!indexes.length) {
            return vm.selectPeriod(null);
        }

        var period = vm.periods()[indexes[0]];
        vm.selectPeriod(period);
    }

    return vm;
});