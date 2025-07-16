define([], function () {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        var serverModel = {
            CategoryId: ko.observable().extend({ required: true }),
            LevelId: ko.observable(),
            CredentialTypes: ko.observable().extend({ required: true }),
            SkillId: ko.observable().extend({ required: true }),
            PathTypeId: ko.observable()
        };

        var cleanModel = ko.toJS(serverModel);

        serverModel.CategoryId.subscribe(selectCategory);
        serverModel.CredentialTypes.subscribe(selectLevel);
        serverModel.SkillId.subscribe(selectSkill);

        self.request = serverModel;
        self.credentialRequests = params.credentialRequests;
        self.categories = ko.observableArray();
        self.levels = ko.observableArray();
        self.skills = ko.observableArray();
        self.addingCredentialRequest = ko.observable(false);
        self.duplicatedCredentialRequestMessage = ko.observable();
        self.credentialsPromise = params.credentialsPromise;
        self.categoriesPromise = params.categoriesPromise;
        self.deleteCredentialPromise = params.deleteCredentialPromise;
        self.createCredentialPromise = params.createCredentialPromise;
        self.levelsPromise = params.levelsPromise;
        self.skillsPromise = params.skillsPromise;
        self.validation = params.validation;

        var newCredentialRequestValidation = ko.validatedObservable(serverModel);


        self.newCredentialRequest = function () {
            ko.viewmodel.updateFromModel(serverModel, cleanModel);
            resetCredentialValidation();
            self.levels([]);
            self.skills([]);

            $('#addCredentialRequestModal').modal('show');
        };

        self.removeCredentialRequest = function (credentialRequest) {
            mbox.remove({ title: 'Remove credential request', content: 'Are you sure that you want to remove this application request?' }).then(function (answer) {
                if (answer === 'yes') {
                    self.deleteCredentialPromise(credentialRequest).then(function () {
                        self.credentialRequests.remove(credentialRequest);
                        resetValidation();
                    });
                }
            })
        };

        self.addCredentialRequest = function () {
            if (!newCredentialRequestValidation.isValid()) {
                return newCredentialRequestValidation.errors.showAllMessages();
            }


            var hasDuplicated = ko.utils.arrayFirst(self.credentialRequests(), function (c) {
                var data = ko.toJS(serverModel);
                return c.CategoryId === data.CategoryId && c.LevelId === data.LevelId && c.SkillId === data.SkillId;
            });

            if (hasDuplicated) {
                return self.duplicatedCredentialRequestMessage('This credential is already added.');
            }

            self.addingCredentialRequest(true);
            self.duplicatedCredentialRequestMessage('');

            var req = request();
            $.extend(req, ko.toJS(serverModel));

            self.createCredentialPromise(req).then(function (data) {
                var newCredential = ko.toJS(serverModel);
                newCredential.Id = data.Id;
                newCredential.PathId = data.PathTypeId;
                newCredential.Category = ko.utils.arrayFirst(self.categories(), function (d) { return d.Id === newCredential.CategoryId }).DisplayName;
                newCredential.Level = ko.utils.arrayFirst(self.levels(), function (d) { return d.Id === newCredential.LevelId }).DisplayName;
                newCredential.Skill = ko.utils.arrayFirst(self.skills(), function (d) { return d.Id === newCredential.SkillId }).DisplayName;
                self.credentialRequests.push(newCredential);
                self.addingCredentialRequest(false);
                $('#addCredentialRequestModal').modal('hide');
            });
        };

        self.computedLevels = ko.pureComputed(function () {
            var levels = self.levels();
            var distinct = [];

            for (var i = 0; i < levels.length; i++) {
                var level = levels[i];
                if (typeof (distinct[level.DisplayName]) === 'undefined' || distinct[level.DisplayName] == level.Id) {
                    distinct[level.DisplayName] = level.Id;
                }
                else {
                    distinct[level.DisplayName] += ',' + level.Id;
                }
            }

            var result = [];
            for (var displayName in distinct) {
                result.push({ Id: distinct[displayName], DisplayName: displayName })
            }

            return result;
        });

        self.categories.subscribe(function (options) { setIfSingle(options, serverModel.CategoryId); });
        self.computedLevels.subscribe(function (options) { setIfSingle(options, serverModel.CredentialTypes); });
        self.skills.subscribe(function (options) { setIfSingle(options, serverModel.SkillId); });

        $.extend(self, {
            categoryOptions: {
                value: serverModel.CategoryId,
                multiple: false,
                disable: ko.computed(function () { return !self.categories().length; }),
                options: self.categories,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            },
            levelOptions: {
                value: serverModel.CredentialTypes,
                multiple: false,
                disable: ko.computed(function () { return !self.levels().length; }),
                options: self.computedLevels,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            },
            skillOptions: {
                value: serverModel.SkillId,
                multiple: false,
                multiselect: { enableFiltering: true },
                disable: ko.computed(function () { return !self.skills().length; }),
                options: self.skills,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            },
        });

        function load() {
            resetValidation();

            ko.viewmodel.updateFromModel(serverModel, cleanModel);

            self.credentialsPromise().then(function (data) {
                self.credentialRequests(data);
                resetValidation();
            });

            self.categoriesPromise().then(self.categories);

            if (self.validation.errors) {
                self.validation.errors.showAllMessages(false);
            }
        };

        function resetCredentialValidation() {
            if (newCredentialRequestValidation.errors) {
                newCredentialRequestValidation.errors.showAllMessages(false);
            }
            self.duplicatedCredentialRequestMessage('');
            self.addingCredentialRequest(false);
        }

        function resetValidation() {
            if (self.validation.errors) {
                self.validation.errors.showAllMessages(false);
            }
        }

        function setIfSingle(options, observable) {
            resetCredentialValidation();
            if (options.length === 1) {
                observable(options[0].Id);
            }
        }

        function selectCategory(category) {
            serverModel.CredentialTypes(null);

            if (!category) {
                return self.levels([]);
            }

            self.levelsPromise().then(self.levels);
        }

        function selectLevel(level) {
            if (!level) {
                return;
            }

            self.skillsPromise().then(self.skills);
        }

        function selectSkill() {
            var skillId = serverModel.SkillId();
            if (!skillId) {
                return serverModel.LevelId(skillId);
            }

            var skill = ko.utils.arrayFirst(self.skills(), function (s) {
                return s.Id === skillId;
            });

            if (!skill) {
                return serverModel.LevelId(skill);
            }

            serverModel.LevelId(skill.CredentialTypeId);
        }

        function request() {
            var request = {
                CategoryId: serverModel.CategoryId(),
                LevelId: serverModel.LevelId(),
                CredentialTypes: serverModel.CredentialTypes() || '',
                SkillId: serverModel.SkillId(),
            };

            return request;
        }

        load();
    }

    return ViewModel;
});