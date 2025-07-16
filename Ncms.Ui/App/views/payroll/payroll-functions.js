define([
    'services/util'
],
function (util) {
    var vm = {
        parseResults: function (data) {
            return $.map(data, function (model) {
                var examiner = ko.viewmodel.fromModel(model);

                examiner.headId = util.guid();
                examiner.childId = util.guid();
                examiner.expanded = ko.observable(false);
                examiner.IncludeInPayRun = ko.observable(false);
                
                $.each(examiner.Markings(), function (j, marking) {
                    marking.Examiner = examiner;

                    $.each(marking.Tests(), function (k, test) {
                        test.Marking = marking;
                    });

                    marking.JobExaminerIds = ko.pureComputed(function () {
                        return ko.utils.arrayMap(marking.Tests(), function (test) {
                            return test.JobExaminerId();
                        });
                    });
                });

                examiner.JobExaminerIds = ko.pureComputed(function () {
                    return ko.utils.arrayMap(examiner.Markings(), function (marking) {
                        return marking.JobExaminerIds();
                    }).reduce(function (a, b) { return a.concat(b); }, []);
                });

                return examiner;
            });
        }
    };

    return vm;
});
