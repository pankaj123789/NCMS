using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class BaseMapper
    {
        internal static NativeModels.BaseModel ToNativeBaseModel(this PublicModels.BaseModel baseModel)
        {
            return new NativeModels.BaseModel
            {
                HasValidationErrors = baseModel.HasValidationErrors,
                ValidationErrors = baseModel.ValidationErrors.Select(x => new NativeModels.ValidationError { Message = x.Message, ErrorCode = x.ErrorCode }).ToList()
            };
        }

        internal static PublicModels.BaseModel ToPublicBaseModel(this NativeModels.BaseModel baseModel)
        {
            return new PublicModels.BaseModel()
            {
                HasValidationErrors = baseModel.HasValidationErrors,
                ValidationErrors = baseModel.ValidationErrors?.Select(x => new PublicModels.ValidationError { Message = x.Message, ErrorCode = x.ErrorCode }).ToList()
            };
        }

        internal static string Trunc(this string content, int maxLength)
        {
            return content.Length <= maxLength ? content : content.Substring(0, maxLength - 3) + "...";
        }
    }
}
