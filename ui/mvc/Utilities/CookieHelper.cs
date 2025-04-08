namespace mvc.Utilities;

public static class CookieHelper
{
    /// <summary>
    /// set cookie
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <param name="key">unique identifier</param>
    /// <param name="value">value to store in cookie object</param>
    /// <param name="expireTime">expiration time</param>
    public static void CookieSet(HttpContext httpContext, string key, string value, int? expireTime)
    {
        CookieOptions option= new CookieOptions();

        if (expireTime.HasValue)
        {
            option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
        }
        else
        {
            //virtually no lifespan by default
            option.Expires = DateTime.Now.AddMilliseconds(10);
        }
        httpContext.Response.Cookies.Append(key, value, option);
    }
}
