using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace GameTwistAssignment
{
    [Binding]
    public class SeleniumContext
    {
      public SeleniumContext() 
        { 
          
            Console.WriteLine(System.Environment.CurrentDirectory);
            WebDriver = new ChromeDriver(System.Environment.CurrentDirectory);

        }
        public IWebDriver WebDriver { get;  set; }
        
        [AfterScenario]
        public void afterScenarioRun()
        {
            WebDriver.Close();
            WebDriver.Quit();
            
        }

    }
}
