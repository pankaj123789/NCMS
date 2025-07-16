define([],
    function () {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var vm = {
                question: options.question
            };

            vm.question.Response.extend({
                validation: {
                    validator: function (val) {
                        return (vm.question.Responses() && vm.question.Responses().length) || val;
                    },
                    message: 'This answer is required.'
                },
                maxLength: 500
            });

            vm.loaderOptions = {
                name: 'loader',
                params: {
                    show: ko.observable(true),
                    text: ko.observable('Loading options...')
                }
            };

            var tryValue = ko.observable();
            tryValue.subscribe(function (answer) {
                if (answer === vm.question.Response()) {
                    return;
                }

                var questions = vm.question.section.Questions();
                if (vm.question.current()) {
                    // I'm using timeout just to prevent the same event from 'change'
                    return setTimeout(function () { vm.question.Response(answer); }, 0);
                }

                mbox.confirm({ title: 'Change Answer', content: 'The next sections will reload if you change the answer. Are you sure that you want to change the answer?' }).then(function (argument) {
                    if (argument === 'yes') {
                        vm.question.Response(answer);
                    }
                    else if (argument === 'no') {
                        tryValue(vm.question.Response());
                    }
                });
            });

            vm.validation = ko.validatedObservable([vm.question.Response]);

            vm.getSelectOptions = function () {
                var selectOptions = vm.question.getSelectOptions();
                selectOptions.value = tryValue;
                selectOptions.options = ko.observableArray();
                selectOptions.loadPromise().then(function (options) {
                    selectOptions.options(options);
                    vm.loaderOptions.params.show(false);
                });
                return selectOptions;
            };

            return vm;
        }
    });