# EbayScrappGoogleAI
Filtering ebay products with google image ai help

Hi, im so glad to see you here.
Current project works in bundle with EbayScrappingPhones project.
Application start TCPServer on local adress on port 80. Please use dynamic ip adress, to be able to connect from phone.

MAIN IDEA:
Sometimes its very hard to find good stuff for good price on ebay. 
1) Its necessary to enter main searching tag url (For example "https://www.ebay.de/sch/i.html?_from=R40&_trksid=p2380057.m570.l1313&_nkw=uhr&_sacat=0");
2) Enter searching options (For example "G-shock");
3) Application will scrap searching products, and IF NAME CONTAINS SEARCHING TAG (For example "G-shock") it WON'T be taken in scrapping list;
4) If product name DOESN'T contains searching options, product image will be taken and checked with google image AI;
5) If product name doesn't contains searching options, but google tells that it could be you product it will add it;

It is mega useful for searching. If someone sells your wishful gucci bag, but called it just bag, no one besides you could find it. Usually because of unpopularity it has best price, and sellers are glad to sell it to you with sale 100%. Trust me.

HOW TO USE:
1) Run application and press enter;
2) Enter options file path (File first line link, second line searching options). Example of options file:
https://www.ebay.de/sch/i.html?_from=R40&_nkw=uhr&_sacat=0&_udhi=20&_sop=15
Baby-G | Baby G | Casio | G Shock | G-Shock | 5600 | 5610 | 5630 | 5000 | 5025 | 5030
3) Enter filename where you would like to store products info (File can be empty);
4) Wait and enjoy;
5) To quit just enter quit in terminal every time you want;
6) Your founded products you can find in file (step 3), or in application EbayScrappingPhones (see on my github);


