define(['views/wizard/check-question'],
	function (checkQuestion) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
            var instance = checkQuestion.getInstance(options);
		    var defer = Q.defer();
            instance.promise = defer.promise;

			options.question.Responses.subscribe(refreshCredentialRequest);            
            
			 options.credentialApplication.getRecertificaitonOptions().then(function (data) {
				var answers = ko.viewmodel.fromModel(data);
				ko.utils.arrayForEach(answers(), function (a) {
					options.question.Responses.push(a.Id());
                    a.question = options.question;
				});
                
				options.question.Answers(answers());
                refreshCredentialRequest();
                options.question.Response(true);
			    defer.resolve();
			});

			function refreshCredentialRequest() {
				var data = options.question.Responses();

				options.credentialRequests([]);

				ko.utils.arrayForEach(data, function (d) {
					var option = ko.utils.arrayFirst(options.question.Answers(), function (o) {
						return o.Id() === d;
					});

					if (option) {
						options.credentialRequests.push({ LevelId: option.CredentialTypeId() });
					}
				});
			}
            
			return instance;
		}
	});