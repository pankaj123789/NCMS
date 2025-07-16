define([
        'durandal/app',
        'services/requester',
    'views/logbook/logbook-work-practice-edit', 'plugins/router', 'views/logbook/logbook-period-selector'],
    function (app, requester, workPracticeEdit, router, periodsSelector) {
        var logbook = new requester('logbook');

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
            Calculated: ko.observable(),
            RecertificationActivitesStartDate :ko.observable(),
        };

        var vm = {
            credential: serverModel,
            certificationPeriods: ko.observableArray([]),
            selectedPeriod: ko.observable(),
            tableDefinition: {
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' },
                columnDefs: [{
                    targets: -1,
                    orderable: false
                }, {
                    targets: 0,
                    render: $.fn.dataTable.render.moment('', CONST.settings.shortDateDisplayFormat, true)
                }],
                order: [[0, "desc"]]
            },
            workPractices: ko.observableArray(),
            chart: {
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
                                    stepSize: ko.observable()
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
                        backgroundColor: "rgba(0,140,126,1)",
                        callbacks: {
                            footer: getToolTipTitle,
                            label: getToolTipFooter
                        }
                    },
                }
            },
        };

        function getToolTipTitle (tooltipItem, data) {
            
            return ""+ vm.credential.Points() +" out of " +
                vm.credential.Requirement() +
                " " +
                vm.credential.WorkPracticeUnits();
        }

        function getToolTipFooter (tooltipItem, data) {
            return "";
        }
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

        vm.editable = ko.pureComputed(function () {

            return vm.selectedPeriod() && vm.selectedPeriod().Editable;
        });
        var workPracticeEditInstance = workPracticeEdit.getInstance(vm.credential, vm.editable);
        var periodSelectorInstance = periodsSelector.getInstance();
        vm.periodSelectorOptions = {
            view: 'views/logbook/logbook-period-selector',
            model: periodSelectorInstance,
        };

        vm.title = ko.pureComputed(function () {
            return serverModel.CredentialType() + " - " + serverModel.Skill() + " Work Practice";
        });

   

        vm.canActivate = function (credentialId, certificationPeriodId) {
            if (certificationPeriodId == undefined) {
                certificationPeriodId = -1;
            }
            var defer = Q.defer();
            credentialId = parseInt(credentialId);
            var periodId = parseInt(certificationPeriodId);
            logbook.getFluid('CredentialCertificationPeriods', { credentialId: credentialId }).then(function (periods) {
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

                return logbook.getFluid('CertificationPeriodCredential', { credentialId: credentialId, certificationPeriodId: vm.selectedPeriod().Id }).then(function (data) {
                    ko.viewmodel.updateFromModel(serverModel, data);
                    loadChart();
                    defer.resolve(true);
                });

            });

            return defer.promise;
            
        };

        vm.statusTooltip = function (workPractice) {

            switch (workPractice.Status) {
            case ENUMS.WorkPracticeStatus.Current:
                return "To be included in the current Recertification";
            case ENUMS.WorkPracticeStatus.Future:
            case ENUMS.WorkPracticeStatus.Recertified:
            default:
                return "";
            }
        };

        vm.statusIcon = function (workPractice) {
            switch (workPractice.Status) {
            case ENUMS.WorkPracticeStatus.Current:
                return 'fa fa-check text-success';
            case ENUMS.WorkPracticeStatus.Future:
                return 'fa fa-clock-o';
            case ENUMS.WorkPracticeStatus.Recertified:
                return 'fa fa-refresh';
            default:
                return "";
            }
        };

        vm.activate = function () {
            app.breadCrumbs([
                { href: window.baseUrl, text: 'Home' },
                { href: window.baseUrl + '/Logbook', text: 'My Logbook' },
                { href: '#', text: vm.title },
            ]);

            logbook.getFluid('getworkpractices', { credentialId: serverModel.Id(), certificationPeriodId: vm.selectedPeriod().Id }).then(vm.workPractices);
        };

        vm.workPracticeEditOptions = {
            view: 'views/logbook/logbook-work-practice-edit',
            model: workPracticeEditInstance,
        };

        vm.add = function () {
            workPracticeEditInstance.add().then(function () {
                vm.canActivate(serverModel.Id()).then(vm.activate);
                });
        };

        vm.edit = function (workPractice) {
            workPracticeEditInstance.edit(workPractice).then(function () {
                vm.canActivate(serverModel.Id()).then(vm.activate);
            });
        };

        vm.remove = function (workPractice) {
            mbox.remove({ title: 'Remove work practice', content: 'Are you sure that you want to remove this work practice?' }).then(function (answer) {
                if (answer === 'yes') {
                    logbook.remove(workPractice.Id, 'deleteworkpractice').then(function () {
                        vm.canActivate(serverModel.Id()).then(vm.activate);
                    
                        toastr.success('Work practice removed.');
                    });
                }
            })
        }

        function loadChart() {

            if (!vm.credential.Calculated()) {
                return;
            }
            vm.chart.options.scales.xAxes[0].ticks.stepSize(vm.credential.Requirement() / 10);
            vm.chart.data.datasets[0].data([Math.min(vm.credential.Points(), vm.credential.Requirement())]);
            vm.chart.data.datasets[1].data([Math.max( vm.credential.Requirement()-vm.credential.Points(),0)]);
           // vm.chart.data.datasets[0].backgroundColor("rgba(200,50,100, 1)");
            //vm.chart.data.labels("");
          //  vm.chart.options.elements.center.text('  ' + vm.credential.Points() + ' / ' + vm.credential.Requirement() + '  ');

        }
        vm.showCurrentPeriod = function () {
            router.navigate('credential/'+vm.credential.Id()+'/' + vm.currentPeriod().Id);
        };

        vm.showPeriods = function () {
            periodSelectorInstance.show(vm.certificationPeriods()).then(function (periodId) {
                router.navigate('credential/' + vm.credential.Id() + '/' + periodId);
            });
        };

        return vm;
    }
);