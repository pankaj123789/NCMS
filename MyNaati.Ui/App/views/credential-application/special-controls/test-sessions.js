define([],
    function () {
        var maxToNearlyFull = 4;
        var maxToFillingUp = 25;

        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var credentialApplication = options.credentialApplication;

            var vm = {
                applicationId: options.applicationId,
                loaderOptions: options.loaderOptions,
                testSessions: ko.observableArray(),
                otherLocationSessions: ko.observableArray(),
                token: options.token,
                validation: { isValid: function () { return true; } },
                canContinue: ko.observable(false),
                showOtherLocations: ko.observable(false),
                showingOtherLocations: ko.observable(false),
                enableActions: options.enableActions,
            };

            vm.loaderOptions.params.text('Searching for test sessions...');

            vm.showOtherLocations = function() {
                vm.showingOtherLocations(true);
            }

            vm.load = function () {
   
                var preferedLocationSessions = [];
                var othereLocationSessions = []; 
                vm.enableActions(false);
                vm.loaderOptions.params.show(true);

                var preferedLocationSessions = [];
                var othereLocationSessions = []; 
                vm.enableActions(false);
                vm.loaderOptions.params.show(true);

                credentialApplication.testSessions({ ApplicationId: vm.applicationId(), Token: vm.token() }).then(function (data) {
                    ko.utils.arrayForEach(data, function (d) {
                        d.TestDateTime = moment(d.TestDateTime).format();
                        d.TestTimeStart = moment(d.TestDateTime).format('LT');
                        d.TestTimeEnd = moment(d.TestDateTime).add({ minutes: d.Duration }).format('LT');
                        d.Availability = d.AvailableSeats <= maxToNearlyFull ? 'Nearly Full' : (d.AvailableSeats <= maxToFillingUp ? 'Filling Up' : 'Seats Available');
                        d.AvailabilityCss = d.AvailableSeats <= maxToNearlyFull ? 'warning' : (d.AvailableSeats <= maxToFillingUp ? 'primary' : 'success');

                        if (d.IsPreferedLocation) {
                            preferedLocationSessions.push(d);
                        }
                        else {
                            othereLocationSessions.push(d);
                        }
                    });
                    vm.canContinue(true);
                    vm.testSessions(preferedLocationSessions);
                    vm.otherLocationSessions(othereLocationSessions);
                    vm.enableActions(true);
                    vm.loaderOptions.params.show(false);
                });
            };

            return vm;
        }
    });