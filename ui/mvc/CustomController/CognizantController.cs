using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mvc.CustomController;

/// <summary>
/// Makes the controller self awar. 
/// knows its own name and what action was called
/// </summary>
public class CognizantController : Controller
{
    internal string ControllerName()
    {
        return ControllerContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
    }

    internal string ActionName()
    {
        return ControllerContext.RouteData.Values["action"]?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// use regular expression to slot input string at each capital letter and add a space before each capital letter
    /// Trim() removes leading and trailing spaces
    /// </summary>
    /// <param name="input">string in camel case to split into words</param>
    /// <returns>camel case string split into words</returns>
    internal static string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex
            .Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
    }

    /// <summary>
    /// Executes before action method is called
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        SetViewDataOnActionExecuting();
        base.OnActionExecuting(context);
    }

    

    /// <summary>
    /// for async
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        SetViewDataOnActionExecuting();
        return base.OnActionExecutionAsync(context, next);
    }


    private void SetViewDataOnActionExecuting()
    {
        string ControllerFriendlyName = SplitCamelCase(ControllerName());

        ViewData["ControllerName"] = ControllerName();
        ViewData["ControllerFriendlyName"] = ControllerFriendlyName;
        ViewData["ActionName"] = ActionName();

        
        var title = (ActionName() == "Index" ? ControllerName() : ( ActionName() == "Details" ? ControllerName() + " " + ActionName() : ActionName() + " " + ControllerName() ) );
        ViewData["Title"] = title;
    }


}
