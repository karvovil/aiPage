using Microsoft.AspNetCore.Mvc.RazorPages;
namespace YourNamespace.Pages
{
  public class DatasetModel : PageModel
  {
      public string Link { get; set; }

      public void OnGet(string link)
      {
          Link = link;
      }
  }
}