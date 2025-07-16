define([
    'services/screen/message-service',
    'services/entity-data-service',
    'services/finance-data-service',
    'services/util'
],
function (message, entityService, financeService, util) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var isInitializing = false;
        var serverModel = {
            NaatiNumber: ko.observable(),
            EntityId: ko.observable().extend({ required: true }),
            ProductSpecificationId: ko.observable().extend({ required: true }),
            GLCode: ko.observable(),
            ExGst: ko.observable(),
            Gst: ko.observable(),
            Quantity: ko.observable(1).extend({ required: true }),
            IncGstCostPerUnit: ko.observable().extend({ required: true }),
            ExGstCostPerUnit: ko.observable(),
            GSTApplies: ko.observable(false), // incorrect naming style required to match product spec from entity service
            Description: ko.observable().extend({ required: true })
        };

        var preventLoadGst = false;

        function loadGst() {
            if (preventLoadGst || isInitializing) {
                return;
            }

            serverModel.ExGstCostPerUnit(serverModel.GSTApplies()
                ? serverModel.IncGstCostPerUnit() * (10 / 11)
                : serverModel.IncGstCostPerUnit());
            serverModel.Gst(serverModel.GSTApplies() ? serverModel.IncGstCostPerUnit() / 11 : 0);
        }

        serverModel.ProductSpecificationId.subscribe(function () {
            if (isInitializing) {
                return;
            }

            preventLoadGst = true;
            var product = getSelectedProduct();
            ko.viewmodel.updateFromModel(serverModel, product, true).onComplete(function () {
                serverModel.IncGstCostPerUnit(product.CostPerUnit);
                preventLoadGst = false;
                loadGst();
            });
        });

        serverModel.GSTApplies.subscribe(loadGst);
        serverModel.IncGstCostPerUnit.subscribe(loadGst);

        var defer = null;
        var cleanServerModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        var vm = {
            item: serverModel,
            originalCandidate: null,
            invoiceOnBehalfOfOtherCandidate: ko.observable(false),
            displayInactiveProducts: ko.observable(false),
            productSpecification: ko.observableArray([]),
            glCode: ko.observableArray(),
            compositionComplete: compositionComplete,
            close: close
        };

        vm.invoiceOnBehalfOfOtherCandidate.subscribe(function (newValue) {
            if (!newValue) {
                vm.item.EntityId(vm.originalCandidate.EntityId);
                vm.item.NaatiNumber(vm.originalCandidate.NaatiNumber);
                vm.invoiceOnBehalfOfOptions.textValue(vm.originalCandidate.Name);
            }
        });

        vm.tryDisplayInactiveProducts = function () {
            var newValue = !vm.displayInactiveProducts();
            var product = getSelectedProduct() || {};

            if (!newValue && product.Inactive) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                    content: ko.Localization('Naati.Resources.Finance.resources.InactiveProductSelected')
                }).then(function (answer) {
                    if (answer === 'yes') {
                        vm.displayInactiveProducts(newValue);
                    }
                });

                return false;
            }

            return true;
        };

        vm.dirtyFlag = new ko.DirtyFlag([vm.item], false);
        vm.filteredProductSpecification = ko.computed(function () {
            return ko.utils.arrayFilter(vm.productSpecification(), function (p) {
                return p.ProductSpecificationId === serverModel.ProductSpecificationId() || p.Inactive && vm.displayInactiveProducts() || !p.Inactive;
            });
        });

        vm.invoiceOnBehalfOfOptions = {
            attr: { id: 'invoiceOnBehalfOf' },
            source: function (query, callback) {
                financeService.getFluid('customer', { term: query }).then(callback);
            },
            multiple: false,
            valueProp: 'EntityId',
            labelProp: 'Name',
            template: 'invoiceto-template',
            textValue: ko.observable(),
            value: serverModel.EntityId,
            options: ko.observableArray(),
            resattr: {
                placeholder: 'Naati.Resources.Finance.resources.Candidate'
            },
            select: function (e, data) {
                if (data.item.data) {
                    serverModel.NaatiNumber(data.item.data.NaatiNumber);
                }
            }
        };

        vm.productOptions = {
            options: vm.filteredProductSpecification,
            value: serverModel.ProductSpecificationId,
            multiple: false,
            optionsValue: 'ProductSpecificationId',
            optionsText: 'CodeAndName'
        };

        vm.activate = function () {
            entityService.getFluid('GLCode').then(vm.glCode);
        };

        vm.add = function (originalCandidate) {
            return vm.edit($.extend({}, cleanServerModel, originalCandidate), originalCandidate);
        };

        vm.edit = function (item, originalCandidate) {
            defer = Q.defer();

            vm.originalCandidate = originalCandidate;

            entityService.getFluid('ProductSpecification').then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    d.CodeAndName = '{0} - {1}'.format(d.Code, d.Name);
                    d.CostPerUnit = d.CostPerUnit.toFixed(2);
                });

                data = data.sort(util.sortBy('Code'));
                vm.productSpecification(data);
            });

            vm.invoiceOnBehalfOfOptions.textValue(null);

            isInitializing = true;
            ko.viewmodel.updateFromModel(vm.item, item, true).onComplete(function () {
                vm.item.NaatiNumber(item.NaatiNumber);
                vm.invoiceOnBehalfOfOptions.textValue(item.Name);

                vm.displayInactiveProducts(false);
                vm.invoiceOnBehalfOfOtherCandidate(originalCandidate.NaatiNumber !== item.NaatiNumber),

                vm.dirtyFlag().reset();

                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }

                isInitializing = false;
            });

            util.resetModel(serverModel);
            $('#addInvoiceDetailModal').modal('show');

            return defer.promise;
        };

        vm.isDirty = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var item = ko.toJS(serverModel);
            var product = getSelectedProduct();

            $.extend(item, product, {
                Name: vm.invoiceOnBehalfOfOptions.textValue(),
                GSTApplies: item.GSTApplies, // Ignore the value from select product
                Total: product.CostPerUnit * item.Quantity
            });

            defer.resolve(item);
            close();
        };

        var canClose = false;

        function compositionComplete() {
            $('#addInvoiceDetailModal').on('hide.bs.modal', function (e) {
                tryClose(e);
            });

            $(window).on('examinerCancel', close);
        }

        function tryClose(event) {
            if (event.target.id !== 'addInvoiceDetailModal') {
                return;
            }

            if (canClose) {
                return;
            }

            event.preventDefault();
            event.stopImmediatePropagation();

            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                }).then(function (answer) {
                    if (answer === 'yes') {
                        close();
                    }
                });
            } else {
                close();
            }
        }

        function close() {
            canClose = true;
            $('#addInvoiceDetailModal').modal('hide');
            canClose = false;
        }

        function getSelectedProduct() {
            var product = ko.utils.arrayFirst(vm.productSpecification(), function (p) {
                return p.ProductSpecificationId === vm.item.ProductSpecificationId();
            });

            var glCode = ko.utils.arrayFirst(vm.glCode(), function (g) {
                return g.GLCodeId === product.GLCodeId;
            });

            if (glCode) {
                product.GLCode = glCode.Code;
            }

            return product;
        };

        return vm;
    }
});
