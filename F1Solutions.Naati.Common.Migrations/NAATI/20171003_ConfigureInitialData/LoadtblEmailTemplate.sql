INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Application Rejected' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>Thank you for submitting your transition application. Unfortunately, we have chosen to reject your application.</p>
<p>[[Action Public Note]]</p>
<p>We would urge you to look at the information regarding NAATI certification on our website to learn more about alternative pathways of gaining a NAATI credential.</p>
<p>If you have any further questions, please email <a href="mailto://info@naati.com.au">info@naati.com.au</a></p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Application Outcome – Application [[Application Reference]]' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Credential Request Cancelled' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>This email is to confirm that we have cancelled your transition to [[Credential Type]] [[Skill]].</p>
<p>[[Action Public Note]]</p>
<p>Should you wish to submit a new application to obtain a NAATI credential in future, please head to our website  <a href="http://www.naati.com.au">(http://www.naati.com.au)</a>.</p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Cancellation Request – Application [[Application Reference]]' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Assessment Failed' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>After reviewing your transition application we have chosen not to award you <b>[[Credential Type]]</b> [[Skill]].</p>
<p>[[Action Public Note]]</p>
<p>We would urge you to look at the information regarding NAATI certification on our website to learn more about alternative pathways of gaining your preferred NAATI credential.</p>
<p>If you have any further questions, please email <a href="mailto://info@naati.com.au">info@naati.com.au</a>.</p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Assessment Outcome – Application [[Application Reference]]' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Pending Assessment Failed' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>After reviewing your transition application and the additional information you have provided, we have chosen not to award you <b>[[Credential Type]]</b> [[Skill]].</p>
<p>[[Action Public Note]]</p>
<p>We would urge you to look at the information regarding NAATI certification on our website to learn more about alternative pathways of gaining your preferred NAATI credential.</p>
<p>If you have any further questions, please email <a href="mailto://info@naati.com.au">info@naati.com.au</a>.</p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Assessment Outcome – Application [[Application Reference]]' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Assessment Pending' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>After reviewing your transition application, we have determined that we will need some further information before we can finalise your application to transition to <b>[[Credential Type]]</b> [[Skill]].</p>
<p>[[Action Public Note]]</p>
<p>Once you have supplied us with the relevant information, we will notify you of the outcome of your application within 14 business days.</p>
<p>If you have any further questions, please email <a href="mailto://info@naati.com.au">info@naati.com.au</a>.</p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'NAATI Application Update – Application [[Application Reference]]' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Credential Issued' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>On behalf of NAATI, we would like to congratulate you on obtaining your [[Credential Type]] [[Skill]] credential. Attached to this email are:
<ul>
<li>Your official credentialing letter (including the conditions of your certification); and </li>
<li>An electronic certificate </li>
</ul>
</p>
<p>You will be issued a <TRANSLATOR STAMP/ID CARD> to use and have your contact details published in our Online Directory of Translators and Interpreters. This directory enables third parties to search for and contact practitioners.</p>
<p>As a NAATI credentialed practitioner, you also have been granted access to our exclusive online practitioner portal. Practitioners can use the online portal to:
<ul>
<li>Access industry information and resources for recertification;</li>
<li>Access and submit applications;</li>
<li>Order additional products;</li>
<li>Update your details; and</li>
<li>Edit or opt out of your online directory listing.</li>
</ul>
</p>
<p>Please click here to complete your portal registration.</p>
<p>All individuals who are awarded NAATI credentials can have these credentials verified via an online verification tool available on the  <a href="http://www.naati.com.au">NAATI website</a>. This verification tool will display:
<ul>
<li>Your name;</li>
<li>The state you are located in;</li>
<li>Your photograph (as supplied with your original application);</li>
<li>Each credential you have been awarded; and</li>
<li>The expiry date of each credential you have been awarded. </li>
</ul>
</p>
<p>Practitioners cannot opt out of the online verification tool. Please consult NAATI’s Privacy Policy for more information around how NAATI uses your information. </p>
<p>Congratulations once again. </p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Congratulations! You have been awarded a NAATI Credential' 
)


INSERT INTO tblEmailTemplate 
			([Name], [Content], [Active], [Subject]) 
VALUES 
(
   N'Application Submitted' 
  ,N'<html><head><style>body{{font-family:calibri}}</style></head><body>
<p>Dear [[Given Name]],</p>
<p>Thank you for submitting your transition application. Your application reference number is [[Application Reference]].</p>

<p><b>What happens next?</b></p>
<p>NAATI will now begin processing your application. Should we need to follow up any missing or additional information we will contact you using this email address.</p>
<p>We will notify you of the outcome of your application via email in approximately 4-6 weeks.</p>
<p>
	Sincerely, 
	<br/>
	<br/>
	NAATI Certification Team
	<br/>
	National Office
</p>
<table>
	<tbody>
		<tr>
			<td> <img src="https://naati.com.au/wp-content/uploads/2019/07/Primary_logo_RGB.jpg" height="80" width="80"/></td>
			<td style="border-left:#b1c2bc 1px solid;">
				<table  style="margin-left:20px;margin-top:-20px;font: 14px Calibri, sans-serif;" >
					<tbody>
						<tr style=" height: 25px;"><td>National Accreditation Authority for Translators and Interpreters Ltd</td></tr>
						<tr style=" height: 25px;"><td><span style="color:#b1c2bc">T:</span> +61 02 6260 3035</td></tr>
						<tr style=" height: 25px;"><td>P.O Box 223 Deakin West ACT 2600 AUSTRALIA</td></tr>						
					</tbody>
				</table>				
			</td>
		</tr>
	</tbody>
</table>
<span style="font: 10px Calibri, sans-serif; font-style: italic;">
This email, and any attachments, may be confidential and privileged. If you are not the intended recipient, please notify the sender and delete all copies of this transmission along with any <br/> attachments immediately. You should not copy or use it for any purpose, nor disclose its contents to any other person.
</span>
</body></html>' 
  ,1  
  ,'Application Submitted – Application [[Application Reference]]' 
)

