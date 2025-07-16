define(['services/dataset-data-service'], function (datasetService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;
        var vm = {
            job: ko.observable({
                Note: ko.observable()
            }),
            load: load,
            save: save
        };
        return vm;

        function load(job) {
            defer = Q.defer();
            vm.job(job);
            return defer.promise;
        }

        function save() {
            var job = ko.toJS(vm.job);
            datasetService.post({ Job: [job] }, "dsJob/" + job.JobId);
        }
    }
});