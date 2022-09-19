using OpenQA.Selenium;
using System.Text.RegularExpressions;

public class EbayProduct : IComparable<EbayProduct>
{
    public float price { private set; get; }
    public string name { private set; get; }
    public string url { private set; get; }
    public float deliveryCosts { private set; get; }
    public string imgUrl { private set; get; }
    public bool sofortKaufen { private set; get; }
    public bool auktionKaufen { private set; get; }

    public EbayProduct(IWebElement element)
    {
        try
        {
            Regex priceReg = new Regex("EUR [0-9]+[,][0-9]{2}");
            string priceString = element.Text.Split("\n").Where(l => priceReg.IsMatch(l)).First();

            price = float.Parse(priceString.Replace(",", ".").Replace("EUR ", ""));
        }
        catch (Exception)
        {
            price = 0;
        }

        sofortKaufen = element.Text.Contains("Sofort-Kaufen") || element.Text.Contains("Preisvorschlag");
        auktionKaufen = element.Text.Contains("Gebot");

        var itemLink = element.FindElement(By.ClassName("s-item__link"));
        name = itemLink.FindElement(By.ClassName("s-item__title")).Text;
        url = itemLink.GetAttribute("href");

        deliveryCosts = 0;

        imgUrl = element.FindElement(By.ClassName("s-item__image-img")).GetAttribute("src");
    }

    public void ChangeDeliveryPrice(WebDriver driver)
    {
        driver.Navigate().GoToUrl(url);

        try
        {
            string stringPrice = driver
                .FindElement(By.XPath("//div[@class='vim d-shipping-minview']"))
                .FindElements(By.TagName("span"))
                .Where(el => el.GetAttribute("class")
                .Equals("ux-textspans ux-textspans--BOLD"))
                .First().Text;

            deliveryCosts = float.Parse(stringPrice.Replace(",", ".").Replace("EUR ", ""));
        }
        catch (Exception) { return; }

    }

    public override string ToString()
    {
        return "Name: " + name + "\nPrice: " + price + " | Delivery: " + (deliveryCosts == 0 ? "Free shiping" : deliveryCosts.ToString()) + "\nLink:\n" + url;
    }

    public string ToFileString()
    {
        return $"{name}\n{price}\n{deliveryCosts}\n{url}\n{imgUrl}\n{sofortKaufen}\n{auktionKaufen}";
    }

    public int CompareTo(EbayProduct? other)
    {
        return url.CompareTo(other.url);
    }
}