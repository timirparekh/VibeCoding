using DocumentFormat.OpenXml.Packaging;
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

        // Only process DOCX files
        if (uploadedFile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || uploadedFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(ms);
                    ms.Position = 0;
                    // Read DOCX contents
                    string docxText = ReadDocxContents(ms);
                    TempData["DocxContent"] = docxText;
                }
            }
            catch (Exception ex)
            {
                TempData["DocxContent"] = $"Error reading DOCX file: {ex.Message}";
            }
        }
        else
        {
            TempData["DocxContent"] = null;
        }

        return RedirectToAction("Index");
    }
    // Helper method to read DOCX contents
    private string ReadDocxContents(Stream docxStream)
    {
        try
        {
            using (var wordDoc = WordprocessingDocument.Open(docxStream, false))
            {
                if (wordDoc.MainDocumentPart == null || wordDoc.MainDocumentPart.Document == null || wordDoc.MainDocumentPart.Document.Body == null)
                {
                    return "DOCX file is empty or invalid.";
                }
                var body = wordDoc.MainDocumentPart.Document.Body;
                return body.InnerText;
            }
        }
        catch
        {
            return "Unable to read DOCX contents.";
        }
    }
    }
