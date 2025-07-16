namespace MyNaati.Ui.ViewModels.CredentialApplication
{
    public class PaidTestReviewConfirmationMessages : ISponsoredPaymentConfirmationMessages
    {
        private const string ConfirmationTrustedPayer = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI.Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>NAATI will now begin processing your application. Should we need to follow up any missing or additional information, we will contact you.</li>
        <li>After your application has been checked and approved, your test will be reviewed, and you will be advised of the outcome – this can take up to 8 weeks.</li>
    </ol>
</ul>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        private const string ConfirmationNonTrustedPayer = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>NAATI will now begin processing your application. Should we need to follow up any missing or additional information, we will contact you.</li>
        <li>After your application has been checked and approved, your sponsoring organisation will be invoiced.</li>
        <li>After the payment has been processed, your test will be reviewed, and you will be advised of the outcome – this can take up to 8 weeks.</li>
    </ol>
</ul>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        private const string ConfirmationCCLCreditCard = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<p>Also note your credit card transaction reference: <strong>[[Securepay Reference]]</strong> </p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>Your payment will be processed by NAATI immediately and you will receive a second email containing your invoice within 24 hours.</li>
        <li>Your application will then begin to be processed. Should we need to follow up any missing or additional information, we will contact you.</li>
        <li>After your application has been checked and approved, your test will be reviewed, and you will be advised of the outcome – this can take up to 8 weeks.</li>
    </ol>
</ul>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        private const string ConfirmationCCLPayPal = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<p>Also note your PayPal transaction reference: <strong>[[PayPal Reference]]</strong> </p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ol>
        <li>Your payment will be processed by NAATI immediately and you will receive a second email containing your invoice within 24 hours.</li>
        <li>Your application will then begin to be processed. Should we need to follow up any missing or additional information, we will contact you.</li>
        <li>After your application has been checked and approved, your test will be reviewed, and you will be advised of the outcome – this can take up to 8 weeks.</li>
    </ol>
</ul>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        private const string ConfirmationContentCashDirectDeposit = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your review application to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong>

<ul>
    <ol>
        <li>NAATI will email your invoice within 24 hours. All invoices need to be paid within 3 days, otherwise NAATI reserves the right to cancel your application.</li>
        <li>NAATI will then reconcile your payment. This can take up to 72 hours.</li>
        <li>Once reconciled, your application is then added to the processing queue for NAATI to check.</li>
        <li>Your application will then begin to be processed. Should we need to follow up any missing or additional information, we will contact you.</li>
        <li>After the application has been checked and approved, your test will be reviewed, and you will be advised of the outcome – this can take up to 8 weeks.</li>
    </ol>
</ul>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        private const string ErrorContent = @"<h2>Sorry, we were unable to process your request.</h2>
        <div class=""text-center wrapper-xl"">
        <span class=""fa fa-warning text-warning fa-5x""></span>
        <h4>Reason: [[Error Message]]</h4>    
        </div>
        <p>Please try again or contact <a href=""mailto://info@naati.com.au"">info@naati.com.au</a></p> 
        <p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

        public string TrustedPayer => ConfirmationTrustedPayer;

        public string NonTrustedPayer => ConfirmationNonTrustedPayer;

        public string CreditCard => ConfirmationCCLCreditCard;

        public string CashDirectDeposit => ConfirmationContentCashDirectDeposit;
        public string Error => ErrorContent;

        public string PayPal => ConfirmationCCLPayPal;
    }
}