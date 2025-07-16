using F1Solutions.Naati.Common.Wiise;
using System;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public abstract class WiiseOperation
    {
        public abstract string Output { get; protected set; }
        public abstract object Result { get; }
        public abstract Task PerformOperation(IWiiseIntegrationService wiiseService, WiiseToken token);
        public int OperationId { get; set; }
    }

    public class WiiseOperation<TRequest, TWiiseObjectType> : WiiseOperation
        where TWiiseObjectType : class
    {
        protected TWiiseObjectType Input { get; set; }
        protected TWiiseObjectType ProtectedResult { get; set; }

        public override object Result
        {
            get { return ProtectedResult; }
        }

        public override string Output { get; protected set; }

        public TWiiseObjectType GetPreparedInput()
        {
            PrepareInput();
            return Input;
        }

        protected virtual Task PrepareInput()
        {
            throw new NotImplementedException();
        }

        protected virtual Task<TWiiseObjectType> ProtectedPerformOperation()
        {
            throw new NotImplementedException();
        }

        protected virtual void PrepareOutput() { }

        public override async Task PerformOperation(IWiiseIntegrationService wiiseService, WiiseToken token)
        {
            WiiseService = wiiseService;
            Token = token;
            await PrepareInput();
            ProtectedResult = await ProtectedPerformOperation();
            PrepareOutput();
        }

        public TRequest Request { 
            get; 
            set; 
        }

        protected IWiiseIntegrationService WiiseService { get; private set; }
        protected WiiseToken Token { get; private set; }

        protected IWiiseAccountingApi Wiise
        {
            get
            {
                if (WiiseService == null)
                {
                    throw new Exception("Must provide a WiiseIntegrationService,");
                }
                return WiiseService.Api;
            }
        }
    }

    public abstract class WiiseCompletionOperation
    {
        public abstract void PerformOperation(object operationResult);
    }
}
