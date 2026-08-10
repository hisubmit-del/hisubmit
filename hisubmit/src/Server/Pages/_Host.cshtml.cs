using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HiSubmit.Server.Pages;

public class _Host : PageModel
{
    public void OnGet()
    {
    }

    public bool CheckPrerender()
    {
        var url = HttpContext.Request.Path;

        return _authorizeRequiredPages.Any(aUrl => url.StartsWithSegments(aUrl));
    }

    private List<string> _authorizeRequiredPages = new()
    {
        "/user",
        "/project",
        "/admin",
        "/festivals",
        "/Referees",
        "/festival",
        "/chat",
        "/account",
        "/identity",
        "/login",
        "/register"
    };
}