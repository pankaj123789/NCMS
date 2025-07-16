using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmailService" in both code and config file together.
    
    public interface IEmailService : IInterceptableservice
    {
        
        SendEmailResponse SendMail(SendEmailRequest request);
    }

    
    public class SendEmailRequest
    {
        /// <summary>
        /// Initialises the recipient list and adds the recipient and initialises the token dictionary.
        /// </summary>
        /// <param name="templateKey"></param>
        /// <param name="recipient"></param>
        public SendEmailRequest(EmailTemplate templateKey, string recipient)
        {
            TemplateKey = templateKey;
            Recipients = new List<string> { recipient };
            Tokens = new Dictionary<EmailTokens, string>();
        }

        public SendEmailRequest(){ }


        public EmailTemplate TemplateKey { get; set; }

        
        public IList<string> Recipients { get; set; }

        
        public IDictionary<EmailTokens, string> Tokens { get; set; }
    }

    
    public class SendEmailResponse
    {
        
        public bool Success { get; set; }

        
        public string Exception { get; set; }
    }

    public enum EmailTemplate
    {
        ResetPassword,
        RegisterSuccess,
        NonCandidateRegisterSuccess,
        EoiSubmission,
        ApplicationSubmission,
        NewPersonCreated,
        EmailChangeRequest,
        EmailChangeConfirmation,
        SendAccessCode
    }

    public enum EmailTokens
    {
        GivenName,
        Password,
        NaatiNumber,
        Url,
        Email,
        Hours,
        AccessCode,
        AccessCodeValidity,
    }
}
