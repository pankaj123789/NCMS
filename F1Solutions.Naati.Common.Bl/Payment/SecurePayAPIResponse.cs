using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Bl.Payment
{
    public class SecurePayAPIResponse
    {
        public IEnumerable<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Detail { get; set; }
        public Source Source { get; set; }
    }

    public class Source
    {
        public string Pointer { get; set; }
    } 
}
