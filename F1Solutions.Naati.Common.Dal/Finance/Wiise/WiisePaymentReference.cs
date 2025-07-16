using System.Collections.Generic;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Wiise.PublicModels;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public sealed class WiisePaymentReference : WiiseReference
    {
        // Reference part constants
        private const string ChequeAbbreviation = "CHQ";
        private const string EftPosAbbreviation = "EFTPOS";
        private const string CashAbbreviation = "CASH";
        private const string AmexAbbreviation = "AMEX";

        // Display payment type constants
        private const string DirectDeposit = "Direct Deposit";
        private const string Cheque = "Cheque";
        private const string EftPos = "EFTPOS";
        private const string Cash = "Cash";
        private const string Amex = "AMEX";

        // Reference part indexes
        private const int PaymentTypeReferencePartIndex = 1;
        private const int BsbReferencePartIndex = 2;
        private const int ChequeNumberReferencePartIndex = 3;
        private const int BankNameReferencePartIndex = 4;
        private const int EftMachineReferencePartIndex = 2;

        // Payment type indexes
        private const int PaymentTypeIndex = 0;
        private const int BsbIndex = 1;
        private const int ChequeNumberIndex = 2;
        private const int BankNameIndex = 3;
        private const int EftMachineIndex = 1;

        public Payment Payment { get; }
        public string PaymentAccount { get; }
        public bool IsDirectDeposit => Payment?.Account != PaymentAccount;
        public bool IsChequePayment => PaymentType.Equals(Cheque);
        public bool IsEftPayment => PaymentType.Equals(EftPos);
        public bool IsAmexPayment => PaymentType.Equals(Amex);

        private string[] _paymentTypeParts;
        private string[] PaymentTypeParts => _paymentTypeParts ?? (_paymentTypeParts = ParsePaymentTypeParts());

        private string _paymentType;
        public string PaymentType => _paymentType ?? (_paymentType = PaymentTypeParts.Length > PaymentTypeIndex ? PaymentTypeParts[PaymentTypeIndex] : string.Empty);

        private string _bsb;
        public string Bsb => _bsb ?? (_bsb = IsChequePayment && PaymentTypeParts.Length > BsbIndex ? PaymentTypeParts[BsbIndex] : string.Empty);

        private string _chequeNumber;
        public string ChequeNumber => _chequeNumber ?? (_chequeNumber = IsChequePayment && PaymentTypeParts.Length > ChequeNumberIndex ? PaymentTypeParts[ChequeNumberIndex] : string.Empty);

        private string _bankName;
        public string BankName => _bankName ?? (_bankName = IsChequePayment && PaymentTypeParts.Length > BankNameIndex ? PaymentTypeParts[BankNameIndex] : string.Empty);

        private string _eftMachine;
        public string EftMachine => _eftMachine ?? (_eftMachine = (IsEftPayment || IsAmexPayment) && PaymentTypeParts.Length > EftMachineIndex ? PaymentTypeParts[EftMachineIndex] : string.Empty);

        public WiisePaymentReference(Payment payment, IEnumerable<Office> offices, string wiisePaymentAccount)
            : base(payment?.Reference, offices)
        {
            Payment = payment;
            PaymentAccount = wiisePaymentAccount;
        }

        /// <summary>
        /// Cash constructor
        /// </summary>
        /// <param name="office"></param>
        public WiisePaymentReference(Office office)
        {
            Office = office;
            Reference = $"{OfficeAbbreviation}-{CashAbbreviation}";
        }

        /// <summary>
        /// EFT constructor
        /// </summary>
        /// <param name="office"></param>
        /// <param name="eftMachine"></param>
        /// <param name="isAmex"></param>
        public WiisePaymentReference(Office office, string eftMachine, bool isAmex)
            : this(office)
        {
            _eftMachine = eftMachine;
            Reference = $"{OfficeAbbreviation}-{(isAmex ? AmexAbbreviation : EftPosAbbreviation)}-{_eftMachine}";
        }

        public WiisePaymentReference(Office office, string payPalReference)
            : this(office)
        {
            Reference = payPalReference;
        }

        /// <summary>
        /// Cheque constructor
        /// </summary>
        /// <param name="office"></param>
        /// <param name="bsb"></param>
        /// <param name="chequeNumber"></param>
        /// <param name="bankName"></param>
        public WiisePaymentReference(Office office, string bsb, string chequeNumber, string bankName)
            : this(office)
        {
            _bsb = bsb;
            _chequeNumber = chequeNumber;
            _bankName = bankName;
            Reference = $"{OfficeAbbreviation}-{ChequeAbbreviation}-{bsb}-{chequeNumber}-{bankName}";
        }

        private string[] ParsePaymentTypeParts()
        {
            var paymentTypeParts = new List<string>();
            var paymentTypePart = ReferenceParts.Length > PaymentTypeReferencePartIndex ? ReferenceParts[PaymentTypeReferencePartIndex] : string.Empty;
            var paymentUpper = paymentTypePart.ToUpper();


            if (IsDirectDeposit)
            {
                paymentTypeParts.Add(DirectDeposit);
                return paymentTypeParts.ToArray();
            }

            switch (paymentUpper)
            {
                case ChequeAbbreviation:
                    paymentTypeParts.Add(Cheque);

                    if (ReferenceParts.Length > BsbReferencePartIndex)
                    {
                        paymentTypeParts.Add(ReferenceParts[BsbReferencePartIndex]);
                    }

                    if (ReferenceParts.Length > ChequeNumberReferencePartIndex)
                    {
                        paymentTypeParts.Add(ReferenceParts[ChequeNumberReferencePartIndex]);
                    }

                    if (ReferenceParts.Length > BankNameReferencePartIndex)
                    {
                        paymentTypeParts.Add(ReferenceParts[BankNameReferencePartIndex]);
                    }

                    break;
                case EftPosAbbreviation:
                    paymentTypeParts.Add(EftPos);
                    AddEftMachinePart(paymentTypeParts);
                    break;
                case AmexAbbreviation:
                    paymentTypeParts.Add(Amex);
                    AddEftMachinePart(paymentTypeParts);
                    break;
                case CashAbbreviation:
                    paymentTypeParts.Add(Cash);
                    break;
                default:
                    paymentTypeParts.Add(Unknown);
                    break;

            }

            return paymentTypeParts.ToArray();
        }

        private void AddEftMachinePart(ICollection<string> paymentTypeParts)
        {
            if (ReferenceParts.Length > EftMachineReferencePartIndex)
            {
                paymentTypeParts.Add(ReferenceParts[EftMachineReferencePartIndex]);
            }
        }
    }
}
