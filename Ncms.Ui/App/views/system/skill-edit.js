define([
    'services/setup-data-service',
    'views/system/skill-details',
    'modules/enums'
],
    function (setupService, skillDetails, enums) {

        var serverModel = {
            SkillId: ko.observable(),
            SkillTypeId: ko.observable().extend({ required: true }),
            Language1Id: ko.observable().extend({ required: true }),
            Language2Id: ko.observable(),
            DirectionTypeId: ko.observable().extend({ required: true }),
            NumberOfExistingCredentials: ko.observable(),
            NumberOfCredentialRequests: ko.observable(),
            CredentialApplicationTypeId: ko.observableArray(),
            Note: ko.observable().extend({ maxLength: 500 })
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            skill: serverModel,
            title: ko.pureComputed(function () {
                return '{0} - #{1}'.format(ko.Localization('Naati.Resources.Skill.resources.EditSkill'), serverModel.SkillId());
            }),
            languages: ko.observableArray(),
            skillTypes: ko.observableArray(),
            directions: ko.observableArray()
        };

        serverModel.Language2Id.extend({
            required: {
                onlyIf: function () {
                    var direction = ko.utils.arrayFirst(vm.directions(), function (d) {
                        return d.Id === serverModel.DirectionTypeId();
                    });

                    if (!direction) {
                        return false;
                    }

                    return direction.DisplayName.indexOf('[Language 2]') > -1;
                }
            }
        });

        var isReadOnly = ko.computed(function () {
            return parseInt(serverModel.NumberOfCredentialRequests());
        });

        var skillDetailsInstance = skillDetails.getInstance(serverModel, isReadOnly);
        skillDetailsInstance.load();

        vm.skillDetailsOptions = {
            view: 'views/system/skill-details.html',
            model: skillDetailsInstance
        };

        vm.name = skillDetailsInstance.name;

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.Skill, enums.SecVerb.Update) ;
        });

        var validation = ko.validatedObservable(serverModel);

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);

            setupService.post(request, 'skill')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.canActivate = function (id, query) {

            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.SkillId(id);

            return loadSkill();
        };

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadSkill() {
            return setupService.getFluid('skill/' + serverModel.SkillId()).then(function (data) {
                data.CredentialApplicationTypeId = ko.utils.arrayMap(data.ApplicationTypes, function (at) {
                    return at.Id;
                });

                ko.viewmodel.updateFromModel(serverModel, data);

                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        return vm;
    });

