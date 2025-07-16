define([
    'services/application-data-service',
    'services/institution-data-service'
], function (applicationService, institutionService) {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        //---------------------------------------------
        self.endorsedQualifications = ko.observableArray();
        function parseSearchTerm() {
            var json = {
                InstitutionNaatiNumberIntList: null,
                LocationString: null,
                QualificationString: null,
                CredentialTypeIntList: null,
                EndorsementFromString: null,
                EndorsementToString: null
            };
            return JSON.stringify(json);
        }

        self.loadEndorsedQualifications = function () {
            var defer = Q.defer;
            institutionService.getFluid('searchEndorsedQualifications', { request: { Skip: null, Take: null, Filter: parseSearchTerm() } }).then(
                function (data) {
                    self.endorsedQualifications(data);
                    defer.resolve(true);
                    return defer.promise;
                });
        };

        self.loadEndorsedQualifications();
        
        self.qualificationNames = ko.observableArray();
        self.institutionNaatiNumbers = ko.observableArray();
        self.locationNames = ko.observableArray();
        //----------------------------------------------

        self.institution = params.Institution || {};
        self.institution.selectedOptions = self.institution.selectedOptions || [];
        self.institution.selectedOptions = ko.observableArray(self.institution.selectedOptions);
        self.institution.options = ko.observableArray();
        self.institution.selectedOptions.subscribe(function () { self.LocationloadOptions(); });
        self.institution.options.subscribe(function () { self.LocationloadOptions(); });

        self.location = params.Location || {};
        self.location.selectedOptions = self.location.selectedOptions || [];
        self.location.selectedOptions = ko.observableArray(self.location.selectedOptions);
        self.location.options = ko.observableArray();
        self.location.selectedOptions.subscribe(function () { self.QualificationloadOptions(); });
        self.location.options.subscribe(function () { self.QualificationloadOptions(); });

        self.qualification = params.Qualification || {};
        self.qualification.selectedOptions = self.qualification.selectedOptions || [];
        self.qualification.selectedOptions = ko.observableArray(self.qualification.selectedOptions);
        self.qualification.options = ko.observableArray();
        
        self.endorsedQualificationIds = params.EndorsedQualificationIds || {};
        self.endorsedQualificationIds.selectedOptions = self.endorsedQualificationIds.selectedOptions || [];
        self.endorsedQualificationIds.selectedOptions = ko.observableArray(self.endorsedQualificationIds.selectedOptions);
        self.endorsedQualificationIds.options = ko.observableArray();

        self.endorsementPeriod = $.extend(params.EndorsementPeriod || {}, { allowNone: true });
        
        var paraminstitutionNaatiNumbers = self.institution.selectedOptions();

        self.LocationloadOptions = function () {

            var defer = Q.defer;
            self.qualification.selectedOptions([]);
            self.location.selectedOptions([]);

            var institutionNaatiNumbers = self.institution.selectedOptions();
            if (!institutionNaatiNumbers.length) {
                institutionNaatiNumbers = ko.utils.arrayMap(self.institution.options(), function (item) {
                    return item.Id;
                });
            }
            if (!institutionNaatiNumbers.length) {
                return;
            }

            paraminstitutionNaatiNumbers = institutionNaatiNumbers;
            applicationService.getFluid('GetEndorsementLocationLookup/', { request: { InstitutionNaatiNumbers: institutionNaatiNumbers } }).then(
                function (data) {
                    self.location.options(data);
                    defer.resolve(true);
                    return defer.promise;
                });
        };

        self.QualificationloadOptions = function () {

            var defer = Q.defer;
            self.qualification.selectedOptions([]);

            var locations = self.location.selectedOptions();
            if (!locations.length) {
                locations = ko.utils.arrayMap(self.location.options(),
                    function (item) {
                        return item.DisplayName;
                    });
            } else {
                locations = ko.utils.arrayMap(locations,
                    function (item) {
                        var j = ko.utils.arrayFilter(self.location.options(), function (option) {
                            if (option.Id === item) {
                                return option;
                            }
                        });
                        return j[0].DisplayName;
                    });
            }

            if (!locations.length) {
                return;
            }

            applicationService.post({ Locations: locations, InstitutionNaatiNumbers: paraminstitutionNaatiNumbers }, 'GetEndorsementQualificationLookup').then(
                function (data) {
                    self.qualification.options(data);
                    defer.resolve(true);
                    return defer.promise;
                });

        };

        self.processFilter = function () {

            var institutions = self.institution.selectedOptions();
            if (!institutions.length) {
                ko.utils.arrayForEach(self.institution.options(),
                    function(item) {
                        self.institutionNaatiNumbers.push(item.Id);
                    });
                institutions = self.institutionNaatiNumbers();
            }

            var locations = self.location.selectedOptions();
            if (!locations.length) {
                ko.utils.arrayForEach(self.location.options(),
                    function(item) {
                        self.locationNames.push(item.DisplayName);
                    });
                locations = self.locationNames();
            } else {
                locations = ko.utils.arrayMap(locations,
                    function(item) {
                        var j = ko.utils.arrayFilter(self.location.options(),
                            function(option) {
                                if (option.Id === item) {
                                    return option;
                                }
                            });
                        return j[0].DisplayName;
                    });
            }

            var qualifications = self.qualification.selectedOptions();
            if (!qualifications.length) {
                ko.utils.arrayForEach(self.qualification.options(),
                    function(item) {
                        self.qualificationNames.push(item.DisplayName);
                    });
                qualifications = self.qualificationNames();
            } else {
                qualifications = ko.utils.arrayMap(qualifications,
                    function(item) {
                        var j = ko.utils.arrayFilter(self.qualification.options(),
                            function(option) {
                                if (option.Id === item) {
                                    return option;
                                }
                            });
                        return j[0].DisplayName;
                    });
            }

            //Filtering
            var endorsedQualificationsData = self.endorsedQualifications();

            ko.computed(function() {
                var filteredArray = ko.utils.arrayFilter(endorsedQualificationsData,
                    function(data) {
                        return institutions.indexOf(data.InstitutionNaatiNumber) !== -1;
                    });
                endorsedQualificationsData = filteredArray;
            });

            ko.computed(function() {
                var filteredArray = ko.utils.arrayFilter(endorsedQualificationsData,
                    function(data) {
                        return qualifications.indexOf(data.Qualification) !== -1;
                    });
                endorsedQualificationsData = filteredArray;
            });

            ko.computed(function() {
                var filteredArray = ko.utils.arrayFilter(endorsedQualificationsData,
                    function(data) {
                        return locations.indexOf(data.Location) !== -1;
                    });
                endorsedQualificationsData = filteredArray;
            });

            self.endorsedQualificationIds.selectedOptions([]);
            ko.utils.arrayForEach(endorsedQualificationsData,
                function(item) {
                    self.endorsedQualificationIds.selectedOptions.push(item.EndorsedQualificationId);
                });
        };

        self.getJson = function () {

            self.processFilter();
            return {

                Institution: self.institution.component.getJson(),
                Qualification: self.qualification.component.getJson(),
                Location: self.location.component.getJson(),
                EndorsementPeriod: self.endorsementPeriod.component.getJson(),
                EndorsementQualificationIds: self.endorsedQualificationIds.component.getJson()
            };
        };
        
        self.QualificationloadOptions();
        self.LocationloadOptions();
    }

    return ViewModel;
});