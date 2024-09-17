using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CryptoGRAPHic.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet() { }

        public IActionResult OnGetPredictions()
        {
            // Fetch the predictions from the predictions.json file
            var data = System.IO.File.ReadAllText("predictions.json");
            return new JsonResult(JsonConvert.DeserializeObject(data));
        }
    }
}
