define([
    'modules/enums',
    'modules/collapser',
    'services/application-data-service',
    'services/screen/date-service'
],
    function (enums, collapser, applicationService, dateService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                sections: ko.observableArray(),
                applicationId: ko.observable(),
                valid: ko.observable(true)
            };

            vm.collapser = collapser.create(vm.sections);

            var dirtyFlag = new ko.DirtyFlag([vm.sections], false);

            vm.isDirty = ko.pureComputed(function () {
                return dirtyFlag().isDirty();
            });

            vm.validate = function () {
                vm.valid(ko.utils.arrayFilter(vm.sections(), function (section) {
                    return !section.applicationFieldsOptions.component.validate();
                }).length === 0);

                return vm.valid();
            };

            vm.clearValidation = function () {
                $.each(vm.sections(), function (i, section) {
                    section.applicationFieldsOptions.component.clearValidation();
                });
                vm.valid(true);
            };

            vm.resetDirtyFlag = function () {
                dirtyFlag().reset();
            };

            vm.load = function (applicationId) {
                vm.applicationId(applicationId);
                applicationService.getFluid(vm.applicationId() + '/header').then(function (data) {
                    var sections = ko.viewmodel.fromModel(data);

                    $.each(sections(), function (i, section) {
                        ko.utils.arrayForEach(section.Fields(), function (field) {
                            if ((field.DataTypeId() === enums.ApplicationFieldTypes.Date || field.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationStartDate || field.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationEndDate) && field.Value()) {

                                var date = dateService.toUIDate(field.Value());
                                field.Value(date);
                            }
                            field.Value.extend({ required: field.Mandatory() });
                        });

                        section.valid = ko.observable(true);
                        section.applicationFieldsOptions = {
                            fields: section.Fields,
                            valid: section.valid,
                            changed: function () {
                                dirtyFlag().reset();
                            }
                        };
                    });

                    vm.sections(sections());
                    dirtyFlag().reset();
                });
            };
            
            vm.sectionsToPostDate = function() {
                $.each(vm.sections(), function (i, section) {
                    ko.utils.arrayForEach(section.Fields(), function (field) {
                        if ((field.DataTypeId() === enums.ApplicationFieldTypes.Date || field.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationStartDate || field.DataTypeId() === enums.ApplicationFieldTypes.EndorsedQualificationEndDate) && field.Value()) {

                            var date = dateService.toPostDate(field.Value());
                            field.Value(date);
                        }
                    });
                });
            };

            vm.save = function() {
                var defer = Q.defer();
                var promise = defer.promise;

                vm.sectionsToPostDate();

                var models = ko.viewmodel.toModel(vm.sections);
                applicationService.post({ ApplicationId: vm.applicationId(), Sections: models }, 'sections').then(
                    function() {
                        defer.resolve('fulfilled');
                    });

                return promise;
            };

            return vm;
        }
    });
