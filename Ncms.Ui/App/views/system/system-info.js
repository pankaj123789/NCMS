
define(['services/system-data-service', 'services/security-data-service', 'modules/enums'],
    function (systemService, securityService, enums) {

        var vm = {
            systemInfo: ko.observableArray([]),
            userPermissions: ko.observableArray([]),
            activate: activate,
            clearCache: clearCache,
            refreshCookieCache: refreshCookieCache,
            refreshUsersCache: refreshUsersCache,
            syncNcmsReportLogs: syncNcmsReportLogs,
            executeNcmsReports: executeNcmsReports,
            processApplications: processApplications,
            sendCandidateBrief: sendCandidateBrief,
            issueTestResults: issueTestResults,
            sendPendingEmails: sendPendingEmails,
            deleteTemporaryFiles: deleteTemporaryFiles,
            deleteOldSystemFiles: deleteOldSystemFiles,
            processRefund: processRefund,
            refreshNotifications: refreshNotifications,  
            deleteBankDetails: deleteBankDetails,
            sendRecertificationReminders,
            sendTestSessionReminders,
            sendTestSessionAvailabilityNotices,
            sendTestMaterialReminders: sendTestMaterialReminders,
            processFileDeletesPastExpiryDate: processFileDeletesPastExpiryDate,
             
        }

        function processApplications() {

            systemService.post({}, 'ProcessPendingApplications').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function clearCache() {
            systemService.post({}, 'clearCache').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function refreshCookieCache() {
            systemService.post({}, 'refreshCookieCache').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function refreshUsersCache() {
            systemService.post({}, 'refreshUsersCache').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function deleteOldSystemFiles() {
            systemService.post({}, 'DeletePodDataHistory').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function refreshNotifications() {
            systemService.post({}, 'refreshNotifications').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function deleteBankDetails() {
            systemService.post({}, 'deleteBankDetails').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }     


        function deleteTemporaryFiles() {
            systemService.post({}, 'deleteTemporaryFiles').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function syncNcmsReportLogs() {
            systemService.post({}, 'syncNcmsReportLogs').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function executeNcmsReports() {
            systemService.post({}, 'ExecuteNcmsReports').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendCandidateBrief() {
            systemService.post({}, 'sendCandidateBriefs').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendPendingEmails() {
            systemService.post({}, 'sendPendingEmails').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function issueTestResults() {
            systemService.post({}, 'issueTestResultsAndCredentials').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendRecertificationReminders() {
            systemService.post({}, 'sendRecertificationReminders').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendTestSessionReminders() {
            systemService.post({}, 'sendTestSessionReminders').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendTestSessionAvailabilityNotices() {
            systemService.post({}, 'sendTestSessionAvailabilityNotices').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function processRefund() {
            systemService.post({}, 'processRefund').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function sendTestMaterialReminders() {
            systemService.post({}, 'sendTestMaterialReminders').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }

        function processFileDeletesPastExpiryDate() {
            systemService.post({}, 'processFileDeletesPastExpiryDate').then(function (data) {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.JobExecuted'));
            });
        }


        function activate(test) {
            systemService.getFluid('/info')
                .then(
                    function (data) {

                        vm.systemInfo([]);

                        $.each(data,
                            function (i, d) {
                                addRow(i, d);
                            });

                        addRow('User name', currentUser.Name);

                        securityService.getFluid('/verbs').then(
                            function (verbs) {
                                var userPermissions = [...currentUser.Permissions];
                                userPermissions.sort((x, y) => x.NounName.localeCompare(y.NounName));
                                for (var r = 0; r < userPermissions.length; r++) {
                                    var permission = userPermissions[r];
                                    var nounVerbs = [];
                                    for (var v = 0; v < verbs.length; v++) {
                                        var verb = verbs[v];
                                        if ((verb.Value & permission.Permissions) === verb.Value) {
                                            nounVerbs.push(verb.DisplayName);
                                        }
                                    }
                                    if (nounVerbs.length) {
                                        vm.userPermissions.push({
                                            noun: permission.NounDisplayName,
                                            verb: nounVerbs.join(', ')
                                        });
                                    }
                                }
                            });
                    });

            if (test) {
                doTest(test);
            }
        }

        function doTest(test) {
            systemService.getFluid(test);
        }

        function addRow(aspect, info) {
            vm.systemInfo.push({ aspect: aspect, info: info });
        }

        return vm;
    });