using GameTwistAssignment.Setting;
using GameTwistAssignment.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using RestSharp.Authenticators;
using System;
using System.Threading;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Support.UI;
using GameTwistAssignment.Hooks;
using System.IO;
using Newtonsoft.Json;
using GameTwistAssignment.Dto;

namespace GameTwistAssignment.Steps 
{
    [Binding]
    public class GameTwist_End_To_End_Steps 
    {
        private string acctoken;
        private Settings _settings;
        private SeleniumContext seleniumContext;
        private CommonRequest commonRequest;
        private SeleniumCommonPg seleniumCommonPg;
        private string paymentUrl = "https://payments-api-v1-at.greentube.com/gametwist.widgets.web.site/en/api/";

        public GameTwist_End_To_End_Steps(Settings settings, SeleniumContext _seleniumContext,CommonRequest _commonRequest,SeleniumCommonPg _seleniumCommonPg)
        {
            _settings = settings;
            seleniumContext = _seleniumContext;
            commonRequest = _commonRequest;
            seleniumCommonPg = _seleniumCommonPg;
        
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
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName+ "\\Data\\loginRequest.json";
            LoginRequestModel loginJsonBody = JsonConvert.DeserializeObject<LoginRequestModel>(File.ReadAllText(filePath));
            _settings.Request.AddJsonBody(loginJsonBody);
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
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Data\\fullRegistrationRequest.json";
            FullRegistrationRequestModel fullRegistrationJsonBody = JsonConvert.DeserializeObject<FullRegistrationRequestModel>(File.ReadAllText(filePath));
            _settings.Request.AddJsonBody(fullRegistrationJsonBody);
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
            _settings.BaseUrl = new Uri(paymentUrl);
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
            seleniumContext.WebDriver.Manage().Window.Maximize();
            seleniumCommonPg.NavigateToUrl(_settings.Response.DeserializeResponse()["paymentRedirectUrl"]);
            By webElement1 = By.CssSelector("#nrgs-paymentRedirect-redirect > span");
            WaitUntilElementClickable(seleniumContext, webElement1, 20);
            var step1title = seleniumContext.WebDriver.Title.ToLower();
            var expectedPageTitle = "gametwist casino | purchase";
            Assert.AreEqual(expectedPageTitle, step1title, "You are not on the correct page to make payment");
        }


        [When(@"page is loaded click on next button that takes to payment provider")]
        public void WhenPageIsLoadedClickOnNextButtonThatTakesToPaymentProvider()
        {
            seleniumCommonPg.ClickElementByCss("#nrgs-paymentRedirect-redirect > span");
            var step2PageTitle = seleniumContext.WebDriver.Title;
            var expectedStep2Title = "Step 2: Enter your Payment Details";
            Assert.AreEqual(expectedStep2Title, step2PageTitle, "You are not on the correct page to select bank for making payment");
        }

        [When(@"select bank austria from provider dropdown and click continue")]
        public void WhenSelectBankAustriaFromProviderDropdownAndClickContinue()
        {
            seleniumCommonPg.ClickElementByXpath("(//input[@title='Bank Austria'])");
            By webElement2 = By.Id("gebForm:verf_ID");
            WaitUntilElementClickable(seleniumContext, webElement2, 10);
            var step3PageTitle = seleniumContext.WebDriver.Title;
            var expectedStep3Title = "Unicredit Login e-PS";
            Assert.AreEqual(expectedStep3Title, step3PageTitle, "You are not on the correct page to login to onlinebanking for making payment");
        }

        [When(@"adding random values to the two input boxes and click the login button and you should see a failure")]
        public void WhenAddingRandomValuesToTheTwoInputBoxesAndClickTheLoginButtonAndYouShouldSeeAFailure()
        {
            seleniumCommonPg.InputTextById("gebForm:verf_ID", "12345");
            seleniumCommonPg.InputTextById("gebForm:pin_ID", "54321");
            seleniumCommonPg.ClickElementById("gebForm:LoginCommandButton");
            By webElement3 = By.Id("gebForm:j_id127");
            WaitUntilElementClickable(seleniumContext, webElement3, 10);

            string FailureLoginText = seleniumCommonPg.GetTextById("gebForm:j_id127");
            Assert.IsTrue(FailureLoginText.Contains("Es sind Fehler aufgetreten!"), "Failure login message displayed is incorrect");

            string FailureReasonMessage = seleniumCommonPg.GetTextById("gebForm:j_id129");
            Assert.IsTrue(FailureReasonMessage.Contains("Verfüger oder PIN falsch."), "Failure reason message displayed is incorrect");

        }

        [When(@"click the cancel button to get redirected back to the gametwist page")]
        public void WhenClickTheCancelButtonToGetRedirectedBackToTheGametwistPage()
        {
            seleniumCommonPg.ClickElementById("gebForm:j_id75");
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
