define(['services/requester', 'views/logbook/logbook-work-practice-attachments'],
    function (requester, attachments) {
        return {
            getInstance: getInstance
        };

        function getInstance(credential, editable) {
            var logbook = new requester('logbook');

            var serverModel = {
                Id: ko.observable(),
                CredentialId: ko.observable().extend({ required: true }),
                Date: ko.observable().extend({ required: true }),
                Points: ko.observable().extend({
                    required: true, 
                    number: {
                        message: 'Please enter numbers only'
                    }
                }),
                Description: ko.observable().extend({ required: true, maxLength: 200 }),
            };

            var attachmentsInstance = attachments.getInstance({ workPracticeId: serverModel.Id, editable: editable });
            var emptyModel = ko.toJS(serverModel);
            var validation = ko.validatedObservable(serverModel);

            var vm = {
                credential: credential,
                workPractice: serverModel,
                editable: editable
            };

            vm.attachmentsOptions = {
                view: 'views/logbook/logbook-work-practice-attachments',
                model: attachmentsInstance
            };

            var defer = null;
            vm.add = function () {
                return edit(emptyModel);
            };

            vm.edit = function (workPractice) {
                var editModel = $.extend({}, workPractice);
                editModel.Date = moment.utc(editModel.Date).toDate();
                return edit(editModel);
            };

            vm.save = function () {
                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    return;
                }

                var request = ko.toJS(serverModel);
                logbook.post(request, 'createorupdateworkpractice').then(function (data) {
                    serverModel.Id(data.Id);
                    attachmentsInstance.upload().then(function () {
                        $('#addWorkPracticeModal').modal('hide');
                        toastr.success('Work practice saved.');
                        defer.resolve(data);
                    });
                });
            };

            function edit(workPractice) {
                defer = Q.defer();

                workPractice.CredentialId = vm.credential.Id();
                ko.viewmodel.updateFromModel(serverModel, workPractice);
                $('#addWorkPracticeModal').modal('show');
                attachmentsInstance.load();
                clearValidation();

                return defer.promise;
            }

            function clearValidation() {
                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }
            }

            return vm;
        }
    }
);