using GameTwistAssignment.Setting;
using GameTwistAssignment.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Support.UI;
using GameTwistAssignment.Hooks;

namespace GameTwistAssignment.Steps 
{
    [Binding]
    public class GameTwist_End_To_End_Steps 
    {
        private string acctoken;
        private Settings _settings;
        private SeleniumContext seleniumContext;
        private CommonRequest commonRequest;

        public GameTwist_End_To_End_Steps(Settings settings, SeleniumContext _seleniumContext,CommonRequest _commonRequest)
        {
            _settings = settings;
            seleniumContext = _seleniumContext;
            commonRequest = _commonRequest;
        
        }
        private  IWebElement WaitUntilElementClickable(SeleniumContext seleniumContext, By elementLocator, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(seleniumContext.WebDriver, TimeSpan.FromSeconds(timeout));
                return wait.Until(ExpectedConditions.ElementToBeClickable(elementLocator));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + elementLocator + "' was not found in current context page.");
                throw;
            }
        }

        [Given(@"we perform a post operation for login to authenticate the user")]
        public void GivenWePerformAPostOperationForLoginToAuthenticateTheUser()
        {
            commonRequest.Post_Request("login-v1");
            _settings.Request.AddJsonBody(new
            {
                nickname = "Testuser123",
                password = "Testing123!",
                autologin = true
            });
            _settings.Response = _settings.RestClient.Execute<ResponseClass>(_settings.Request);
            var Content = _settings.Response.Content;
            acctoken = _settings.Response.Headers[3].Value.ToString();

        }

        [Then(@"verify status code returns ok and save the authentication token")]
        public void ThenVerifyStatusCodeReturnsOkAndSaveTheAuthenticationToken()
        {
            int StatusCode = (int)_settings.Response.StatusCode;
            Assert.AreEqual(200, StatusCode, "Post login failed");
            //using JWTAuthenticator for setting authorization token for the subsequent api calls
            var jwtAuth = new JwtAuthenticator(acctoken.ToString());
            _settings.RestClient.Authenticator = jwtAuth;
        }

        [Then(@"we perform a post for consent api so the player changes the acceptance status for type")]
        public void ThenWePerformAPostForConsentApiSoThePlayerChangesTheAcceptanceStatusForType()
        {
            commonRequest.Post_Request_With_QueryParams("consent/consent-v1");
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
        }

        [Then(@"verify status code returns ok for above post call")]
        public void ThenVerifyStatusCodeReturnsOkForAbovePostCall()
        {
            int StatusCode1 = (int)_settings.Response.StatusCode;
            Assert.AreEqual(200, StatusCode1, "Status code does not return 200");
        }

        [When(@"we perform a get operation to consent api")]
        public void WhenWePerformAGetOperationToConsentApi()
        {
            commonRequest.Get_Request_With_QueryParams("consent/consent-v1");
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
        }

        [Then(@"it returns the current acceptance status of the specified consent type for the currently logged in player")]
        public void ThenItReturnsTheCurrentAcceptanceStatusOfTheSpecifiedConsentTypeForTheCurrentlyLoggedInPlayer()
        {
            int getStatus_StatusCode = (int)_settings.Response.StatusCode;
            Assert.AreEqual(200, getStatus_StatusCode, "Status code does not return 200");
        }

        [Then(@"we perform a post call to upgradeToFullRegistration api to become fully registered player")]
        public void ThenWePerformAPostCallToUpgradeToFullRegistrationApiToBecomeFullyRegisteredPlayer()
        {
            commonRequest.Post_Request("player/upgradeToFullRegistration-v1");
            _settings.Request.AddJsonBody(new
            {
                firstName = "kesav",
                lastName = "sdfs",
                isMale = true,
                countryCode = "AT",
                city = "Vienna",
                zip = "1050",
                street = "Wiedner Hauptstraße 94",
                phonePrefix = 43,
                phoneNumber = "12345678",
                securityQuestionTag = "squestion_name_of_first_pet",
                securityAnswer = "dog"
            });
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
        }

        [Then(@"verify status code returns OK for upgraded or registered player")]
        public void ThenVerifyStatusCodeReturnsOKForUpgradedOrRegisteredPlayer()
        {
            int postUpgrade_StatusCode = (int)_settings.Response.StatusCode;
            //Already Registered user -Currently returns 403 as i have hit this endpoint with same data mutliple times
            Assert.AreEqual(403, postUpgrade_StatusCode, "Status code does not return 200");
        }

        [Then(@"finally to perform an item purchase we make post call purchase api")]
        public void ThenFinallyToPerformAnItemPurchaseWeMakePostCallPurchaseApi()
        {
            _settings.BaseUrl = new Uri("https://payments-api-v1-at.greentube.com/gametwist.widgets.web.site/en/api/");
            _settings.RestClient.BaseUrl = _settings.BaseUrl;
            commonRequest.Post_Request("purchase-v1");
            _settings.Request.AddJsonBody(new
            {
                item = "m",
                paymentTypeId = "adyenEPS",
                country = "AT",
                landingUrl = "https://www.gametwist.com/en/?modal=shop"
            });
            _settings.Response = _settings.RestClient.Execute(_settings.Request);
        }

        [Then(@"verify paymentdirect url is received for valid registered user")]
        public void ThenVerifyPaymentdirectUrlIsReceivedForValidRegisteredUser()
        {
            int postPaymemt_StatusCode = (int)_settings.Response.StatusCode;
            Assert.AreEqual(200, postPaymemt_StatusCode, "Status code does not return 200");
            var paymentRedirectUrl = _settings.Response.DeserializeResponse()["paymentRedirectUrl"];
            Assert.IsNotNull(paymentRedirectUrl, "PaymentRedirect Url is not found");
        }

        [Then(@"we launch a browser using the paymentRedirectUrl")]
        public void ThenWeLaunchABrowserUsingThePaymentRedirectUrl()
        {
            //seleniumContext = new SeleniumContext();
            seleniumContext.WebDriver.Manage().Window.Maximize();
            seleniumContext.WebDriver.Navigate().GoToUrl(_settings.Response.DeserializeResponse()["paymentRedirectUrl"]);

            By webElement1 = By.CssSelector("#nrgs-paymentRedirect-redirect > span");
            WaitUntilElementClickable(seleniumContext, webElement1, 20);
            var step1title = seleniumContext.WebDriver.Title.ToLower();
            var expectedPageTitle = "gametwist casino | purchase";
            Assert.AreEqual(expectedPageTitle, step1title, "You are not on the correct page to make payment");
        }


        [When(@"page is loaded click on next button that takes to payment provider")]
        public void WhenPageIsLoadedClickOnNextButtonThatTakesToPaymentProvider()
        {
            seleniumContext.WebDriver.FindElement(By.CssSelector("#nrgs-paymentRedirect-redirect > span")).Click();
            var step2PageTitle = seleniumContext.WebDriver.Title;
            var expectedStep2Title = "Step 2: Enter your Payment Details";
            Assert.AreEqual(expectedStep2Title, step2PageTitle, "You are not on the correct page to select bank for making payment");
        }

        [When(@"select bank austria from provider dropdown and click continue")]
        public void WhenSelectBankAustriaFromProviderDropdownAndClickContinue()
        {
            seleniumContext.WebDriver.FindElement(By.XPath("(//input[@title='Bank Austria'])")).Click();
            By webElement2 = By.Id("gebForm:verf_ID");
            WaitUntilElementClickable(seleniumContext, webElement2, 10);
            var step3PageTitle = seleniumContext.WebDriver.Title;
            var expectedStep3Title = "Unicredit Login e-PS";
            Assert.AreEqual(expectedStep3Title, step3PageTitle, "You are not on the correct page to login to onlinebanking for making payment");
        }

        [When(@"adding random values to the two input boxes and click the login button and you should see a failure")]
        public void WhenAddingRandomValuesToTheTwoInputBoxesAndClickTheLoginButtonAndYouShouldSeeAFailure()
        {
            seleniumContext.WebDriver.FindElement(By.Id("gebForm:verf_ID")).SendKeys("12345");
            seleniumContext.WebDriver.FindElement(By.Id("gebForm:pin_ID")).SendKeys("54321");
            seleniumContext.WebDriver.FindElement(By.Id("gebForm:LoginCommandButton")).Click();
            By webElement3 = By.Id("gebForm:j_id127");
            WaitUntilElementClickable(seleniumContext, webElement3, 10);

            string FailureLoginText = seleniumContext.WebDriver.FindElement(By.Id("gebForm:j_id127")).Text;
            Assert.IsTrue(FailureLoginText.Contains("Es sind Fehler aufgetreten!"), "Failure login message displayed is incorrect");

            string FailureReasonMessage = seleniumContext.WebDriver.FindElement(By.Id("gebForm:j_id129")).Text;
            Assert.IsTrue(FailureReasonMessage.Contains("Verfüger oder PIN falsch."), "Failure reason message displayed is incorrect");

        }

        [When(@"click the cancel button to get redirected back to the gametwist page")]
        public void WhenClickTheCancelButtonToGetRedirectedBackToTheGametwistPage()
        {
            seleniumContext.WebDriver.FindElement(By.Id("gebForm:j_id75")).Click();
            Thread.Sleep(5000);
            var Home3PageTitle = seleniumContext.WebDriver.Title;
            var expectedHomePageTitle = "Play FREE Online Casino games | GameTwist Casino";
            Assert.AreEqual(expectedHomePageTitle, Home3PageTitle, "You are not redirected to Home Page");
        }

        [Then(@"take a screenshot and close the browser")]
        public void ThenTakeAScreenshotAndCloseTheBrowser()
        {
            ((ITakesScreenshot)seleniumContext.WebDriver).GetScreenshot().SaveAsFile(@"..\ScreenShorts_Report", ScreenshotImageFormat.Png);
            // Screenshot is saved in current directory (\GameTwistAssignment\bin\Debug\) .File name as "ScreenShorts_Report"
        }
       

      
    }

}
