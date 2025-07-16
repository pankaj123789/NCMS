define(['services/requester', 'views/logbook/logbook-attachments'],
    function (requester, attachments) {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var logbook = new requester('logbook');

            var serverModel = {
                Id: ko.observable(),
                DateCompleted: ko.observable().extend({ required: true }),
                Description: ko.observable().extend({ required: true, maxLength: 200 }),
                ProfessionalDevelopmentCategoryId: ko.observable().extend({ required: true }),
                ProfessionalDevelopmentRequirementId: ko.observable().extend({ required: true }),
                Notes: ko.observable().extend({ maxLength: 4000 }),
            };

            serverModel.ProfessionalDevelopmentCategoryId.subscribe(loadRequirements);

            var attachmentsInstance = attachments.getInstance({ activityId: serverModel.Id, editable : options.editable  });
            var emptyModel = ko.toJS(serverModel);
            var validation = ko.validatedObservable(serverModel);

            var vm = {
                activity: serverModel,
                categories: ko.observableArray(),
                requirements: ko.observableArray(),
                editable: options.editable
            };
          
            vm.requirement = ko.pureComputed(function () {
                return ko.utils.arrayFirst(vm.requirements(), function (r) {
                    return r.Id === serverModel.ProfessionalDevelopmentRequirementId();
                }) || {};
            });

            vm.category = ko.pureComputed(function () {
                return ko.utils.arrayFirst(vm.categories(), function (r) {
                    return r.Id === serverModel.ProfessionalDevelopmentCategoryId();
                }) || {};
            });

            logbook.getFluid('categories').then(vm.categories);

            vm.attachmentsOptions = {
                view: 'views/logbook/logbook-attachments',
                model: attachmentsInstance
            };

            vm.categoryOptions = {
                value: serverModel.ProfessionalDevelopmentCategoryId,
                multiple: false,
                disable: ko.computed(function () { return !vm.editable() || !vm.categories().length; }),
                options: vm.categories,
                optionsValue: 'Id',
                optionsText: 'Name',
                selectIfSingle: true
            };

            vm.requirementOptions = {
                value: serverModel.ProfessionalDevelopmentRequirementId,
                multiple: false,
                disable: ko.computed(function () { return !vm.editable() || !vm.requirements().length; }),
                options: vm.requirements,
                optionsValue: 'Id',
                optionsText: 'Name',
                selectIfSingle: true,
                multiselect: {
                    optionClass: function () { return 'w-s-normal'; }
                }
            };

            var defer = null;
            vm.add = function () {
                return edit(emptyModel);
            };

            vm.edit = function (activity) {
                var editModel = $.extend({}, activity);
                editModel.DateCompleted = moment.utc(editModel.DateCompleted).toDate();
                return edit(editModel);
            };

            vm.saveActivity = function () {
                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    return;
                }

                var request = ko.toJS(serverModel);
                logbook.post(request, 'createorupdateprofessionaldevelopmentactivity').then(function (data) {
                    serverModel.Id(data.Id);
                    attachmentsInstance.upload().then(function () {
                        $('#addActivityModal').modal('hide');
                        toastr.success('Activity saved.');
                        defer.resolve(data);
                    });
                });
            };

            function edit(activity) {
                defer = Q.defer();

                ko.viewmodel.updateFromModel(serverModel, activity);
                $('#addActivityModal').modal('show');
                attachmentsInstance.load();
                clearValidation();

                return defer.promise;
            }

            function clearValidation() {
                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }
            }

            function loadRequirements(categoryId) {
                if (!categoryId) {
                    return vm.requirements([]);
                }
                serverModel.ProfessionalDevelopmentRequirementId(null);
                logbook.getFluid('requirements', { categoryId: categoryId }).then(vm.requirements);
            }

            return vm;
        }
    }
);