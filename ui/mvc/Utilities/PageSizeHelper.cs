using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc.Utilities;

public static class PageSizeHelper
{
    /// <summary>
    /// Get the page size coming from either the Select Control or the cookie
    /// otherwise it sets the default of 5 if the user has never set preference.
    /// The first time they selext a page size on any View, that becomes default for all other views as well
    /// HOweverm if they set a preferred page size on any View after that, it is remembered for that controller
    /// </summary>
    /// <param name="httpContext">httpcontext from controller</param>
    /// <param name="pageSizeId">PageSizeId from the request</param>
    /// <param name="ControllerName">name of the controller</param>
    /// <returns></returns>
    public static int SetPageSize(HttpContext httpContext, int? pageSizeId, string ControllerName = "")
    {
        int pageSize = 0;
        if(pageSizeId.HasValue)
        {
            //Value selected from DDL so use and save it to Cookie
            pageSize = pageSizeId.GetValueOrDefault();
            CookieHelper.CookieSet(httpContext, ControllerName + "pageSizeValue", pageSize.ToString(), 480);
            //Set this value as the default if a custom page size has been set for a controller
            CookieHelper.CookieSet(httpContext, "DefaultpageSizeValue", pageSize.ToString(), 480);
        }
        else
        {
            // not selected so check cookie if there is 
            pageSize = Convert.ToInt32(httpContext.Request.Cookies[ControllerName + "pageSizeValue"]);
        }

        if(pageSize == 0)
        {
            pageSize = Convert.ToInt32(httpContext.Request.Cookies["DefaultpageSizeValue"]);
        }

        return (pageSize == 0) ? 20 : pageSize; // if not cookie and not selection, default 5
    }

    /// <summary>
    /// Select list for page size dropdown
    /// </summary>
    /// <param name="pageSize">Current Value for the selected option</param>
    /// <returns></returns>
    public static SelectList PageSizeList(int? pageSize)
    {
        return new SelectList(new[] { "5", "10", "20", "50", "100" }, pageSize.ToString() );
    }
}
