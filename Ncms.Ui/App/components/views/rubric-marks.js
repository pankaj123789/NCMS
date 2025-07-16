define([
	'services/util',
	'modules/enums'
],
	function (util, enums) {
		function ViewModel(params) {
			var self = this;
			var componentValidations = {};
            var assessmentValidations = {};
            var changedAssessment = ko.observableArray([]);
            var originalBands = ko.observableArray([]);

			self.testComponents = params.testComponents || ko.observableArray();
            self.disable = params.disable || ko.observable(false);
            self.showMinCommentLength = params.showMinCommentLength || ko.observable(false);
            self.showResult = params.showResult;
            self.changedAssessment = changedAssessment;
            self.clearChanges = function () {
                changedAssessment([]);
                originalBands([]);
            };
			self.tabOptions = {
				tabs: ko.pureComputed(function () {
					var active = true;
					return ko.utils.arrayMap(self.testComponents(), function (component) {
						var data = getTemplateData(component);
						var tab = {
							active: active,
							id: util.guid(),
							label: ko.computed(function () {
								var label = "{0}{1}".format(component.TypeLabel(), component.Label());

								if (ko.toJS(component.IsSupplementar)) {
									label = '<span class="info-glyph badge text-dark small">S</span> ' + label;
								}

								return label;
							}),
							tooltip: component.Name,
							type: 'template',
							template: {
								name: 'rubric-marks',
								data: data
							},
							valid: function () {
								return componentValidations[component.Id()].isValid();
							},
							validate: function () {
								return componentValidations[component.Id()].isValid();
							},
							clearValidation: function () {
								if (componentValidations[component.Id()].errors) {
									return componentValidations[component.Id()].errors.showAllMessages(false);
								}
							}
						};

						active = false;
						return tab;
					});
				})
			};

			self.validate = function () {
				return self.tabOptions.component.validate();
			};

			self.testComponents.subscribe(subscribeTestComponents);
			subscribeTestComponents();

			function subscribeTestComponents() {
				componentValidations = {};
				assessmentValidations = {};

				ko.utils.arrayForEach(self.testComponents(), function (component) {
					var assessments = [];
					ko.utils.arrayForEach(component.Competencies(), function (competence) {
                        ko.utils.arrayForEach(competence.Assessments(), function (assessment) {

                            assessment.selectedLevel = ko.pureComputed({
                                read: function () {
                                    return assessment.SelectedBand() !== null ? ko.utils.arrayFilter(assessment.Bands(), function(item) {
                                        return item.Id() === assessment.SelectedBand();
                                    })[0].Level() : '';
                                },
                                write: function (value) {
                                }
                            });
							assessmentValidations["{0}_{1}".format(component.Id(), assessment.Id())] = ko.validatedObservable(assessment);
							assessments.push(assessment.Comment);
						});
					});

					componentValidations[component.Id()] = ko.validatedObservable(assessments);
                });

                changedAssessment([]);
                originalBands([]);
			}

			function getTemplateData(component) {
				var count = assessmentsCount(component);
				var css = columnsSize(count);

				var assessmentIndex = 0;
				var selectedAssessment = ko.observable();

				ko.utils.arrayForEach(component.Competencies(), function (c, i) {
					var assessmentCss = 0;

					ko.utils.arrayForEach(c.Assessments(), function (a, j) {
						assessmentCss += css[assessmentIndex];
						assessmentIndex++;
					});

					c.Css = 'col col-sm-{0} b-white'.format(assessmentCss);
					if ((i + 1) != component.Competencies().length) {
						c.Css += ' b-r';
					}
				});

				component.SuccessfulFlag = ko.pureComputed({
					read: function () {
						return component.Successful() !== null ? component.Successful().toString() : '';
					},
					write: function (value) {
						component.Successful(value === 'true');
					}
				});

				component.WasAttemptedFlag = ko.pureComputed({
					read: function () {
						return component.WasAttempted() !== null ? component.WasAttempted().toString() : 'false';
					},
					write: function (value) {
						component.WasAttempted(value === 'true');
					}
				});

				component.Disable = ko.computed(function () {
					if (self.disable()) {
						return true;
                    }

					return component.MarkingResultTypeId() === enums.MarkingResultType.FromOriginal;
                });

                component.MinCommentLengthText = ko.computed(function () {
                    if (!self.showMinCommentLength() || !component.MinNaatiCommentLength() > 0) {
                        return ko.Localization('Naati.Resources.Test.resources.MaxLengthCommentText').format(component.MaxCommentLength());
			        }
                 
                    return ko.Localization('Naati.Resources.Test.resources.MinMaxLengthCommentText').format(component.MinNaatiCommentLength(), component.MaxCommentLength());
			    });

                component.showResult = ko.pureComputed(function () {
                    return self.showResult() && component.WasAttempted();
                });

                var data = {
                    component: component,
                    selectedAssessment: selectedAssessment,
                    assessmentValidations: assessmentValidations,
                    getExaminerComments: function (assessment) {
                        var comments = ko.utils.arrayMap(assessment.ExaminerResults(), function (e) {
                            return "{0}:\n{1}".format(e.Initials(), e.Comment());
                        });
                        assessment.Comment(comments.join('\n\n'));
                    },
                    assessmentChanged: function (assessment) {
                        return ko.utils.arrayFirst(changedAssessment(), function (a) {
                            return a.assessment === assessment;
                        });
                    },
                };

                data.selectAssessment = function (a) {
                    selectedAssessment(a);
                };

                data.selectBand = function (assessment, band, competence) {
                    if (component.Disable()) {
                        return;
                    }

                    var originalBand = ko.utils.arrayFirst(originalBands(), function (o) {
                        return o.assessment == assessment;
                    });

                    if (!originalBand) {
                        originalBand = {
                            band: ko.utils.arrayFirst(assessment.Bands(), function (b) {
                                return b.Id() == assessment.SelectedBand();
                            }),
                            assessment: assessment
                        };
                        originalBands.push(originalBand);
                    }

                    if (originalBand.band) {
                        var assessmentChanged = data.assessmentChanged(assessment);
                        if (assessmentChanged) {
                            if (assessmentChanged.oldBand === band) {
                                changedAssessment.remove(assessmentChanged);
                            }
                            else {
                                assessmentChanged.newBand = band;
                            }
                        }
                        else if (band != originalBand.band) {
                            changedAssessment.push({
                                assessment: assessment,
                                competence: competence,
                                component: component,
                                oldBand: originalBand.band,
                                newBand: band
                            });
                        }
                    }

                    assessment.SelectedBand(band.Id());
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