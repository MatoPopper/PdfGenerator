using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews()
                        .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); ;

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PdfGenerator", Version = "v1" });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            c.CustomSchemaIds(type => type.FullName);
            c.DescribeAllParametersInCamelCase();
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddSingleton<IPuppeteerSharpService, PuppeteerSharpService>();

        var app = builder.Build();

        //    app.UseHttpsRedirection();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UsePathBase("/");
        app.UseRouting();

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PdfGenerator v1"));

        app.Run();
    }
}

/// <summary>
/// PDF operations
/// </summary>
[ApiController]
[Route("pdf")]
public class PdfController(IPuppeteerSharpService puppeteerSharpService) : ControllerBase
{
    private readonly IPuppeteerSharpService _puppeteerSharpService = puppeteerSharpService;

    [HttpPost]
    [Route("v2/generate")]
    public async Task<IActionResult> GenerateV2Pdf([FromBody] PuppeteerSharpRequest pdfRequest)
    {
        var file = await _puppeteerSharpService.CreatePdf(pdfRequest);
        return File(file, "application/pdf", "file.pdf");
    }
}

/// <summary>
/// PuppeteerSharp lib interface
/// </summary>
public interface IPuppeteerSharpService
{
    Task<byte[]> CreatePdf(PuppeteerSharpRequest pdfRequest);
}

/// <summary>
/// PDF Generator service
/// </summary>
public class PuppeteerSharpService : IPuppeteerSharpService
{
    private static readonly string? _executablePath;

    
    static PuppeteerSharpService()
    {
        // Nastav ExecutablePath iba na Linuxe, kde sa Chromium inštaluje do kontajnera
        if (OperatingSystem.IsLinux())
        {
            _executablePath = "/usr/bin/chromium"; // alebo /usr/bin/chromium podľa tvojej distribúcie
        }
    }
    public async Task<byte[]> CreatePdf(PuppeteerSharpRequest pdfRequest)
    {
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox" },
            ExecutablePath = _executablePath // bude null na Windows – Puppeteer si Chromium stiahne sám
        };

        await using var browser = await Puppeteer.LaunchAsync(launchOptions);
        await using var page = await browser.NewPageAsync();

        await page.SetContentAsync(pdfRequest.ContentHtml);

        var options = pdfRequest.PdfOptions ?? new PdfOptions
        {
            Landscape = false,
            DisplayHeaderFooter = false,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20px",
                Bottom = "20px",
                Left = "10px",
                Right = "10px"
            }
        };

        return await page.PdfDataAsync(options);
    }
}


/// <summary>
/// Request model
/// </summary>
public class PuppeteerSharpRequest
{
    public string ContentHtml { get; set; }
    public PdfOptions PdfOptions { get; set; }
}
