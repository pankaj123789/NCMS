define([
    'modules/common',
    'modules/custom-validator',
    'services/testsession-data-service',
    'services/screen/date-service',
], function (common, customValidator, testSessionService, dateService) {
    var functions = common.functions();

    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            session: null
        };

        $.extend(defaultParams, params);

        var vm = {
            session: defaultParams.session,
            rehearsalDetails: {
                RehearsalDate: ko.observable(),
                RehearsalTime : ko.observable(new Date()),
                RehearsalNotes : ko.observable()
            }
        };
        ko.viewmodel.updateFromModel(vm.rehearsalDetails, ko.toJS(vm.session));
        var validator = customValidator.create(vm.rehearsalDetails);

        vm.rehearsalDateOptions = {
            value: vm.rehearsalDetails.RehearsalDate,
            css: 'form-control'
        };

        vm.isValid = function () {
            var defer = Q.defer();
            ko.viewmodel.updateFromModel(vm.session, ko.toJS(vm.rehearsalDetails));
            var request = ko.toJS(vm.session);
            request.RehearsalDate = request.RehearsalDate ? dateService.toPostDate(request.RehearsalDate) : null;
            request.TestDate = request.TestDate ? dateService.toPostDate(request.TestDate) : null;
            testSessionService.post(request, 'wizard/rehearsalDetails').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields,
                    function (i) {
                        validator.setValidation(i.FieldName, false, i.Message);
                    });

                validator.isValid();

                var isValid = !data.InvalidFields.length;
                defer.resolve(isValid);
            });

            return defer.promise;
        };

        vm.load = function () {
            validator.reset();
        };

        return vm;
    }
});