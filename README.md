# GreenTube_QA_Assignment_API_UI_Test
End To End Test -  API and UI Automation

GreenTube_QA_Assignment :

Tech Stack :

> c#
> RestSharp  for API
> Specflow - BDD
> Selenium Webdriver  for UI
> Nunit test framework
WORKSPACE = GameTwistAssignment/

Test:

Feature File - GameTwistPurchaseFeature.feature

    > End to End Test is captured here
	> The following endpoints are called in order to make a Purchase:
			1.	POST Login-v1
			2.	POST Consent-v1
			3.	GET Consent-v1
			4.	POST UpgradeToFullRegistration-v1
			5.	POST Purchase-v1
	> After successful endpoints call, we would get a payment redirect url where we can make a purchase .
Step-Definition File - GameTwistPurchase_End_To_End_TestStep.cs under 'Steps' directory
