# DataDepot (CMSMeter)

Field operations CMS for **electric, gas, and water meter installation**. It covers projects, appointments, installer allocation, inventory/warehouse, audits, photos, Excel uploads, maps, and reports.

| | |
|---|---|
| Version | v1.0015 |
| Solution | `GarronT.CMS.sln` |
| Runtime | .NET Framework 4.8 |
| UI | ASP.NET MVC 5, Razor, jQuery |
| Data | SQL Server + Entity Framework 6.1.3 |
| Auth | Forms Authentication, ASP.NET Identity, RBAC |

Runtime settings live in `GarronT.CMS.UI/Web.config`. See [Projectconfig.yml](Projectconfig.yml) for a structured map of the solution.

---

## Solution structure

| Project | Type | Role |
|---|---|---|
| `GarronT.CMS.UI` | Web | MVC UI, Web API, ASMX mobile service, ReportViewer |
| `GarronT.CMS.Model` | Library | EF6 `CompassEntities`, DAL, business objects |
| `CMSShedulerService` | Windows service | Scheduled jobs |
| `CMSAuditThread` | Worker | Audit processing |
| `TNMPUploadUtility` | Utility | TNMP uploads |

### MVC areas

| Area | Route | Purpose |
|---|---|---|
| Global | `/Global/{controller}/{action}` | Login and shared entry |
| Admin | `/Admin/{controller}/{action}/{id}` | Projects, inventory, appointments, reports, users |
| Form | `/Form/{controller}/{action}` | Dynamic form management |
| Installer | `/Installer/{controller}/{action}` | Installer dashboard |
| SuperAdmin | `/SuperAdmin/{controller}/{action}` | Super admin dashboard |

### APIs

- MVC site: `GarronT.CMS.UI`
- Web API: `api/Login`, `api/UploadFile`
- ASMX: `CMS_MobileService.asmx`
- Maps: Google Maps + geocoding (`googleapiKey` in Web.config)

---

## Prerequisites

- Windows with Visual Studio 2017+ (or later with .NET Framework 4.8 targeting pack)
- IIS Express or IIS
- SQL Server (local instance used as `CMSMeterLive`)
- NuGet restore for the solution

---

## Run locally

1. Open `GarronT.CMS.sln` in Visual Studio.
2. Set **GarronT.CMS.UI** as the startup project.
3. Confirm `GarronT.CMS.UI/Web.config`:
   - `DefaultConnection` and `CompassEntities` point at your SQL Server.
   - `WebURL` is `http://localhost:6070/`.
4. Restore NuGet packages and build.
5. Run the web project. IIS Express URL: [http://localhost:6070/](http://localhost:6070/)

Login is at `/Global/Login/`.

Live site (reference): `https://www.cmsmeter.com/CMSMA/`

---

## Configuration

Do **not** commit real passwords, mail credentials, or API keys. Keep them in `Web.config` / `App.config` (or a secret store).

Important `appSettings` keys:

- `Version`, `WebURL`
- `WebMasterMailId`, `WebMasterSMTPServer`, `WebMasterPort`
- `UploadFilePath`, `ClockTimeReportUploadPath`, `InventoryUploadPath`
- `googleapiKey`
- `webApiUploadCustomeImage`

Connection strings:

- `DefaultConnection` — ADO.NET
- `CompassEntities` — Entity Framework

Environments currently noted in Web.config (only one should be uncommented):

| Environment | Database | Notes |
|---|---|---|
| Local | `CMSMeterLive` | Active in the current Web.config |
| Staging | `CMSMeter` | Commented |
| Live | `CMSMeter` | Commented |

---

## Main modules

- Projects and project status
- Appointments and installer allocation
- Inventory, warehouse, stock, transfers
- Meter type / make / size
- Customers
- Photos and file uploads
- Project audit
- Reports, auditor reports, PDF export
- Clock time
- Excel bulk upload
- Google Maps / geocoding
- Devices
- Roles and permissions

---

## AI (planned, not enabled)

AI is **not** wired into the app yet (`ai.enabled: false` in `Projectconfig.yml`). Intended provider: **Azure OpenAI**.

Planned uses:

1. Admin dashboard natural-language Q&A on reports
2. Meter photo QA / OCR
3. Excel upload error explanation
4. Audit summary

Rules if implemented:

- Call AI only from server-side C# (never from the browser)
- Reuse existing `[Authorize]` and RBAC
- Send filtered SQL results only, not the full database
- Use `HttpClient` / RestSharp already in the UI project (this app is net48; do not add a .NET 8 Azure OpenAI SDK)

---

## Conventions

- Language: C#
- UI: Razor + jQuery (not a SPA)
- Jobs: Hangfire 1.8.6
- Logging: log4net
- Do not rewrite to .NET 8 unless explicitly requested
