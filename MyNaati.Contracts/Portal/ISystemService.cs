using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    
    public interface ISystemService : IInterceptableservice
    {
        DiagnosticResponse RunDiagnostics(DiagnosticRequest request);
    }

    
    public class DiagnosticResponse
    {
        public DiagnosticResponse()
        {
        }

        public bool FullyFunctional
        {
            get
            {
                return Errors.Count() == 0;
            }
        }

        
        public MessageNode[] Errors { get; set; }
    }

    
    public class MessageNode
    {
        
        public string Message { get; set; }

        
        public MessageNode[] Children { get; set; }
    }

    
    public class DiagnosticRequest
    {
    }
}
