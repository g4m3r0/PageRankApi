[![Deploy API](https://github.com/g4m3r0/PageRankApi/actions/workflows/master_pagerankapi.yml/badge.svg)](https://github.com/g4m3r0/PageRankApi/actions/workflows/master_pagerankapi.yml)

[![Available on RapidAPI](https://img.shields.io/static/v1?label=RapidAPI&message=Google%20PageRank%20API&color=blue&logo=rapidapi)](https://rapidapi.com/gsoftwarelab-gsoftwarelab-default/api/google-pagerank-api)

# PageRankApi

PageRankApi is a simple ASP.NET Core 9 web‐API that fetches the Yandex SQI score for any website and maps it to an approximate “PageRank” level (1–10) based on configurable thresholds.

---

## Table of Contents

- [Features](#features)  
- [Prerequisites](#prerequisites)  
- [Getting Started](#getting-started)  
- [Configuration](#configuration)  
- [Usage](#usage)  
- [Swagger / OpenAPI](#swagger--openapi)  
- [Deployment](#deployment)  
- [Contributing](#contributing)  
- [License](#license)  

---

## Features

- Fetches raw SQI from Yandex’s Siteinfo page  
- Parses SQI via a pre‐compiled regex  
- Maps SQI → PageRank (1–10) using configurable thresholds  
- Clean, testable service via DI  
- Optional Swagger UI for interactive docs  

---

## Prerequisites

- .NET 9 SDK (https://dotnet.microsoft.com)  
- Visual Studio / Rider / VS Code  
- Azure App Service (for production deployment)  

---

## Getting Started

1. **Clone the repo**  
   ```bash
   git clone https://github.com/g4m3r0/PageRankApi.git
   cd PageRankApi
   ```
2. **Configure thresholds** (see [Configuration](#configuration))  
3. **Run locally**  
   ```bash
   dotnet run
   ```  
   By default the API listens on `https://localhost:5001`.  

---

## Configuration

Threshold mappings live in `appsettings.json` under `PageRankThresholds`. Change the SQI cutoffs without code changes:

```json
{
  "PageRankThresholds": {
    "1":    1,
    "2":   10,
    "3":   20,
    "4":   50,
    "5":  100,
    "6":  500,
    "7": 1000,
    "8": 5000,
    "9": 8000,
    "10":10000
  }
}
```

You can also override thresholds via environment variables in Azure.

---

## Usage

### Endpoint

```
GET /pagerank/{host}
```

- **host**: The target domain (e.g. `example.com`).

### Example

```bash
curl -s https://localhost:5001/pagerank/github.com | jq
```

```json
{
  "host": "github.com",
  "sqi": 124000,
  "pageRank": 10
}
```

#### Error Responses

- **400 Bad Request**  
  ```json
  { "error": "Unable to extract SQI" }
  ```
- **500 Internal Server Error**  
  Unexpected failures (network, parsing, etc.).

---

## Swagger / OpenAPI

- **Interactive UI** (Swashbuckle)  
  `https://localhost:5001/swagger`  
- **Raw JSON Spec**  
  `https://localhost:5001/swagger/v1/swagger.json`  

If you’re using the built-in `.AddOpenApi()` only, fetch the spec at:  
```
GET /openapi/v1.json
```

---

## Deployment

This repo includes a GitHub Actions workflow (`.github/workflows/deploy-to-azure.yml`) that:

1. Builds & publishes the API  
2. Deploys to your Azure App Service via your publish profile  

_Setup_:  
- Add `AZURE_WEBAPP_NAME` and `AZURE_WEBAPP_PUBLISH_PROFILE` to your GitHub Secrets.  
- Push to the `main` branch.  

Watch [Actions → Build and deploy ASP.Net Core app to Azure Web App – pagerankapi] for your deployment status.

---

## Contributing

1. Fork the repo  
2. Create a feature branch (`git checkout -b feature/your-feature`)  
3. Commit your changes & push (`git push origin feature/your-feature`)  
4. Open a Pull Request  

Please ensure all new code has unit tests and passes CI (`dotnet test`).

---

## License
MIT © 2025 g4m3r0
