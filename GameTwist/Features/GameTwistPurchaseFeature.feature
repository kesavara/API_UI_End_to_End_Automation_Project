
Feature: GameTwist
End to End Test 
@smoke
Scenario: A registered player should be able to make a purchase on Gametwist.com
	Given we perform a post operation for login to authenticate the user 
	Then verify status code returns ok and save the authentication token
	And we perform a post for consent api so the player changes the acceptance status for type
	Then verify status code returns ok for above post call
	When we perform a get operation to consent api
	Then it returns the current acceptance status of the specified consent type for the currently logged in player 
	And we perform a post call to upgradeToFullRegistration api to become fully registered player
	Then verify status code returns OK for upgraded or registered player
	And finally to perform an item purchase we make post call purchase api
	Then verify paymentdirect url is received for valid registered user
	And we launch a browser using the paymentRedirectUrl
	When page is loaded click on next button that takes to payment provider
	And select bank austria from provider dropdown and click continue
	When adding random values to the two input boxes and click the login button and you should see a failure
	And click the cancel button to get redirected back to the gametwist page
	Then take a screenshot and close the browser




	





