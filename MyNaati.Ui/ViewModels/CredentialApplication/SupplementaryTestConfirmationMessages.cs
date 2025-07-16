namespace MyNaati.Ui.ViewModels.CredentialApplication
{
	public class SupplementaryTestConfirmationMessages : ISponsoredPaymentConfirmationMessages
	{
		private const string ConfirmationTrustedPayer = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application for a Supplementary Test to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ul>
        <li>NAATI will now begin processing your application and you will be contacted when a test date is available.</li>
    </ul>
</ul>

<p>Please note that it may take several months for a test place to become available.</p>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

		private const string ConfirmationNonTrustedPayer = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application for a Supplementary Test to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ul>
        <li>NAATI will now begin processing your application and your sponsoring organisation will be invoiced.</li>
		<li>After payment has been received you will be contacted when a test date is available.</li>
    </ul>
</ul>

<p>Please note that it may take several months for a test place to become available.</p>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

		private const string ConfirmationCreditCard = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application for a Supplementary Test to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<p>You should also note your credit card transaction reference: <strong>[[Securepay Reference]]</strong>.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ul>
        <li>Your payment will be processed by NAATI immediately and you will receive a second email containing your invoice within 24 hours.</li>
		<li>Your application will then begin to be processed and you will then be contacted when a test date becomes available.</li>
    </ul>
</ul>

<p>Please note that it may take several months for a test place to become available.</p>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";

private const string ConfirmationPayPal = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application for a Supplementary Test to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<p>You should also note your PayPal transaction reference: <strong>[[PayPal Reference]]</strong>.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ul>
        <li>Your payment will be processed by NAATI immediately and you will receive a second email containing your invoice within 24 hours.</li>
		<li>Your application will then begin to be processed and you will then be contacted when a test date becomes available.</li>
    </ul>
</ul>

<p>Please note that it may take several months for a test place to become available.</p>

<p><a href=""[[Base Url]]Applications/MyTestResults/"">Click here</a> to return to the My Test Results page.</p>";
        private const string ConfirmationCashDirectDeposit = @"<h2 style=""color:#008C7E;""><strong>Form Submitted Successfully</strong></h2>
<br/>
<p>Thank you for submitting your application for a Supplementary Test to NAATI. Here is your application number for future reference: <strong>[[Application Reference]]</strong></p>
<p>We have also emailed you this information as confirmation.</p>
<br/>
<p><strong>What happens now?</strong></p>

<ul>    
    <ul>
		<li>NAATI will email your invoice within 24 hours. All invoices need to be paid within 3 days, otherwise NAATI reserves the right to cancel your application.</li>
		<li>NAATI will then reconcile your payment. This can take up to 72 hours.</li>
		<li>Once reconciled, your application is then added to the processing queue for NAATI to check. </li>
		<li>Your application will then begin to be processed.  You will then be contacted when a test date is available.</li>
    </ul>
</ul>

<p>Please note that it may take several months for a test place to become available.</p>

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

		public string CreditCard => ConfirmationCreditCard;

		public string CashDirectDeposit => ConfirmationCashDirectDeposit;
	    public string Error => ErrorContent;

        public string PayPal => ConfirmationPayPal;
    }
}