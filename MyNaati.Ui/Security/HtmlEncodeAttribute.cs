using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Ui.ViewModels.ExaminerTools;

namespace MyNaati.Ui.Security
{
    public class HtmlEncodeAttribute : ActionFilterAttribute
    {
        private static IList<string> _encodeAttributeExpressions;
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Type modelType = null;
            try
            {
                if (!actionContext.ActionParameters.ContainsKey("model"))
                {
                    return;
                }

                //pick up any attributes placed on the method being executed.
                //var reflectedDescriptor = actionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
                //var encodeScriptOnly = reflectedDescriptor.GetCustomAttributes<EncodeScriptOnlyAttribute>().Count > 0 ? true : false;
                //var encodeIgnore = reflectedDescriptor.GetCustomAttributes<EncodeIgnoreAttribute>().Count > 0 ? true : false;
                //var encodeHtmlOnly = reflectedDescriptor.GetCustomAttributes<EncodeHtmlOnlyAttribute>().Count > 0 ? true : false;
                //var onEncodeIssueFoundThrowError = reflectedDescriptor.GetCustomAttributes<OnEncodeIssueFoundThrowErrorAttribute>().Count > 0 ? true : false;

                //if (!encodeIgnore)
                {
                    //var request = HttpContext.Current.Request;


                    //user reflection to get the properties for a class
                    modelType = actionContext.ActionParameters["model"].GetType();
                    var props = modelType.GetProperties();
                    //cast the object to its class
                    var model = Convert.ChangeType(actionContext.ActionParameters["model"], modelType);
                    if (modelType == typeof(string))
                    {
                        model = Check((string)model, false, false, false);// encodeScriptOnly, encodeHtmlOnly, onEncodeIssueFoundThrowError);
                    }
                    else
                    {
                        if (modelType == typeof(int))
                        {
                            //do nothing
                        }
                        else
                        {
                            if (modelType == typeof(JObject))
                            {
                                var jObjectModel = (JObject)model;

                                foreach (var node in jObjectModel)
                                {
                                    Iterate(node.Value, jObjectModel, 1);
                                }
                            }
                            else
                            if (modelType == typeof(List<int>) ||
                                modelType == typeof(List<MaterialRequestPayrollUserGroupingModel>) ||
                                modelType == typeof(List<MaterialRequestMemberGroupingModel>) ||
                                modelType == typeof(List<MaterialRequestPanelMembershipModel>))
                            {
                                //ignore. Strings could be investigated later
                            }
                            else
                            {
                                foreach (var prop in props)
                                {
                                    var propValue = prop.GetValue(model);
                                    //only check out strings
                                    var type = propValue?.GetType();
                                    if (type == typeof(string))
                                    {
                                        var stringValue = propValue.ToString();
                                        var result = Check(stringValue, false, false, false);// encodeScriptOnly, encodeHtmlOnly, onEncodeIssueFoundThrowError);
                                        if (!result.Equals(stringValue))
                                        {
                                            //data has changed. Change the model
                                            prop.SetValue(model, result);
                                        }
                                    }
                                    else
                                    {
                                        //TODO check any lists or custom classes that are mebers of the model and iterate through those

                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in HtmlEncodeAttribute: Type: {(modelType != null ? modelType.ToString() : "null")} Exception: {ex.ToString()}");
            }
        }

        private static void Iterate(JToken token, JObject root, int currentLevel)
        {
            if (currentLevel > 10)
            {
                return;
            }
            foreach (var property in token.Children())
            {
                switch (property.Type)
                {
                    case JTokenType.Property:
                    case JTokenType.Object:
                    case JTokenType.Array:
                        Iterate(property, root, ++currentLevel);
                        break;
                    case JTokenType.String:
                        var currentValue = property.Value<string>();
                        var newValue = Check(currentValue, true, false, false);
                        if (!currentValue.Equals(newValue))
                        {
                            var node = root.SelectToken(property.Path);
                            ((JValue)node).Value = newValue;
                        }
                        break;
                }

            }
        }


        private static string Check(string value, bool scriptOnly, bool htmlOnly, bool onEncodeIssueFoundThrowError)
        {
            //take out script hacks before escaping html
            if (!htmlOnly)
            {
                foreach (var expression in ScriptExpressionsToCheck)
                {
                    value = Match(value, expression, string.Empty, onEncodeIssueFoundThrowError);
                }
            }
            if (!scriptOnly)
            {
                value = Match(value, "<", "&lt;", false);
                value = Match(value, ">", "&gt;", false);
            }
            return value;
        }


        private static string Match(string value, string expression, string replaceWith, bool onEncodeIssueFoundThrowError)
        {
            var match = Regex.Match(value, expression);
            if (match.Success)
            {
                if (onEncodeIssueFoundThrowError)
                {
                    throw new Exception($"Content not allowed: {expression} in data: {value}");
                }
                else
                {
                    return value.Replace(match.Captures[0].Value, replaceWith);
                }
            }
            return value;
        }

        private static IList<string> ScriptExpressionsToCheck
        {
            get
            {
                if (_encodeAttributeExpressions == null)
                {
                    var expressions = ConfigurationManager.AppSettings["EncodeAttributeExpressions"];
                    _encodeAttributeExpressions = expressions.Split(',').ToList();
                }

                return _encodeAttributeExpressions;
            }
        }
    }
}