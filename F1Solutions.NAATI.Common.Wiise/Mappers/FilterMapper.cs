using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    public static class FilterMapper
    {
        internal static String ToPayPalPart(this String fullReference)
        {
            var matchText = @"(PAYPAL.*$)";
            var matches = Regex.Match(fullReference, matchText);
            if (matches.Success)
            {
                return matches.Groups[1].Value;
            }
            //failed
            return fullReference;
        }
        internal static string ToSalesInvoiceFilter(this string whereClause)
        {
            var filterProcessor = MatchToFilterProcessor(whereClause);

            foreach (NameValuePair nameValuePair in filterProcessor.NameValuePairs)
            {
                if (nameValuePair.Field == "InvoiceNumber") { nameValuePair.Field = "number"; }
                if (nameValuePair.Field == "Date") { nameValuePair.Field = "invoiceDate"; }
                if (nameValuePair.FieldOperator == "=") { nameValuePair.FieldOperator = "eq"; }
                if (nameValuePair.FieldOperator == ">=") { nameValuePair.FieldOperator = "gt"; }
                if (nameValuePair.Field == "Type") 
                {
                    //nameValuePair.Field = "externalDocumentNumber";
                    nameValuePair.Field = "";
                    nameValuePair.FieldValue = "";
                    nameValuePair.FieldOperator = "";
                }
                if (nameValuePair.Field == "Contact.ContactID")
                {
                    nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "");
                }
                else
                {
                    nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "'");
                }
            }

            var filter = ReAssembleFilter(filterProcessor);

            return filter;
        }

        internal static string ToCreditNotesFilter(this string whereClause)
        {
            var filterProcessor = MatchToFilterProcessor(whereClause);

            foreach (NameValuePair nameValuePair in filterProcessor.NameValuePairs)
            {
                if (nameValuePair.Field == "CreditNoteNumber") { nameValuePair.Field = "number"; }
                if (nameValuePair.Field == "Date") { nameValuePair.Field = "creditMemoDate"; }
                if (nameValuePair.Field == "Contact.ContactID") {     
                    nameValuePair.Field = "customerId";
                    nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "");
                }
                if (nameValuePair.FieldOperator == "=") { nameValuePair.FieldOperator = "eq"; }
                if (nameValuePair.FieldOperator == ">=") { nameValuePair.FieldOperator = "gt"; }
                if (nameValuePair.Field == "Type")
                {
                    //nameValuePair.Field = "externalDocumentNumber";
                    //nameValuePair.FieldValue = "''";
                    nameValuePair.Field = "";
                    nameValuePair.FieldValue = "";
                    nameValuePair.FieldOperator = "";
                }
                nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "'");
            }

            string filter = ReAssembleFilter(filterProcessor);

            return filter;
        }

        internal static string ToPurchaseInvoiceFilter(this string whereClause)
        {
            var filter = @"?$filter=";
            var filterProcessor = MatchToFilterProcessor(whereClause);
            foreach (NameValuePair nameValuePair in filterProcessor.NameValuePairs)
            {
                if (nameValuePair.Field == "InvoiceNumber") { nameValuePair.Field = "vendorInvoiceNumber"; }
                if (nameValuePair.FieldOperator == "=") { nameValuePair.FieldOperator = "eq"; }

                nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "'");                
            }
            filter += ReAssembleFilter(filterProcessor);

            return filter;
        }

        internal static string ToContactFilter(this string whereClause)
        {
            var filter = @"?$filter=";
            var filterProcessor = MatchToFilterProcessor(whereClause);
            foreach (NameValuePair nameValuePair in filterProcessor.NameValuePairs)
            {
                if (nameValuePair.Field == "AccountNumber") { nameValuePair.Field = "number"; }
                if (nameValuePair.FieldOperator == "=") { nameValuePair.FieldOperator = "eq"; }
                //cheat on data and assume string for now
                nameValuePair.FieldValue = nameValuePair.FieldValue.Replace(@"""", "'");
            }
            filter += ReAssembleFilter(filterProcessor);

            return filter;
        }

        private static string ReAssembleFilter(FilterProcessor filterProcessor)
        {
            filterProcessor.FilterBase = filterProcessor.ReplaceFilterBaseOperators();
            var i = 0;
            foreach(NameValuePair nameValuePair in filterProcessor.NameValuePairs)
            {
                filterProcessor.FilterBase = filterProcessor.FilterBase.Replace($"[{i++}]",nameValuePair.ToString());
            }
            return filterProcessor.FilterBase;
        }


        private static FilterProcessor MatchToFilterProcessor(string whereClause)
        {
            whereClause = SimplifyGuids(whereClause);
            var matchText = @"([A-Za-z\.]+\s*[^A-Z\.^a-z^0-9]+\s*[A-Za-z0-9\-""]+)";
            var matches = Regex.Matches(whereClause, matchText);
            return GetFilterProcessor(whereClause, matches);
        }

        private static string SimplifyGuids(string whereClause)
        {
            var matchText = @"(Guid\("")([a-z0-9\-]+)(""\))";
            var matches = Regex.Match(whereClause, matchText);
            if (matches.Success)
            {
                whereClause = whereClause.Replace($"{matches.Groups[1].Value}{matches.Groups[2].Value}{matches.Groups[3].Value}", $"{matches.Groups[2].Value}");
            }
            return whereClause;
        }


        private static FilterProcessor GetFilterProcessor(string whereClause, MatchCollection matches)
        {
            var filterProcessor = new FilterProcessor();
            var i = 0;
            var matchText = @"^([^\s]+)\s*([<>=]+)\s*(.+)$";
            foreach (Match match in matches)
            {
                var matchResult = Regex.Match(match.Value, matchText);
                //important to only replace first occurence. E.g. field=10 and field=100 would be bad.
                whereClause = ReplaceFirst(whereClause,matchResult.Groups[0].Value,$"[{i++}]");
                filterProcessor.NameValuePairs.Add(new NameValuePair()
                {
                    Field = matchResult.Groups[1].Value,
                    FieldOperator = matchResult.Groups[2].Value,
                    FieldValue = matchResult.Groups[3].Value
                });
            }
            filterProcessor.FilterBase = whereClause;
            return filterProcessor;
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }

    internal class FilterProcessor
    {
        internal List<NameValuePair> NameValuePairs{ get; set;}
        internal string FilterBase { get; set; }

        internal FilterProcessor()
        {
            NameValuePairs = new List<NameValuePair>();
        }

        internal string ReplaceFilterBaseOperators()
        {
            FilterBase = FilterBase.Replace("&&", " and ");
            FilterBase = FilterBase.Replace("||", " or ");
            return FilterBase;
        }
    }

    internal class NameValuePair
    {
        internal string Field { get; set; }
        internal string FieldOperator { get; set; }
        internal string FieldValue { get; set; }

        internal string ToString()
        {
            return $"{Field} {FieldOperator} {FieldValue}";
        }
    }
}
