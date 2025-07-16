define([
    'services/examiner-data-service',
    'services/language-data-service'
], function (
    examinerService,
    languageService
) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;
        var chairSuffix = ko.Localization('Naati.Resources.Roles.resources.Chair');

        var vm = {
            mode: ko.observable(),
            languageId: ko.observable(),
            englishLanguageId: null,
            examinersOptions: ko.observableArray(),
            editing: ko.observable(getEditing({})),
            showEdit: showEdit,
            showAdd: showAdd,
            save: save
        };

        vm.primaryContact = ko.pureComputed({
            read: function () {
                return vm.editing().PrimaryContact();
            },
            write: function (value) {
                vm.editing().PrimaryContact(value);
            }
        });

        var validation = ko.pureComputed(function () {
            return ko.validatedObservable(vm.editing());
        });

        languageService.getFluid('English').then(function (data) {
            vm.englishLanguageId = data.LanguageId;
        });

        return vm;

        function save() {
            if (!validation().isValid()) {
                validation().errors.showAllMessages();
                return;
            }

            var json = ko.toJS(vm.editing());

            var grepped = $.grep(vm.examinersOptions(), function (e) {
                return e.EntityId === json.EntityId;
            });

            var examiner = null;

            if (grepped.length > 0) {
                examiner = grepped[0];
            }

            if (examiner) {
                json.PersonName = examiner.PersonName;
                $.extend(examiner, json);
            }
            else {
                examiner = json;
            }

            examiner.ExaminerCost = parseFloat(examiner.ExaminerCost);

            defer.resolve(examiner);
            $('#examinerModal').modal('hide');
        }

        function getEditing(examiner) {
            return {
                EntityId: ko.observable(examiner.EntityId).extend({ required: true }),
                ExaminerCost: ko.observable(examiner.ExaminerCost).extend({ required: true }),
                PersonName: ko.observable(examiner.PersonName),
                PrimaryContact: ko.observable(examiner.PrimaryContact || false)
            };
        }

        function loadExaminersOptions() {
            examinerService.get({ request: JSON.stringify({ LanguageId: [vm.languageId(), vm.englishLanguageId] }) }).then(function (data) {
                ko.utils.arrayForEach(data, function (membership) {
                    membership.PersonNameSuffixed = membership.PersonName;
                    if (membership.IsChair) {
                        membership.PersonNameSuffixed += ' (' + chairSuffix + ')';
                    }
                });

                vm.examinersOptions(data);
            });
        }

        function showEdit(examiner) {
            defer = Q.defer();

            vm.mode('edit');
            vm.languageId(0);

            var json = ko.toJS(examiner);

            // Default primary contact value to true
            json.PrimaryContact = json.PrimaryContact === null ? true : json.PrimaryContact;

            vm.editing(getEditing(json));
            validation().errors.showAllMessages(false);

            $('#examinerModal').modal('show');

            return defer.promise;
        }

        function showAdd(languageId) {
            defer = Q.defer();

            vm.mode('add');
            vm.languageId(languageId);
            loadExaminersOptions();

            vm.editing(getEditing({}));
            validation().errors.showAllMessages(false);

            $('#examinerModal').modal('show');

            return defer.promise;
        }
    }
});
