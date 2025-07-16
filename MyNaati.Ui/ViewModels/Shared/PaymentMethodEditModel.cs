using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class PaymentMethodEditModel
    {
        private const int mCreditCardValue = 1;

        public PaymentMethodEditModel()
        {
            PaymentMethodSelectListItems = new List<SelectListItem>();
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Credit Card (allows online completion of your order)  We accept MasterCard and Visa.", Value = mCreditCardValue.ToString() });
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Australian Money Order", Value = "2" });
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Amex", Value = "6" });
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Bank Cheque (a cheque purchased from a bank)", Value = "3" });
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Overseas Bank Draft", Value = "4" });
            PaymentMethodSelectListItems.Add(new SelectListItem() { Text = "Cash (payable at any NAATI office)", Value = "5" });
        }

        [UIHint("radioButtonListInt")]
        public int PaymentMethodId { get; set; }

        public IList<SelectListItem> PaymentMethodSelectListItems { get; set; }

        public bool IsOnlineOrder
        {
            get
            {
                return PaymentMethodId == mCreditCardValue;
            }
        }
    }
}