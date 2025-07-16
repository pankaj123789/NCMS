define([
    'views/shell',
    'modules/common',
    'services/payroll-data-service',
    'services/finance-data-service',
    'services/screen/message-service',
    'plugins/router',
    'views/payroll/payroll-functions',
    'modules/enums'
], function (shell, common, payrollService, financeService, message, router, payrollFunctions, enums) {
    var commonFunctions = common.functions();

    var vm = {
        examiners: ko.observableArray([]),
        title: shell.titleWithSmall,
        payrollId: null,
        hasFinalisePermission: ko.observable(false)
    };

    vm.activate = function () {
        vm.examiners([]);

        currentUser.hasPermission(enums.SecNoun.PayRun, enums.SecVerb.Finalise).then(vm.hasFinalisePermission);

        return payrollService.getFluid('getMarkingsForInProgressPayroll').then(function (data) {
            if (data) {
                vm.payrollId = data.PayrollId;
                var examiners = payrollFunctions.parseResults(data.Markings);

                ko.utils.arrayForEach(examiners, function (examiner) {
                    examiner.dirtyFlag = new ko.DirtyFlag(examiner.AccountingReference);
                });

                vm.examiners(examiners);
            }

            return true;
        });
    };

    vm.getFirstReceivedDateInfo = function (examiner) {
        return ko.Localization('Naati.Resources.Payroll.resources.OldestReceivedDay').format(examiner.OldestReceivedDays());
    };

    vm.formatExaminerCost = function (examiner) {
        return ko.Localization('Naati.Resources.Payroll.resources.CostFormat').format(vm.parseCost(examiner.TotalCost()));
    };

    vm.toggleExpanded = function (examiner) {
        examiner.expanded(!examiner.expanded());
    };

    vm.parseCost = function (cost) {
        return '$ ' + parseFloat(cost).toFixed(2);
    };

    vm.totalCost = function (marking) {
        return vm.parseCost(marking.ExaminerCost() * marking.Count());
    };

    vm.expandAll = function () {
        $.each(vm.examiners(), function (i, examiner) {
            if (!examiner.expanded()) {
                $('a', '#' + examiner.headId).click();
            }
        });
    };

    vm.collapseAll = function () {
        $.each(vm.examiners(), function (i, examiner) {
            if (examiner.expanded()) {
                $('a', '#' + examiner.headId).click();
            }
        });
    };

    vm.removeExaminerFromPayRun = function (examiner) {
        message.remove({
            yes: '<span class="glyphicon glyphicon-trash"></span><span>' +
                ko.Localization('Naati.Resources.Shared.resources.Yes') +
                '</span>',
            no: ko.Localization('Naati.Resources.Shared.resources.No'),
            content: ko.Localization('Naati.Resources.Payroll.resources.DeleteFromPayRunConfirmation')
        }).then(function (answer) {
            if (answer === 'yes') {
                payrollService.post({
                    PayrollId: vm.payrollId,
                    JobExaminerIds: examiner.JobExaminerIds()
                },
                'removeExaminer').then(function () {
                    toastr.success(ko.Localization('Naati.Resources.Payroll.resources.DeletedFromPayRunSuccessfully'));
                    vm.examiners.remove(examiner);
                });
            }
        });
    };

    vm.canFinalise = ko.pureComputed(function () {
        var result = vm.examiners().length;

        $.each(vm.examiners(), function (i, examiner) {
            result = result && examiner.AccountingReference() && !examiner.dirtyFlag().isDirty();
        });

        result = result && vm.hasFinalisePermission();
        return result;
    });

    vm.finaliseButtonHintResource = ko.computed(function () {
        if (vm.canFinalise()) {
            return ko.Localization('Naati.Resources.Payroll.resources.CanFinaliseHint');
        }

        return ko.Localization('Naati.Resources.Payroll.resources.CannotFinaliseHint');
    });

    vm.finalise = function () {
        message.confirm({
            title: ko.Localization('Naati.Resources.Payroll.resources.FinalisePayRun'),
            content: ko.Localization('Naati.Resources.Payroll.resources.FinalisePayRunConfirmation')
        }).then(function (answer) {
            if (answer === 'yes') {
                payrollService.post(vm.payrollId, 'finalisePayroll').then(function () {
                    toastr.success(ko.Localization('Naati.Resources.Payroll.resources.PayRunFinalised'));

                    // remove() and removeAll() both failed to remove all rows from the view. pop() seems to work.
                    for (var i = 0; i < vm.examiners().length;) {
                        vm.examiners.pop();
                    }

                    router.navigate('#payroll/previous');
                });
            }
        });
    };

    vm.save = function (examiner) {
        var request = {
            PayrollId: vm.payrollId,
            JobExaminerIds: examiner.JobExaminerIds(),
            AccountingReference: examiner.AccountingReference()
        };

        payrollService.post(request, 'saveInvoiceNumber').then(function () {
            examiner.dirtyFlag().reset();
            toastr.success(ko.Localization('Naati.Resources.Payroll.resources.InvoiceNumberSaved'));
        }, vm.activate); // If the save fails, refresh the view
    };

    function performCreateInvoice(examiner, invoiceNumber) {
        commonFunctions.performOperation({
            service: financeService,
            data: {
                InvoiceType: 3, // Bill
                EntityId: examiner.ExaminerEntityId(),
                InvoiceNumber: invoiceNumber,
                LineItems: ko.utils.arrayMap(examiner.Markings(), function (marking) {
                    var attendanceIds = ko.utils.arrayMap(marking.Tests(), function (test) {
                        return test.TestAttendanceId();
                    });

                    return {
                        EntityId: examiner.ExaminerEntityId(),
                        ProductSpecificationId: marking.ProductSpecificationId(),
                        Description: marking.ProductSpecificationCode() + '\n' + attendanceIds.join(', '),
                        Quantity: marking.Tests().length,
                        // The GST for bills is calculated in Wiise
                        // Always create the invoice with the exclusive cost
                        IncGstCostPerUnit: marking.ExaminerCost(),
                        ExGstCostPerUnit: marking.ExaminerCost(),
                        GstApplies: false
                    };
                })
            },
            action: 'invoice',
            retryAction: 'retryInvoice',
            cancelAction: 'cancel',
            errorMessage: ko.Localization('Naati.Resources.Finance.resources.ErrorOnInvoiceCreation')
        }).then(function (data) {
            toastr.success(ko.Localization('Naati.Resources.Payroll.resources.InvoiceCreated'));
            examiner.AccountingReference(data.InvoiceNumber);
            vm.save(examiner);
        });
    }

    vm.createInvoice = function(examiner) {
        var invoiceNumber = 'TM-' + examiner.ExaminerAccountNumber() + '-' + moment().format('YYYYMMDD');

        financeService.getFluid('purchaseinvoice',
            {
                InvoiceNumber: invoiceNumber,
                IncludeFullPaymentInfo: false,
                ExcludePayables: false,
                ExcludeCreditNotes: true
            })
            .then(function (data) {
                if (data.length && data.length > 0) {
                    toastr.error(ko.Localization('Naati.Resources.Payroll.resources.BillExists').format(invoiceNumber));
                } else {
                    performCreateInvoice(examiner, invoiceNumber);
                }
            });
    };

    return vm;
});
