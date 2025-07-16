using System;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using F1Solutions.Naati.Common.Contracts.Dal;

namespace F1Solutions.Naati.Common.Dal
{
    /// <summary>
    /// This auth policy extracts the "Identity" header from the incoming message and sets the current principle from it.
    /// This allows the calling client to pass the current user, mainly for the purposes of auditing and logging.
    ///// </summary>
    //[DebuggerStepThrough]
    //public class CustomAuthPolicy : IAuthorizationPolicy
    //{
    //    public string Id { get; }

    //    public bool Evaluate(EvaluationContext evaluationContext, ref object state)
    //    {
    //        var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
    //        var username = request?.Headers["Identity"];
    //        if (!String.IsNullOrEmpty(username))
    //        {
    //            evaluationContext.Properties["Principal"] = new NcmsPrincipal(username); 
    //        }
    //        return true;
    //    }

    //    public ClaimSet Issuer => ClaimSet.System;
    //}
}