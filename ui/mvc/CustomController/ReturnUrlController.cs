using Microsoft.AspNetCore.Mvc.Filters;
using mvc.Utilities;

namespace mvc.CustomController;

/// <summary>
/// could have be part of Cognizant Controller but professor having fun with inheretance
/// </summary>
public class ReturnUrlController : CognizantController
{

    //list of action that will add the returnUrl to ViewData
    internal string[] ActionWithUrl = ["Details", "Create", "Edit", "Delete", "Add", "Update", "Remove"];


    public override void OnActionExecuting(ActionExecutingContext context)
    {
        MaintainReturnUrl();

        base.OnActionExecuting(context);
    }



    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        MaintainReturnUrl();
        return base.OnActionExecutionAsync(context, next);
    }

    private void MaintainReturnUrl()
    {
        if (ActionWithUrl.Contains(ActionName()))
        {
            ViewData["returnUrl"] = MaintainUrl.ReturnUrl(HttpContext, ControllerName());
        }
        else if (ActionName() == "Index")
        {
            //clear the sort / filter /paging url cookie for controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "Url", "", -1);
        }
    }
}
