define([
    'views/shell',
    'services/screen/message-service',
    'services/finance-data-service',
    'services/setup-data-service'
],
    function (shell, message, financeService, setupService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var serverModel = {
                OfficeId: ko.observable(shell.user().OfficeId).extend({ required: true }),
                PaymentType: ko.observable(1).extend({ required: true }),
                EftMachineId: ko.observable(),
                BankName: ko.observable(),
                ChequeNumber: ko.observable(),
                BSB: ko.observable(),
                Amount: ko.observable(),
                DatePaid: ko.observable(moment().toDate()).extend({ required: true }),
                UserText: ko.observable(),
            };

            var requiredIfEFT = {
                onlyIf: function () {
                    return serverModel.PaymentType() === CONST.paymentTypes.eft || serverModel.PaymentType() === CONST.paymentTypes.amex;
                }
            };

            var chequeValidations = {
                required: {
                    onlyIf: function () {
                        return serverModel.PaymentType() === CONST.paymentTypes.cheque;
                    }
                },
                validation: {
                    validator: function (val) {
                        return !val ? true : val.indexOf('-') === -1;
                    },
                    message: ko.Localization('Naati.Resources.Finance.resources.InvalidDash')
                }
            }

            serverModel.BankName.extend(chequeValidations);
            serverModel.ChequeNumber.extend(chequeValidations);
            serverModel.BSB.extend(chequeValidations);
            serverModel.EftMachineId.extend({ required: requiredIfEFT });

            var defer = null;
            var cleanServerModel = ko.toJS(serverModel);
            var validation = ko.validatedObservable(serverModel);

            shell.user.subscribe(function (newValue) {
                serverModel.OfficeId(newValue.OfficeId);
                cleanServerModel = ko.toJS(serverModel);
            });

            var vm = {
                payment: serverModel,
                balance: ko.observable(),
                paymentTypes: [
                    { value: CONST.paymentTypes.cash, name: ko.Localization('Naati.Resources.Finance.resources.Cash'), reference: 'CASH' },
                    { value: CONST.paymentTypes.cheque, name: ko.Localization('Naati.Resources.Finance.resources.Cheque'), reference: 'CHQ' },
                    { value: CONST.paymentTypes.eft, name: ko.Localization('Naati.Resources.Finance.resources.EFT'), reference: 'EFTPOS' },
                    { value: CONST.paymentTypes.amex, name: ko.Localization('Naati.Resources.Finance.resources.AMEX'), reference: 'AMEX' }
                ],
                isAdding: ko.observable(),
                offices: ko.observableArray(),
                eftMachines: ko.observableArray(),
                paymentAccount: ko.observable(),
                compositionComplete: compositionComplete,
                close: close
            };

            vm.payment.Amount.extend({
                required: true,
                max: vm.balance
            });

            vm.paymentType = ko.computed(function () {
                return ko.utils.arrayFirst(vm.paymentTypes, function (i) {
                    return i.value === serverModel.PaymentType();
                });
            });

            vm.office = ko.computed(function () {
                return ko.utils.arrayFirst(vm.offices(), function (i) {
                    return i.Id === serverModel.OfficeId();
                });
            });

            vm.eftMachine = ko.computed(function () {
                return ko.utils.arrayFirst(vm.eftMachines(), function (i) {
                    return i.Id === serverModel.EftMachineId();
                });
            });

            vm.payment.Reference = ko.computed({
                write: function () { }, //just to prevent ko.viewmodel.updateFromModel errors
                read: function () {
                    if (!vm.office()) {
                        return '';
                    }

                    var paymentType = vm.paymentType();
                    var additionalInfo = '';
                    if (paymentType.value === CONST.paymentTypes.cheque) {
                        if (!serverModel.BankName() || !serverModel.ChequeNumber() || !serverModel.BSB()) {
                            return '';
                        }

                        additionalInfo = '-{0}-{1}-{2}'.format(serverModel.BSB(), serverModel.ChequeNumber(), serverModel.BankName());
                    }
                    else if (paymentType.value === CONST.paymentTypes.eft || paymentType.value === CONST.paymentTypes.amex) {
                        if (!serverModel.EftMachineId() || !vm.eftMachine()) {
                            return '';
                        }

                        additionalInfo = '-{0}'.format(vm.eftMachine().TerminalNumber);
                    }

                    return '{0}-{1}{2}{3}'.format(vm.office().Abbreviation, paymentType.reference, additionalInfo, serverModel.UserText() ? ' - {0}'.format(serverModel.UserText()) : '');
                }
            });

            vm.offices.subscribe(selectOfficeIfSingle);
            vm.eftMachines.subscribe(selectEftMachineIfSingle);
            serverModel.OfficeId.subscribe(changeOffice);

            vm.officeOptions = {
                options: vm.offices,
                value: serverModel.OfficeId,
                multiple: false,
                optionsValue: 'Id',
                optionsText: 'Name',
                enableWithPermission: 'Payment.Create'
            };

            vm.eftMachineOptions = {
                options: vm.eftMachines,
                value: serverModel.EftMachineId,
                multiple: false,
                optionsValue: 'Id',
                optionsText: 'TerminalNumber'
            };

            vm.datePaidOptions = {
                value: serverModel.DatePaid,
                attr: {
                    placeholder: ko.Localization('Naati.Resources.Finance.resources.DatePaid')
                }
            };

            vm.dirtyFlag = new ko.DirtyFlag([vm.payment], false);

            vm.activate = function () {
                changeOffice();
                financeService.getFluid('offices').then(vm.offices);
            };

            vm.add = function (paymentAccount, balance) {
                vm.paymentAccount(paymentAccount);
                cleanServerModel.Amount = balance;
                var promise = vm.edit(cleanServerModel, balance);
                vm.isAdding(true);
                return promise;
            };

            vm.edit = function (payment, balance) {
                vm.balance(balance);

                defer = Q.defer();

                ko.viewmodel.updateFromModel(vm.payment, payment, true).onComplete(function () {
                    vm.dirtyFlag().reset();

                    if (validation.errors) {
                        validation.errors.showAllMessages(false);
                    }
                });

                $("#addPaymentDetailModal").modal("show");
                vm.isAdding(false);

                return defer.promise;
            };

            vm.canSave = ko.computed(function () {
                return validation.isValid() && vm.isAdding() || vm.dirtyFlag().isDirty();
            });

            vm.save = function () {
                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    return;
                }

                var item = ko.toJS(serverModel);
                $.extend(item, { OfficeObject: vm.office(), PaymentTypeObject: vm.paymentType() })
                item.Amount = parseFloat(item.Amount);

                defer.resolve(item);
                close();
            };

            var canClose = false;

            function compositionComplete() {
                $('#addPaymentDetailModal')
                    .on('hide.bs.modal',
                        function (e) {
                            tryClose(e);
                        });
            }

            function tryClose(event) {
                if (event.target.id !== 'addPaymentDetailModal') {
                    return;
                }

                if (canClose) {
                    return;
                }

                event.preventDefault();
                event.stopImmediatePropagation();

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

            function close() {
                canClose = true;
                $('#addPaymentDetailModal').modal('hide');
                canClose = false;
            }

            function changeOffice() {
                vm.eftMachines([]);
                var officeId = serverModel.OfficeId();
                if (officeId) {
                    financeService.getFluid('eftmachines/' + officeId).then(vm.eftMachines);
                }
            }

            function selectOfficeIfSingle() {
                var offices = vm.offices();
                if (offices.length === 1) {
                    cleanServerModel.OfficeId = offices[0].EntityId;
                    serverModel.OfficeId(offices[0].EntityId);
                }
            }

            function selectEftMachineIfSingle() {
                var eftMachines = vm.eftMachines();
                if (eftMachines.length === 1) {
                    cleanServerModel.EftMachineId = eftMachines[0].Id;
                    serverModel.EftMachineId(eftMachines[0].Id);
                }
            }

            return vm;
        }
    });
