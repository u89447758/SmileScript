# SmileScript Blog Platform

<img width="1908" height="995" alt="image" src="https://github.com/user-attachments/assets/1dc12b90-a614-402e-9e5a-80f2445083b1" />

A modern, feature-rich blog platform built with ASP.NET Core. This project serves as a complete, working example of a modern web application, demonstrating concepts like role-based access control, an AJAX-powered comment system, and a dynamic modal-based UI for authentication.

---

## Table of Contents

- [About The Project](#about-the-project)
- [Key Features](#key-features)
- [Built With](#built-with)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [License](#license)
- [Acknowledgements](#acknowledgements)

---

## About The Project

SmileScript is a fully-functional blogging application designed to showcase the power and elegance of the ASP.NET Core ecosystem. It features a secure backend with distinct roles for Administrators and Authors, a public-facing blog for readers, and a rich text editing experience using Markdown.

The entire authentication flow (Login, Register, Forgot Password, Logout) has been rebuilt using Bootstrap modals for a seamless single-page application feel, providing instant feedback to the user with toast notifications.

---

## Key Features

-   **Role-Based Access Control:**
    -   **Admin:** Can manage all users, categories, and posts.
    -   **Author:** Can create and manage their own blog posts.
    -   **User:** Can comment on posts and manage their own profile (default role).
-   **Full Blog Post CRUD:** Create, Read, Update, and Delete blog posts.
-   **Markdown Editor:** A rich, client-side Markdown editor (`Editor.md`) for writing posts, with image upload support.
-   **Post Approval Workflow:** Authors submit posts for review, which an Admin can then approve or reject.
-   **Category Management:** Admins can create, edit, and delete post categories.
-   **User Management Dashboard:** Admins have a dedicated interface to manage users and their roles.
-   **AJAX Comment System:** Users can post comments on articles without a page reload.
-   **Modal-Based Authentication:** A modern UX where login, registration, and password reset happen in modals, not on separate pages.

---

## Built With

This project is built on a modern .NET and web stack.

**Backend:**
-   [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
-   [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
-   [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)

**Database:**
-   [SQL Server](https://www.microsoft.com/en-us/sql-server) (configured for LocalDB)

**Frontend:**
-   HTML5, CSS3, JavaScript
-   [Bootstrap 4](https://getbootstrap.com/docs/4.6/)
-   [jQuery](https://jquery.com/)
-   [AdminLTE 3](https://adminlte.io/) (for the dashboard layout)
-   [Editor.md](https://pandao.github.io/editor.md/en.html) (for the Markdown editor)
-   [Markdig](https://github.com/xoofx/markdig) (for server-side Markdown rendering)

---

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

You will need the following software installed on your machine:
-   [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or newer)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/)
-   [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (usually installed with Visual Studio)

### Installation

1.  **Clone the repository**
    ```sh
    git clone https://github.com/u89447758/SmileScript.git
    ```
2.  **Open the solution**
    -   Open the `SmileScript.sln` file in Visual Studio.
3.  **Update Database Connection String**
    -   Open `appsettings.json`.
    -   Verify that the `DefaultConnection` connection string is correct for your local SQL Server instance. The default is set up for LocalDB.
4.  **Run Database Migrations**
    -   In Visual Studio, go to **Tools -> NuGet Package Manager -> Package Manager Console**.
    -   Run the following command to create and seed the database:
      ```sh
      Update-Database
      ```
5.  **Run the Application**
    -   Press `F5` or the "Run" button in Visual Studio to start the application.
    -   The application will build, and your browser will open to the home page. The database will be seeded with default users and roles on the first run.

---

## Usage

The application is seeded with two default accounts for demonstration purposes.

**It is highly recommended that you change these passwords immediately after logging in for the first time.**

-   **Admin Account**
    -   **Email:** `admin@smilescript.com`
    -   **Password:** `AdminPassword123!`
    -   **Capabilities:** Full control over the site. Can manage users, all posts, and all categories.

-   **Author Account**
    -   **Email:** `author@smilescript.com`
    -   **Password:** `AuthorPassword123!`
    -   **Capabilities:** Can create, edit, and delete their own blog posts. Posts are submitted for admin review.

---

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

---

## Acknowledgements

-   [AdminLTE](https://adminlte.io/)
-   [Editor.md](https://pandao.github.io/editor.md/en.html)
-   [Bootstrap](https://getbootstrap.com/)
-   [jQuery](https://jquery.com/)
