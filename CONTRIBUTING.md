# Contributing to ScrapeWise

Thank you for your interest in contributing to ScrapeWise! This document provides guidelines and information for contributors.

## How to Contribute

### Reporting Issues

1. **Search existing issues** to avoid duplicates
2. **Use issue templates** when creating new issues
3. **Provide clear reproduction steps** for bugs
4. **Include system information** (OS, .NET version, browser)

### Suggesting Features

1. **Check the roadmap** to see if the feature is already planned
2. **Create a feature request** with detailed description
3. **Explain the use case** and potential impact
4. **Consider backwards compatibility**

### Submitting Pull Requests

1. **Fork the repository** and create a feature branch
2. **Follow coding standards** described below
3. **Write comprehensive tests** for new functionality
4. **Update documentation** as needed
5. **Submit PR against the main branch**

## Development Setup

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database
- Git
- IDE (Visual Studio, VS Code, or JetBrains Rider)

### Local Development

```bash
# Clone your fork
git clone https://github.com/your-username/ScrapeWise.git
cd ScrapeWise

# Install dependencies
dotnet restore

# Set up database
dotnet ef database update

# Run the application
dotnet run
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Tests/ScrapeWise.Tests/
```

## Coding Standards

### C# Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use **PascalCase** for public members and **camelCase** for private fields
- Add **XML documentation** for public APIs
- Use **async/await** for I/O operations
- Follow **SOLID principles**

### Code Formatting

```csharp
// Good example
public async Task<ActionResult<ScrapingJob>> CreateJobAsync(CreateJobRequest request)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var job = new ScrapingJob
    {
        TargetUrl = request.Url,
        CssSelector = request.Selector,
        UserId = User.GetUserId()
    };

    await _context.ScrapingJobs.AddAsync(job);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetJob), new { id = job.ScrapingJobId }, job);
}
```

### Frontend Guidelines

- Use **semantic HTML5** elements
- Follow **Bootstrap 5** conventions
- Write **vanilla JavaScript** or TypeScript
- Minimize external dependencies
- Ensure **responsive design**
- Add **accessibility** features (ARIA labels, etc.)

## Testing Guidelines

### Unit Tests

- Test **business logic** thoroughly
- Mock **external dependencies**
- Use **AAA pattern** (Arrange, Act, Assert)
- Maintain **high code coverage** (>80%)

### Integration Tests

- Test **API endpoints**
- Verify **database operations**
- Test **authentication/authorization**
- Test **SignalR hubs**

### Example Test

```csharp
[Fact]
public async Task CreateJob_WithValidData_ReturnsCreatedResult()
{
    // Arrange
    var request = new CreateJobRequest
    {
        Url = "https://example.com",
        Selector = ".content"
    };

    // Act
    var result = await _controller.CreateJobAsync(request);

    // Assert
    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    var job = Assert.IsType<ScrapingJob>(createdResult.Value);
    Assert.Equal(request.Url, job.TargetUrl);
}
```

## Pull Request Process

### Before Submitting

- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Tests added for new functionality
- [ ] Tests pass locally
- [ ] Documentation updated
- [ ] No merge conflicts

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change (fix or feature causing existing functionality to change)
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Screenshots (if applicable)
Include screenshots for UI changes

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review performed
- [ ] Documentation updated
- [ ] Tests added/updated
```

## Code Review Guidelines

### For Contributors

- **Be responsive** to feedback
- **Ask questions** if feedback is unclear
- **Make requested changes** promptly
- **Keep PRs focused** and small when possible

### For Reviewers

- **Be constructive** and respectful
- **Explain reasoning** behind suggestions
- **Approve when ready** or request specific changes
- **Test locally** for complex changes

## Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backwards compatible)
- **PATCH**: Bug fixes (backwards compatible)

### Release Workflow

1. **Feature freeze** for upcoming release
2. **Release candidate** testing
3. **Final testing** and bug fixes
4. **Release tagging** and deployment
5. **Release notes** publication

## Development Priorities

### Current Focus Areas

1. **Performance Optimization**
   - Database query optimization
   - Caching implementation
   - Resource management

2. **User Experience**
   - UI/UX improvements
   - Accessibility enhancements
   - Mobile responsiveness

3. **Features**
   - Advanced scraping options
   - Data export formats
   - Scheduling system

4. **Infrastructure**
   - Monitoring and logging
   - Error handling
   - Security improvements

## Getting Help

### Communication Channels

- **GitHub Issues**: Bug reports and feature requests
- **GitHub Discussions**: General questions and ideas
- **Email**: Direct contact for sensitive issues

### Documentation

- **API Documentation**: Available in Swagger UI
- **Architecture Docs**: See `/docs` folder
- **Deployment Guide**: `AZURE_DEPLOYMENT_GUIDE.md`

## Recognition

Contributors will be recognized in:
- **Contributors list** in README
- **Release notes** for significant contributions
- **Special recognition** for long-term contributors

## Code of Conduct

### Our Standards

- **Be respectful** and inclusive
- **Use welcoming language**
- **Accept constructive criticism**
- **Focus on what's best** for the project
- **Show empathy** towards other contributors

### Enforcement

Violations of the code of conduct should be reported to the project maintainers. All complaints will be reviewed and investigated promptly and fairly.

---

Thank you for contributing to ScrapeWise! Your contributions make this project better for everyone.
