
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace Ncms.Bl.TestSpecifications.Extensions
{
    public static class CompareExtension
    {
        public static GenericResponse<bool> CompareField(this string source, string compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (source == string.Empty)
            {
                response.Errors.Add($"Incomplete: {sheetName}, Column: {fieldName}, Reason: Null Value Found");
                return response;
            }

            if (compare == string.Empty)
            {
                response.Errors.Add($"Missing: {sheetName}, Column: {fieldName}, Reason: Does Not Exist");
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");
            return response;
        }

        public static GenericResponse<bool> CompareField(this int? source, int? compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (!source.HasValue)
            {
                response.Errors.Add($"Incomplete: {sheetName}, Column: {fieldName}, Reason: Null Value Found");
                return response;
            }

            if (!compare.HasValue)
            {
                response.Errors.Add($"Missing: {sheetName}, Column: {fieldName}, Reason: Does Not Exist");
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");

            return response;
        }

        public static GenericResponse<bool> CompareField(this int source, int compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");

            return response;
        }

        public static GenericResponse<bool> CompareField(this double? source, double? compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");

            return response;
        }

        public static GenericResponse<bool> CompareField(this double source, double compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");

            return response;
        }

        public static GenericResponse<bool> CompareField(this bool source, bool compare, string fieldName, GenericResponse<bool> response, string sheetName, string parents)
        {
            if (source.Equals(compare))
            {
                return response;
            }

            if (string.IsNullOrEmpty(parents))
            {
                response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, Old Value: {compare}, New Value: {source}");
                return response;
            }

            response.Messages.Add($"Updated: {sheetName}, Column: {fieldName}, {parents}, Old Value: {compare}, New Value: {source}");

            return response;
        }
    }
}
