using F1Solutions.Naati.Common.Contracts.Bl;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Ui.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Ncms.Ui.App_Start.Security
{
    public class HtmlEncodeAttribute : ActionFilterAttribute
    {
        private static IList<string> _encodeAttributeExpressions;
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Type modelType = null;
            try
            {
                if (actionContext.ActionArguments.Count == 0)
                {
                    return;
                }

                //pick up any attributes placed on the method being executed.
                var reflectedDescriptor = actionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
                var encodeScriptOnly = reflectedDescriptor.GetCustomAttributes<EncodeScriptOnlyAttribute>().Count > 0 ? true : false;
                var encodeIgnore = reflectedDescriptor.GetCustomAttributes<EncodeIgnoreAttribute>().Count > 0 ? true : false;
                var encodeHtmlOnly = reflectedDescriptor.GetCustomAttributes<EncodeHtmlOnlyAttribute>().Count > 0 ? true : false;
                var onEncodeIssueFoundThrowError = reflectedDescriptor.GetCustomAttributes<OnEncodeIssueFoundThrowErrorAttribute>().Count > 0 ? true : false;

                if (!encodeIgnore)
                {
                    var request = HttpContext.Current.Request;

                    foreach (var inputParam in actionContext.ActionArguments)
                    {
                        if (inputParam.Value != null)
                        {
                            //user reflection to get the properties for a class
                            modelType = inputParam.Value.GetType();
                            var props = modelType.GetProperties();
                            //cast the object to its class
                            var model = Convert.ChangeType(inputParam.Value, modelType);
                            if (modelType == typeof(string))
                            {
                                model = Check((string)model, encodeScriptOnly, encodeHtmlOnly, onEncodeIssueFoundThrowError);
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
                                        //dont encode html as it gets inserts into wizard steps
                                        encodeScriptOnly = true;

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
                                        modelType == typeof(List<MaterialRequestPanelMembershipModel>) ||
                                        modelType == typeof(List<RubricQuestionPassRuleModel>) ||
                                        modelType == typeof(List<RubricTestBandRuleModel>) ||
                                        modelType == typeof(List<RubricTestQuestionRuleModel>) ||
                                        modelType == typeof(List<RefundRequestsGroupingModel>)) 
                                    {
                                        //ignore. Strings could be investigated later
                                    }
                                    else
                                    {
                                        if (modelType == typeof(Dictionary<string,string>))
                                        {
                                            var dictModel = (Dictionary<string,string>)model;
                                            var keyList = new List<string>();
                                            foreach(var entry in dictModel)
                                            {
                                                keyList.Add(entry.Key);
                                            }
                                            foreach(var key in keyList)
                                            {
                                                var result = Check(dictModel[key], encodeScriptOnly, encodeHtmlOnly, onEncodeIssueFoundThrowError);
                                                if(result!= dictModel[key])
                                                {
                                                    dictModel[key] = result;
                                                }
                                            }

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
                                                    var result = Check(stringValue, encodeScriptOnly, encodeHtmlOnly, onEncodeIssueFoundThrowError);
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in HtmlEncodeAttribute: Type: {(modelType != null?modelType.ToString():"null")} Exception: {ex.ToString()}");
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
                        var newValue = Check(currentValue, false, false, false);
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
