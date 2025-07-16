define([
    'services/payroll-data-service',
    'services/screen/message-service'
], function (payrollService, message) {
    var pageSize = 50;
    function toggleTests(e) {
        var $tr = $(e.currentTarget);
        $("i", $tr)
            .toggleClass("fa-caret-down")
            .toggleClass("fa-caret-right");

        var $newTr = $tr.next();

        if ($newTr.hasClass("ui-helper-hidden")) {
            $newTr.toggleClass("ui-helper-hidden");
            $("div", $newTr).slideToggle();
        }
        else {
            $("div", $newTr).slideToggle(function () {
                $newTr.toggleClass("ui-helper-hidden");
            });
        }
    }

    function vm(params) {
        var self = this;

        var defaultParams = {
            examiner: null,
            allowRemoveTest: false
        };

        $.extend(defaultParams, params);

        self.examiner = defaultParams.examiner;
        self.allowRemoveTest = defaultParams.allowRemoveTest;

        self.showTests = function (marking, e) {
            toggleTests(e);
            marking.expanded(!marking.expanded());
        };

        self.openTest = function (marking) {
            window.open('#test/' + marking.TestAttendanceId(), '_blank');
        };

        self.parseCost = function (cost) {
            return "$ " + parseFloat(cost).toFixed(2);
        };

        self.totalCost = function (marking) {
            return self.parseCost(marking.ExaminerCost() * marking.Count());
        };

        processExaminer();

        self.removeTest = function (test) {
            var marking = test.Marking;
            var examiner = marking.Examiner;

            message.confirm({
                title: ko.Localization('Naati.Resources.Payroll.resources.RevertMarkingConfirmationTitle'),
                content: ko.Localization('Naati.Resources.Payroll.resources.RevertMarkingConfirmation')
            }).then(function (answer) {
                if (answer == 'yes') {
                    payrollService.post(test.JobExaminerId(), 'revertMarking').then(function () {
                        toastr.success(ko.Localization('Naati.Resources.Payroll.resources.MarkingReverted'));
                        marking.Tests.remove(test);
                        if (!marking.Tests().length) {
                            examiner.Markings.remove(marking);
                        }
                    });
                }
            });
        };

        function processExaminer() {
            if (!self.examiner) return;
            var examiner = self.examiner;

            if (!examiner.Markings) return;
            examiner.Markings.subscribe(processMarkings);
            processMarkings();
        }

        function processMarkings() {
            if (!self.examiner) return;
            var examiner = self.examiner;

            if (!examiner.Markings) return;
            ko.utils.arrayForEach(examiner.Markings(), function (m) {
                m.TotalSize = ko.observable(pageSize);
                m.expanded = ko.observable(false);
                m.VisibleTests = ko.computed(function () {
                    var i = -1;
                    return ko.utils.arrayFilter(m.Tests(), function () {
                        i++;
                        return i < m.TotalSize();
                    });
                });
                m.increaseTotalSize = function (m) {
                    m.TotalSize(m.TotalSize() + pageSize);
                };
            });
        }
    }

    return vm;
});