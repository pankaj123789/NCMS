define([
	'views/document/document-form',
	'services/person-data-service',
	'services/logbook-data-service',
    'services/screen/date-service'],
	function (documentTable, personDataService, logbookService, dateService) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var serverModel = {
				Id: ko.observable(),
				DateCompleted: ko.observable().extend({ required: true }),
				Description: ko.observable().extend({ required: true, maxLength: 200 }),
				ProfessionalDevelopmentCategoryId: ko.observable().extend({ required: true }),
				ProfessionalDevelopmentRequirementId: ko.observable().extend({ required: true }),
				Notes: ko.observable().extend({ maxLength: 4000 })
			};

			serverModel.ProfessionalDevelopmentCategoryId.subscribe(loadRequirements);

			var emptyModel = ko.toJS(serverModel);
			var validation = ko.validatedObservable(serverModel);

			var vm = {
				activity: serverModel,
				naatiNumber: ko.observable(),
				categories: ko.observableArray(),
				requirements: ko.observableArray(),
				documentTableInstance: documentTable.getInstance(),
				applicationId: ko.observable(),
				editable: options.editable
			};

			vm.editable.subscribe(disableDocuments);

			var documentTypes = ['ProfessionalDevelopmentActivity'];

			vm.documentTableInstance.parseFileName = function (fileName) {
				var tmp = fileName.FileName.split('.');
				return tmp.slice(0, tmp.length - 1).join('.');
			};

			vm.documentTableInstance.types(documentTypes);
			vm.documentTableInstance.fileUpload.url = logbookService.url() + '/uploadprofessionaldevelopmentactivityattachment';
			vm.documentTableInstance.fileUpload.formData = ko.computed(function () {
				return {
					id: vm.documentTableInstance.currentDocument.Id() || 0,
					file: vm.documentTableInstance.currentDocument.File(),
					type: vm.documentTableInstance.currentDocument.Type(),
					professionalDevelopmentActivityId: vm.documentTableInstance.relatedId(),
					title: vm.documentTableInstance.currentDocument.Title(),
					storedFileId: vm.documentTableInstance.currentDocument.StoredFileId() || 0
				};
			});

			$.extend(vm.documentTableInstance, {
				getDocumentsPromise: function () {
					disableDocuments();
					return logbookService.getFluid("professionaldevelopmentactivityattachment", {
						activityId: vm.documentTableInstance.relatedId() || 0,
						supressResponseMessages: true
					});
				},
				postDocumentPromise: function () {
					var data = ko.toJS(vm.documentTableInstance.currentDocument);
					data.WorkPracticeId = vm.documentTableInstance.relatedId();
					data.Description = data.Title;
					return logbookService.post(data, "professionaldevelopmentactivityattachment");
				},
				removeDocumentPromise: function (document) {
					return logbookService.remove("professionaldevelopmentactivityattachment/" + document.ProfessionalDevelopmentActivityAttachmentId);
				}
			});

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

			personDataService.getFluid('categories').then(vm.categories);

			vm.categoryOptions = {
				value: serverModel.ProfessionalDevelopmentCategoryId,
				multiple: false,
				disable: ko.computed(function () {
					return !vm.editable() || !vm.categories().length;
				}),
				options: vm.categories,
				optionsValue: 'Id',
				optionsText: 'Name',
				selectIfSingle: true
			};

			vm.requirementOptions = {
				value: serverModel.ProfessionalDevelopmentRequirementId,
				multiple: false,
				disable: ko.computed(function () {
					return !vm.editable() || !vm.requirements().length;
				}),
				options: vm.requirements,
				optionsValue: 'Id',
				optionsText: 'Name',
				selectIfSingle: true,
				multiselect: {
					optionClass: function () { return 'w-s-normal'; }
				}
			};

			var defer = null;
			vm.add = function (naatiNumber, applicationId) {
				vm.applicationId(applicationId);
				return edit(naatiNumber, emptyModel, applicationId);
			};

			vm.edit = function (naatiNumber, activity) {
				var editModel = $.extend({}, activity);
				editModel.DateCompleted = moment.utc(editModel.DateCompleted).toDate();
				return edit(naatiNumber, editModel);
			};

			vm.saveActivity = function () {
				if (!validation.isValid()) {
					validation.errors.showAllMessages();
					return;
				}

				var request = ko.toJS(serverModel);
				request.NaatiNumber = vm.naatiNumber();
				request.CredentialApplicationId = vm.applicationId();
                request.DateCompleted = dateService.toPostDate(request.DateCompleted);

				logbookService.post(request, 'professionaldevelopmentactivity').then(function (data) {
					serverModel.Id(data.Id);
					$('#addActivityModal').modal('hide');
					toastr.success('Activity saved.');
					defer.resolve(data);
				});
			};

			function disableDocuments() {
                var permission = null;
				if (!vm.editable()) {
					permission = 'Logbook.Manage';
				}

                vm.documentTableInstance.editShowWithPermission(permission);
                vm.documentTableInstance.deleteShowWithPermission(permission);
                vm.documentTableInstance.uploadShowWithPermission(permission);
			}

			function edit(naatiNumber, activity) {
				defer = Q.defer();

				vm.naatiNumber(naatiNumber);
				vm.documentTableInstance.documents([]);

				vm.documentTableInstance.relatedId(activity.Id);
				vm.documentTableInstance.search();

                ko.viewmodel.updateFromModel(serverModel, activity);
				$('#addActivityModal').modal('show');
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
			    clearValidation();
                personDataService.getFluid('requirements', { categoryId: categoryId }).then(vm.requirements);
			}

			return vm;
		}
	}
);