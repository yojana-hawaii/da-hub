namespace mvc.Utilities;

public static class MaintainUrl
{
    /// <summary>
    /// Maintain the URL for an index View including filter, sort and paging information
    /// uses CookieHelper Utility
    /// working with /Controller when Index.cshtml is not visible
    /// if anything other than index, need to update searchText with action > call ActionName()
    /// </summary>
    /// <param name="httpContext">httpcontext from controller</param>
    /// <param name="ControllerName">controller anem from CognizantController</param>
    /// <returns>the Index URL with parameters if required</returns>
    public static string ReturnUrl(HttpContext httpContext, string ControllerName)
    {
        string cookieName = ControllerName + "URL";
        string searchText = "/" + ControllerName + "?";

        //get url of the page that sent us here
        string returnUrl = httpContext.Request.Headers["Referer"].ToString();

        if (returnUrl.Contains(searchText))
        {
            //came here from the index with some paramters, save parameter in a cookie
            returnUrl = returnUrl[returnUrl.LastIndexOf(searchText)..]; // everything after searchText goes to returnUrl
            CookieHelper.CookieSet(httpContext, cookieName, returnUrl, 30);
            return returnUrl;
        }
        else
        {
            //get from cookie 
            //if cookie does not have it return to Index page without parameters
            returnUrl = httpContext.Request.Cookies[cookieName] ?? "/" + ControllerName;
            return returnUrl;
        }
    }
}
