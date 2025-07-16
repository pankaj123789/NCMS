using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Bl.DTO
{
    public class GenericResponse<T> : BusinessServiceResponse
    {
        public GenericResponse() { }

        public GenericResponse(T data)
        {
            Data = data;
        }

        public GenericResponse<T> Absorb(BusinessServiceResponse response) 
        {
            if (response != null)
            {
                Errors.AddRange(response.Errors);
                Warnings.AddRange(response.Warnings);
                Messages.AddRange(response.Messages);
                Success = response.Success;
            }

            return this;
        }

        public T Data { get; set; }

        public static implicit operator GenericResponse<T>(T data)
        {
            return new GenericResponse<T>(data);
        }
    }
}
