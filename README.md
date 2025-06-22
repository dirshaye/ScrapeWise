# ScrapeWise - Intelligent Web Scraping Dashboard

![ScrapeWise Home Page](./assets/images/home.png)

ScrapeWise is a modern, stylish web dashboard built with ASP.NET Core MVC that allows users to create, manage, and review web scraping jobs. It features a dynamic, glassmorphism-inspired UI and demonstrates key data relationship concepts, including one-to-one (User to Profile) and one-to-many (User to Jobs, Job to Results).

---

## ✨ Features

- **Modern & Dynamic UI:** A beautiful "Aurora Glass" theme provides a cutting-edge, professional user experience.
- **Job Creation:** Easily create new web scraping jobs by providing a target URL and a CSS selector.
- **Dashboard Overview:** View all scraping jobs in a clean, organized table with key details at a glance.
- **Job Details:** Dive into the specifics of any job, including the parameters and all extracted results.
- **Job Deletion:** Safely remove jobs you no longer need via a confirmation modal.
- **User Profile Management:** Edit your display name, avatar, and other settings on the Profile page (demonstrates a one-to-one relationship).
- **Persistent Storage:** All jobs, results, and user profiles are saved to a database using Entity Framework Core.

---

## 🛠️ Technology Stack

- **Backend:** C#, ASP.NET Core MVC
- **Database:** Entity Framework Core, SQL Server (or any other EF Core compatible database)
- **Frontend:** HTML, CSS, JavaScript, Bootstrap
- **Scraping Library:** HtmlAgilityPack

---

## 🚀 Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or the version you used)
- A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)
- SQL Server or another database supported by Entity Framework Core

### Installation & Setup

1.  **Clone the repository:**
    ```sh
    git clone <your-repo-url>
    cd ScrapeWise-Intelligent-Web-Scraping-Dashboard-ASP.NET-Core-MVC-
    ```

2.  **Configure the database connection:**
    - Open `appsettings.json`.
    - Modify the `ConnectionString` to point to your local database instance.
    - Example for SQL Server LocalDB:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ScrapeWiseDB;Trusted_Connection=True;MultipleActiveResultSets=true"
      }
      ```

3.  **Apply database migrations:**
    - Open your terminal in the project root and run the following commands to create and seed the database:
    ```sh
    dotnet ef database update
    ```
    *(If you need to create migrations first, run `dotnet ef migrations add InitialCreate`)*

4.  **Run the application:**
    ```sh
    dotnet run
    ```

5.  **Open your browser** and navigate to `https://localhost:5001` (or the port specified in your terminal).

---

## 📸 Screenshots

*Replace the placeholders below with screenshots of your application.*

| Dashboard | New Scrape |
| :---: | :---: |
| ![Dashboard Screenshot](./assets/images/dashboard.png) | ![New Scrape Screenshot](./assets/images/new-scrape.png) |

| Job Details | Profile Page |
| :---: | :---: |
| ![Job Details Screenshot](./assets/images/job-details.png) | ![Profile Page Screenshot](./assets/images/profile.png) |
