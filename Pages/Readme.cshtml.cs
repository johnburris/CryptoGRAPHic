using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CryptoGRAPHic.Pages;

public class ReadmeModel : PageModel
{
    private readonly ILogger<ReadmeModel> _logger;

    public ReadmeModel(ILogger<ReadmeModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

