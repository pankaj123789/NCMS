using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetPersonImageResponse
    {
        public byte[] PersonImageData { get; set; }
    }

    public class ApiPersonImageResponse : BaseResponse
    {
        [JsonIgnore]
        public bool IsPersonExist { get; set; }
        [JsonIgnore]
        public bool ShowPhotoOnline { get; set; }
        [JsonIgnore]
        public bool IsDeceased { get; set; }
        public byte[] PersonImageData { get; set; }
    }
}