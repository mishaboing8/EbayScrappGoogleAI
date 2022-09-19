using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using EbaySellerStuffSearch;
using OpenQA.Selenium.Interactions;
using System.Threading;

internal class Program
{

    public static string WRITE_FILE_PATH;

    private static async Task Main(string[] args)
    {
        string optionsPath = "";
        string[] lines;

        Console.WriteLine("Before starting with programm you can always type \'quit\' to quit\nEnter something else to continue");

        if (Console.ReadLine().ToUpper() == "QUIT") return;

        while (true)
        {
            Console.WriteLine("Please write file path, where we could take your web request and searching tags:");
            optionsPath = Console.ReadLine();

            if (optionsPath.ToUpper() == "QUIT") return;

            if (!File.Exists(optionsPath))
            {
                Console.WriteLine("Sorry we cant find file with path:\n" + optionsPath);
                continue;
            }

            try
            {
                lines = File.ReadAllLines(optionsPath);
                Console.WriteLine("Your webrequest is:\n" + lines[0] + "\nAnd searching tags are:\n" + lines[1]);

                Console.WriteLine("Are they correct? Enter y to continue");
                if (Console.ReadLine().ToUpper() == "Y") break;

            }
            catch (Exception)
            {
                Console.WriteLine("Sorry there are problems with your text file\nPlease be sure it has 2 lines\nFirst must be web url\nSecond must have options separated with \' | \'");
                continue;
            }
        }

        while (true)
        {
            Console.WriteLine("Please write file path, where we could save our products:");
            WRITE_FILE_PATH = Console.ReadLine();

            if (WRITE_FILE_PATH.ToUpper() == "QUIT") return;

            if (File.Exists(WRITE_FILE_PATH))
            {
                Console.WriteLine("We found file:\n" + WRITE_FILE_PATH + "\nYour file will be rewritten. Enter y to continue");

                if (Console.ReadLine().ToUpper() == "Y") break;

                continue;
            }

            Console.WriteLine("Sorry we cant find file with path:\n" + WRITE_FILE_PATH);
        }

        string[] options = lines[1].Trim().Split(" | ").Where(s => s != "").ToArray();
        List<string[]> scrappOptions = new List<string[]>();

        Task.Run(() => ToStopApp());
        Task.Run(() => MainServer.RunServer());

        string link = lines[0];
        var products = await EbayProductFactory.GetEbayProducts(scrappOptions, link, 2, options, false);
    }

    static async Task ToStopApp()
    {
        bool stoppedServer = false;
        while (true)
        {
            string input = Console.ReadLine();
            if (input.ToUpper().Contains("STOP SCRAP"))
            {
                EbayProductFactory.stopApp = true;
            }

            if (input.ToUpper().Contains("STOP SERVER") && !stoppedServer)
            {
                if (MainServer.StopServer()) stoppedServer = true;
            }

            if (input.ToUpper().Contains("QUIT"))
            {
                EbayProductFactory.stopApp = true;

                if (!stoppedServer)
                {
                    if (MainServer.StopServer())
                    {
                        break;
                    }
                }
                else break;
            }
        }
    }
}