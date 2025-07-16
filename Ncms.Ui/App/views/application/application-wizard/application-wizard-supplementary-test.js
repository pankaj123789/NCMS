define([
    'modules/custom-validator',
    'services/application-data-service',
    'services/test-data-service',
    'modules/enums'
], function (customValidator, applicationService, testService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null
        };

        $.extend(defaultParams, params);

        var testModel = {
            TestMarkingTypeId: ko.observable(),
            TestResultId: ko.observable(),
            Components: ko.observableArray()
        };

        var serverModel = {
            Components: ko.observableArray()
        };

        var validator = customValidator.create(serverModel);

        var vm = {
            test: testModel,
            specification: serverModel,
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            isRubric: ko.computed(function () {
                return testModel.TestMarkingTypeId() === enums.MarkingSchemaType.Rubric;
            })
        };

        ko.computed(function () {
            var components = [];

            ko.utils.arrayForEach(testModel.Components(), function (c) {
                if (c.Resit()) {
                    components.push(c.Id());
                }
            });

            serverModel.Components(components);
        });

        vm.load = function () {

            applicationService.getFluid('wizard/supplementarytest/'+ vm.credentialRequest.Id()).then(function (data) {
                ko.utils.arrayForEach(data.Components, function (d) {
                    d.Requirements = " ({0} / {1})".format(d.PassMark, d.TotalMarks);
                    d.WasAttempted = false;
                    d.Successful = false;
                    d.Resit = false;
                });

                ko.viewmodel.updateFromModel(testModel, data);
                loadRubric();
            });
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        var validation = ko.validatedObservable(serverModel);
        vm.isValid = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                defer.resolve(false);
                return defer.promise;
            }

            var request = ko.toJS(serverModel);
            $.extend(request,
            {
                ApplicationId: vm.application.ApplicationId(),
                Action: vm.action().Id,
                CredentialRequestId: vm.credentialRequest.Id()
            });
            applicationService.post(request, 'wizard/supplementarytest').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields,
                    function (i) {
                        validator.setValidation(i.FieldName, false, i.Message);
                    });

                validator.isValid();

                defer.resolve(!data.InvalidFields.length);
            });

            return defer.promise;
        };

        vm.toggleSelection = function (component) {
            component.Resit(!component.Resit());
        };

        function loadRubric() {
            testService.getFluid('rubricfinal/' + testModel.TestResultId()).then(function (data) {
                ko.utils.arrayForEach(testModel.Components(), function (component) {
                    var rubricResult = ko.utils.arrayFirst(data.TestComponents, function (d) {
                        return d.Id === component.Id();
                    });

                    if (rubricResult) {
                        var isResit = (rubricResult.WasAttempted && !rubricResult.Successful) ? true : false;
                        component.Successful(rubricResult.Successful);
                        component.WasAttempted(rubricResult.WasAttempted);
                        component.Resit(isResit);
                    }
                });
            });
        }

        return vm;
    }
});