using Core.Crawling;
using OpenQA.Selenium.Chrome;

namespace Facebook.Crawling
{
    public static class DriverExtensions
    {
        public static void WaitForMain(this ChromeDriver driver, bool isViewSource)
        {
            if (isViewSource)
            {
                driver.ExecuteScript("document.body.innerHTML = document.body.innerText");
            }
        }

        public static void DumpPrefetchedRequests(this ChromeDriver driver)
        {
            driver.InjectUtils("Scripts/JsUtils.js");
            driver.InjectUtils("Scripts/Facebook/Utils.js");
            driver.ExecuteScript("return __DumpPrefetchedFacebookRequests()");
        }

        public static void ScrollToPageBottom(this ChromeDriver driver)
        {
            driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
        }

        public static void LoadMoreComments(this ChromeDriver driver)
        {
            driver.InjectUtils("Scripts/ReactUtils.js");
            driver.ExecuteScript(
                "var cf = __WalkFiberRecursive(__GetFiber(document.querySelector('[role=main]')), cf => {" +
                "  if (cf.pendingProps?.loadMoreComments) return cf" +
                "});" +
                "var li = __FindClosestFiber(cf, x => x.stateNode)?.stateNode;" +
                "if (li) {" +
                "  li.scrollIntoView();" +
                "  li.querySelector('[role=button]')?.click();" +
                "}"
            );
        }
    }
}
