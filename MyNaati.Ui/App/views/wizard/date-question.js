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

            vm.validation = ko.validatedObservable([vm.question.Response]);

            return vm;
        }
    });