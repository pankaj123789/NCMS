define([
    'services/audit-data-service'
], function (auditService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var vm = {
            logs: ko.observableArray([])
        };

        vm.getHeader = function (log) {
            var logHeader = ko.Localization('Naati.Resources.Shared.resources.LogHeader');
            var date = '<b>' + moment(log.DateTime).format('l') + '</b>';
            var userName = '<b>' + log.Username + '</b>';

            return logHeader.format(date, userName);
        };
        
        vm.formatLog = function (log) {
            if (!log) {
                return null;
            }

            var tmp = log.split('Changed values:');
            if (tmp.length === 1) {
                return log;
            }

            tmp = tmp[1].split(';');

            var newTmp = $.map(tmp, function (t) {
                var s = t.split('=');
                if (s.length > 1) {
                    return '<b>' + s[0] + ': </b>' + s[1];
                }

                s = t.split('changed from');
                if (s.length > 1) {
                    return '<b>' + s[0] + '</b> changed from ' + s[1];
                }

                return t;
            });

            return newTmp.join('<br/>');
        };

        vm.load = function (testAttendanceId, testResultId, jobId) {
            vm.logs([]);
            loadLog('TestAttendance', testAttendanceId);

            if (testResultId) {
                loadLog('TestResult', testResultId);
            }

            if (jobId) {
                loadLog('Job', jobId);
            }
        };

        return vm;

        function loadLog(recordName, recordId) {
            auditService.get({ RecordName: recordName, RecordId: recordId, PageSize: 1000, PageNumber: 1 }).then(function (data) {
                var logs = vm.logs();

                logs = logs.concat(data);
                logs = logs.sort(function (left, right) {
                    return moment(left.DateTime).diff(moment(right.DateTime)) * -1;
                });

                vm.logs(logs);
            });
        }
    }
});
