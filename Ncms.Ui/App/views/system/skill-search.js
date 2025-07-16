define([
	'plugins/router',
	'views/shell',
	'modules/enums',
	'services/setup-data-service',
	'views/system/skill-create'
], function (router, shell, enums, setupService, skillCreate) {
	var searchType = enums.SearchTypes.Skill;
	var searchTerm = ko.observable({});
	var tableId = 'skillTable';
	var tableDefinition = {
		id: tableId,
		headerTemplate: 'skills-header-template',
		rowTemplate: 'skills-row-template'
	};

	var vm = {
		searchOptions: {
			name: 'search-component',
			params: {
				title: shell.titleWithSmall,
				filters: [
					{ id: 'SkillType' },
					{ id: 'Language' },
					{ id: 'DirectionType' },
					{ id: 'ApplicationType' },
				],
				searchType: searchType,
				searchTerm: searchTerm,
				searchCallback: searchCallback,
				tableDefinition: tableDefinition,
				additionalButtons: [{
					'class': 'btn btn-success',
					icon: 'glyphicon glyphicon-plus',
					resourceName: 'Naati.Resources.Skill.resources.NewSkill',
                    click: create,
                    disable: !currentUser.hasPermissionSync(enums.SecNoun.Skill, enums.SecVerb.Create),
                }],
                idSearchOptions: {
                    search: function (value) {
                        var filter = JSON.stringify({ SkillIdIntList: [value] });
                        setupService.getFluid('skill', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                            .then(function (data) {
                                if (!data || !data.length) {
                                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Skill', value));
                                }
                                router.navigate('system/skill/' + value);
                            });
                    }
                }
			}
		},
		skills: ko.observableArray([])
	};

	var skillCreateInstance = skillCreate.getInstance();

	vm.skillCreateOptions = {
		view: 'views/system/skill-create.html',
		model: skillCreateInstance
	};

	tableDefinition.dataTable = {
		source: vm.skills,
		order: [
			[0, "asc"],
		]
	};

	function create() {
		skillCreateInstance.show().then(function (data) {
			skillCreateInstance.hide();
			router.navigate('system/skill/' + data.Id);
		});
	}

	function parseSearchTerm(searchTerm) {
		var json = {
			SkillTypeIntList: searchTerm.SkillType ? searchTerm.SkillType.Data.selectedOptions : null,
			LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
			DirectionTypeIntList: searchTerm.DirectionType ? searchTerm.DirectionType.Data.selectedOptions : null,
            ApplicationTypeIntList: searchTerm.ApplicationType ? searchTerm.ApplicationType.Data.selectedOptions : null,
		};

		return JSON.stringify(json);
	}

	function createSkill() {
	}

	function searchCallback() {
        setupService.getFluid('skill', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(searchTerm()) } }).then(function (data) {
            var skillsData = [];
            ko.utils.arrayForEach(data, function(s) {

		        s.ApplicationTypesDisplay =  s.ApplicationTypes.reduce(function(acumulator, currentValue) {
                    return acumulator + '<li>' + currentValue.DisplayName+'</li>' ;
                }, '<ul style="list-style-position: inside; padding-left: 0;">') +'</ul>';
                skillsData.push(s);
            });

            vm.skills(skillsData);
        });
	};

	vm.activate = function () {
		$.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
	};

	return vm;
});