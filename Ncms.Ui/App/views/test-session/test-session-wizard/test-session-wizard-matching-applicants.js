define([
        'services/testsession-data-service',
        'modules/enums',
],
    function (testsessionService, enums) {
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
                visibleSteps: defaultParams.visibleSteps,
                selectedIds: ko.observableArray(),
                applicants: ko.observableArray(),
                getStep: defaultParams.getStep,
                scrollStep: defaultParams.scrollStep,
                skillIds: ko.observableArray([])
        };

            vm.wizardNewApplicantsOptions = {
                selectedIds: vm.selectedIds,
                applicants: vm.applicants,
                allowSelfAssign: vm.session.AllowSelfAssign,
                venueCapacity: vm.session.Capacity,
                fillSeats: false,
                isValidPromise: function () {
                    return testsessionService.post(vm.postData(), 'wizard/matchingapplicants');
                },
                skillIds: vm.skillIds
             
            };
            
            vm.load = function () {
                var session = ko.toJS(vm.session);
                var skills = vm.getStep("wizardSkills").compose.model.postData().Skills;
                var skillIds = [];
                ko.utils.arrayForEach(skills,
                    function (skill) {
                        if (skill.Selected) {
                            skillIds.push(skill.SkillId);
                        }
                    });
                session.SkillIds = skillIds;
                vm.skillIds(skillIds);
                testsessionService.post(session, 'wizard/getmatchingapplicants').then(vm.applicants);
            };

            vm.isValid = function () {
                return vm.wizardNewApplicantsOptions.component.isValid();
            };

            vm.postData = function () {
                return { Applicants: vm.selectedIds() };
            };

            return vm;
        }
    });