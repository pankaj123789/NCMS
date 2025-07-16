namespace MyNaati.Contracts.BackOffice.SMS
{
    
    public interface IInboundSmsService
    {
        
        InboundSmsMessageNotifyResponse NotifyInboundSmsMessage(InboundSmsMessageNotifyRequest request);
    }
}
