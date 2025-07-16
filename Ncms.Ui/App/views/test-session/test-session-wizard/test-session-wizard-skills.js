define([
    'services/testsession-data-service',
], function (testSessionService) {
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
            skills: ko.observableArray(),
            errors: ko.observableArray(),
            warnings: ko.observableArray(),
            getStep: defaultParams.getStep,
            scrollStep: defaultParams.scrollStep
        };

        vm.tableDefinition = {
            searching: false,
            paging: false,
            info: false,
            oLanguage: { sInfoEmpty: '', sInfo: '' },
            columnDefs: [
                {
                    orderable: false,
                    className: 'select-checkbox',
                    targets: 0
                }
            ],
            select: {
                style: 'multi+shift'
            },
            drawCallback: drawTable,
            events: {
                select: selectTable,
                deselect: selectTable
            }
        };

        vm.isValid = function () {
            var defer = Q.defer();
            var request = ko.toJS(vm.session);
            request.TestSessionId = request.Id;
            $.extend(request, vm.postData());

            var previousWarnings = vm.warnings();
            testSessionService.post(request, 'wizard/skills').then(function (data) {
                var newWarings = false;
                for (var i = 0 ; i < data.WarningFields.length ; i++) {
                   
                    var warningMessage = data.WarningFields[i].Message;

                    if (!previousWarnings.find(function (element) { return element.Message === warningMessage; })) {
                        newWarings = true;
                    }
                }

                vm.errors(data.InvalidFields);
                vm.warnings(data.WarningFields);
                var isValid = !data.InvalidFields.length && !newWarings;
                if (!isValid) {
                    vm.scrollStep("wizardSkills");
                }
                
                defer.resolve(isValid);
            });

            return defer.promise;
        };

        vm.load = function () {
            vm.errors([]);
            vm.warnings([]);
            var testSession = ko.toJS(vm.session);
            testSessionService.getFluid('wizard/skills', testSession).then(function (data) {
                ko.viewmodel.updateFromModel(vm.skills, data);
            });
        };

        vm.postData = function () {
            return { Skills: ko.toJS(vm.skills) };
        };

        var bypassSelect = false;
        function selectTable(e, dt) {
            if (bypassSelect || !dt) {
                return;
            }

            ko.utils.arrayForEach(vm.skills(), function (s) {
                s.Selected(false);
            });

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                vm.skills()[indexes[i]].Selected(true);
            }
        }

        function drawTable() {
            bypassSelect = true;

            var $table = $(this).DataTable();
            var skills = vm.skills();

            for (var i = 0; i < skills.length; i++) {
                if (skills[i].Selected()) {
                    $table.row(i).select();
                }
            }

            bypassSelect = false;
        }

        return vm;
    }
});