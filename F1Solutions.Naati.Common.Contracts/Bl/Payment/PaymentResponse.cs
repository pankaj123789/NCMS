namespace F1Solutions.Naati.Common.Contracts.Bl.Payment
{
    public class PaymentResponse
    {
        public bool PaymentApproved { get;  set; }

        /// <summary>
        /// An error that occurred while trying to call the remote payment system. May be a comms problem
        /// or an internal coding problem somewhere (possibly in the remote system).
        /// </summary>
        /// <remarks>
        /// The presence of an error message here doesn't mean the payment wasn't approved.
        /// </remarks>
        public string SystemErrorMessage { get;  set; }

        /// <summary>
        /// An error code returned from the remote payment system.
        /// </summary>
        public string PaymentErrorCode { get;  set; }

        /// <summary>
        /// An error message returned from the remote payment system.
        /// </summary>
        public string PaymentErrorDescription { get;  set; }

        /// <summary>
        /// This is expected to have a value if PaymentApproved == true
        /// </summary>
        public string AssignedTransactionId { get;  set; }

        public CardDetails CardDetailsForReceipt { get; set; }
    }

    public class CardDetails
    {
        public string CardNumber { get;  set; }

        public string ExpiryMonth { get;  set; }

        public CardType Type { get;  set; }
    }

    public enum CardType
    {
        Unknown = 0,

        JCB = 1,

        Amex = 2,

        DinersClub = 3,

        Bankcard = 4,

        MasterCard = 5,

        Visa = 6
    }
}
