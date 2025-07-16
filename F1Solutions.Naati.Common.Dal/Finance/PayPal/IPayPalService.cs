using PayPal.Api;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.PayPal
{
    public interface IPayPalService
    {
        Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId);

        DetailedRefund ExecuteRefund(APIContext apiContext, string amount, string saleId);

        Payment CreatePayment(APIContext apiContext, string redirectUrl, string paymentReference, string unitType, string amount);

    }
}
