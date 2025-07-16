define([
        'services/screen/date-service',
        'services/audit-data-service',
        'plugins/router',
],
    function (dateService, auditService, router) {

        var serverModel = {
            AbbreviatedName: ko.observable(),
            NaatiNumber: ko.observable(),
            InstitutionId: ko.observable(),
            EntityId: ko.observable(),
            Name: ko.observable(),
            AuditHistory: ko.observableArray()
        }

        var vm = {
            dateService: dateService,
            audithistory: serverModel,
            canActivate: canActivate,
            close: close,
            dateFormatted: dateFormatted
        };

        $.extend(vm,
            {
                windowTitle: 'Organisation Audit History'
            });

        function dateFormatted(date) {
            return moment(date).format("h:mm a DD/MM/YYYY");
        }

        function canActivate(params) {
            return loadAuditHistory(params);
        }

        function loadAuditHistory(params) {

            var defer = Q.defer();
            var paramsList = params.split('-');
            var entityId = paramsList[0];
            var naatiNumber = paramsList[1];
            var institutionId = paramsList[2];

            var promises = [
                getDataWithParentId(entityId),
                getDataWithParentId(institutionId),
                getDataThatContains("Institution=\'0".replace('0', institutionId)),
                getDataThatContains("Entity\'0".replace('0', naatiNumber))
            ];

            Q.all(promises).done(function (result) {
                var data = [];
                ko.utils.arrayForEach(result,
                    function (value) {
                        data = data.concat(value);
                    });

                ko.viewmodel.updateFromModel(vm.audithistory.AuditHistory, data.sort(function(a, b) {
                    var dateA = new Date(b.DateTime);
                    var dateB = new Date(a.DateTime);
                    return dateA - dateB;
                }));
                defer.resolve(true);
            });

            return defer.promise;
        }

        function getDataWithParentId(parentId) {
            return auditService.get({ ParentId: parentId, PageSize: 1000, PageNumber: 1 })
                .then(function (data) { return data }, function () { return []; });
        }

        function getDataThatContains(text) {
            return auditService.get({
                ParentId: -1,
                changeDetailPart: text,
                PageSize: 1000,
                PageNumber: 1
            }).then(function (data) { return data }, function () { return []; });
        }

        function close() {
            router.navigateBack();
        }

        return vm;
    });