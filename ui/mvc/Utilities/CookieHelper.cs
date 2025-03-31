namespace mvc.Utilities;

public static class CookieHelper
{
    /// <summary>
    /// set cookie
    /// </summary>
    /// <param name="_context">HttpContext</param>
    /// <param name="key">unique identifier</param>
    /// <param name="value">value to store in cookie object</param>
    /// <param name="expireTime">expiration time</param>
    public static void CookieSet(HttpContext _context, string key, string value, int? expireTime)
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
        _context.Response.Cookies.Append(key, value, option);
    }
}
