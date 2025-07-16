using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.DTO
{
    public class BusinessServiceResponse
    {
        public BusinessServiceResponse()
        {
            Success = true;
            Messages = new List<string>();
            Warnings = new List<string>();
            Errors = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Errors { get; set; }

        public static BusinessServiceResponse Succeeded => new BusinessServiceResponse();
        public static BusinessServiceResponse Failed => new BusinessServiceResponse { Success = false };
    }
}