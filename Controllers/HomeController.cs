using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VibeCoding.Models;

namespace VibeCoding.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile uploadedFile)
    {
        if (uploadedFile == null || uploadedFile.Length == 0)
        {
            TempData["UploadMessage"] = "No file selected.";
            return RedirectToAction("Index");
        }

        TempData["UploadMessage"] = "Success in uploading file";
        return RedirectToAction("Index");
    }
}
