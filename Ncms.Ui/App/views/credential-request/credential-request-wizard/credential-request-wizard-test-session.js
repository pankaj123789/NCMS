define([
        'modules/common',
        'modules/custom-validator',
        'modules/enums',
        'services/screen/date-service',
        'services/credentialrequest-data-service'
],
    function (common, customValidator,  enums, dateService, credentialrequestService) {
        var functions = common.functions();

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
                Id: ko.observable(),
                Capacity: ko.observable(),
                AllowSelfAssign: ko.observable(),
                Skills: ko.observable()
            };

            var emptyModel = ko.toJS(serverModel);

            var validator = customValidator.create(serverModel);

            var vm = {
                summary: defaultParams.summary,
                session: serverModel,
                testSessions: ko.observableArray(),
                venues: ko.observableArray(),
                readOnly: ko.observable(false),
                tableDefinition: {
                    columnDefs: [
                        {
                            orderable: false,
                            className: 'select-checkbox',
                            targets: 0
                        },
                        {
                            targets: 3,
                            render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
                        }
                    ],
                    order: [
                        [1, "asc"]
                    ],
                    select: {
                        style: 'single',
                        info: false
                    },
                    events: {
                        select: selectTestSession,
                        deselect: selectTestSession,
                        'user-select': function (e) {
                            if (vm.readOnly()) {
                                e.preventDefault();
                            }
                        }
                    }
                },
            };

            vm.isValid = function () {
                var defer = Q.defer();
                var request = vm.postData();
                $.extend(request, vm.summary.Request());

                credentialrequestService.post(request, 'testsession').then(function (data) {
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
                credentialrequestService.getFluid('testsession', vm.summary.Request()).then(vm.testSessions);
                credentialrequestService.getFluid('venues', vm.summary.Request()).then(vm.venues);
            };

            vm.postData = function () {
                var json = ko.toJS(serverModel);
                return json;
            };


            function selectTestSession(e, dt) {
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
                    return serverModel.Id(null);
                }

                validator.reset();
                var testSession = vm.testSessions()[indexes[0]];
                serverModel.Id(testSession.Id);
                serverModel.Capacity(testSession.Capacity);
                serverModel.AllowSelfAssign(testSession.AllowSelfAssign);
                serverModel.Skills(testSession.Skills);
                ko.viewmodel.updateFromModel(defaultParams.selectedTestSession, serverModel);
            }

            return vm;
        }
    });