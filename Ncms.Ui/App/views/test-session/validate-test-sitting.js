define([],
    function () {

        function getInstance() {

            var vm = {
                issues: ko.observableArray(),
                show: show,
                showHideDetails: showHideDetails
            };

            function show(data) {

                ko.utils.arrayForEach(data, function (issue) {

                    issue.showDetails = ko.observable(false);
                    issue.TotalFoundMessage = ko.Localization('Naati.Resources.TestSession.resources.TotalIssuesFound').format(issue.Details.length, issue.TotalFound)
                });

                vm.issues(data);
                $('#validateTestSittingModal').modal('show');
            }

            function showHideDetails(issue) {

                issue.showDetails(!issue.showDetails());
            }

            return vm;
        }

        return {
            getInstance: getInstance
        };
    });

