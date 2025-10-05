using Microsoft.AspNetCore.Mvc.Filters;

namespace mvc.CustomController;

public class CustomLookupsController : CognizantController
{
    // each controller inherit from this so appropriate URL is available
    // This requires controller named Lookup.
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        SetViewData();
        base.OnActionExecuting(context);
    }
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        SetViewData();
        return base.OnActionExecutionAsync(context, next);
    }
    private void SetViewData()
    {
        ViewData["ControllerAndQuery"] = "Lookup/?Tab=" + ControllerName() + "-Tab";
        ViewData["actionWithQuery"] = "?/Tab=" + ControllerName() + "-Tab";
        ViewData["QueryStringValueOrNavTabName"] = ControllerName() + "-Tab";
    }
}
