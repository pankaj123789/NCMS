define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'modules/custom-validator',
    'services/util',
    'services/testsession-data-service',
    'services/screen/date-service',
    'services/application-data-service'
],
    function (router, common, enums, customValidator, util, testSessionService, dateService, applicationService) {
        var compositionComplete = false;
        var queryString;
        var chartColour = 'gray';

        var serverModel = {
            TestSessionId: ko.observable(0),
            VenueId: ko.observable(0).extend({ required: true }),
            TestDate: ko.observable().extend({
                required: true
            }),
            TestTime: ko.observable().extend({
                required: true,
                timeValid: 'HH:MM'
            }),
            Name: ko.observable().extend({ required: true, maxLength: 100 }),
            Duration: ko.observable().extend({ maxLength: 3, min: 15, max: 480, step: 1 }),
            ArrivalTime: ko.observable().extend({ maxLength: 3, min: 0, max: 180, step: 1 }),
            DefaultTestSpecificationId: ko.observable(0).extend({ required: true }),
            NewCandidatesOnly: ko.observable(),
            ActiveFlag: ko.observable()
        };

        var validator = customValidator.create(serverModel);

        var cleanModel = ko.toJS(serverModel);

        var vm = {
            barChartHeight: ko.observable(),
            testsession: serverModel,
            venues: ko.observableArray(),
            actions: ko.observableArray(),
            testSessionName: ko.observable(),
            testSessionAddress: ko.observable(),
            testSessionAddressUrl: ko.observable(),
            testSittingUrl: ko.observable(),
            testDate: ko.observable(),
            testTime: ko.observable(),
            arrivalTime: ko.observable(),
            newCandidatesOnly: ko.observable(),
            testEnd: ko.observable(),
            credentialType: ko.observable(),
            testSpecification: ko.observable(),
            isActiveTestSpecification: ko.observable(),
            venueName: ko.observable(),
            testsessionApplicants: ko.observableArray(),
            showAllocateRolePlayers: ko.observable(),
            showManageRolePLayers: ko.observable(),
            showViewRolePLayers: ko.observable(),
            chart: {
                type: 'doughnut',
                data: {
                    datasets: [
                        {
                            data: ko.observableArray([0, 0]),
                            backgroundColor: [
                                "#5cb85c",
                                "coral",
                                "#5bc0de",
                                "#435070",
                                "#ad9c00",
                                "#f0f0f0"
                            ]
                        }
                    ],
                    labels: [ko.Localization('Naati.Resources.TestSession.resources.TestSatLabel'),
                        ko.Localization('Naati.Resources.TestSession.resources.CheckedInLabel'),
                        ko.Localization('Naati.Resources.TestSession.resources.ConfirmedLabel'),
                        ko.Localization('Naati.Resources.TestSession.resources.ProcessingInvoice'),
                        ko.Localization('Naati.Resources.TestSession.resources.AwitingTestPayment'),
                        ko.Localization('Naati.Resources.TestSession.resources.FreeSeatsLabel')]
                },
                options: {
                    cutoutPercentage: 65,
                    elements: {
                        center: {
                            text: ko.observable('  0 / 0  '),
                            fontColor: chartColour
                        }
                    },
                    observeChanges: true,
                    tooltips: {
                        enabled: true
                    },

                    legend: {
                        display: false
                    },
                }

            },
            barChart: {
                type: 'horizontalBar',
                data: {
                    datasets: [
                        {
                            label: ko.Localization('Naati.Resources.TestSession.resources.TestSatLabel'),
                            backgroundColor: "#5cb85c",
                            data: ko.observableArray()
                        },
                        {
                            label: ko.Localization('Naati.Resources.TestSession.resources.CheckedInLabel'),
                            backgroundColor: "coral",
                            data: ko.observableArray()
                        }, {
                            label: ko.Localization('Naati.Resources.TestSession.resources.ConfirmedLabel'),
                            backgroundColor: "#5bc0de",
                            data: ko.observableArray()
                        },
                        {
                            label: ko.Localization('Naati.Resources.TestSession.resources.ProcessingInvoice'),
                            backgroundColor: "#435070",
                            data: ko.observableArray()
                        },
                        {
                            label: ko.Localization('Naati.Resources.TestSession.resources.AwitingTestPayment'),
                            backgroundColor: "#ad9c00",
                            data: ko.observableArray()
                        },
                        {
                            label: ko.Localization('Naati.Resources.TestSession.resources.FreeSeatsLabel'),
                            backgroundColor: "#f0f0f0",
                            data: ko.observableArray()
                        }
                    ],
                    labels: ko.observableArray()
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
                                    stepSize: 1
                                }
                            }
                        ],
                        yAxes: [
                            {
                                stacked: true,
                                ticks: {
                                    stepSize: 1
                                }
                            }
                        ]
                    },
                    tooltips: {
                        enabled: false,
                        custom: function (tooltipModel) {
                            if (!tooltipModel.body) {
                                return vm.barChartPopover.$element.popover('hide');
                            }

                            var datasets = [];

                            for (var i = 0; i < tooltipModel.body.length; i++) {
                                datasets.push({
                                    backgroundColor: tooltipModel.labelColors[i].backgroundColor,
                                    label: tooltipModel.body[i].lines[0],
                                });
                            }

                            var content = ko.generateTemplate('barchart-tooltip-template', datasets);

                            vm.barChartPopover.content(content);
                            vm.barChartPopover.title(tooltipModel.title[0]);
                            vm.barChartPopover.$element.popover('show');
                        }
                    }
                }
            },
            showTestSessionDetails: ko.observable(false),
            tableDefinition: {
                id: 'testsessionApplicantTable',
                headerTemplate: 'testsession-applicant-header-template',
                rowTemplate: 'testsession-applicant-row-template'
            }
        };

        vm.formatPhone = function (phone) {
            return common.functions().formatPhone(phone);
        };

        vm.compositionComplete = function () {
            compositionComplete = true;
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        vm.tableDefinition.dataTable = {
            source: vm.testsessionApplicants,
            columnDefs: [
                {
                    targets: -1,
                    orderable: false
                }
            ],
            order: [
                [0, "asc"]
            ]
        };

        var validation = ko.validatedObservable(serverModel);

        $.extend(vm, {
            title: ko.computed(function () {
                return '{0}'.format(vm.testSessionName());
            }),
            subtitle: ko.computed(function () {
                return '{0}'.format(vm.testSessionAddress());
            })
        });

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(vm.testsession);
            request.TestDate = dateService.toPostDate(request.TestDate);

            testSessionService.post(request, 'update')
                .then(function () {
                    loadtopsection();
                    loadSkillBarChart();
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });
            return defer.promise;
        };

        vm.canActivate = function (testsessionId, naatiNumber, query) {
            queryString = query || {};
            testsessionId = parseInt(testsessionId);

            ko.viewmodel.updateFromModel(serverModel, cleanModel);

            serverModel.TestSessionId(testsessionId);

            return loadtestsession();
        };

        vm.getActions = function (applicant) {
            testSessionService.getFluid('actions', { TestSittingId: applicant.TestSittingId }).then(function (data) {
                if (!data.length) {
                    data = [
                        {
                            Name: ko.Localization(
                                'Naati.Resources.CredentialRequestSummary.resources.NoActionsAvailable'),
                            Disabled: true
                        }
                    ];
                }

                applicant.Actions(data);
            });
        };

        vm.takeAction = function (applicantInfo, action) {

            if (action.Disabled) {
                event.preventDefault();
                return false;
            }

            var request = {

                ApplicationStatusId: applicantInfo.ApplicationStatusId,
                ActionId: action.Id,
                ApplicationId: applicantInfo.ApplicationId,
                CredentialRequestId: applicantInfo.CredentialRequestId,
                CredentialTypeId: applicantInfo.CredentialTypeId,
                ApplicationTypeId: applicantInfo.ApplicationTypeId
            };

            applicationService.post(request, 'action').then(function (data) {
                if (!checkAndShowMessages(data)) {
                    applicationService.getFluid('steps', request).then(function (steps) {
                        if (steps.length) {
                            return router.navigate('application-wizard/' + applicantInfo.ApplicationId + '/' + action.Id + '/' + applicantInfo.CredentialRequestId);
                        }

                        applicationService.post(request, 'wizard').then(function () {
                            toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                            loadtestsession();
                        });
                    });
                }
            });
        };

        vm.back = function () {
            router.navigateBack();
            return false;
        };

        vm.barChartPopover = {
            title: ko.observable(),
            content: ko.observable(),
            animation: false,
            trigger: 'manual',
            container: 'body',
            placement: 'top',
            html: true
        };

        function checkAndShowMessages(messages) {
            var genericMessages = [];

            ko.utils.arrayForEach(messages, function (m) {
                if (!m.Field) {
                    genericMessages.push(m.Message);
                }
            });

            if (genericMessages.length) {
                toastr.error(genericMessages.join('<br /><br />'), null, {
                    closeButton: true,
                    timeOut: 0
                });
            }

            resetValidation();

            ko.utils.arrayForEach(messages, function (i) {
                if (!i.Field) {
                    return;
                }
                validator.setValidation(i.Field, false, i.Message);
            });

            validator.isValid();

            return messages && messages.length;
        }

        function resetValidation() {
            validator.reset();

            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadtopsection() {
            testSessionService.getFluid(serverModel.TestSessionId() + '/topsection').then(function (data) {

                var strIsCompleted = data.IsCompletedStatus ? 'Completed' : 'To be sat';

                var headingFirstLine = 'TS' + data.Id + ', ' + data.Name + ' - ' + strIsCompleted;
                var headingSecondLine = data.VenueAddress;
                var headingSecondLineUrl = "https://www.google.com.au/maps/place/" + headingSecondLine.replace('/', '%2F');
                vm.testSessionName(headingFirstLine);
                vm.testSessionAddress(headingSecondLine);
                vm.testSessionAddressUrl(headingSecondLineUrl);

                vm.testSittingUrl('#test-session/test-sittings/' + data.Id);

                vm.testDate(moment(data.TestDate).format(CONST.settings.shortDateTimeDisplayFormat));
                vm.arrivalTime(moment(data.ArrivalTime).format(CONST.settings.timeDisplayFormat));
                vm.testEnd(moment(data.TestEnd).format(CONST.settings.timeDisplayFormat));
                
                vm.credentialType(data.CredentialTypeInternalName);
                vm.testSpecification(data.TestSpecificationDescription);

                vm.newCandidatesOnly(data.NewCandidatesOnly ? 'Yes' : 'No');
                
                vm.isActiveTestSpecification(data.IsActiveTestSpecification);
                if (!vm.isActiveTestSpecification()) {
                    var inactiveMessage = vm.testSpecification() + ' ' +ko.Localization('Naati.Resources.TestSession.resources.InactiveTestSpecification');
                    vm.testSpecification(inactiveMessage);
                }
                vm.venueName(data.VenueName);

                vm.chart.data.datasets[0].data([
                    data.SatAttendees, data.CheckedInAttendees, data.ConfirmedAttendees, data.ProcessingInvoiceAttendees, data.AwaitingPaymentAttendees,
                    data.Attendees > data.Capacity ? 0 : data.Capacity - data.Attendees
                ]);
                vm.chart.options.elements.center.text('  ' + data.Attendees + ' / ' + data.Capacity + '  ');

				vm.showAllocateRolePlayers(data.RolePlayersRequired);
                vm.showViewRolePLayers(data.HasRolePLayers);
                vm.showManageRolePLayers(data.HasRolePLayers);

                vm.testsession.ActiveFlag(data.IsActive);
                if (data.IsActive) {
                    vm.testsession.ActiveFlag('[ACTIVE]');
                }
                if (!data.IsActive) {
                    vm.testsession.ActiveFlag('[INACTIVE]');
                }
            });
        }

        function loadSkillBarChart() {
            testSessionService.getFluid(serverModel.TestSessionId() + '/barChart').then(function(data) {
                var valueChart = {
                    Skills: [],
                    Confirmed: [],
                    CheckedIn: [],
                    Sat: [],
                    FreeSeats: [],
                    ProcessingInvoice: [],
                    AwaitingPayment: [],
                    
                };
                ko.utils.arrayForEach(data.Result,
                    function(item) {
                        valueChart.Skills.push(item.DisplayName);
                        valueChart.Confirmed.push(item.ConfirmedCount);
                        valueChart.CheckedIn.push(item.CheckedInCount);
                        valueChart.Sat.push(item.SatCount);
                        valueChart.FreeSeats.push(item.FreeSeats);
                        valueChart.ProcessingInvoice.push(item.ProcessingInvoiceCount);
                        valueChart.AwaitingPayment.push(item.AwaitingPaymentCount);
                    });
                return valueChart;
            }).then(function(data) {
                vm.barChart.data.labels(data.Skills);
                vm.barChart.data.datasets[0].data(data.Sat);
                vm.barChart.data.datasets[1].data(data.CheckedIn);
                vm.barChart.data.datasets[2].data(data.Confirmed);
                vm.barChart.data.datasets[3].data(data.ProcessingInvoice);
                vm.barChart.data.datasets[4].data(data.AwaitingPayment);
                vm.barChart.data.datasets[5].data(data.FreeSeats);

                var heightValue = 30 * data.Skills.length + 60;
                vm.barChartHeight(heightValue + "px");
            });
        }

        function loadtestsession() {

            loadtopsection();
            loadSkillBarChart();

            testSessionService.getFluid(serverModel.TestSessionId() + '/allAppplicants').then(function (data) {
                var statuses = enums.CredentialRequestStatusTypes;

                ko.utils.arrayForEach(data, function (item) {
                    var status = item.Status;
                
                    item.ShowAttendanceLink = status === statuses.TestSat || status === statuses.TestFailed || status === statuses.IssuedPassResult || status === statuses.CertificationIssued || status === statuses.UnderPaidTestReview;
                    item.CredentialRequestStatusId = item.Status;
                    item.StatusCss = util.getTestSittingStatusCss(item);
                    item.Status = util.getTestSittingStatus(item);
                    item.Actions = ko.observableArray();
                    item.CharacterTypeTraditional = item.LanguageCharacterType == 'Traditional Chinese';
                    item.CharacterTypeSimplified = item.LanguageCharacterType == 'Simplified Chinese';
                });

                vm.testsessionApplicants(data);

            });

            return testSessionService.get({ id: serverModel.TestSessionId() }).then(function (data) {

                ko.viewmodel.updateFromModel(serverModel, data);
                if (vm.testsession.TestDate()) {
                    vm.testsession.TestDate(moment(vm.testsession.TestDate()).format(CONST.settings.shortDateDisplayFormat));
                }
                resetValidation();
                dirtyFlag().reset();
                return true;
            });
        }

        return vm;

    });