# 📄 Final Project Specification Document
## Web Development
### Bachelor's in Computer Engineering
### 2nd Year, 2nd Semester

---

## 🏷️ Application Name:
**ScrapeWise - Intelligent Web Scraping Dashboard**

## 👨‍💻 Author(s):
**Name:** [Your Name]  
**Student Number:** [Your Student Number]

---

## 📝 Brief Description of the Application:
ScrapeWise is an intelligent web scraping dashboard built with ASP.NET Core MVC that allows users to create, manage, and monitor web scraping jobs. The application features a modern Aurora Glass UI theme with real-time updates, comprehensive API endpoints, and robust database management. Users can extract data from websites using CSS selectors, view scraping results, manage their profiles, and organize jobs with tags.

---

## 👥 Identification and Description of User Types:

| User Type | Description | Access Level |
|-----------|-------------|--------------|
| **Admin** | Full system access, can manage all users and jobs | Complete access |
| **User** | Standard user, can create and manage their own jobs | Limited to own data |
| **Guest** | Read-only access to public features | View-only access |

---

## ⚙️ Features / Functionalities:

### Core Features:
- ✅ **Web Scraping Jobs Management**: Create, edit, delete, and monitor scraping jobs
- ✅ **Real-time Updates**: SignalR integration for live job status updates
- ✅ **Modern UI**: Aurora Glass theme with glassmorphism effects
- ✅ **User Authentication**: Role-based access control
- ✅ **Profile Management**: Customizable user profiles with avatars
- ✅ **Tag System**: Categorize jobs with colored tags (M-N relationship)
- ✅ **API Endpoints**: RESTful API for external integrations
- ✅ **Database Persistence**: SQL Server with Entity Framework Core
- ✅ **Responsive Design**: Mobile-friendly interface
- ✅ **Help System**: Visual CSS selector guide for non-developers

### Technical Features:
- ✅ **1-N Relationships**: User → Jobs, Job → Results
- ✅ **M-N Relationships**: Jobs ↔ Tags
- ✅ **SignalR Hub**: Real-time communication
- ✅ **API Controller**: RESTful endpoints
- ✅ **Docker Support**: Containerized deployment
- ✅ **CI/CD Pipeline**: GitHub Actions automation

---

## 🖼️ Interface Mockups (minimum 3):

| Function | Description / Screenshot |
|----------|-------------------------|
| **Dashboard** | Main dashboard showing all scraping jobs with status, tags, and quick actions |
| **New Job Creation** | Form to create new scraping jobs with URL, CSS selector, and help guide |
| **Job Details** | Detailed view of scraping results with extracted data and metadata |
| **User Profile** | Profile management page with avatar, settings, and preferences |
| **API Documentation** | Interactive API endpoints for external integrations |

---

## 🧩 DERE Diagram:
```
┌─────────────┐    1:N    ┌──────────────┐    1:N    ┌──────────────┐
│    User     │──────────▶│ ScrapingJob  │──────────▶│ScrapingResult│
│             │           │              │           │              │
│ - UserId    │           │ - JobId      │           │ - ResultId   │
│ - Username  │           │ - TargetUrl  │           │ - Extracted  │
│ - Email     │           │ - CssSelector│           │ - ScrapedAt  │
│ - Role      │           │ - CreatedAt  │           │              │
│ - IsActive  │           │ - UserId     │           │              │
└─────────────┘           └──────────────┘           └──────────────┘
       │                           │
       │ 1:1                       │ M:N
       │                           │
       ▼                           ▼
┌─────────────┐           ┌──────────────┐
│   Profile   │           │     Tag      │
│             │           │              │
│ - ProfileId │           │ - TagId      │
│ - DisplayName│          │ - Name       │
│ - AvatarUrl │           │ - Color      │
│ - UserAgent │           │              │
│ - UserId    │           │              │
└─────────────┘           └──────────────┘
```

---

## 🔗 Links

**Public GitHub Repository:**  
https://github.com/dirshaye/ScrapeWise

**Deployed Web App Link:**  
https://scrapewise-production.up.railway.app  
*(Deployment in progress - will be available after CI/CD setup)*

---

## 🧑‍🔧 Roles or Other User Segmentations to Be Implemented:

| Role | Function |
|------|----------|
| **Admin** | Manage all users, jobs, and system settings |
| **User** | Create and manage personal scraping jobs |
| **Guest** | View public jobs and documentation |

---

## 🔐 Preloaded Users in the App:

| Role | Username | Password | Component Access |
|------|----------|----------|------------------|
| **Admin** | admin@scrapewise.com | admin123 | Full system access |
| **User** | user@scrapewise.com | user123 | Personal jobs only |
| **Guest** | guest@scrapewise.com | guest123 | Read-only access |

---

## 🛢️ Database Access
The evaluation requires the app to interact with a database involving 1-N and M-N relationships. Indicate where this interaction happens by writing the browser link seen during access.

### 1-N Relationship
| Operation | URL |
|-----------|-----|
| Insert | https://scrapewise-production.up.railway.app/Scraper/NewJob |
| Edit | https://scrapewise-production.up.railway.app/Scraper/JobDetails/{id} |
| Delete | https://scrapewise-production.up.railway.app/Scraper/Delete/{id} |
| Select | https://scrapewise-production.up.railway.app/Scraper/Dashboard |

### M-N Relationship
| Operation | URL |
|-----------|-----|
| Insert | https://scrapewise-production.up.railway.app/api/tags (POST) |
| Edit | https://scrapewise-production.up.railway.app/api/jobs/{id}/tags (PUT) |
| Delete | https://scrapewise-production.up.railway.app/api/tags/{id} (DELETE) |
| Select | https://scrapewise-production.up.railway.app/api/tags (GET) |

---

## 🔌 API
Implemented Features & Endpoints

| Function | URL (endpoint) |
|----------|----------------|
| Get All Jobs | https://scrapewise-production.up.railway.app/api/jobs |
| Get Job by ID | https://scrapewise-production.up.railway.app/api/jobs/{id} |
| Get All Tags | https://scrapewise-production.up.railway.app/api/tags |
| Create Tag | https://scrapewise-production.up.railway.app/api/tags (POST) |
| Assign Tags to Job | https://scrapewise-production.up.railway.app/api/jobs/{id}/tags (PUT) |
| Get Statistics | https://scrapewise-production.up.railway.app/api/stats |

---

## 📡 SignalR
What was implemented using SignalR:
- **Real-time Job Status Updates**: Live notifications when scraping jobs start, complete, or fail
- **Live Result Streaming**: Real-time display of extracted data as jobs run
- **Job Progress Tracking**: Live progress indicators for long-running scraping operations
- **Hub Connection**: `/scrapingHub` endpoint for WebSocket connections

---

## 🧪 Instructions to Run the Application:

### Prerequisites:
- .NET 9.0 SDK
- SQL Server (LocalDB or Azure SQL)
- Git

### Local Development Setup:
```bash
# Clone the repository
git clone https://github.com/dirshaye/ScrapeWise.git
cd ScrapeWise

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the application
dotnet run
```

### Docker Deployment:
```bash
# Build and run with Docker
docker build -t scrapewise .
docker run -p 8080:80 scrapewise
```

### Production Deployment:
The application is configured with GitHub Actions CI/CD pipeline that automatically:
1. Builds the application on push to main branch
2. Runs tests
3. Deploys to Railway (or other configured platform)

### Environment Variables:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ASPNETCORE_ENVIRONMENT`: Set to "Production" for deployment

---

## 🚀 Deployment Status:
- ✅ **GitHub Repository**: https://github.com/dirshaye/ScrapeWise
- ✅ **CI/CD Pipeline**: GitHub Actions configured
- ✅ **Docker Support**: Containerized application
- 🔄 **Live Deployment**: Railway deployment in progress
- ✅ **Database**: SQL Server with Entity Framework migrations

---

## 📊 Project Statistics:
- **Total Lines of Code**: 2,500+
- **API Endpoints**: 6 RESTful endpoints
- **Database Tables**: 5 tables with relationships
- **UI Components**: 8+ modern pages
- **Real-time Features**: SignalR hub with 5 methods
- **Test Coverage**: Unit tests for core functionality

---

*This project demonstrates advanced ASP.NET Core MVC development with modern web technologies, real-time communication, and production-ready deployment practices.* 