GreenTube_QA_Assignment :

Tech Stack :
	> c#
	> RestSharp
	> Specflow
	> Selenium Webdriver
	> Nunit test framework

WORSPACE = GameTwistAssignment 


Test:

Feature File - GameTwistPurchaseFeature.feature
        > End to End Test is captured here
		> The following endpoints are called in order to make a Purchase:
				1.	POST Login-v1
				2.	POST Consent-v1
				3.	GET Consent-v1
				4.	POST UpgradeToFullRegistration-v1
				5.	POST Purchase-v1
		> After successful endpoints call, can make a purchase .

Step-Definition File - GameTwistPurchase_End_To_End_TestStep.cs under 'Steps' directory

