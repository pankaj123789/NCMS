define([], function(){
    return {
        create: create
    };

    function create(model) {
        return new customValidator(model);
    }

    function customValidator(model) {
        var validators = {};
        var validation = null;

        createValidations();

        this.reset = function () {
            for (var v in validators) {
                validators[v].message(null);
                validators[v].isValid(true);
            }
        };

        this.setValidation = function (fieldName, isValid, message) {
            validators[fieldName].isValid(isValid);
            validators[fieldName].message(message);
        };

        this.isValid = function () {
            var valid = validation.isValid();
            if (!valid) {
                validation.errors.showAllMessages();
            }
            return valid;
        };

        function createValidations() {
            for (var propertyName in model) {
                var property = model[propertyName];
                if (ko.isObservable(property)) {
                    var validator = createValidation(propertyName);
                    extendProperty(property, validator);
                }
            }

            validation = ko.validatedObservable(model);
        }

        function createValidation(propertyName) {
            validators[propertyName] = {
                message: ko.observable(),
                isValid: ko.observable()
            };

            return validators[propertyName];
        }

        function extendProperty(property, validator) {
            property.extend({
                validation: {
                    validator: function () {
                        return validator.isValid();
                    },
                    message: function () {
                        return validator.message();
                    }
                }
            });

            property.onValueChanged = function() {
                validator.isValid(true);
                validator.message('');
            }

            property.subscribe(property.onValueChanged);
        }
    }
});