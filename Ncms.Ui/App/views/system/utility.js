define([
    'views/shell'
], function (shell) {

    var vm = {
        title: shell.titleWithSmall,
        parts: [
            'assign-past-test-session',
            'process-file-deletes-report',
            'progress-invoice',
            'progress-credit-note',
/*            'progress-invoice',*/
        ],
        processedParts: ko.observableArray(),
        systemValueConfigs: ko.observableArray([])   
    };

    vm.activate = function (message) {
        // get system value configs from function
        getSystemValueConfigs();
        vm.processedParts([]);
        var parts = [];

        $.each(vm.parts, function (i, p) {
            parts.push('views/system/utility-parts/' + p);
        });

        if (vm.systemValueConfigs().length > 0) {
            parts.push('views/system/utility-parts/change-system-values');
        }

        require(parts, function () {
            $.each(arguments, function (i, part) {
                if (part.__moduleId__ == "views/system/utility-parts/change-system-values") {
                    vm.processedParts.push({
                        view: parts[i],
                        model: part.getInstance(vm.systemValueConfigs())
                    });
                    return;
                };
                vm.processedParts.push({
                    view: parts[i],
                    model: part.getInstance()
                });
            });
        });

        if (message) {
            toastr.info(ko.Localization('Naati.Resources.SystemResources.resources.' + message));
        }
    }

    function getSystemValueConfigs() {
        // set all system value configs here. ConfigName needs to be added to SystemResources ResX file as
        // "Change{ConfigName}SystemValues"
        // System Value Keys should have an entry in SystemResources ResX file as well as
        // '{Key}Label'
        // This is for the rendering of text for the change-system-values module
        // Value is left blank intentionally as it is populated in the change-system-values.js back end call
        // DataType is the type of data that should be put into the input field for the system value, this is used for sanity checks
        // DataTypes: string, int, bool
        var systemValueConfigs =
            [
                //{
                //    ConfigName: "SomeOtherSystemValueGroup",
                //    SystemValues:
                //        [
                //            { Key: "SystemValue1", Value: "", DataType: "string" },
                //            { Key: "SystemValue2", Value: "" DataType: "int"}
                //            { Key: "SystemValue2", Value: "" DataType: "bool" }
                //            etc...
                //        ]
                //},
                {
                    ConfigName: "GeneralBatchReportSettings",
                    SystemValues:
                        [
                            { Key: "SendSuccessfulBatchReport", Value: "", DataType: "bool" }
                        ]
                },
                {
                    ConfigName: "ProcessFileDeletesPastExpiry",
                    SystemValues:
                        [
                            { Key: "ProcessFileDeletesPastExpiryDateRetentionDays", Value: "", DataType: "int" },
                            { Key: "ProcessFileDeletesPastExpiryDateBatchSize", Value: "", DataType: "int" },
                            { Key: "ProcessFileDeletesPastExpiryDateIncludePreviouslyQueued", Value: "", DataType: "bool" }
                        ]
                }
                
            ]

        // set vm.systeValueConfigs as newly set systemValueConfigs
        vm.systemValueConfigs(systemValueConfigs);
    }

    return vm;
});