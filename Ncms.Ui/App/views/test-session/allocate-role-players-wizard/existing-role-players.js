//define([
//	'services/testsession-data-service',
//	'modules/enums',
//	'modules/common',
//	'modules/custom-validator',
//],
//	function (testsessionService, enums, common, customValidator) {
//		return {
//			getInstance: getInstance
//		};

//		function getInstance(params) {
//			var defaultParams = {
//                summary: null

//			};

//			$.extend(defaultParams, params);

//			var vm = {
//				summary: defaultParams.summary,
//				selectedIds: ko.observableArray(),
//				applicants: ko.observableArray(),
//			};

//		    vm.load = function() {
//		        vm.readOnly(false);
//		        validator.reset();
//		        vm.selectedIds([]);

//		        testsessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/getExistingroleplayers').then(vm.applicants);
//		    };

//			vm.isValid = function () {
//				var defer = Q.defer();

//                testsessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/existingroleplayers').then(function (data) {
//					validator.reset();

//					ko.utils.arrayForEach(data.InvalidFields, function (i) {
//						validator.setValidation(i.FieldName, false, i.Message);
//					});

//					validator.isValid();

//					var isValid = !data.InvalidFields.length;
//					defer.resolve(isValid);
//					vm.readOnly(isValid);
//				});

//				return defer.promise;
//			};

//			vm.postData = function () {
//				return { Applicants: vm.selectedIds() };
//			};

//			vm.readOnly = ko.observable(false);
//			vm.tableDefinition = {
//				searching: false,
//				paging: false,
//				oLanguage: { sInfoEmpty: '', sInfo: '' },
//				columnDefs: [
//					{
//						orderable: false,
//						className: 'select-checkbox',
//						targets: 0
//					}
//				],
//				select: {
//					style: 'multi+shift',
//					info: false
//				},
//				events: {
//					select: selectTable,
//					deselect: selectTable,
//					'user-select': function (e) {
//						if (vm.readOnly()) {
//							e.preventDefault();
//						}
//					},
//				}
//			};

//			var serverModel = {
//				Applicants: vm.selectedIds
//			};

//			var validator = customValidator.create(serverModel);

//			var bypassSelect = false;
//			function selectTable(e, dt) {
//				if (bypassSelect) {
//					return;
//				}

//				vm.selectedIds([]);
//				var indexes = dt.rows('.selected').indexes();

//				if (!indexes.length) {
//					return;
//				}

//				for (var i = 0; i < indexes.length; i++) {
//					vm.selectedIds.push(vm.applicants()[indexes[i]].CustomerNo);
//				}
//			}

//			return vm;
//		}
//	});