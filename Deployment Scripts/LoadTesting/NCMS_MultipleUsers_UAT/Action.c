//   *****************************************************************************************************************************************
//   ****   PLEASE NOTE: This is a READ-ONLY representation of the actual script. For editing please press the "Develop Script" button.   ****
//   *****************************************************************************************************************************************

Action()
{
	lr_start_transaction("01_Navigate to myNaati");
	truclient_step("1", "Navigate to 'https://uatmynaati.naati.com.au/'", "snapshot=Action_1.inf");
	lr_end_transaction("01_Navigate to myNaati",0);
	truclient_step("2", "Wait 3 seconds", "snapshot=Action_2.inf");
	truclient_step("3", "Click on User name textbox", "snapshot=Action_3.inf");
	truclient_step("4", "Type TC.getParam('UserID') in User name textbox", "snapshot=Action_4.inf");
	truclient_step("5", "Click on Password passwordbox", "snapshot=Action_5.inf");
	truclient_step("6", "Type ************ in Password passwordbox", "snapshot=Action_6.inf");
	lr_start_transaction("02_Login");
	truclient_step("7", "Click on Sign in button", "snapshot=Action_7.inf");
	lr_end_transaction("02_Login",0);
	truclient_step("120", "Click on Manage My Tests link", "snapshot=Action_120.inf");
	truclient_step("121", "Click on Select Test Session button", "snapshot=Action_121.inf");
	truclient_step("122", "Click on Select this Test Session button", "snapshot=Action_122.inf");
	truclient_step("123", "Click on Next button", "snapshot=Action_123.inf");
	truclient_step("124", "Click on This field is required. label", "snapshot=Action_124.inf");
	truclient_step("125", "Click on Card number textbox", "snapshot=Action_125.inf");
	truclient_step("126", "Type TC.getParam('CardNumber') in Card number textbox", "snapshot=Action_126.inf");
	truclient_step("127", "Click on Expiry date textbox", "snapshot=Action_127.inf");
	truclient_step("128", "Type 12 / 25 in Expiry date textbox", "snapshot=Action_128.inf");
	truclient_step("129", "Click on CVV textbox", "snapshot=Action_129.inf");
	truclient_step("130", "Type 321 in CVV textbox", "snapshot=Action_130.inf");
	truclient_step("131", "Click on Next button", "snapshot=Action_131.inf");
	truclient_step("132", "Wait 2 seconds", "snapshot=Action_132.inf");
	truclient_step("133", "Click on This answer is required. radio", "snapshot=Action_133.inf");
	truclient_step("134", "Wait 3 seconds", "snapshot=Action_134.inf");
	lr_start_transaction("03_Payment");
	truclient_step("135", "Click on Finish button", "snapshot=Action_135.inf");
	lr_end_transaction("03_Payment",0);
	lr_start_transaction("04_Back to home page");
	truclient_step("136", "Click on Click here link", "snapshot=Action_136.inf");
	lr_end_transaction("04_Back to home page",0);

	return 0;
}
