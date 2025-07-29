# ScrapeWise - Deployment Guide for Teacher

## ✅ Build Status: SUCCESSFUL
**Date:** July 28, 2025  
**Build Result:** Success with 14 warnings (no errors)  
**Target Framework:** .NET 9.0

## Prerequisites for Teacher's PC

### 1. Required Software
- **.NET 9.0 SDK** (Required)
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verify installation: `dotnet --version` (should show 9.0.x)

### 2. Optional but Recommended
- **Visual Studio 2022** (Community Edition is free)
  - Or **Visual Studio Code** with C# extension
- **Git** for cloning the repository

## Quick Start Instructions

### Option 1: Direct Run (Recommended)
```bash
# 1. Navigate to project folder
cd ScrapeWise-Intelligent-Web-Scraping-Dashboard-ASP.NET-Core-MVC-

# 2. Restore packages
dotnet restore

# 3. Build the project
dotnet build

# 4. Run the application
dotnet run
```

### Option 2: Using Visual Studio
1. Open `ScrapeWise.sln` in Visual Studio
2. Press `F5` or click "Start Debugging"

## Application Access
- **URL:** https://localhost:7074 or http://localhost:5074
- **Swagger API:** https://localhost:7074/swagger

## Database Configuration
- **Database:** PostgreSQL (cloud-hosted)
- **Connection:** Pre-configured and ready to use
- **No local database setup required**

## Key Features to Demonstrate

### 1. Authentication System
- **Registration:** Create new account
- **Login/Logout:** Microsoft Identity integration
- **Location:** Areas/Identity/Pages/

### 2. Web Scraping Dashboard
- **Create Jobs:** /Scraper/Create
- **View Jobs:** /Scraper/Dashboard
- **Real-time Updates:** SignalR integration

### 3. API Endpoints (with Swagger)
- **Jobs API:** `/api/jobs` - Demonstrates 1-N relationships
- **Tags API:** `/api/tags` - Demonstrates M-N relationships
- **Authentication:** All APIs require login

### 4. SignalR Testing
- **Test Page:** /Home/SignalRTest
- **Real-time:** Live scraping progress updates

## Project Architecture

### Controllers (Single Responsibility)
- `JobsController` → ScrapingJob entities only
- `TagsController` → Tag entities only
- `ResultsController` → ScrapingResult entities only
- `AdminController` → User management only
- `ProfileController` → User profiles only

### API Controllers
- `JobsApiController` → REST API for jobs (1-N relationships)
- `TagsApiController` → REST API for tags (M-N relationships)

### Models
- `MyUser` → Inherits from IdentityUser (renamed from User)
- `ScrapingJob` → Main scraping entity
- `ScrapingResult` → Results of scraping jobs
- `Tag` → Tags for categorization

### Relationships
- **1-N:** MyUser → ScrapingJob → ScrapingResult
- **M-N:** ScrapingJob ↔ Tag (via junction table)

## Documentation
- **XML Documentation:** Complete coverage of all public APIs
- **Algorithm Comments:** Detailed step-by-step explanations
- **Swagger Integration:** Interactive API documentation

## Troubleshooting

### If Build Fails
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### If Database Connection Fails
- The database is cloud-hosted and should work automatically
- Check internet connection
- Connection string is pre-configured in appsettings.json

### If Port is Already in Use
- The application will automatically find an available port
- Check the console output for the actual URL

## Test Scenarios for Presentation

### 1. User Registration/Login
1. Navigate to /Identity/Account/Register
2. Create a new account
3. Login with the account

### 2. Create Scraping Job
1. Go to /Scraper/Create
2. Enter a URL (e.g., https://example.com)
3. Add tags
4. Submit and watch real-time progress

### 3. API Testing
1. Navigate to /swagger
2. Login first (click "Authorize")
3. Test GET /api/jobs (shows 1-N relationships)
4. Test GET /api/tags (shows M-N relationships)

### 4. SignalR Testing
1. Go to /Home/SignalRTest
2. Click "Connect to SignalR"
3. Verify connection status
4. Check console for detailed logs

## File Structure Overview
```
ScrapeWise/
├── Controllers/           # MVC Controllers (single responsibility)
├── Controllers/Api/       # API Controllers (REST endpoints)
├── Models/               # Domain models (MyUser, ScrapingJob, etc.)
├── Views/                # Razor views
├── Areas/Identity/       # Microsoft Identity pages
├── Hubs/                 # SignalR hub
├── Data/                 # Database context and seed data
├── wwwroot/             # Static files (CSS, JS)
└── appsettings.json     # Configuration
```

## Final Notes
- ✅ All 10 teacher requirements have been implemented
- ✅ Build is successful with no errors
- ✅ Database is cloud-hosted (no local setup needed)
- ✅ Comprehensive documentation included
- ✅ Professional architecture following .NET Core best practices

**The project is ready for demonstration and evaluation.**
