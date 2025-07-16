define([
    'views/shell',
    'services/screen/date-service',
    'services/payroll-data-service',
    'services/util',
    'views/payroll/payroll-functions'
], function (shell, dateService, payrollService, util, payrollFunctions) {

    var vm = {
        payrolls: ko.observableArray([]),
        title: shell.titleWithSmall,
        from: ko.observable().extend({ required: { message: 'From date is required.' } }),
        to: ko.observable().extend({ required: { message: 'To date is required.' }}),
        searched: ko.observable(false)
    };

    vm.dateRangeOptions = {
        fromDateOptions: { value: vm.from },
        toDateOptions: { value: vm.to },
        dateRangeFilterOption: 'month'
    };

    vm.activate = function () {
        vm.payrolls([]);
    };

    vm.canFind = function() {
        if (dateService.toPostDate(vm.from()) !== '' && dateService.toPostDate(vm.to()) !== '')
            return true;
        return false;
    }

    vm.find = function() {

        var request = {
            to: dateService.toPostDate(vm.to()),
            from: dateService.toPostDate(vm.from())
        };

        if (request.from === '' || request.to === '') {
            return false;
        } else {
            return payrollService.post(request, 'getMarkingsForPreviousPayroll').then(
                function(data) {
                    vm.searched(true);
                    $.each(data,
                        function(i, payroll) {
                            var examiners = payrollFunctions.parseResults(payroll.Examiners);
                            ko.utils.arrayForEach(examiners,
                                function(examiner) {
                                    examiner.dirtyFlag = new ko.DirtyFlag(examiner.AccountingReference);
                                });

                            payroll.headId = util.guid();
                            payroll.childId = util.guid();
                            payroll.expanded = ko.observable(false);

                            payroll.Examiners = ko.observableArray(examiners);

                        });

                    vm.payrolls(data);
                    return true;
                });
        }

    };

    vm.toggleExpanded = function (examiner) {
        examiner.expanded(!examiner.expanded());
    };

    vm.completedBy = function (payroll) {
        return ko.Localization('Naati.Resources.Payroll.resources.CompletedBy').format(payroll.PayrollModifiedUser);
    };

    vm.formatCost = function (cost) {
        return ko.Localization('Naati.Resources.Payroll.resources.CostFormat').format(vm.parseCost(cost));
    };

    vm.parseCost = function (cost) {
        return '$ ' + parseFloat(cost).toFixed(2);
    };

    vm.totalCost = function (marking) {
        return vm.parseCost(marking.ExaminerCost * marking.Count);
    };

    vm.expandAll = function () {
        $.each(vm.payrolls(), function (i, payroll) {
            if (!payroll.expanded()) {
                vm.toggleGroups(payroll);
            }

            $.each(payroll.Examiners(), function (j, examiner) {
                if (!examiner.expanded()) {
                    $('a', '#' + examiner.headId).click();
                }
            });
        });
    };

    vm.collapseAll = function () {
        $.each(vm.payrolls(), function (i, payroll) {
            if (payroll.expanded()) {
                vm.toggleGroups(payroll);
            }

            $.each(payroll.Examiners(), function (j, examiner) {
                if (examiner.expanded()) {
                    $('a', '#' + examiner.headId).click();
                }
            });
        });
    };

    vm.toggleGroups = function (payroll) {
        var $head = $('#' + payroll.headId)
        var $icon = $('i', $head);
        $icon.toggleClass('fa-plus-circle');
        $icon.toggleClass('fa-minus-circle');
        $('#' + payroll.childId).slideToggle();
        payroll.expanded(!payroll.expanded());
    };

    return vm;
});
