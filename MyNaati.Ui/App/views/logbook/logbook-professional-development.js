define([
    'durandal/app',
    'services/requester',
    'services/util',
    'views/logbook/logbook-activity-edit', 'plugins/router', 'views/logbook/logbook-period-selector'],

    function (app, requester, util, activityEdit, router, periodsSelector) {
        var logbook = new requester('logbook');
 
        var vm = {
            popoverId: util.guid(),
            popoverTitle: null,
            popoverContent: null,
            tableDefinition: {
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' },
                columnDefs: [
                    {
                        targets: -1,
                        orderable: false
                    }, {
                        targets: 0,
                        render: $.fn.dataTable.render.moment('', CONST.settings.shortDateDisplayFormat)
                    }
                ],
                order: [[0, "desc"]]
            },
            tablePointsDefinitions: {
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' },
            },
            activities: ko.observableArray(),
            certificationPeriods: ko.observableArray([]),
            selectedPeriod: ko.observable(),
            pdCatalogue: ko.observable(),
            showHistoric: ko.observable(false),
            preRequisites: ko.observableArray(),
            sections: ko.observableArray([]),
            pointsCalculated: ko.observable(),
            minPoints: ko.observable(),
            points: ko.observable(),
            ponintStatus: ko.observable(),
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

            }
        };

        function getToolTipTitle(tooltipItem, data) {
            return data.labels[tooltipItem.index];
        }
        function getToolTipFooter(tooltipItem, data) {

            var sectionIndex = parseInt(tooltipItem[0].index / 2);
            var section = vm.sections()[sectionIndex];
            var text = "" + section.Points + ' out of ' + section.MinPoints + " points";
            return text;
        }
        vm.showPeriodsButton = ko.pureComputed(function () {
            return vm.certificationPeriods().length > 1;
        });

        vm.currentPeriod = ko.pureComputed(function () {
            return vm.certificationPeriods().length ? vm.certificationPeriods()[0] : undefined;
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

        vm.defaultPeriod = ko.pureComputed(function () {

            return vm.certificationPeriods().length ? vm.certificationPeriods()[0] : undefined;
        });


        vm.currentPeriod = ko.pureComputed(function () {
            return vm.certificationPeriods().find(function (element) {
                return element.IsCurrent;
            });
        });

        vm.isCurrentPeriod = ko.pureComputed(function () {
            return vm.currentPeriod() && vm.selectedPeriod() && vm.selectedPeriod().Id === vm.currentPeriod().Id;
        });

        vm.editable = ko.pureComputed(function () {
            return vm.selectedPeriod() && vm.selectedPeriod().Editable;
        });
        var activityEditInstance = activityEdit.getInstance({ editable: vm.editable });
        var periodSelectorInstance = periodsSelector.getInstance();

        vm.popover = {
            html: true,
            animation: false,
            trigger: 'manual',
            title: function () {
                return vm.popoverTitle;
            },
            content: function () {
                return vm.popoverContent;
            },
            placement: 'top',
        };


      
        vm.canActivate = function (certificationPeriodId) {

            if (certificationPeriodId == undefined) {
                certificationPeriodId = -1;
            }
            var defer = Q.defer();
            var periodId = parseInt(certificationPeriodId);
            logbook.getFluid('certificationPeriods').then(function (periods) {
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

                loadActivityPoints().then(function () {
                    defer.resolve(true);
                });
            });

            return defer.promise;
        }

        vm.statusTooltip = function (activity) {

            if (activity.Included) {
                return "To be included in the current Recertification";
            }
            return "";
        };

        vm.statusIcon = function (activity) {

            if (activity.Included) {
                return 'fa fa-check text-success';
            }
            return "";
        };

        vm.activate = function () {
            app.breadCrumbs([
                { href: window.baseUrl, text: 'Home' },
                { href: window.baseUrl + '/Logbook', text: 'My Logbook' },
                { href: '#', text: 'Professional Development' },
            ]);

            loadChart();
            return logbook.getFluid('activities', { certificationPeriodId: vm.selectedPeriod().Id }).then(
                function (data) {

                    ko.utils.arrayForEach(data.List,
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
                    vm.activities(data.List);
                    vm.pdCatalogue(data.Catalogue);
                    return true;
                });

        };


        vm.activityEditOptions = {
            view: 'views/logbook/logbook-activity-edit',
            model: activityEditInstance,
        };
        vm.periodSelectorOptions = {
            view: 'views/logbook/logbook-period-selector',
            model: periodSelectorInstance,
        };

        vm.addActivity = function () {
            activityEditInstance.add().then(function () {
                vm.canActivate(vm.selectedPeriod().Id).then(vm.activate);
            });
        };

        vm.showCurrentPeriod = function () {
            router.navigate('professional-development/' + vm.currentPeriod().Id);
        };

        vm.showPeriods = function () {
            periodSelectorInstance.show(vm.certificationPeriods()).then(function (periodId) {
                router.navigate('professional-development/' + periodId);
            });
        };

        vm.editActivity = function (activity) {
            activityEditInstance.edit(activity).then(function () {
                vm.canActivate(vm.selectedPeriod().Id).then(vm.activate);
            });
        };

        vm.removeActivity = function (activity) {
            mbox.remove({ title: 'Remove activity', content: 'Are you sure that you want to remove this activity?' }).then(function (answer) {
                if (answer === 'yes') {
                    logbook.remove(activity.Id, 'deleteprofessionaldevelopmentactivity').then(function () {
                        vm.canActivate(vm.selectedPeriod().Id).then(vm.activate);
                        toastr.success('Activity removed');
                    });
                }
            })
        }


        function loadActivityPoints() {

            var defer = Q.defer();
            logbook.getFluid('ActivityPoints', { certificationPeriodId: vm.selectedPeriod().Id }).then(function (data) {
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

        return vm;
    }
);