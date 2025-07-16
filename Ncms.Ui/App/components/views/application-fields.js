define([
    'modules/enums',
    'services/util',
    'services/application-data-service',
    'services/institution-data-service',
    'modules/common'
], function (enums, util, applicationService, institutionService, common) {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        var functions = common.functions();

        self.componentId = util.guid();
        self.countries = ko.observableArray();
        self.fields = params.fields || ko.observableArray();
        self.valid = params.valid || ko.observable(true);
        self.changed = params.changed || function () { };

        function parseSearchTerm() {
            var json = {
                InstitutionIntList: null,
                LocationString: null,
                QualificationString: null,
                CredentialTypeIntList: null,
                EndorsementFromString: null,
                EndorsementToString: null
            };
            return JSON.stringify(json);
        }
        self.endorsedQualificationData = ko.observableArray();
        self.endorsedQualificationId = ko.observable();

        var defer = Q.defer;

        self.isApplicationFieldType = function (actualTypeId, expectedTypeId) {
            return actualTypeId === enums.ApplicationFieldTypes[expectedTypeId];
        };

        self.yesNoComponentClick = function (data, e) {
            var value = data.Value();

            if (value === null || typeof (value) === 'undefined') {
                data.Value('True');
            }
            else if (value) {
                data.Value(false);
            }
            else {
                data.Value(null);
            }

            updateYesNoComponent(data.Value(), $(e.currentTarget), data.FieldEnable());
        };

        self.updateYesNoComponent = function (array, data) {
            var index = self.fields().indexOf(data);
            if (self.isApplicationFieldType(data.DataTypeId(), 'YesNo')) {
                updateYesNoComponent(data.Value(), $('#{0}_{1}'.format(self.componentId, index === 0 ? "0" : index)), data.FieldEnable());
            }
        };

        var validation = null;
        self.validate = function () {
            self.valid(validation.isValid());
            return self.valid();
        };

        self.clearValidation = function () {
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
        };

        init();
        function init() {

            var valueArray = [];

            ko.utils.arrayForEach(self.fields(), function (f) {

                checkAndBindEndorsedQualificationOptions(f);

                if (self.isApplicationFieldType(f.DataTypeId(), 'Email')) {
                    f.Value.extend({
                        pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
                    });
                }

                valueArray.push(f.Value);
            });

            validation = ko.validatedObservable(valueArray);
        }

        function checkAndBindEndorsedQualificationOptions(f) {
            if (f.DataTypeId() !== enums.ApplicationFieldTypes.EndorsedQualificationLookup) {
                return;
            }

            var endorsedQualificationOptions = {};

            setOptionValue(endorsedQualificationOptions);
            setInstitutionOptions(endorsedQualificationOptions);
            setLocationOptions(endorsedQualificationOptions);
            setQualificationOptions(endorsedQualificationOptions, f);

            f.getEndorsedQualificationOptions = function () {
                return endorsedQualificationOptions;
            };
        }

        function setOptionValue(endorsedQualificationOptions) {

            institutionService.getFluid('searchEndorsedQualifications', { request: { Skip: null, Take: null, Filter: parseSearchTerm() } }).then(
                function (data) {

                    self.endorsedQualificationData(data);

                    ko.utils.arrayForEach(self.fields(), function (item) {
                        if (item.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationIdText) {
                            if (item.Value()) {

                                var endorsedQualificationId = item.Value();
                                var endorsedQualificationData = ko.utils.arrayFilter(self.endorsedQualificationData(),
                                    function (data) {
                                        return data.EndorsedQualificationId === parseInt(endorsedQualificationId);
                                    });

                                endorsedQualificationOptions.institution.value(endorsedQualificationData[0].InstitutionNaatiNumber);
                                endorsedQualificationOptions.existingLocationOptionNameValue = endorsedQualificationData[0].Location;
                                endorsedQualificationOptions.existingQualificationNameValue = endorsedQualificationData[0].Qualification;
                            }
                        }
                    });

                    defer.resolve(true);
                    return defer.promise;
                });
        }

        function setInstitutionOptions(endorsedQualificationOptions) {

            endorsedQualificationOptions.institution = {
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                multiple: false,
                value: ko.observable(),
                options: ko.observableArray(),
                afterRender: function () { functions.getLookup('EndorsedQualificationsInstitution').then(this.options) },
                multiselect: { enableFiltering: false }
            };
            endorsedQualificationOptions.institution.value(endorsedQualificationOptions.existingInstitutionNaatiNumberValue);
            endorsedQualificationOptions.institution.value.subscribe(function () { endorsedQualificationOptions.LocationloadOptions(); });
        }

        function setLocationOptions(endorsedQualificationOptions) {
            endorsedQualificationOptions.location = {
                optionsValue: 'DisplayName',
                optionsText: 'DisplayName',
                multiple: false,
                value: ko.observable(),
                options: ko.observableArray(),
                multiselect: { enableFiltering: false }
            };
            endorsedQualificationOptions.location.value.subscribe(function () { endorsedQualificationOptions.QualificationloadOptions(); });

            endorsedQualificationOptions.LocationloadOptions = function () {
                var args = {
                    fieldName: "Location",
                    field: endorsedQualificationOptions.location.value,
                    oldValue: endorsedQualificationOptions.location.value(),
                    newValue: null
                };
                endorsedQualificationOptions.location.value(null);
                self.changed(args);

                var institutionNaatiNumbers = endorsedQualificationOptions.institution.value();
                endorsedQualificationOptions.QualificationloadOptions();

                if (!institutionNaatiNumbers) {
                    endorsedQualificationOptions.location.options([]);
                    return;
                }

                applicationService.getFluid('GetEndorsementLocationLookup/', { request: { InstitutionNaatiNumbers: [institutionNaatiNumbers] } }).then(
                    function (data) {
                        endorsedQualificationOptions.location.options(data);

                        if (endorsedQualificationOptions.existingLocationOptionNameValue) {
                            var j = ko.utils.arrayFilter(data, function (option) {
                                if (option.DisplayName === endorsedQualificationOptions.existingLocationOptionNameValue) {
                                    return option;
                                }
                            });

                            endorsedQualificationOptions.location.value(j[0].DisplayName);
                        }
                    });
            };
        }

        function setQualificationOptions(endorsedQualificationOptions, field) {
            endorsedQualificationOptions.qualification = {
                value: field.FieldOptionId,
                options: ko.observableArray(),
                multiple: false,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            };
            endorsedQualificationOptions.qualification.options = ko.observableArray();
            endorsedQualificationOptions.qualification.value.subscribe(function () { endorsedQualificationOptions.EndorsedQualificationIdSet(field); });

            endorsedQualificationOptions.QualificationloadOptions = function () {

                var locations = endorsedQualificationOptions.location.value();
                if (!locations) {
                    endorsedQualificationOptions.qualification.options([]);
                    return;
                }

                var institutionNaatiNumbers = endorsedQualificationOptions.institution.value();
                if (!institutionNaatiNumbers) {
                    endorsedQualificationOptions.qualification.options([]);
                    return;
                }

                applicationService.post({ Locations: [locations], InstitutionNaatiNumbers: [institutionNaatiNumbers] }, 'GetEndorsementQualificationLookup').then(
                    function (data) {
                        endorsedQualificationOptions.qualification.options(data);

                        if (endorsedQualificationOptions.existingQualificationNameValue) {
                            var j = ko.utils.arrayFirst(data, function (option) {
                                return option.DisplayName === endorsedQualificationOptions.existingQualificationNameValue;
                            });
                            var args = {
                                fieldName: "Qualification",
                                field: endorsedQualificationOptions.qualification.value,
                                oldValue: endorsedQualificationOptions.qualification.value(),
                                newValue: j.Id
                            };
                            endorsedQualificationOptions.qualification.value(j.Id);
                            self.changed(args);
                        }

                    });
            };

            endorsedQualificationOptions.EndorsedQualificationIdSet = function (field) {
                var qualificationOptions = endorsedQualificationOptions.qualification.options();
                if (field.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationLookup) {

                    var j = ko.utils.arrayFilter(qualificationOptions, function (option) {
                        if (option.Id === field.FieldOptionId()) {
                            return option;
                        }
                    });
                    endorsedQualificationOptions.qualificationNameValue = j[0].DisplayName;

                    endorsedQualificationOptions.ProcessFiltering();

                    //////setting up the final value to be store in db
                    ko.utils.arrayForEach(self.fields(), function (item) {
                        if (item.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationIdText) {
                            item.Value = self.endorsedQualificationId();
                        }
                    });
                }
            };

            endorsedQualificationOptions.ProcessFiltering = function () {
                var institutionNaatiNumberValue = endorsedQualificationOptions.institution.value();
                var locationNameValue = endorsedQualificationOptions.location.value();
                var qualificationNameValue = endorsedQualificationOptions.qualificationNameValue;

                var endorsedQualificationData = self.endorsedQualificationData();

                endorsedQualificationData = ko.utils.arrayFilter(endorsedQualificationData,
                    function (data) {
                        return data.InstitutionNaatiNumber === institutionNaatiNumberValue;
                    });

                endorsedQualificationData = ko.utils.arrayFilter(endorsedQualificationData,
                    function (data) {
                        return data.Location === locationNameValue;
                    });

                endorsedQualificationData = ko.utils.arrayFilter(endorsedQualificationData,
                    function (data) {
                        return data.Qualification === qualificationNameValue;
                    });

                if (endorsedQualificationData.length) {
                    self.endorsedQualificationId(endorsedQualificationData[0].EndorsedQualificationId);
                }
            };

        }

        function updateYesNoComponent(value, $target, fieldEnable) {
            if (value === null) {
                $target.prop('indeterminate', true);
                $target.prop('checked', false);
            }
            else if (value === 'True') {
                $target.prop('checked', true);
                $target.prop('indeterminate', false);
            }
            else {
                $target.prop('checked', false);
                $target.prop('indeterminate', false);
            }

            if (fieldEnable) {
                $target.prop("disabled", false);
            } else {
                $target.prop("disabled", true);
            }
        }
    }

    return ViewModel;
});