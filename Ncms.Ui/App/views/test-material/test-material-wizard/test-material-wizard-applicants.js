define([
    'modules/custom-validator',
    'services/test-material-data-service'
],
    function (customValidator, testMaterialService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null,
                selectedTestSession: null
            };

            $.extend(defaultParams, params);

            var serverModel = {
                TestSpecificationId: ko.observable()
            };

            var emptyModel = ko.toJS(serverModel);

            var validator = customValidator.create(serverModel);

            serverModel.TestSpecificationId.subscribe(function (value) {
                vm.summary.TestSpecificationId(value);
            });

            var vm = {
                summary: defaultParams.summary,
                applicants: ko.observableArray(),
                readOnly: ko.observable(false),
                tableDefinition: {
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' },
                    order: [
                        [0, "asc"]
                    ]
                },
                barChart: {
                    type: 'horizontalBar',
                    data: {
                        datasets: [
                            {
                                backgroundColor: "#5cb85c",
                                data: ko.observableArray(),
                                label: "{0}".format(ko.Localization('Naati.Resources.TestMaterial.resources.New'))
                            },
                            {
                                backgroundColor: "#d9534f",
                                data: ko.observableArray(),
                                label: ko.Localization('Naati.Resources.TestMaterial.resources.WillBeAssigned')
                            }
                        ],
                        labels: ko.observableArray([ko.Localization('Naati.Resources.TestMaterial.resources.New'), ko.Localization('Naati.Resources.TestMaterial.resources.Overwritten')])
                    },
                    responsive: true,
                    legend: {
                        position: 'right',
                        display: false
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
                                            if (Math.floor(label) === label) {
                                                return label;
                                            }
                                        }
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
                            enabled: true,
                            position: 'nearest',
                            callbacks: {
                                label: getLabel
                            }

                        },
                    }
                },
            };

            vm.isValid = function () {
                var defer = Q.defer();

                testMaterialService.post(vm.summary.Request(), 'testSpecifications').then(function (data) {
                    validator.reset();

                    ko.utils.arrayForEach(data.InvalidFields,
                        function (i) {
                            validator.setValidation(i.FieldName, false, i.Message);
                        });

                    validator.isValid();

                    var isValid = !data.InvalidFields.length;
                    defer.resolve(isValid);
                    vm.readOnly(isValid);
                });

                return defer.promise;
            };

            vm.load = function () {
                vm.readOnly(false);
                validator.reset();
                ko.viewmodel.updateFromModel(serverModel, emptyModel);
                testMaterialService.post(vm.summary.Request(),'getTestMaterialSummary').then(function (data) {
                    buildChartSeries(data.TotalNewApplicantsNotSat, data.TotalNewApplicantsSat, data.TotalApplicantsToOverrideNotSat, data.TotalApplicantsToOverrideSat);
                    vm.applicants(data.ApplicantsAlreadySat);
                });
            };

            return vm;

            function getLabel(tooltipItem, data) {

                var label = vm.barChart.data.datasets[tooltipItem.datasetIndex].label;
                return label + ": " + tooltipItem.xLabel;
            }

            function buildChartSeries(totalNewNoSat, totalNewSat, totalOverrideNotSat, totalOverrideSat) {
                vm.barChart.data.datasets[0].data([totalNewNoSat, totalOverrideNotSat]);
                vm.barChart.data.datasets[1].data([totalNewSat, totalOverrideSat]);
            }
        }
    });