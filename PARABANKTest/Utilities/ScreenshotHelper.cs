using OpenQA.Selenium;  
using System;

namespace PARABANKTest.Utilities
{
    public static class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string testName)
        {
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

                string folder = @"C:\BDCLPM\PARABANKTest\PARABANKTest\Image";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = testName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

                string fullPath = Path.Combine(folder, fileName);

                ss.SaveAsFile(fullPath);

                return fullPath;
            }
            catch
            {
                return "Cannot capture screenshot";
            }
        }
    }
}