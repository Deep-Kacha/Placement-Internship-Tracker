# 📋 Placement & Internship Tracker

A web-based application built with **ASP.NET (C#)** to help students and administrators efficiently manage and monitor placement and internship opportunities — from application to final selection.

---

## 🚀 Features

- 📌 **Track Applications** — Add, update, and monitor placement and internship applications in one place
- 🏢 **Company Management** — Maintain a list of companies visiting for placement/internship drives
- 🎓 **Student Records** — Manage student profiles, eligibility, and application history
- 📊 **Status Tracking** — Monitor application status (Applied → Interview → Selected / Rejected)
- 🔍 **Search & Filter** — Quickly find records by company, student, status, or date
- 🗂️ **Organized Dashboard** — Get a clear overview of all ongoing and completed drives

---

## 🛠️ Tech Stack

| Layer        | Technology                    |
|--------------|-------------------------------|
| Frontend     | HTML, CSS, ASP.NET Web Forms  |
| Backend      | C# (.NET Framework)           |
| Database     | Microsoft SQL Server          |
| IDE          | Visual Studio                 |

---

## 📁 Project Structure

```
Placement-Internship-Tracker/
│
├── Placement-Internship-Tracker-WebApp/   # Main web application
│   ├── *.aspx                             # Web form pages (UI)
│   ├── *.aspx.cs                          # Code-behind files (C# logic)
│   ├── Web.config                         # App configuration & DB connection
│   └── ...
│
└── README.md
```

---

## ⚙️ Prerequisites

Before running this project, make sure you have the following installed:

- [Visual Studio 2019 / 2022](https://visualstudio.microsoft.com/) (with ASP.NET workload)
- [.NET Framework 4.x](https://dotnet.microsoft.com/en-us/download/dotnet-framework)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) *(recommended)*

---

## 🏗️ Setup & Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Deep-Kacha/Placement-Internship-Tracker.git
cd Placement-Internship-Tracker
```

### 2. Open in Visual Studio

- Open `Placement-Internship-Tracker-WebApp/` in Visual Studio.

### 3. Configure the Database

- Open **SQL Server Management Studio (SSMS)** and create a new database (e.g., `PlacementTrackerDB`).
- Run the SQL script (if provided) to set up the required tables.
- Update the connection string in `Web.config`:

```xml
<connectionStrings>
  <add name="PlacementDB"
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=PlacementTrackerDB;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4. Build & Run

- Press **F5** or click **Start** in Visual Studio to build and launch the application.
- The app will open in your default browser.

---

## 📸 Usage

1. **Register/Login** as a student or admin.
2. **Add Companies** participating in placement/internship drives.
3. **Students** can apply to listed companies through the portal.
4. **Admins** can update application statuses and manage all records.
5. Track the entire journey — from initial application to final result.

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m 'Add: your feature description'`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a **Pull Request**

---

## 👤 Author

**Deep Kacha**
- GitHub: [@Deep-Kacha](https://github.com/Deep-Kacha)

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

> *Built with ❤️ to simplify the placement process for students.*
