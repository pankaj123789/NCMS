define([
    'services/examiner-data-service',
    'services/testresult-data-service',
    'services/screen/message-service',
    'services/screen/date-service',
    'services/util',
    'modules/enums',
], function (examinerService, testResultService, message, dateService, util, enums) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;
        var eventDefer = Q.defer();
        var chairSuffix = ko.Localization('Naati.Resources.Roles.resources.Chair');

        var vm = {
            examiners: ko.observableArray([]),
            event: eventDefer.promise,
            getExaminerTypeText: getExaminerTypeText,
            dataTable: ko.observable(),
            edit: edit,
            remove: remove,
            testAttendanceId: ko.observable(),
            addMode: ko.observable(),
            loadExaminers: loadExaminers,
            formatDate: formatDate,
            getPersonName: getPersonName,
            disable: ko.observable(true),
            testIsInvalidated: ko.observable(false),
            // Automatic Issuing Variables
            setNewDataTable: setNewDataTable,
            automaticIssuingExaminerValue: ko.observable(),
            dataTableLoaded: ko.observable(false)
        };

        vm.disable.subscribe(enableDisableDataTableButtons);
        vm.canEditExaminer = ko.observable(currentUser.hasPermissionSync(enums.SecNoun.Examiner, enums.SecVerb.Update));
        vm.canRemoveExaminer = ko.observable(currentUser.hasPermissionSync(enums.SecNoun.Examiner, enums.SecVerb.Delete));

        function enableDisableDataTableButtons(newValue) {
            if (!vm.dataTableLoaded()) {
                return setTimeout(function () { enableDisableDataTableButtons(newValue); }, 100);
            }

            if (!$.fn.DataTable.isDataTable($('#' + vm.dataTable().id))) {
                return setTimeout(function () { enableDisableDataTableButtons(newValue); }, 100);
            }

            var buttons = $('#' + vm.dataTable().id).DataTable().buttons('*');
            if (!buttons.length) {
                return setTimeout(function () { enableDisableDataTableButtons(newValue); }, 100);
            }

            if (newValue || vm.testIsInvalidated()) {
                buttons.disable();
            }
            else if (currentUser.hasPermissionSync(enums.SecNoun.Examiner, enums.SecVerb.Create)) {
                buttons.enable();
            }
        }

        function getExaminerTypeText(typeName) {

            return ko.Localization('Naati.Resources.Test.resources.ExaminerType' + typeName);
        }

        function formatDate(date) {
            return dateService.toUIDate(date);
        }

        function getPersonName(e) {
            var personName = e.Examiner.PersonName;
            var suffix = [];

            if (moment(e.Examiner.MembershipEndDate) < moment()) {
                suffix.push(ko.Localization('Naati.Resources.Shared.resources.Inactive'));
            }

            if (e.Examiner.IsChair) {
                suffix.push(chairSuffix);
            }

            if (suffix.length) {
                personName += ' (' + suffix.join(' | ') + ')';
            }

            return personName;
        }

        function edit(e) {
            eventDefer.notify({
                name: 'EditExaminer',
                data: e
            });
        }

        function remove(examiner) {
            message.confirm({ title: ko.Localization('Naati.Resources.Shared.resources.Warning'), content: ko.Localization('Naati.Resources.Shared.resources.ThisRecordWillBeDeleted') }).then(function (answer) {
                if (answer === 'yes') {
                    examinerService.remove('examiner/' + examiner.JobExaminerId).then(function () {
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));

                        eventDefer.notify({
                            name: 'ExaminerRemoved',
                            data: examiner.JobExaminerId
                        });
                    });
                }
            });
        }

        function loadExaminers(testAttendanceId, examiners, addMode) {
            vm.testAttendanceId(testAttendanceId);
            vm.addMode(addMode);
            vm.examiners(examiners());
        }

        function setNewDataTable(credentialTypeId, testResultId) {
            // reset dataTable
            vm.dataTableLoaded(false);
            vm.dataTable({});

            // set default table
            var newDataTable =
            {
                id: util.guid(),
                source: ko.observableArray([]),
                columnDefs: [
                    { targets: -1, orderable: false }
                ],
                buttons: {
                    dom: {
                        button: {
                            tag: 'label',
                            className: ''
                        },
                        buttonLiner: {
                            tag: null
                        }
                    },
                    buttons: [{
                        text: '<span class="glyphicon glyphicon-plus"></span><span>' +
                            ko.Localization('Naati.Resources.Test.resources.AddExaminer') + '</span>',
                        className: 'btn btn-success',
                        enabled: false,
                        action: function (e, dt, node, config) {
                            eventDefer.notify({
                                name: 'AddExaminer',
                                data: e
                            });
                        }
                    }]
                },
                initComplete: function () {
                    enableDisableDataTableButtons(vm.disable());
                }
            };

            // if credential type is not CCL or practice CCL set default datatable and return
            var elligibleCredentialTypeForAutomaticIssuing = [14, 38];
            if (!elligibleCredentialTypeForAutomaticIssuing.includes(credentialTypeId)) {
                setDataTable(newDataTable);
                return;
            }

            // if there is no test result then set default table and return
            if (testResultId == null || testResultId == 0) {
                setDataTable(newDataTable);
                return;
            }

            // if automaticIssuing for testSpecification is set to false then set default dataTable and return;
            getAutomaticIssuing().then(function (automaticIssuing) {
                if (automaticIssuing == false) {
                    setDataTable(newDataTable);
                    return;
                }

                // if credential type is CCL or CCL Practice and automatic issuing is true for test specification then push ready for automation button and set datatable
                getAutomaticIssuingExaminer().then(function (automaticIssuingExaminer) {
                    var readyForAutomationButton =
                    {
                        text: '<input id="automaticIssuing" type="checkbox" checked/><i></i><span>' + ko.Localization('Naati.Resources.Test.resources.ReadyForAutomation') + '</span>',
                        className: 'i-switch i-switch-md m-t-s m-r m-l',
                        init: function (dt, node, config) {
                            // initialise the button with the AutomaticIssuingExaminer field from db tblTestResult
                            if (automaticIssuingExaminer == null) {
                                var automaticIssueRequest = {
                                    TestSittingId: vm.testAttendanceId,
                                    AutomaticIssuingExaminer: true
                                }

                                var request = ko.toJS(automaticIssueRequest);
                                postAutomaticIssuingExaminer(request).then(function (result) {
                                    if (result == true) {
                                        toastr.success('Automatic Issuing Examiner has been automatically set to active.')
                                        node.children('input[type="checkbox"]').prop('checked', true);
                                    }

                                    if (result == false || result == null) {
                                        toast.error("Automatic Issuing Examiner could not be set correctly.")
                                        node.children('input[type="checkbox"]').prop('checked', false);
                                    }
                                });
                                return;
                            }
                            node.children('input[type="checkbox"]').prop('checked', automaticIssuingExaminer);
                        },
                        enabled: false,
                        action: function (e, dt, node, config) {
                            var checked = !node.children('input[type="checkbox"]').prop('checked');
                            node.children('input[type="checkbox"]').prop('checked', checked);
                            vm.automaticIssuingExaminerValue(checked);
                        }
                    };
                    newDataTable.buttons.buttons.push(readyForAutomationButton);
                    setDataTable(newDataTable);
                    return;
                });
                return;
            });   
        }

        // get automaticIssuingExaminer from db tblTestResult
        function getAutomaticIssuingExaminer() {
            return new Promise(function (resolve, reject) {
                testResultService.getFluid(vm.testAttendanceId() + '/getAutomaticIssuingExaminer').then(
                    (response) => {
                        var result = response;
                        resolve(result);
                    },
                    (error) => {
                        reject(error);
                    }
                );
            });
        }

        // get automaticIssuing from db tblTestSpecification
        function getAutomaticIssuing() {
            return new Promise(function (resolve, reject) {
                testResultService.getFluid(vm.testAttendanceId() + '/getTestSpecificationAutomaticIssuing').then(
                    (response) => {
                        var result = response;
                        resolve(result);
                    },
                    (error) => {
                        reject(error);
                    }
                );
            });
        }

        // when switch is changed, post the new value to db
        vm.automaticIssuingExaminerValue.subscribe(function () {
            var automaticIssueRequest = {
                TestSittingId: vm.testAttendanceId,
                AutomaticIssuingExaminer: vm.automaticIssuingExaminerValue
            }

            var request = ko.toJS(automaticIssueRequest);
            postAutomaticIssuingExaminer(request).then(function (result) {
                if (result == true) {
                    toastr.success('Automatic Issuing Examiner Updated Successfully');
                }

                if (result == false) {
                    toast.error("Automatic Issuing Examiner Failed to Update.");
                    vm.automaticIssuingExaminerValue(!vm.automaticIssuingExaminerValue());
                }
            });
        });

        function postAutomaticIssuingExaminer(request) {
            return new Promise(function (resolve, reject) {
                testResultService.post(request, 'automationReady').then(
                    (response) => {
                        var result = response;
                        resolve(result);
                    },
                    (error) => {
                        reject(error);
                    }
                );
            });
        }

        function setDataTable(newDataTable) {
            vm.dataTable(newDataTable);
            vm.dataTable().source(vm.examiners());
            vm.dataTableLoaded(true);
            return;
        }

        return vm;
    }
});