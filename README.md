
# PdfGenerator

**PdfGenerator** is a .NET 8.0 application for generating PDF documents using the [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) library. It includes an ASP.NET Core API that receives HTML content and converts it to PDF format.

## Contents
- [Installation](#installation)
- [Running the Project](#running-the-project)
- [API Documentation](#api-documentation)
- [Testing](#testing)

## Installation

1. **Clone the repository:**
   ```bash
   git clone <url_repository>
   ```

2. **Install dependencies:**
   The project requires .NET SDK 8.0 or newer and PuppeteerSharp.
   On the first run, it will automatically download Chromium required for PDF generation.

3. **Set up secrets for development (if needed):**
   ```bash
   dotnet user-secrets init
   ```

## Running the Project

1. Navigate to the project directory:
   ```bash
   cd PdfGenerator
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. **Swagger API Documentation** will be available at:
   ```
   https://localhost:5001/swagger
   ```

## API Documentation

The application provides the following API endpoints:

- **POST /pdf/v2/generate**: Generates a PDF file from the HTML content sent in the request.

**Example request**:
```json
POST /pdf/v2/generate
Content-Type: application/json

{
  "ContentHtml": "<html><head><style>h1 { color: red; }</style></head><body><h1>Hello, PDF!</h1></body></html>",
  "PdfOptions": {
    "PrintBackground": true,
    "MarginOptions": {
      "Top": "20px",
      "Bottom": "20px",
      "Left": "10px",
      "Right": "10px"
    }
  }
}
```

The response contains the PDF file as `application/pdf`.

## Testing

1. **Navigate to the test directory:**
   ```bash
   cd PdfGenerator.Test
   ```

2. **Run tests:**
   ```bash
   dotnet test
   ```

The tests cover basic API endpoints and check if a valid PDF file is returned.

---

