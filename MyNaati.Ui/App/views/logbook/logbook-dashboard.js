define([
	'durandal/app',
	'plugins/router',
	'services/requester'],
	function (app, router, requester) {
		var logbook = new requester('logbook');
		var chartColour = 'gray';
		var maxCredentialsColumns = 4;
		var vm = {
			credentials: ko.observableArray(),
			showAllCredentials: ko.observable(false),
			recertificationFormUrl: ko.observable(),
		};

		vm.activate = function () {
			app.breadCrumbs([
				{ href: window.baseUrl, text: 'Home' },
				{ href: '#', text: 'My Logbook' },
			]);

			var credentialsPromise = logbook.getFluid('credentials');
			var recertificationOptionsPromisse = logbook.getFluid('recertificationOptions');

			Promise.all([credentialsPromise, recertificationOptionsPromisse]).then(function (values) {
				var credentials = values[0];
				var recertificationOptions = values[1];
				var isRecertification = recertificationOptions.IsRecertification;
				var recertificationFormUrl = recertificationOptions.RecertificationFormUrl;

				vm.recertificationFormUrl(recertificationFormUrl);
				credentials.unshift({ IsPD: true });

				if (isRecertification) {
					credentials.unshift({ IsRecertification: true });
				}

				vm.credentials(credentials);
			});
		};

		vm.gotoProfessionalDevelopment = function () {
			router.navigate('professional-development');
		};

		vm.gotoWorkPractice = function (credential) {
			router.navigate('credential/' + credential.Id);
		};

		vm.gotoRecertificationForm = function () {
			document.location.href = vm.recertificationFormUrl();
		};

		vm.certificationName = function (c) {
			return c.CredentialType + '<br/>' + c.Skill;
		};

		vm.formSelectorClass = ko.pureComputed(function () {
			if (!vm.credentials() || !vm.credentials().length) {
				return 'col-sm-12';
			}

			return 'col-sm-' + Math.max(parseInt(12 / vm.credentials().length), 4);
		});

		return vm;
	}
);