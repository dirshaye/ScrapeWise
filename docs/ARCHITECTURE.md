# Architecture Overview

## System Architecture

ScrapeWise follows a modern, cloud-native architecture built on Microsoft technologies and best practices.

## Technology Stack

### Backend
- **ASP.NET Core 9.0 MVC** - Web application framework
- **C# 12** - Programming language with latest features
- **Entity Framework Core** - ORM for data access
- **Azure SQL Database** - Cloud database service
- **SignalR** - Real-time communication

### Frontend
- **Razor Views** - Server-side rendering
- **Bootstrap 5** - Responsive UI framework
- **JavaScript ES6+** - Client-side functionality
- **Chart.js** - Data visualization

### Infrastructure
- **Azure App Service** - Web hosting platform
- **Azure Container Registry** - Container management
- **Docker** - Containerization
- **GitHub Actions** - CI/CD pipeline
- **Terraform** - Infrastructure as Code

## Architecture Patterns

### Design Patterns
- **Model-View-Controller (MVC)** - Application structure
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Inversion of control
- **Command Query Responsibility Segregation (CQRS)** - For complex operations

### Security Patterns
- **Authentication & Authorization** - ASP.NET Core Identity
- **Role-Based Access Control (RBAC)** - User permissions
- **Data Protection** - Sensitive data encryption
- **Anti-Forgery Tokens** - CSRF protection

## Data Architecture

### Database Design
- **Entity Framework Code-First** - Database schema management
- **Migrations** - Version-controlled schema changes
- **Connection Pooling** - Performance optimization
- **Indexing Strategy** - Query optimization

### Data Models
- **User Management** - ASP.NET Core Identity tables
- **Job Management** - Scraping job configurations
- **Results Storage** - Scraped data persistence
- **Audit Logging** - Change tracking

## Deployment Architecture

### Containerization
```
Azure Container Registry
├── Base Image: mcr.microsoft.com/dotnet/aspnet:9.0
├── Application Layer: ScrapeWise.dll
├── Configuration: appsettings.json
└── Static Assets: wwwroot/
```

### Cloud Infrastructure
```
Azure Resource Group
├── App Service Plan (Linux)
├── App Service (Container)
├── SQL Server
├── SQL Database
└── Container Registry
```

## Performance Considerations

### Scalability
- **Horizontal Scaling** - Multiple app service instances
- **Database Connection Pooling** - Efficient resource usage
- **Caching Strategy** - In-memory and distributed caching
- **Asynchronous Operations** - Non-blocking I/O

### Monitoring
- **Application Insights** - Performance monitoring
- **Health Checks** - Service availability
- **Logging** - Structured logging with Serilog
- **Metrics** - Custom performance counters

## Security Architecture

### Authentication Flow
1. User submits credentials
2. ASP.NET Core Identity validates
3. JWT token or cookie issued
4. Subsequent requests authenticated

### Authorization Layers
- **Controller-level** - Route protection
- **Action-level** - Method-specific permissions
- **Resource-level** - Data access control
- **UI-level** - Conditional rendering

## Development Workflow

### GitFlow
```
main (production)
├── develop (integration)
├── feature/* (new features)
├── hotfix/* (critical fixes)
└── release/* (version preparation)
```

### CI/CD Pipeline
1. **Code Push** - GitHub repository
2. **Build** - .NET compilation and tests
3. **Containerize** - Docker image creation
4. **Deploy** - Azure App Service deployment
5. **Verify** - Health checks and smoke tests
