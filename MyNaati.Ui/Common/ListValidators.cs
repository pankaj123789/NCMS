using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyNaati.Ui.Common
{
    public class MinimumItemCountAttribute : ValidationAttribute
    {
        public MinimumItemCountAttribute(int count, Func<object, bool> listWhereFunc, bool errorOnNull, bool errorOnEmpty)
        {
            this.ErrorOnNull = errorOnNull;
            this.ErrorOnEmpty = errorOnEmpty;
            this.ListWhereFunc = listWhereFunc;
            this.Count = count;
        }

        public MinimumItemCountAttribute(int count)
        {
            this.ErrorOnNull = true;
            this.ErrorOnEmpty = true;
            this.ListWhereFunc = e => true;
            this.Count = count;
        }

        public bool ErrorOnNull { get; set; }

        public bool ErrorOnEmpty { get; set; }

        public int Count { get; set; }

        public Func<object, bool> ListWhereFunc { get; set; } 

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (IList)value;

            if (model == null && !this.ErrorOnNull)
            {
                return null;
            }

            if (model != null)
            {
                if (model.Count == 0 && !this.ErrorOnEmpty)
                {
                    return null;
                }

                int whereCount = model.Cast<object>().Count(item => this.ListWhereFunc(item));

                if (whereCount >= this.Count)
                {
                    return null;
                }
            }

            return new ValidationResult(this.ErrorMessage);
        }
    }

    public class MaximumItemCountAttribute : ValidationAttribute
    {
        public MaximumItemCountAttribute(int count, Func<object, bool> listWhereFunc, bool errorOnNull)
        {
            this.ErrorOnNull = errorOnNull;
            this.ListWhereFunc = listWhereFunc;
            this.Count = count;
        }

        public MaximumItemCountAttribute(int count)
        {
            this.ErrorOnNull = true;
            this.ListWhereFunc = e => true;
            this.Count = count;
        }

        public bool ErrorOnNull { get; set; }

        public int Count { get; set; }

        public Func<object, bool> ListWhereFunc { get; set; } 

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (IList)value;

            if (model == null && !this.ErrorOnNull)
            {
                return null;
            }

            if (model != null)
            {
                int whereCount = model.Cast<object>().Count(item => this.ListWhereFunc(item));
                if (whereCount <= this.Count)
                {
                    return null;
                }
            }

            return new ValidationResult(this.ErrorMessage);
        }
    }

    public class ValidateListItemsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            IEnumerable enumerable = value as IEnumerable;
            if (enumerable == null)
            {
                return true;
            }

            foreach (object item in enumerable)
            {
                IEnumerable<PropertyInfo> properties = item.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(ValidationAttribute), true).Count() > 0);
                
                foreach (PropertyInfo property in properties)
                {
                    IEnumerable<ValidationAttribute> validationAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();
                    foreach (ValidationAttribute validationAttribute in validationAttributes)
                    {
                        object propertyValue = property.GetValue(item, null);
                        if (!validationAttribute.IsValid(propertyValue))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}