# TaskFlow – Task Management System

A full-featured Task Management web application built with **ASP.NET Core MVC**, **Entity Framework Core (In-Memory)**, and the **AdminLTE 3** dashboard template.

---

## ✨ Features

- **Task Creation** — Add tasks with a Title, Description, Status, and optional file attachment (Image or PDF)
- **Task Listing** — View all tasks in a structured table with color-coded status badges
- **File Handling** — Upload and download image/PDF attachments per task
- **AJAX Delete** — Delete tasks without page reload, with confirmation dialog and toast notifications
- **Live Search** — Filter tasks by title or description instantly
- **Summary Cards** — At-a-glance counts for Pending, In Progress, and Completed tasks
- **AdminLTE 3 UI** — Professional dashboard layout loaded via CDN

---

## 🛠️ Tech Stack

| Layer      | Technology                          |
|------------|-------------------------------------|
| Backend    | ASP.NET Core 8 MVC                  |
| Database   | Entity Framework Core (MSSQL)   |
| Frontend   | AdminLTE 3, Bootstrap 4, jQuery     |
| Icons      | Font Awesome 6                      |
| Fonts      | DM Sans, DM Mono (Google Fonts)     |

---

## 📁 Project Structure

```
TaskManagement/
├── Controllers/
│   └── TasksController.cs       # Index, Create, Delete (AJAX), Download
├── Data/
│   └── AppDbContext.cs          # EF Core In-Memory DbContext
├── Models/
│   └── TaskItem.cs              # Task entity model
├── ViewModels/
│   └── CreateTaskViewModel.cs   # Form ViewModel with file upload
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml       # AdminLTE 3 master layout
│   └── Tasks/
│       └── Index.cshtml         # Main page (form + table)
├── wwwroot/
│   ├── css/
│   │   └── site.css             # Custom styles on top of AdminLTE
│   ├── js/
│   │   └── site.js              # AJAX delete, toasts, live search
│   └── uploads/                 # Uploaded files stored here
├── appsettings.json
├── Program.cs
└── TaskManagement.csproj
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Any IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/)

### Run Locally

```bash
# 1. Clone the repository
git clone https://github.com/mahmoudalam1973/TaskManagement.git
cd TaskManagement

# 2. Restore dependencies
dotnet restore

# 3. Run the application
dotnet run

# 4. Open in browser
# https://localhost:5001  or  http://localhost:5000
```

> **Note:** The app uses an In-Memory database, so data resets on each restart. No database setup is required.

---

## ⚙️ Configuration

The `appsettings.json` file contains basic logging configuration. No connection string is needed for the default In-Memory setup.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=Name of Server;Database=TaskManagementDb; Trusted_Connection=True;Trust Server Certificate=True"
    //In Case of Cloud Server
    //"DefaultConnection": "Data Source=Server IP;Initial Catalog=Database Name;User Id=Username;Password=Password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Switching to SQL Server

1. Install the EF Core SQL Server package:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

2. Update `appsettings.json`:

// Common connection string variants:
// Local SQL Server instance
// Server=.;Database=TasksDb;Trusted_Connection=True;TrustServerCertificate=True;

// Named instance
// Server=.\SQLEXPRESS;Database=TasksDb;Trusted_Connection=True;TrustServerCertificate=True;

// With username & password
// Server=.;Database=TasksDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;

// Remote server
// Server=192.168.1.100,1433;Database=TasksDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\SQLEXPRESS;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Update `Program.cs`:
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(opts =>
       opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

4. Apply migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## 🔒 File Upload Constraints

| Property        | Value                              |
|-----------------|------------------------------------|
| Allowed types   | JPG, JPEG, PNG, GIF, WEBP, PDF     |
| Max file size   | 10 MB                              |
| Storage location| `wwwroot/uploads/` (GUID filename) |

---

## 🔄 AJAX Delete Flow

1. User clicks the **Delete** button on a task row
2. A **Bootstrap modal** appears asking for confirmation
3. On confirm, a `DELETE` request is sent via **jQuery AJAX** with the anti-forgery token
4. On success:
   - The table row is **animated out** and removed from the DOM
   - A **green toast** notification appears
5. On failure, a **red toast** notification appears with the error message

---

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

---

## 🌐 CDN Dependencies

All frontend libraries are loaded via CDN — no `npm install` needed.

- [AdminLTE 3.2.0](https://adminlte.io/)
- [Bootstrap 4.6.2](https://getbootstrap.com/docs/4.6/)
- [jQuery 3.7.1](https://jquery.com/)
- [Font Awesome 6.5.0](https://fontawesome.com/)
- [Google Fonts – DM Sans & DM Mono](https://fonts.google.com/)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
