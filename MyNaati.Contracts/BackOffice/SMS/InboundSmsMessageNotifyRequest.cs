namespace MyNaati.Contracts.BackOffice.SMS
{
    
    public class InboundSmsMessageNotifyRequest
    {
        
        public string Message { get; set; }

        
        public string FromNumber { get; set; }

        
        public string ToNumber { get; set; }
    }
}