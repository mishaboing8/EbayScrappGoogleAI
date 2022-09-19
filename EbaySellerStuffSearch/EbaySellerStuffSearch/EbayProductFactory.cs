using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace EbaySellerStuffSearch
{
    public class EbayProductFactory
    {
        private EbayProductFactory() { }

        public static bool stopApp = false;


        public static async Task<HashSet<EbayProduct>> GetEbayProducts(List<string[]> searchOptions, string url, int GoogleCheckingBrowsers, string[] googleOptions, bool nameContainsOption)
        {
            HashSet<EbayProduct> products = new HashSet<EbayProduct>();

            await GoogleCheckBrowserHolder.AddBrowsers(GoogleCheckingBrowsers);

            ChromeOptions chrome_options = new ChromeOptions();
            chrome_options.AddArgument("--disable-extensions");

            WebDriver driver = new ChromeDriver(chrome_options);
            driver.Navigate().GoToUrl(url);

            if (!driver.Url.Contains("&LH_BIN"))
            {
                driver.Navigate().GoToUrl(driver.Url + "&LH_BO=1");
            }

            Console.Clear();

            Actions actions = new Actions(driver);
            actions.Click(driver.FindElements(By.XPath("//span[@class='srp-format-tabs-h2']")).Where(e => e.Text.Equals("Alle")).First());
            actions.Perform();

            bool writeFirstTime = true;

            while (true)
            {
                bool CheckOnOptions(string name)
                {
                    var con = GoogleCheckBrowserHolder.ContainsOneOf(name, googleOptions);
                    return nameContainsOption || !con || (GoogleCheckBrowserHolder.ContainsOneOf(name, googleOptions) == nameContainsOption);
                }

                Console.WriteLine("Starting scrapping products from Ebay");

                var scrappedProducts = (await listToProducts(driver, driver.FindElement(By.XPath("//ul[@class='srp-results srp-list clearfix']")), searchOptions))
                    .Where(p => CheckOnOptions(p.name)).ToList();

                Console.WriteLine("\nBy Google will be checked " + scrappedProducts.Count() + " products.");
                var checkedProducts = await GoogleCheckBrowserHolder.checkGoogleOnTags(scrappedProducts, googleOptions, nameContainsOption);

                products.UnionWith(checkedProducts);

                string output = "";
                checkedProducts
                    .ToList()
                    .ForEach(p => output += p.ToFileString() + "\n-----------------------\n");

                if (writeFirstTime)
                {
                    File.WriteAllText(Program.WRITE_FILE_PATH, output);
                    writeFirstTime = false;
                } else
                {
                    File.AppendAllText(Program.WRITE_FILE_PATH, output);
                }

                Console.Clear();

                Console.WriteLine("Products found after filtering: " + products.Count());

                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,document.body.scrollHeight)");

                if (driver.Url.Contains("_pgn=40"))
                {
                    var price = driver.FindElement(By.XPath("//div[@class='srp-river-results clearfix']")).FindElements(By.ClassName("s-item__price")).Last().Text;
                    price = price.Replace("EUR ", "");

                    driver.FindElement(By.XPath("//input[@id='s0-51-12-0-1-2-6-0-6[4]-0-textrange-beginParamValue-textbox']")).SendKeys(price);

                    driver.FindElement(By.XPath("//button[@aria-label='Preisspanne senden']")).Click();
                }
                var elements = driver.FindElements(By.XPath("//a[@aria-label='Zur nächsten Seite']"));

                if (stopApp) break;

                if (elements.Count() == 0) break;

                try
                {
                    if (elements.First().GetAttribute("aria-disabled").Equals("true")) break;
                }
                catch (Exception) { }

                actions.Click(elements.First());

                actions.Perform();
            }

            Console.WriteLine("Ended scrapping products!");

            GoogleCheckBrowserHolder.AllQuit();
            driver.Quit();
            return products;
        }

        private static async Task<List<EbayProduct>> listToProducts(IWebDriver driver, IWebElement elementList, List<string[]> options)
        {
            EbayProduct sayNumber(IWebElement el)
            {
                string index = el.GetAttribute("data-view");
                index = index.Substring(index.IndexOf("iid:") + 4);
                Console.Write("\rOn this page founded " + index + " products.");

                return new EbayProduct(el);
            }

            List<Task<EbayProduct>> tasks = new List<Task<EbayProduct>>();

            foreach(var el in elementList.FindElements(By.TagName("li")))
            {
                if(el.GetAttribute("class").Equals("s-item s-item__pl-on-bottom s-item--watch-at-corner"))
                {
                    tasks.Add(Task.Run(() => new EbayProduct(el)));
                }
            }

            var result = await Task.WhenAll(tasks);
            if (options.Count() == 0) return result.ToList();

            return result.Where(prod => containsOneOf(prod.name, options)).ToList();
        }

        static private bool containsOneOf(string name, List<string[]> options)
        {
            if (options.Count() == 0) return true;

            foreach (string[] option in options)
            {
                bool output = false;

                foreach (string s in option)
                    if (name.Contains(s))
                    {
                        output = true;
                        break;
                    }

                if (!output) return false;
            }

            return true;
        }

    }
}

