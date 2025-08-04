# Security Policy

## Supported Versions

We release patches for security vulnerabilities for the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |

## Reporting a Vulnerability

We take the security of ScrapeWise seriously. If you believe you have found a security vulnerability, please report it to us as described below.

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, please send an email to dirshayeyishak2@gmail.com with the following information:

- Type of issue (e.g. buffer overflow, SQL injection, cross-site scripting, etc.)
- Full paths of source file(s) related to the manifestation of the issue
- The location of the affected source code (tag/branch/commit or direct URL)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the issue, including how an attacker might exploit the issue

We prefer all communications to be in English.

## Response Timeline

- **Acknowledgment**: We will acknowledge receipt of your vulnerability report within 48 hours.
- **Initial Assessment**: We will provide an initial assessment within 5 business days.
- **Regular Updates**: We will send you regular updates about our progress every 5 business days.
- **Resolution**: We aim to resolve critical vulnerabilities within 30 days of the initial report.

## Security Measures

This project implements several security measures:

- **Authentication**: ASP.NET Core Identity with secure password policies
- **Authorization**: Role-based access control
- **Data Protection**: ASP.NET Core Data Protection APIs
- **HTTPS**: Enforced HTTPS in production
- **SQL Injection Prevention**: Entity Framework Core with parameterized queries
- **XSS Protection**: Razor view engine with automatic encoding
- **CSRF Protection**: Anti-forgery tokens

## Preferred Languages

We prefer all communications to be in English.

## Attribution

We will acknowledge security researchers who responsibly disclose vulnerabilities to us.
