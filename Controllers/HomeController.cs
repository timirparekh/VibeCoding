using DocumentFormat.OpenXml.Packaging;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VibeCoding.Models;

namespace VibeCoding.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly VibeCoding.LLM.OpenAI _openAI;

    public HomeController(ILogger<HomeController> logger, VibeCoding.LLM.OpenAI openAI)
    {
        _logger = logger;
        _openAI = openAI;
    }

    public IActionResult Index(FinalReport? report = null)
    {
        return View(report);
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
            ViewBag.UploadMessage = "No file selected.";
            return View("Index", null);
        }

        FinalReport? aiSummary = null;
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

                    aiSummary = await _openAI.GetSummary(docxText);
                }
            }
            catch (Exception ex)
            {
                ViewBag.UploadMessage = $"Error reading DOCX file: {ex.Message}";
                return View("Index", null);
            }
        }
        else
        {
            ViewBag.UploadMessage = "Unsupported file type.";
            return View("Index", null);
        }

        return View("Index", aiSummary);
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
