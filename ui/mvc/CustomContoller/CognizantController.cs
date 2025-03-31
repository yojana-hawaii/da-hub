using Microsoft.AspNetCore.Mvc;

namespace mvc.CustomContoller;

/// <summary>
/// Makes the controller self awar. 
/// knows its own name and what action was called
/// </summary>
public class CognizantController : Controller
{
    internal string ContollerName()
    {
        return ControllerContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
    }

    internal string ActionName()
    {
        return ControllerContext.RouteData.Values["action"]?.ToString() ?? string.Empty;
    }
}
