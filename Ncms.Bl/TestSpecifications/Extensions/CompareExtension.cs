
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace Ncms.Bl.TestSpecifications.Extensions
{
    public static class CompareExtension
    {

        private const string compareIsMissing = "Comparison from database does not exist for";
        private const string sourceIsMissing = "Source from spreadsheet does not exist for";
        private const string sourceAndCompareDifferent = "Source from spreadsheet and comparison from database are different for";

        public static GenericResponse<bool> CompareField(this string source, string compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (source == string.Empty)
            {
                response.Messages.Add($"{sourceIsMissing} {source} in {fieldName}.");
                return response;
            }

            if (compare == string.Empty)
            {
                response.Messages.Add($"{compareIsMissing} {compare} in {fieldName}.");
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }

        public static GenericResponse<bool> CompareField(this int? source, int? compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (!source.HasValue)
            {
                response.Messages.Add($"{sourceIsMissing} {source} in {fieldName}.");
                return response;
            }

            if (!compare.HasValue)
            {
                response.Messages.Add($"{compareIsMissing} {compare} in {fieldName}.");
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }

        public static GenericResponse<bool> CompareField(this int source, int compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }

        public static GenericResponse<bool> CompareField(this double? source, double? compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (!source.HasValue)
            {
                response.Messages.Add($"{sourceIsMissing} {source} in {fieldName}.");
                return response;
            }

            if (!compare.HasValue)
            {
                response.Messages.Add($"{compareIsMissing} {compare} in {fieldName}.");
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }

        public static GenericResponse<bool> CompareField(this double source, double compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }

        public static GenericResponse<bool> CompareField(this bool source, bool compare, string fieldName, GenericResponse<bool> response)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            response.Messages.Add($"{sourceAndCompareDifferent} {source} and {compare} in {fieldName}.");

            return response;
        }
    }
}
