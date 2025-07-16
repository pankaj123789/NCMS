using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

namespace MyNaati.Bl.Portal.Security
{
    /// <summary>
    /// Inserts and retrieves WebUser in WCF headers.
    /// </summary>
    //public class ContextUserMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    //{
    //    private Func<NaatiWebUser> mGetWebUser;

    //    public ContextUserMessageInspector()
    //        : this(null) { }

    //    /// <summary>
    //    /// Use this constructor to define how to obtain the current WebUser.
    //    /// </summary>
    //    /// <param name="getWebUser"></param>
    //    public ContextUserMessageInspector(Func<NaatiWebUser> getWebUser)
    //    {
    //        mGetWebUser = getWebUser;
    //    }

    //    private NaatiWebUser CurrentWebUser
    //    {
    //        get
    //        {
    //            if (mGetWebUser == null)
    //                return null;

    //            return mGetWebUser.Invoke();
    //        }
    //    }

    //    #region IClientMessageInspector Members

    //    public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    //    {
    //        //do nothing
    //    }

    //    public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
    //    {
    //        if (CurrentWebUser != null)
    //        {
    //            if (request.Headers.FindHeader(WcfHeaderKeys.HEADER_NAME, WcfHeaderKeys.NAMESPACE) < 0)
    //            {
    //                request.Headers.Add(MessageHeader.CreateHeader(
    //                    WcfHeaderKeys.HEADER_NAME,
    //                    WcfHeaderKeys.NAMESPACE,
    //                    CurrentWebUser));
    //            }
    //            else
    //            {
    //                //The header must have already been set by some other means; probably via a 
    //                //[MessageHeaderAttribute] like on PractitionerDirectoryUpdateListingRequest
    //            }
    //        }
    //        return null;
    //    }

    //    #endregion

    //    #region IDispatchMessageInspector Members

    //    public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
    //    {
    //        Int32 headerPosition = request.Headers.FindHeader(WcfHeaderKeys.HEADER_NAME, WcfHeaderKeys.NAMESPACE);
    //        if (headerPosition >= 0)
    //        {
    //            NaatiWebUser user = request.Headers.GetHeader<NaatiWebUser>(
    //                WcfHeaderKeys.HEADER_NAME,
    //                WcfHeaderKeys.NAMESPACE);

    //            OperationContext.Current.IncomingMessageProperties.Add(WcfHeaderKeys.CONTEXT_USER_KEY, user);
    //        }

    //        return null;
    //    }

    //    public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    //    {
    //        //do nothing
    //    }

    //    #endregion
    //}
}