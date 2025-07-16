define([
    'services/person-data-service',
    'services/logbook-data-service',
    'plugins/router',
    'services/screen/message-service',
    'views/logbook/logbook-credential-edit',
    'views/logbook/logbook-period-selector',
    'views/logbook/logbook-professional-credential-attach',
    'modules/enums'
],
    function (personDataService, logbookService, router, message, logbookCredentialEdit, periodsSelector, activityAttach, enums) {
        var person = {
            GivenName: ko.observable(),
            Surname: ko.observable(),
            NaatiNumber: ko.observable(),
            DisplayName: ko.pureComputed(function () {
                return person.GivenName() + ' ' + person.Surname();
            }),
            PractitionerNumber: ko.observable()
        };

        var serverModel = {
            Id: ko.observable(),
            WorkPracticeUnits: ko.observable(),
            StartDate: ko.observable(),
            EndDate: ko.observable(),
            Requirement: ko.observable(),
            Points: ko.observable(),
            CredentialType: ko.observable(),
            Skill: ko.observable(),
            RecertificationEligibilityDate: ko.observable(),
            Calculated: ko.observable()
        };

        var vm = {
            activate: activate,
            certificationPeriods: ko.observableArray([]),
            selectedPeriod: ko.observable(),
            credential: serverModel,
            workPractices: ko.observableArray(),
            close: close,
            showAll: ko.observable(false),
            naatiNumber: ko.observable(),
            credentialId: ko.observable(),
            tableDefinition: {
                id: 'logbookCredentialTable',
                headerTemplate: 'logbook-credential-header-template',
                rowTemplate: 'logbook-credential-row-template'
            },
            barChart: {
                type: 'horizontalBar',
                data: {
                    datasets: [
                        {
                            label: ko.pureComputed(function () {
                                return "";
                            }),
                            backgroundColor: "rgba(200,50,100, 1)",
                            data: ko.observableArray()
                        },
                        {
                            label: ko.pureComputed(function () {
                                return "";
                            }),
                            backgroundColor: "rgba(200,50,100, 0.1)",
                            data: ko.observableArray()
                        }
                    ],
                    labels: ko.observableArray([])
                },
                responsive: true,
                legend: {
                    position: 'right',
                },
                options: {
                    maintainAspectRatio: false,
                    elements: {
                        center: {
                            text: null,
                        }
                    },
                    observeChanges: true,
                    scales: {
                        xAxes: [
                            {
                                stacked: true,
                                ticks: {
                                    stepSize: ko.observable(),
                                    userCallback: function (label, index, labels) {
                                        return Math.floor(label);
                                    }
                                }
                            }
                        ],
                        yAxes: [
                            {
                                stacked: true,
                                ticks: {
                                    stepSize: 10000
                                }
                            }
                        ]
                    },
                    legend: {
                        display: false
                    },
                    tooltips: {
                        enabled: true,
                        position: 'nearest',
                        backgroundColor: "rgba(200,50,100, 1)",
                        callbacks: {
                            footer: getToolTipTitle,
                            label: getToolTipFooter
                        }
                    }
                }
            },
            edit: edit,
            remove: remove,
            logbookCredentialInstance: logbookCredentialEdit.getInstance(),
            periodSelectorInstance: periodsSelector.getInstance(),
            activityAttachInstance: activityAttach.getInstance(),
            hasUpdatePermission: ko.observable(false)
        };

        vm.showPeriodsButton = ko.pureComputed(function () {
            return vm.certificationPeriods().length > 1;
        });

        vm.defaultPeriod = ko.pureComputed(function () {
            return vm.certificationPeriods().length ? vm.certificationPeriods()[0] : undefined;
        });

        vm.currentPeriod = ko.pureComputed(function () {
            return vm.certificationPeriods().find(function (element) {
                return element.IsCurrent;
            });
        });

        vm.showCurrentPeriod = function () {
            router.navigate('#logbook/' + vm.naatiNumber() + '/credential/' + vm.credentialId() + "/" + vm.currentPeriod().Id);
        };

        vm.showPeriods = function () {
            vm.periodSelectorInstance.show(vm.certificationPeriods(), vm.naatiNumber()).then(function (periodId) {
                router.navigate('#logbook/' + vm.naatiNumber() + '/credential/' + vm.credentialId() + "/" + + periodId);
            });
        };

        vm.showCurrentPeriodButton = ko.pureComputed(function () {
            return vm.currentPeriod() && vm.selectedPeriod().Id !== vm.currentPeriod().Id;
        });

        vm.message = ko.pureComputed(function () {
            if (vm.showCurrentPeriodButton()) {
                return "Showing logbook activities submitted with application APP" + vm.selectedPeriod().SubmittedRecertificationApplicationId + " on " + moment(vm.selectedPeriod().RecertificationEnteredDate).format('LL');
            }
            if (vm.showPeriodsButton()) {
                return "Showing current logbook";
            }
            return "";
        });

        vm.isCurrentPeriod = ko.pureComputed(function () {
            return vm.currentPeriod() && vm.selectedPeriod() && vm.selectedPeriod().Id === vm.currentPeriod().Id;
        });

        currentUser.hasPermission(enums.SecNoun.Logbook, enums.SecVerb.Update).then(vm.hasUpdatePermission);

        vm.editable = ko.pureComputed(function() {
            return vm.hasUpdatePermission() && vm.selectedPeriod() && vm.selectedPeriod().Editable;
        });


        function edit(item) {
            vm.logbookCredentialInstance.show(item, vm.credentialId(), vm.naatiNumber(), vm.selectedPeriod().Id, vm.credential.WorkPracticeUnits()).then(function (data) {
                vm.canActivate(vm.naatiNumber(), vm.credentialId(), vm.selectedPeriod().Id).then(vm.activate);
            });
        }

        function remove(item) {
            message.remove({
                yes: '<span class="glyphicon glyphicon-trash"></span><span>' +
                    ko.Localization('Naati.Resources.Shared.resources.Yes') +
                    '</span>',
                no: ko.Localization('Naati.Resources.Shared.resources.No'),
                content: ko.Localization('Naati.Resources.Person.resources.DeleteWorkPractice')
            }).then(function (answer) {
                if (answer === 'yes') {
                    logbookService.remove("deleteWorkPractice/" + item.Id()).then(function () {
                        vm.canActivate(vm.naatiNumber(), vm.credentialId(), vm.selectedPeriod().Id).then(vm.activate);
                    });
                }
            });
            
        }

        function showEditPopup() {
            vm.logbookCredentialInstance.show(null, vm.credentialId(), vm.naatiNumber(), vm.selectedPeriod().Id, vm.credential.WorkPracticeUnits()).then(function (data) {
                vm.canActivate(vm.naatiNumber(), vm.credentialId(), vm.selectedPeriod().Id).then(vm.activate);
            });
        }

        function enableDisableDataTableButtons() {
            var $table = $('#' + vm.tableDefinition.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var dataTable = $table.DataTable();
            var buttons = dataTable.buttons('*');

            if (buttons.length != vm.tableDefinition.dataTable.buttons.buttons.length) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            if (vm.editable()) {
                dataTable.button(0).enable();
            }
            else {
                dataTable.button(0).disable();
            }

            if (vm.attachable()) {
                dataTable.button(1).enable();
            }
            else {
                dataTable.button(1).disable();
            }
        };

        function attachActivity() {
            vm.activityAttachInstance.attach(vm.credentialId(), vm.naatiNumber(), vm.currentPeriod().Id).then(function (workPracticeId) {

                var request = {
                    workPracticeId: workPracticeId,
                    credentialApplicationId: vm.selectedPeriod().SubmittedRecertificationApplicationId,
                    credentialId: vm.credentialId()
                };
                logbookService.post(request, 'attachWorkPractice')
                    .then(function () {
                        vm.canActivate(vm.naatiNumber(), vm.credentialId()).then(vm.activate);
                        toastr.success('Activity Attached');
                    });
            });
        }

        vm.canActivate = function (naatiNumber, credentialId, certificationPeriodId) {
            vm.naatiNumber(naatiNumber);
            vm.credentialId(credentialId);

            var defer = Q.defer();
            credentialId = parseInt(credentialId);
            var periodId = parseInt(certificationPeriodId);

            personDataService.getFluid(naatiNumber).then(function (data) {
                ko.viewmodel.updateFromModel(person, data);
            });

            logbookService.getFluid('CredentialCertificationPeriods', { naatiNumber: naatiNumber, credentialId: credentialId }).then(function (periods) {
                vm.certificationPeriods(periods);
                var selectedPeriod = ko.utils.arrayFilter(periods, function (p) {
                    return p.Id === periodId;
                });
                if (selectedPeriod.length) {
                    vm.selectedPeriod(selectedPeriod[0]);
                }
                else {
                    vm.selectedPeriod(vm.defaultPeriod());
                }

                return logbookService.getFluid('CertificationPeriodCredential', { naatiNumber: naatiNumber, credentialId: credentialId, certificationPeriodId: vm.selectedPeriod().Id }).then(function (data) {
                    ko.viewmodel.updateFromModel(serverModel, data);
                    defer.resolve(true);
                });
            });

            return defer.promise;

        };
        var logbookHeadingFormat = ko.Localization('Naati.Resources.Logbook.resources.LogbookPdHeadingFormat');
        $.extend(vm,
            {
                windowTitle: ko.pureComputed(function () {
                    return logbookHeadingFormat.format(person.NaatiNumber(),
                        person.DisplayName(),vm.credential.CredentialType() + " " + vm.credential.Skill() + " Work Practice");
                }),
                detach: function (activity) {
                    message
                        .remove({
                            title: 'Detach activity',
                            content: 'Are you sure that you want to detach this activity?',
                            yesText: 'Detach'
                        }).then(function(answer) {
                            if (answer === 'yes') {
                                logbookService
                                    .remove('detachWorkPractice/' + activity.Id()).then(function() {
                                        vm.activate();
                                        toastr.success('Activity detached');
                                    });
                            }
                        });
                }
            });
        function getToolTipTitle(tooltipItem, data) {

            return "" + vm.credential.Points() + " out of " +
                vm.credential.Requirement() +
                " " +
                vm.credential.WorkPracticeUnits();
        }

        function getToolTipFooter(tooltipItem, data) {
            return "";
        }

        function activate() {
            loadCredential(vm.naatiNumber(), vm.credentialId());
            loadChart();
        }

        function loadCredential(naatiNumber, credentialId) {
            logbookService.getFluid('workPractices', { naatiNumber: naatiNumber, credentialId: credentialId, certificationPeriodId: vm.selectedPeriod().Id }).then(
                function (data) {
                    ko.viewmodel.updateFromModel(vm.workPractices, data);
                    return true;
                });
        }

        function loadChart() {
            if (!vm.credential.Calculated()) {
                return;
            }

            vm.barChart.options.scales.xAxes[0].ticks.stepSize(vm.credential.Requirement() / 10);
            vm.barChart.data.datasets[0].data([Math.min(vm.credential.Points(), vm.credential.Requirement())]);
            vm.barChart.data.datasets[1].data([Math.max(vm.credential.Requirement() - vm.credential.Points(), 0)]);
        }

        function close() {
            router.navigateBack();
        }

        vm.attachable = ko.pureComputed(function () {
            //return vm.currentPeriod() && vm.selectedPeriod() && vm.selectedPeriod().Id === vm.currentPeriod().Id;
            return !vm.isCurrentPeriod() && vm.editable() && vm.selectedPeriod().SubmittedRecertificationApplicationId;
        });
        vm.tableDefinition.dataTable = {
            source: vm.workPractices,
            buttons: {
                dom: {
                    button: {
                        tag: 'label',
                        className: 'm-r-xs'
                    },
                    buttonLiner: {
                        tag: null
                    }
                },
                buttons: [{
                        text: '<span class="glyphicon glyphicon-plus"></span><span>' +
                            ko.Localization('Naati.Resources.Person.resources.AddWorkPractice') + '</span>',
                        className: 'btn btn-success',
                        enabled: false,
                        action: showEditPopup
                    },
                    {
                        enabled: false,
                        text: '<span class="fa fa-link m-r-xs"></span><span>' +
                            ko.Localization('Naati.Resources.Logbook.resources.AttachActivity') + '</span>',
                        className: 'btn btn-success',
                        action: attachActivity
                    }]
            },
            initComplete: enableDisableDataTableButtons,
            columnDefs: [
                {
                    targets: 0,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                },
                {
                    targets: -1,
                    orderable: false
                }
            ],
            order: [
                [0, "asc"]
            ]
        };

        vm.editable.subscribe(enableDisableDataTableButtons);
        vm.attachable.subscribe(enableDisableDataTableButtons);
        return vm;
    });
