using Microsoft.AspNetCore.Mvc.Testing;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using System.Net.Http.Json;
using FluentAssertions;

namespace PdfGenerator.Test
{
    public class PdfGenerationTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GeneratePdf_ShouldReturnPdfFile()
        {
            // Arrange
            var pdfRequest = new PuppeteerSharpRequest
            {
                ContentHtml = "<html><head><style>h1 { color: red; }</style></head><body><h1>Hello, PDF!</h1></body></html>",
                PdfOptions = new PdfOptions
                {
                    PrintBackground = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "20px",
                        Bottom = "20px",
                        Left = "10px",
                        Right = "10px"
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/pdf/v2/generate", pdfRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/pdf");

            var pdfData = await response.Content.ReadAsByteArrayAsync();
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0); // Ensure PDF has content
        }
    }
}