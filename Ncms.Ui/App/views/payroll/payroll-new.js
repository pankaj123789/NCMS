define([
    'views/shell',
    'services/payroll-data-service',
    'views/payroll/payroll-functions'
],
function (shell, payrollService, payrollFunctions) {
    var vm = {
        examiners: ko.observableArray([]),
        title: shell.titleWithSmall
    };

    vm.activate = function () {
        vm.examiners([]);
        return payrollService.getFluid('getMarkingsForNewPayroll').then(function (data) {
            vm.examiners(payrollFunctions.parseResults(data));
        });
    }

    vm.getFirstReceivedDateInfo = function (examiner) {
        return ko.Localization('Naati.Resources.Payroll.resources.OldestReceivedDay').format('<span class="font-grey-bold text-md">' + examiner.OldestReceivedDays() + '</span>');
    };

    vm.formatExaminerCost = function (examiner) {
        return ko.Localization('Naati.Resources.Payroll.resources.CostFormat').format('$ ' + parseFloat(examiner.TotalCost()).toFixed(2));
    };

    vm.toggleExpanded = function (examiner) {
        examiner.expanded(!examiner.expanded());
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

    vm.selectAll = function () {
        includeInPayRun(true);
    };

    vm.unselectAll = function () {
        includeInPayRun(false);
    };

    vm.includedExaminers = ko.pureComputed(function () {
        return ko.utils.arrayFilter(vm.examiners(), function (examiner) {
            return examiner.IncludeInPayRun();
        });
    });

    vm.jobExaminerIds = ko.pureComputed(function () {
        return ko.utils.arrayMap(vm.includedExaminers(), function (examiner) {
            return examiner.JobExaminerIds();
        }).reduce(function (a, b) { return a.concat(b); }, []);
    });

    vm.createNewPayRun = function () {
        var idsToInclude = vm.jobExaminerIds();

        if (idsToInclude.length > 0) {
            payrollService.post(idsToInclude, 'createPayroll').then(function () {
                vm.activate();
                toastr.success(ko.Localization('Naati.Resources.Payroll.resources.PayRunCreatedSuccessfully'));
            });
        }
    };

    return vm;

    function includeInPayRun(include) {
        $.each(vm.examiners(), function (i, u) {
            u.IncludeInPayRun(include);
        });
    }
});
