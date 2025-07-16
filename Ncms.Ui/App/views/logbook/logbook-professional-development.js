define([
    'services/person-data-service',
    'services/system-data-service',
    'services/logbook-data-service',
    'services/screen/message-service',
    'services/util',
    'views/logbook/logbook-professional-development-activity-edit',
    'views/logbook/logbook-professional-development-activity-attach',
    'views/logbook/logbook-period-selector',
    'plugins/router',
    'modules/enums',

],
    function (personDataService, systemDataService, logbookService, messageService, util, personPDActivityEdit, personPDAttachActivity, periodsSelector, router, enums) {

        var person = {
            GivenName: ko.observable(),
            Surname: ko.observable(),
            NaatiNumber: ko.observable(),
            DisplayName: ko.pureComputed(function () {
                return person.GivenName() + ' ' + person.Surname();
            }),
            PractitionerNumber: ko.observable()
        };

        var logbook = {
            StartDate: ko.observable(),
            RecertificationEligibilityDate: ko.observable(),
            EndDate: ko.observable()
        };

        var vm = {
            activate: activate,
            canActivate: canActivate,
            logbook: logbook,
            naatiNumber: ko.observable(),
            activities: ko.observableArray(),
            certificationPeriods: ko.observableArray([]),
            selectedPeriod: ko.observable(),
            sections: ko.observableArray(),
            pointsCalculated: ko.observable(),
            minPoints: ko.observable(),
            points: ko.observable(),
            preRequisites: ko.observableArray(),
            popoverId: util.guid(),
            popoverTitle: null,
            popoverContent: null,
            catalogueHtml: ko.observable(),
            includedActivitiesIds: ko.observableArray([]),
            chart: {
                type: 'doughnut',

                data: {
                    datasets: [
                        {
                            data: ko.observableArray([]),
                            backgroundColor: ko.observableArray([])
                        }
                    ],

                    labels: ko.observableArray([])
                },
                options: {
                    cutoutPercentage: 65,
                    elements: {
                        center: {
                            text: ko.observable(''),
                            fontColor: 'rgba(0,140,126,1)'
                        }
                    },
                    observeChanges: true,
                    tooltips: {
                        enabled: false,
                        custom: function (tooltipModel) {
                            if (tooltipModel.title) {
                                var label = tooltipModel.body[0].lines[0];
                                var color = tooltipModel.labelColors[0].backgroundColor;
                                var footer = getToolTipFooter(tooltipModel.dataPoints);
                                vm.popoverTitle = footer;
                                vm.popoverContent = '<div style="width: 20px; height: 20px; float: left; margin-right: 5px; background-color: ' + color + ';">&nbsp;</div> ' + label;
                                $('#' + vm.popoverId).popover('show');
                            }
                            else {
                                $('#' + vm.popoverId).popover('hide');
                            }
                        }
                    },
                    title: {
                        display: true,
                        text: ko.observable('')
                    },

                    legend: {
                        display: false
                    },
                }
            },
            hasUpdatePermission: ko.observable(false)
        };

        vm.selectedPeriod.subscribe(function (value) {
            ko.viewmodel.updateFromModel(logbook, value);
        });

        currentUser.hasPermission(enums.SecNoun.Logbook, enums.SecVerb.Update).then(vm.hasUpdatePermission);

        vm.editable = ko.pureComputed(function () {
            return vm.hasUpdatePermission() && vm.selectedPeriod() && vm.selectedPeriod().Editable;
        });

        var personPDActivityEditInstance = personPDActivityEdit.getInstance({
            editable: vm.editable
        });

        var personPDAttachActivityInstance = personPDAttachActivity.getInstance();

        vm.attachActivityOptions = {
            view: 'views/logbook/logbook-professional-development-activity-attach',
            model: personPDAttachActivityInstance,
        };

        var periodSelectorInstance = periodsSelector.getInstance();

        vm.activityEditOptions = {
            view: 'views/logbook/logbook-professional-development-activity-edit',
            model: personPDActivityEditInstance,
        };

        vm.periodSelectorOptions = {
            view: 'views/logbook/logbook-period-selector',
            model: periodSelectorInstance,
        };


        vm.popover = {
            html: true,
            animation: false,
            trigger: 'manual',
            container: 'body',
            title: function () {
                return vm.popoverTitle;
            },
            content: function () {
                return vm.popoverContent;
            },
            placement: 'top',
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

        vm.showCurrentPeriodButton = ko.pureComputed(function () {
            return vm.currentPeriod() && vm.selectedPeriod().Id !== vm.currentPeriod().Id;
        });

        vm.message = ko.pureComputed(function () {
            if (vm.showCurrentPeriodButton()) {
                return ko.Localization('Naati.Resources.Logbook.resources.ShowingSubmitted').format(vm.selectedPeriod().SubmittedRecertificationApplicationId, moment(vm.selectedPeriod().RecertificationEnteredDate).toDate());
            }
            if (vm.showPeriodsButton()) {
                return ko.Localization('Naati.Resources.Logbook.resources.ShowingCurrent');
            }
            return "";
        });

        vm.isCurrentPeriod = ko.pureComputed(function () {
            return vm.currentPeriod() && vm.selectedPeriod() && vm.selectedPeriod().Id === vm.currentPeriod().Id;
        });

        vm.attachable = ko.pureComputed(function () {
            return !vm.isCurrentPeriod() && vm.editable() && vm.selectedPeriod().SubmittedRecertificationApplicationId;
        });
        vm.dataTable = {
            id: util.guid(),
            columnDefs: [
                {
                    targets: 0,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
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
                    enabled: false,
                    text: '<span class="glyphicon glyphicon-plus m-r-xs"></span><span>' +
                        ko.Localization('Naati.Resources.Logbook.resources.AddActivity') + '</span>',
                    className: 'btn btn-success',
                    action: addActivity
                },
                {
                    enabled: false,
                    text: '<span class="fa fa-link m-r-xs"></span><span>' +
                        ko.Localization('Naati.Resources.Logbook.resources.AttachActivity') + '</span>',
                    className: 'btn btn-success',
                    action: attachActivity
                }]
            },
            initComplete: enableDisableDataTableButtons
        };

        function enableDisableDataTableButtons() {
            var $table = $('#' + vm.dataTable.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var dataTable = $table.DataTable();
            var buttons = dataTable.buttons('*');

            if (buttons.length != vm.dataTable.buttons.buttons.length) {
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

        var logbookHeadingFormat = ko.Localization('Naati.Resources.Logbook.resources.LogbookPdHeadingFormat');

        $.extend(vm,
            {
                windowTitle: ko.pureComputed(function () {
                    return logbookHeadingFormat.format(person.NaatiNumber(),
                        person.DisplayName(), "Professional Development");
                }),
                edit: function (activity) {
                    personPDActivityEditInstance.edit(person.NaatiNumber(), activity).then(function () {
                        vm.activate();
                    });
                },
                remove: function (activity) {
                    messageService.remove({ title: 'Remove activity', content: 'Are you sure that you want to remove this activity?' }).then(function (answer) {
                        if (answer === 'yes') {
                            logbookService.remove('professionaldevelopmentactivity/' + activity.Id).then(function () {
                                vm.activate();
                                toastr.success('Activity removed');
                            });
                        }
                    })
                },
                detach: function (activity) {
                    messageService.remove({ title: 'Detach activity', content: 'Are you sure that you want to detach this activity?', yesText: 'Detach' }).then(function (answer) {
                        if (answer === 'yes') {
                            logbookService.remove('detachProfessionaldevelopmentactivity/' + activity.Id+'/'+ vm.selectedPeriod().SubmittedRecertificationApplicationId).then(function () {
                                vm.activate();
                                toastr.success('Activity detached');
                            });
                        }
                    })
                }

            });

        function getToolTipFooter(tooltipItem, data) {
            var sectionIndex = parseInt(tooltipItem[0].index / 2);
            var section = vm.sections()[sectionIndex];
            var text = "" + section.Points + ' out of ' + section.MinPoints + " points";
            return text;
        }

        function loadChart() {

            if (!vm.pointsCalculated()) {
                return;
            }
            var initialRead = 200;
            var initialGreen = 50;
            var initialBlue = 100;

            var opacity = 1;
            var chartData = [];
            var chartColors = [];
            var labels = [];

            for (var sectionIndex = 0; sectionIndex < vm.sections().length; sectionIndex++) {
                var section = vm.sections()[sectionIndex];

                var colorDetails = (parseInt(initialRead) + sectionIndex * 20) + "," +
                    (parseInt(initialGreen) + sectionIndex * 50) + "," +
                    (parseInt(initialBlue) - sectionIndex * 2);
                var color = "rgba(" + colorDetails + "," + opacity + ")";

                chartData.push(section.Points);
                chartColors.push(color);
                labels.push(section.SectionName);

                // Missing Points
                var missinPointsColor = "rgba(" + colorDetails + "," + parseInt(opacity) * 0.1 + ")";
                var missingPoints = Math.max(section.MinPoints - section.Points, 0);
                chartData.push(missingPoints);
                chartColors.push(missinPointsColor);
                labels.push(section.SectionName);

            }

            vm.chart.data.datasets[0].data(chartData);
            vm.chart.data.datasets[0].backgroundColor(chartColors);
            vm.chart.data.labels(labels);
            vm.chart.options.elements.center.text('  ' + vm.points() + ' / ' + vm.minPoints() + '  ');
        }

        function addActivity() {
            personPDActivityEditInstance.add(vm.naatiNumber(), vm.selectedPeriod().SubmittedRecertificationApplicationId).then(function () {
                vm.activate();
            });
        }

        function attachActivity() {
            personPDAttachActivityInstance.attach(vm.naatiNumber(), vm.currentPeriod().Id).then(function (activityId) {

                var request = {
                    ActivityId: activityId,
                    CredentialApplicationId: vm.selectedPeriod().SubmittedRecertificationApplicationId
                };
                logbookService.post(request, 'attachProfessionaldevelopmentActivity')
                    .then(function () {
                    vm.activate();
                    toastr.success('Activity Attached');
                });

               
            });
        }

        function loadCatalogueLink() {
            logbookService.getFluid('value/PdCatalogue').then(function (value) {
                vm.catalogueHtml(ko.Localization('Naati.Resources.Logbook.resources.ProfessionalDevelopmentCatalogue').format(value));
            });
        }

        function canActivate(naatiNumber, certificationPeriodId) {

            if ( certificationPeriodId == undefined) {
                certificationPeriodId = -1;
            }
            var defer = Q.defer();
            var periodId = parseInt(certificationPeriodId);
            personDataService.getFluid(naatiNumber).then(function (data) {
                ko.viewmodel.updateFromModel(person, data);
            });
            personDataService.getFluid('certificationPeriodsDetails/' + naatiNumber).then(function (periods) {
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
                vm.naatiNumber(naatiNumber);
                defer.resolve(true);
            });

            return defer.promise;
        }

        vm.editable.subscribe(enableDisableDataTableButtons);
        vm.attachable.subscribe(enableDisableDataTableButtons);

        function activate() {
            var defer = Q.defer();
            loadActivityPoints().then(function () {
                loadCatalogueLink();
                loadChart();
                return personDataService.getFluid(vm.naatiNumber() + '/activities/' + vm.selectedPeriod().Id).then(
                    function (data) {

                        ko.utils.arrayForEach(data,
                            function (activity) {
                                if (activity.SectionIds.length > 1) {
                                    activity.PointDescription = '';
                                    for (var i = 0; i < activity.SectionIds.length; i++) {
                                        activity.PointDescription =
                                            activity.PointDescription +
                                            'Category ' +
                                            activity.SectionIds[i] +
                                            ': ' +
                                            activity.Points +
                                            '<br/>';
                                    }

                                } else {
                                    activity.PointDescription = activity.Points;
                                }

                                activity.Included = vm.includedActivitiesIds().find(function (element) {
                                    return element === activity.Id;
                                });

                            });
                        vm.activities(data);

                        defer.resolve(true);
                    });
            });

            return defer.promise;
        }

        function loadActivityPoints() {

            var defer = Q.defer();
            personDataService.getFluid(vm.naatiNumber() + '/ActivityPoints/' + vm.selectedPeriod().Id).then(function (data) {
                vm.pointsCalculated(data.Calculated);
                vm.sections(data.Sections);
                vm.minPoints(data.MinPoints);
                vm.points(data.Points);
                vm.includedActivitiesIds(data.IncludedActivitiesIds);
                var preRequisitesData = [];
                ko.utils.arrayForEach(data.Sections,
                    function (section) {

                        preRequisitesData = preRequisitesData.concat(section.PreRequisites.filter(function (item) {
                            return !preRequisitesData.find(function (i) {
                                return i.Message === item.Message;
                            });
                        }));
                    });

                preRequisitesData = preRequisitesData.concat(data.PreRequisites);

                vm.preRequisites($.unique(preRequisitesData));

                defer.resolve(true);

            });
            return defer.promise;
        }

        vm.showCurrentPeriod = function () {
            router.navigate('#logbook/' + vm.naatiNumber() + '/professional-development/' + vm.currentPeriod().Id);
        };

        vm.showPeriods = function () {
            periodSelectorInstance.show(vm.certificationPeriods(), vm.naatiNumber()).then(function (periodId) {
                router.navigate('#logbook/' + vm.naatiNumber() + '/professional-development/' + periodId);
            });
        };

        return vm;
    });
