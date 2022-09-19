using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EbaySellerStuffSearch
{
    public class GoogleCheckBrowserHolder
    {
        static List<WebDriver> drivers = new List<WebDriver>();
        static bool AllOn = false;

        private GoogleCheckBrowserHolder()
        {
        }

        public async static Task AddBrowsers(int count)
        {
            if (drivers.Count() > 0 || AllOn) return;

            AllOn = true;

            async Task<WebDriver> AddDriver()
            {
                var driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://images.google.com/");
                driver.FindElement(By.CssSelector("button[id='W0wltc'] div[role='none']")).Click();

                driver.Navigate().GoToUrl("https://images.google.com/");

                return driver;
            }

            List<Task<WebDriver>> tasks = new List<Task<WebDriver>>();

            for (int i = 0; i < count; i++)
            {
                tasks.Add(Task.Run(() => AddDriver()));
            }

            var res = await Task.WhenAll(tasks);

            foreach (var d in res) drivers.Add(d);


        }


        public static void AllQuit()
        {
            if (drivers.Count() <= 0 || !AllOn) return;

            AllOn = false;

            foreach(var d in drivers) d.Quit();
        }

        public static async Task<HashSet<EbayProduct>> checkGoogleOnTags(IEnumerable<EbayProduct> inputProducts, string[] options, bool nameContainsOption)
        {
            List<List<EbayProduct>> lists = new List<List<EbayProduct>>();
            var productList = inputProducts.ToList<EbayProduct>();

            var CountForOne = productList.Count() / drivers.Count();

            List<Task<HashSet<EbayProduct>>> tasks = new List<Task<HashSet<EbayProduct>>>();

            if (productList.Count() < drivers.Count())
            {
                for (int i = 0; i < productList.Count(); i++)
                {
                    tasks.Add(Task.Run(() => checkingName(i, productList, i, i+1, options, nameContainsOption)));

                    await Task.Delay(200);
                }
            }
            else
            {
                for (int i = 0; i < drivers.Count(); i++)
                {
                    tasks.Add(Task.Run(() => checkingName(i, productList, CountForOne * i, (i == drivers.Count() - 1 ? productList.Count() : CountForOne * (i + 1)), options, nameContainsOption)));

                    await Task.Delay(200);
                }
            }

            HashSet<EbayProduct> returnSet = new HashSet<EbayProduct>();

            foreach(var set in (await Task.WhenAll(tasks)))
            {
                returnSet.UnionWith(set);
            }

            return returnSet;
            
        }


        private static async Task<HashSet<EbayProduct>> checkingName(int driverPos, List<EbayProduct> products, int startPos, int endPos, string[] options, bool nameContainsOption)
        {
            var localDriver = drivers[driverPos];

            HashSet<EbayProduct> result = new HashSet<EbayProduct>();

            for(int i = startPos; i < endPos; i++)
            {
                var prod = products[i];

                bool containsName = ContainsOneOf(prod.name, options);

                if (!nameContainsOption && containsName)
                {
                    Console.WriteLine("Skiped product(Has option in name): " + prod.name);

                    continue;
                } else if(nameContainsOption && containsName)
                {
                    Console.WriteLine("Added and skipped product(Has option in name): " + prod.name);
                    result.Add(prod);

                    continue;
                }

                try
                {
                    localDriver.Navigate().GoToUrl("https://images.google.com/");

                    localDriver.FindElement(By.XPath("//img[@class='Gdd5U']")).Click();

                    localDriver.FindElement(By.XPath("//input[@class='cB9M7']")).SendKeys(prod.imgUrl);

                    localDriver.FindElement(By.XPath("//div[@class='Qwbd3']")).Click();

                    string text = "";
                    int lineCount = 0;
                    foreach (var s in localDriver.FindElement(By.XPath("//div[@class='swmPnf']")).Text.Split("\n"))
                    {
                        text += "\n" + s;

                        lineCount++;

                        if (lineCount > 10) break;
                    }

                    if (ContainsOneOf(text, options))
                    {
                        result.Add(prod);
                        Console.WriteLine("Checking...YES: " + prod.name);
                    }
                    else
                    {
                        Console.WriteLine("Checking...NO: " + prod.name);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception by Google Checking");
                }


            }
            return result;
        }

        public static bool ContainsOneOf(string text, string[] options)
        {
            text = text.ToUpper();
            foreach (var option in options)
                if (text.Contains(option))
                {
                    return true;
                }

            return false;
        }
    }
}

