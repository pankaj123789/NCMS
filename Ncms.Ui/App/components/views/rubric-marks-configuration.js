define([
    'services/util',
    'services/screen/message-service'
],
    function (util, messageService) {
        function ViewModel(params) {
            var self = this;

            self.isWriter = params.isWriter || ko.observable(false);
            self.showDescriptions = params.showDescriptions || ko.observable(false);
            self.testComponents = params.testComponents || ko.observableArray();
            self.configurations = params.configurations || ko.observableArray();
            self.testSpecificationId = params.testSpecificationId || ko.observable();
            self.addBand = params.addBand;
            self.addConditionOptions = params.addConditionOptions;

            self.tabOptions = {
                tabs: ko.pureComputed(function () {
                    var active = true;
                    return ko.utils.arrayMap(self.testComponents(), function (component) {
                        var data = getTemplateData(component);
                        var tab = {
                            active: active,
                            id: util.guid(),
                            label: component.TypeLabel,
                            tooltip: component.Name,
                            type: 'template',
                            template: {
                                name: 'rubric-marks-configuration',
                                data: data
                            }
                        };

                        active = false;
                        return tab;
                    });
                })
            };

            function getTemplateData(component) {
                var count = assessmentsCount(component);
                var css = columnsSize(count);

                var assessmentIndex = 0;
                var bands = 0;

                ko.utils.arrayForEach(component.Competencies(), function (c, i) {
                    var assessmentCss = 0;

                    ko.utils.arrayForEach(c.Assessments(), function (a, j) {
                        assessmentCss += css[assessmentIndex];
                        assessmentIndex++;

                        if (a.Bands().length > bands) {
                            bands = a.Bands().length;
                        }
                    });

                    c.Css = 'col col-sm-{0} b-white'.format(assessmentCss);
                    if ((i + 1) != component.Competencies().length) {
                        c.Css += ' b-r';
                    }
                });

                var bandIndex = 0;
                var componentBands = ko.utils.arrayMap(new Array(bands), function (a) {
                    return bandIndex++;
                });

                var data = {
                    component: component,
                    componentBands: componentBands,
                    groupConditions: ko.observableArray(),
                    selectedConditions: ko.observableArray(),
                    showDescriptions: self.showDescriptions,
                    isWriter: self.isWriter,
                    addConditionOptions: self.addConditionOptions,
                    addBand: self.addBand,
                    addCondition: function (assessment, band, markingBand, callback) {
                        var args = {
                            assessment: assessment,
                            band: band,
                            testComponent: data.component,
                            markingBand: markingBand
                        };
                        callback(args);
                    },
                    getBand: function (index, assessement) {
                        var bands = assessement.Bands();
                        if (index >= bands.length) return {};
                        return bands[index];
                    },
                    getExaminerComments: function (assessment) {
                        var comments = ko.utils.arrayMap(assessment.ExaminerResults(), function (e) {
                            return "{0}:\n{1}".format(e.Initials(), e.Comment());
                        });
                        assessment.Comment(comments.join('\n\n'));
                    }
                };

                return data;
            }

            function assessmentsCount(component) {
                var count = 0;
                ko.utils.arrayForEach(component.Competencies(), function (c) {
                    count += c.Assessments().length;
                });
                return count;
            }

            function columnsSize(length) {
                var maxColumnsCount = 12;

                if (length > maxColumnsCount) {
                    throw "Max assessments length exceeded.";
                }

                var result = [];

                if (maxColumnsCount % length == 0) {
                    var defaultSize = maxColumnsCount / length;
                    for (var i = 0; i < length; i++) {
                        result.push(defaultSize);
                    }
                }

                if (length == 5) {
                    result = [3, 3, 2, 2, 2];
                }

                if (length == 7) {
                    result = [2, 2, 2, 2, 2, 1, 1];
                }

                if (length == 8) {
                    result = [2, 2, 2, 2, 1, 1, 1, 1];
                }

                if (length == 9) {
                    result = [2, 2, 2, 1, 1, 1, 1, 1, 1];
                }

                if (length == 10) {
                    result = [2, 2, 1, 1, 1, 1, 1, 1, 1, 1];
                }

                if (length == 11) {
                    result = [2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
                }

                return result;
            };

            params.component = self;
        }

        return ViewModel;
    });