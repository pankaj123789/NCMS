using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace MyNaati.Bl.Portal.Security
{
    /// <summary>
    /// Attaches the CurrentUserInspector to all WCF calls this is configured 
    /// to handle.
    /// Virtual property NewInspector must be overridden to define a way for 
    /// current WebUser to be retrieved.
    /// </summary>
    //public class ContextUserBehavior : BehaviorExtensionElement, IEndpointBehavior
    //{
    //    #region IEndpointBehavior Members

    //    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    //    {
    //        return;
    //    }

    //    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    //    {
    //        clientRuntime.MessageInspectors.Add(NewInspector);
    //    }

    //    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    //    {
    //        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(NewInspector);
    //    }

    //    public void Validate(ServiceEndpoint endpoint)
    //    {
    //        return;
    //    }

    //    #endregion

    //    protected virtual ContextUserMessageInspector NewInspector
    //    {
    //        get
    //        {
    //            return new ContextUserMessageInspector();
    //        }
    //    }

    //    public override Type BehaviorType
    //    {
    //        get
    //        {
    //            return typeof(ContextUserBehavior);
    //        }
    //    }

    //    protected override object CreateBehavior()
    //    {
    //        return new ContextUserBehavior();
    //    }
    //}
}