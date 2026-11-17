# DataDepot (GarronT.CMS) — Tester Guide

Use this file to **test the live or local web app** without reading the source code.

**Product:** DataDepot / GarronT.CMS (also called CMSMeter)  
**Version:** v1.0015  
**What it is:** Office staff plan utility-meter projects; field installers complete visits on a mobile app; auditors sample completed work; inventory and reports sit in the same portal.

In-app help (screenshots / older manual): after login, open the Help link, or go to `GarronT.CMS.UI/CMSHelpManual/quick/toc.html`.  
Developer encyclopedia (tables, APIs, jobs): [`Projectconfig.yml`](Projectconfig.yml)

**Does this file cover the entire project?**  
Yes for **functional QA of every live module** (sidebar, off-menu screens, mobile ASMX, Web API, Windows jobs). Use the **coverage matrix** in section 2.1 as the master tick-list.  
Also included for testers: owner ask-list, test-data naming, how to call ASMX/API, local vs live, blocked-vs-bug, and a sign-off box.  
It does **not** include automated unit/UI scripts, performance/load, or the native phone APK (this repo is the **server**; test the app against the ASMX/API URLs). Commented jobs are listed so you do not fail them as bugs.

---

## 1. Start here (5 minutes)

### 1.1 URLs

| Environment | Open this | Login page |
|-------------|-----------|------------|
| **Local** | http://localhost:6070/ | http://localhost:6070/Global/Login |
| **Live** | https://www.datadepotonline.com/ | https://www.datadepotonline.com/Global/Login |

| Channel | How to test it |
|---------|----------------|
| Web (browser) | Login form: **User Name** + **Password** + optional Remember me |
| REST | `POST /api/login` with JSON `{ "UserName": "...", "Password": "...", "RememberMe": false }` |
| Mobile service | Open `/CMS_MobileService.asmx` and call **UserLogin** |

Local ASMX: http://localhost:6070/CMS_MobileService.asmx  
Live ASMX: https://www.datadepotonline.com/CMS_MobileService.asmx

**Browser:** Chrome or Edge (current). IE is not a target.

### 1.2 Credentials (there is no seeded admin in source)

Ask the project owner for **one SysAdmin / Admin** username on the environment you are testing.

| Fact | What to use |
|------|-------------|
| Usernames live in SQL (`tblUsers` / `AspNetUsers`), not in this repo | Get them from the owner or from **Administration → User Master** after you have an admin |
| New users created in User Master | Default password from Web.config `DefaultPassword`: **`admin@123`** (unless the creator typed another password) |
| Old commented hint in code | `sanjiv@kairee.in` / `Admin@123` — **do not assume this exists** |
| Deactivated user | Login shows **User not active.** |
| Wrong password | Login shows **Invalid login attempt.** |
| Empty fields | **User Name Required** / **Password Required** |

**Do not test against live with dummy data** unless the owner gives you a sandbox project. Prefer local `CMSMeterLive` or a named test project.

### 1.3 Roles you need for a full pass

Ask admin to create (or point you to) these users. New-user password is usually `admin@123`.

| Role (typical) | Used for |
|----------------|----------|
| **SysAdmin** | User Role Rights; bypasses RBAC |
| **Admin / office** | Masters, project, allocation, inventory, reports |
| **Installer** | Mobile field visit + inventory on phone |
| **Auditor** | Mobile audit form (form type 1) |

If a menu is missing, the user has no RBAC permission. Opening the URL directly should redirect to **You do not have access permission.** (`Admin/Unauthorised`).

### 1.4 Ask the owner *before* you start (or tests will be Blocked)

Copy this list into chat/email:

| Ask | Why |
|-----|-----|
| Admin / SysAdmin login for **this** environment | No seeded user in source |
| May I create a project named `QA_Test_YYYYMMDD`? | Live must not mix with real utilities |
| Is Google geocode key working? | TC-08 fails for all rows if key is dead |
| Is SMTP (`WebMasterMail*`) working? | Forgot Password + missing-inventory mail |
| Are `CMSShedulerService` / `CMSAuditThread` running? | TC-14, TC-26 |
| Mobile APK + which server URL it points to | Phone tests vs ASMX-only |
| Sample Excel for this customer (or use in-app sample download) | Tab 6 mapping |
| Which warehouse / existing masters can I reuse? | Saves Day 1 |

### 1.5 Test data naming (so cleanup is easy)

| Thing | Name it |
|-------|---------|
| Project | `QA_Test_YYYYMMDD` |
| Customer | `QA_Customer_YYYYMMDD` |
| Installer login | `qa.installer.YYYYMMDD` |
| Auditor login | `qa.auditor.YYYYMMDD` |
| Meter/service masters | prefix `QA-` |

Do **not** use live project ids **10087, 10122, 10124, 10134** unless the owner says you are testing that utility.

**Password on new users:** `admin@123` unless you typed another. User Master also sets **PasswordExpiryDays = 15** — after 15 days the same password may fail; reset via User Master / Forgot Password.

---

## 2. How the product flows (so you know what to expect)

Test in this order. Each letter is a test area below.

```
 A Login / RBAC
 B Masters (dropdown data)
 C Customer
 D Form Management (what the phone shows)
 E Project wizard (6 steps) + Excel accounts
 F Geocode
 G Inventory receive
 H Installer allocation (+ appointment)
 I Mobile download + SaveData / Skip
 J Photos on disk / S3
 K Audit sample + auditor visit
 L Dashboard + reports
```

Happy path in one sentence: **upload accounts → allocate installer → installer saves visit on phone → photos go to cloud → scheduler samples audit → office runs reports.**

### 2.1 Entire-project coverage matrix (tick this)

Every live piece of the product maps to a test. If a row is blank, that area is **not done**.

**Sidebar — Administration**

| Screen | Test |
|--------|------|
| City Logo Master | TC-03 |
| Form Type Master | TC-03 |
| Meter Make / Type / Size | TC-03 |
| Picture Master | TC-03 |
| Reason Master | TC-03 |
| Service Master | TC-03 |
| User Master (create, roles, deactivate) | TC-04 |
| User Master extras (vehicle, photo, manager, I-card, form types) | TC-18 |
| User Role Rights (SysAdmin) | TC-02 |

**Sidebar — Inventory**

| Screen | Test |
|--------|------|
| Warehouse / Category / Product Master | TC-03, TC-09 |
| Inventory Upload / Receive / View | TC-09 |
| View Stock Request | TC-09, TC-13 |
| Inventory Transfer / Warehouse Transfer / Movement | TC-09, TC-19 |
| Inventory Request Configuration | TC-19 |

**Sidebar — Operations**

| Screen | Test |
|--------|------|
| Alert Master | TC-17 |
| Audit | TC-14 |
| Customer | TC-05 |
| Project (6-tab wizard, clone, deactivate) | TC-07, TC-08 |
| Installer Allocation | TC-10 |
| Appointment | TC-20 |
| Form Management SPA | TC-06 |

**Sidebar — Dashboard / reports**

| Screen | Test |
|--------|------|
| Dashboard (all filters + charts) | TC-16, TC-21 |
| Project Status | TC-16 |
| Time Clock / Duplicate / Export / Installer / Auditor / Skip History / Budget | TC-16 |
| Warehouse Stock / Inventory Stock / Movement / INV Installer | TC-16 |

**Off-menu / extra web (no sidebar or menu commented)**

| Screen | Test |
|--------|------|
| Device master | TC-22 |
| Employee I-card | TC-18 |
| Country / State / City | TC-22 |
| Role Master (menu commented; URL if used) | TC-22 |
| Admin Form CRUD (`FormController`) vs SPA | TC-06 note |
| Installer / SuperAdmin dashboard stubs | TC-22 |
| Unauthorised + Error pages | TC-01, N2, N8 |
| Account / Manage (password) | TC-01, TC-18 |
| Help manual | S15 |

**Mobile ASMX (`CMS_MobileService.asmx`) — every active method**

| Group | Methods | Test |
|-------|---------|------|
| Auth / sync | `getServerDate`, `checkDate`, `UserLogin`, `UserNotificationData`, `UserProjectData`, `OfflineUserDataAll`, `setLastSynchFlag` | TC-11, TC-23 |
| Field | `SaveData`, `UpdateMediaFlag`, `GetAccountAssignedStatus`, `GetProjectUserList`, `get_AccountSummaryReport` | TC-12, TC-23 |
| Inventory | all receive / request / return / running / balance / sign-off / reconcile / missing-mail / auto-sync / product lists / clear | TC-13, TC-24 |
| Clock | `GetClockTimeMasterRecord`, `Save_Update_UserClockTimeRecord` | TC-13 |

Commented ASMX (`ForgotPassword`, `ChangePassword`, old inventory lists): **do not test as live.**

**Web API**

| API | Test |
|-----|------|
| `POST /api/login` | TC-25 |
| `UploadFileController` (service/custom/skip/audit, image/audio/video, from-web) | TC-25 |
| `CalculateLatLongController` | TC-08, TC-25 |
| `AuditAllocationController.AllocateAuditVisit` | TC-14, TC-25 |

**Background jobs**

| Job | Test |
|-----|------|
| `CMSShedulerService` audit every 3 hours | TC-14 |
| `CMSAuditThread` mobile `SyncEnable = 0` | TC-26 |
| `TNMPUploadUtility.ProcessCloudUrls` | TC-15 |
| SFTP ingest / export email | Out of scope (commented) — section 6 |

---

## 3. Smoke test (about 30–45 minutes)

Use an **Admin** user. Tick Pass / Fail.

| # | Action | Expected |
|---|--------|----------|
| S1 | Open login URL | Login page with User Name, Password, Remember me, Forgot Password |
| S2 | Submit empty form | Validation: User Name Required, Password Required |
| S3 | Wrong password | **Invalid login attempt.** Stay on login |
| S4 | Valid admin login | Lands on **Dashboard** (or first permitted Admin screen) |
| S5 | Sidebar visible | Dashboard, Project Status, Administration, Inventory, Operations, Form Management, Reports, Inventory Reports |
| S6 | Dashboard | Filters: Project Status (Active/Completed), Utility Type (Electric/Gas/Water). Charts/widgets load, no yellow error page |
| S7 | Project Status | List or filters load |
| S8 | Operations → Project | Project list. Create / Edit / Clone links work |
| S9 | Operations → Customer | Customer list loads |
| S10 | Form Management | `/FormManagement` Angular list loads |
| S11 | Administration → User Master | User grid loads |
| S12 | Inventory → Warehouse Master | Warehouse list loads |
| S13 | Reports → Installer Report | Page + project dropdown load (run is optional in smoke) |
| S14 | Logout / close session, hit an Admin URL | Redirected to login |
| S15 | Help manual | Opens, TOC visible |

If S4–S6 fail, stop and log a blocker. Do not start the full pack.

---

## 4. Full test pack (click-by-click)

Use a **throwaway test project name** like `QA_Test_YYYYMMDD` so live data is obvious.

Mark each case **Pass / Fail / Blocked**. Note the project id, user, and screenshot on Fail.

---

### TC-01 Login and session

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Login with valid Admin | Dashboard (or permitted home) |
| 2 | Tick Remember me, login, close browser, reopen site | Session/cookie behavior matches env (cookie auth; do not fail local if Remember me is weak) |
| 3 | Click **Forgot Password?**, enter a known username | Message like **Password reset successfully. Please check your mail!!!** (mail depends on SMTP — if mail is not configured, log as known limitation, not product crash) |
| 4 | Forgot Password with unknown user | Error / not-found, no crash |
| 5 | Login as **deactivated** user | **User not active.** |
| 6 | Login as user with **no menu permissions** | Still logs in; menus empty or first permission; typing `/Admin/Project` → Unauthorised page |

---

### TC-02 RBAC (permissions)

Need: SysAdmin + a **restricted** office user.

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | SysAdmin → **User Role Rights** | Permission matrix (View/Add/Edit/Delete) saves |
| 2 | Restrict the test user: remove Project View | That user does **not** see Project in sidebar |
| 3 | That user opens `/Admin/Project` by URL | **You do not have access permission.** |
| 4 | Restore View, deny Add | List visible; Create blocked or Unauthorised |
| 5 | User Master → User Permission overrides | Override wins over role (allow or deny) after re-login |

---

### TC-03 Masters (do before a new project)

**Administration** menu. For each master: open list → Add → save → appears in grid → Edit → Deactivate → does not appear in Project dropdowns → Activate again.

| Master | Menu | What to type | Used later in |
|--------|------|--------------|---------------|
| City Logo | City Logo Master | City + logo image | User I-card / branding |
| Form Type | Form Type Master | Confirm type **1 = Audit**, **2 = Field visit** exist | Forms + mobile |
| Meter Make / Type / Size | Meter masters | e.g. QA-Make, QA-Type, 5/8 | Project step 2 |
| Picture | Picture Master | Photo type name | Form photo fields |
| Reason | Reason Master | Skip reason | Mobile skip |
| Service | Service Master | e.g. QA Install | Project step 2–3 |
| User | User Master | See TC-04 | Allocation + mobile |

**Inventory masters** (same Add/Edit/Deactivate pattern): Warehouse, Category, Product.

Pass: new values appear in **Project Create** dropdowns. Fail: save error, blank list, or deactivated row still selectable.

---

### TC-04 Create test users

**Administration → User Master → Create.**

Create at least:

1. **Installer** — role installer, Form Type = field (type 2), Active.
2. **Auditor** — role auditor, Form Type = audit (type 1), Active.
3. Optional office user with limited rights.

| Check | Pass if |
|-------|---------|
| Required fields block empty save | Validation, no 500 |
| Login Id + password (or default `admin@123`) | New row in grid, Active |
| Same installer can log in on **web** (if role allows) and you will use the same Login Id on **mobile** | User exists; password works |
| Deactivate installer | Cannot login (**User not active.**) |
| Activate again | Can login |

---

### TC-05 Customer

**Operations → Customer**

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Create customer (utility name, contacts) | Saved; listed |
| 2 | Edit contact | Changes persist |
| 3 | Open Audit settings on customer if present | Audit % / failure settings save |

You need this customer on Project step 1.

---

### TC-06 Form Management (phone form)

Open **Form Management** (`/FormManagement`).

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | List forms | Grid loads |
| 2 | **Add** a field form, Form Type = **2** (project/visit) | Sections + fields save |
| 3 | Add a required photo field using Picture Master | Field shows in View |
| 4 | **View** / **Edit** / **Clone** | Clone creates a new form; original unchanged |
| 5 | Add / confirm an **Audit** form, Form Type = **1** | Saved separately from field form |
| 6 | Skip reasons available for the form type | Reasons from Reason Master appear |

Pass: View screen shows the same sections you designed. Fail: blank designer, save with no error but fields missing.

---

### TC-07 Project wizard (core office flow)

**Operations → Project → Create**

Complete **all 6 steps**. Do not skip. After each step, Next should enable the following circle.

| Step | Tab name | What to fill | Pass if |
|------|----------|--------------|---------|
| 1 | Create Project | Unique name, **your QA customer**, city/state, warehouse, dates, Utility = Electric/Gas/Water, Status Active | Project Id created; name shows in header |
| 2 | Master Fields | Select meter make/type/size, **services**, **your QA installer** | Saved; lists stick on Edit |
| 3 | Configure Services | Rates, meter pay rate, service–product qty | Saved; no JS error |
| 4 | Standard Fields | Move standard fields into the project | Fields listed on Edit |
| 5 | Custom Fields | Add one custom field, deactivate, activate | Status toggles |
| 6 | Upload Customer Data | Map Excel columns to CMS fields, upload accounts | Rows appear under the project |

**Excel:** download the sample from the upload screen if the button exists, or use `~/DownLoadFile/Sample Upload File.xlsx` (`ProjectSampleFile` in Web.config).

Column mapping must include street/address (and lat/long columns if the sample has them). STREET / ADDRESS LATITUDE / ADDRESS LONGITUDE are what geocode uses.

Also test:

- **Edit** the same project — all 6 tabs still populated.
- **Clone project** — new project copies setup; accounts handling is as designed (confirm with owner if clone copies Excel rows).
- **Deactivate project** — drops off Active dashboard / allocation (or is clearly inactive).

---

### TC-08 Account list and geocode

Still on the project (Tab 6 / uploaded data).

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Open uploaded accounts | Row count matches Excel |
| 2 | Edit one street/address | Save persists |
| 3 | Run **Calculate pending lat/long** (project UI or ask admin) | LATITUDE / LONGITUDE fill for valid US addresses |
| 4 | Bad / empty street | Row stays pending or error is visible — **no yellow screen** |

Geocode needs a valid `googleapiKey` in Web.config. If all rows fail, log **config/key** as blocked, not a UI bug.

**SFTP inbound route files are NOT live** (commented in the utility). Do **not** fail the product because yesterday’s TNMP CSV did not appear.

---

### TC-09 Inventory (web)

Do this on the **same QA project**.

| Step | Menu | Pass if |
|------|------|---------|
| 1 | Product Master — product exists and is linked on Project services | Product selectable |
| 2 | Inventory Upload — download sample `Sample Inventory Upload File.xlsx`, upload | File accepted; rows in View Upload Records |
| 3 | Inventory Receive | Stock increases |
| 4 | Warehouse Stock report | Qty matches receive |
| 5 | View Stock Request | Page loads (approve a request if installer already requested from mobile) |
| 6 | Inventory Transfer / Warehouse Transfer | Qty moves; Movement Report shows it |

Negative: upload a broken Excel / wrong columns → clear error, not a 500.

---

### TC-10 Installer allocation

**Operations → Installer Allocation**

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Select **QA project** | Cycles / routes / addresses load from uploaded Excel |
| 2 | Pick **QA installer**, From/To date = **today** (or a window that includes today) | Date controls work |
| 3 | Select some unallocated addresses → Save | Success; addresses no longer in unallocated pool |
| 4 | Export allocation Excel if button exists | File downloads |
| 5 | Re-assign same address to another installer in overlapping dates | App blocks or warns (no silent double-book) |

**Operations → Appointment:** create an appointment for one allocated account if the screen is in use. Pass: appointment listed and linked to installer.

---

### TC-11 Mobile — login and download (no phone required)

You can test the **service** from the ASMX page or Postman. The native Android/iOS app is a separate binary; this repo is the **server**.

**UserLogin** (`CMS_MobileService.asmx`)

| Input | Expected JSON |
|-------|----------------|
| Valid installer + today’s date | `status: Success`, `userId`, `roleId`, `UserName`, `processes` |
| Bad password | Fail + message |
| Inactive user | Fail: **User is not active.** |
| Empty userName/password | Fail, missing parameter |

Then, using the returned `userId`:

| Method | Pass if |
|--------|---------|
| `UserProjectData` | QA project is in the list **for today’s date** (allocation window must include today) |
| `OfflineUserDataAll` | JSON includes assigned accounts, form fields, not an empty success with no work |
| `setLastSynchFlag` | Success |

If the installer has **no allocation for today**, empty work is **correct**, not a bug. Allocate today and retry.

---

### TC-12 Mobile — complete a visit (`SaveData`)

Best with the real mobile app pointed at the same URL. If you only have ASMX, you still verify the server accepted JSON.

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Download work (TC-11) | Accounts match allocation |
| 2 | Open one account, fill required fields, capture GPS | Validation blocks submit if required empty |
| 3 | Take required photos | Upload succeeds |
| 4 | Submit | `SaveData` Success; account shows visited |
| 5 | **Office:** Project Status / Installer Report / Export Report | That account is completed (form type 2) |
| 6 | Skip another account with a Reason Master reason | Skip saved; **Skip History Report** shows it |
| 7 | Try to save the same completed account again | Duplicate handled (error or reopen flow) — note actual behavior |

**Photos on disk (local):**  
`GarronT.CMS.UI` upload folder `MobileUploadData\{projectId}\{formId}\{account}\{fieldId}\`  
**Live:** `C:\inetpub\vhosts\datadepotonline.com\httpdocs\MobileUploadData\...`

If `strCloudUrl` is empty after save, that is **allowed** (best-effort S3). Nightly utility backfills (TC-15). Do not fail the visit if the form saved and the file is on disk.

---

### TC-13 Mobile — inventory and time clock

With installer logged into the app (or ASMX):

| Method / screen | Pass if |
|-----------------|---------|
| Receive warehouse / user inventory | Stock on user increases |
| Submit stock request | Appears in web **View Stock Request** |
| Return inventory | Qty returns |
| View running / balance | Numbers match warehouse reports (within known timing) |
| Time clock in / out | **Time Clock Report** in Admin shows the punches |

---

### TC-14 Audit

Need: completed visits (TC-12), customer audit %, **auditor user**, form type 1.

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | **Operations → Audit** | Page loads; settings / allocations visible |
| 2 | Wait for **CMSShedulerService** (every **3 hours**) **or** ask admin to trigger allocate | Some completed visits appear as audit sample. Auditor ≠ installer |
| 3 | Auditor mobile: download audit work, complete audit form | Saves to audit tables; **Auditor Report** shows the visit |
| 4 | Fail % high enough to extra-sample (if failure settings exist) | Extra days / extra sample per customer settings — confirm with owner |

If the Windows service is **not installed on local**, mark TC-14.2 **Blocked (service not running)** — that is environment, not a UI defect.

---

### TC-15 Cloud URL backfill (live server only)

**Do not run this on your laptop.** Files must exist under the live `httpdocs` folder.

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Find a visit attachment with empty `strCloudUrl` and path containing `MobileUploadData` | Row exists |
| 2 | Ops runs `TNMPUploadUtility.exe` on the web server (default project arg **10122** unless told otherwise) | Batch up to 5000; `strCloudUrl` filled |
| 3 | Broken-URL validate | **Off** unless `ValidateExistingCloudUrls=true` — skip |

---

### TC-16 Dashboard and reports

Login as Admin. For each report: open → select **QA project** + date range that covers TC-12 → Generate / Export.

| Menu | Pass if |
|------|---------|
| Dashboard | Active vs Completed, Electric/Gas/Water filters; non-admin sees **only permitted projects** |
| Project Status | QA visits reflected |
| Time Clock Report | Punches from TC-13 |
| Duplicate Report | Loads; flags duplicates if you created one |
| Export Report | Completed visits export |
| Installer Report | QA installer + completed counts |
| Auditor Report | Audit visits from TC-14 |
| Skip History | Skipped account from TC-12 |
| Budget Status | Loads; running balance calculated (not stored per period) |
| Warehouse Stock / Inventory Stock / Movement / Transfers / INV Installer | Match inventory actions from TC-09 / TC-13 |

Export **email/SFTP jobs are commented** — do not expect nightly CSV on SFTP or Beaumont WITH_METER / WITHOUT_METER mail.

---

### TC-17 Alerts

**Operations → Alert Master**

| Step | Pass if |
|------|---------|
| Create alert / email config if used | Saves |
| Low inventory / missing inventory (if stock below threshold) | Alert or mail path does not 500 |

Mail needs `WebMasterMail*` in Web.config. No mail server = blocked, not a functional fail.

---

### TC-18 User Master extras (I-card, vehicle, hierarchy)

**Administration → User Master** (edit the QA installer).

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Profile / installer photo upload + remove | Image shows; remove clears it |
| 2 | Vehicle make / model / vehicle photo | Saves on user; no 500 |
| 3 | Manager + Field Supervisor (if roles exist) | Dropdowns filter by role/project; save sticks |
| 4 | Form Type assignment (field vs audit) | Installer gets type 2; auditor type 1; mobile `processes` matches |
| 5 | City logo message (customer + state + city) | Get/Save logo message works |
| 6 | Employee I-card screen (`EmployeeCard`) | Card generates / prints for the user |
| 7 | Change password from Manage/Account if the menu exists | New password works on next login |

---

### TC-19 Inventory extras (config, transfers, reports)

| Step | Menu / action | Pass if |
|------|----------------|---------|
| 1 | Inventory Request Configuration | Rules save; mobile request respects them |
| 2 | Inventory Bulk Transfer (Inventory Reports → Inventory Transfer) | Qty leaves source, arrives dest; Movement Report updates |
| 3 | Warehouse Transfer | Same for warehouse-to-warehouse |
| 4 | Inventory View — open an uploaded batch | Line details match Excel |
| 5 | Approve / reject stock request from web | Installer running inventory matches |
| 6 | INV Installer Report | Per-installer stock after receive/return |
| 7 | Low stock / missing inventory | Alert or `SendMailForMissingInventory` does not 500 |

---

### TC-20 Appointments (full)

**Operations → Appointment** (`AppointmentAllocation` + `Appointment`).

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Open appointment list | Grid loads; project/installer filters work |
| 2 | Create appointment: QA project, account, preferred dates, installer | Saved in `Appointments` |
| 3 | Allocate / link installer | `tblInstallerMapData.AppointmentFlag` / allocation shows the appointment |
| 4 | Edit dates | Persist |
| 5 | Installer mobile download for that date | Account present |
| 6 | Cancel / complete if the UI has status | List reflects status; no orphan allocation |

---

### TC-21 Dashboard charts (every widget)

**Dashboard.** Set Project Status + Utility Type + a known project. Each hidden URL in the page must return data, not an error page.

| Widget / action | Pass if |
|-----------------|---------|
| Project Status + Utility Type + Project dropdown | Options load; non-admin sees only permitted projects |
| Complete vs pending chart | Loads |
| Installations per month | Loads |
| Performance | Loads |
| Project installation | Loads |
| Installation per installer | Loads |
| Project completion % | Loads |
| Default / current active project details | Loads |
| Installation vs audit chart | Loads |
| Budget dashboard (`GetBudgetDashboardData`) if shown | Filters by date; no timeout yellow screen |

---

### TC-22 Off-menu screens and stubs

Type URLs while logged in as Admin (sidebar may hide them).

| Screen | How | Pass if |
|--------|-----|---------|
| Device | `/Admin/Device` | List Add/Edit/Deactivate; no 500 |
| Country / State / City | `/Admin/Country`, `/Admin/State`, `/Admin/City` | CRUD; Project step 1 dropdowns use them |
| Role Master | `/Admin/Role` (menu commented) | Loads or Unauthorised — note actual; do not fail “missing menu” |
| Installer dashboard | `/Installer/InstallerDashboard` | Stub page, no crash |
| SuperAdmin dashboard | `/SuperAdmin/SuperAdminDashboard` | Stub page, no crash |
| Help | Help link / `CMSHelpManual/quick/toc.html` | TOC opens |

---

### TC-23 Mobile ASMX — remaining field/sync methods

From `CMS_MobileService.asmx` (JSON). Use installer `userId` from `UserLogin`.

| Method | Pass if |
|--------|---------|
| `getServerDate` | Server datetime JSON |
| `checkDate` | Accepts current date; rejects nonsense |
| `UserNotificationData` | Success (empty list is OK) |
| `GetAccountAssignedStatus` | Assigned vs not for a known Excel row |
| `GetProjectUserList` | Users on the QA project |
| `get_AccountSummaryReport` | Counts match office Project Status (approx.) |
| `UpdateMediaFlag` | After photo upload, pending media flag clears |
| `GetAccountAssignedStatus` after SaveData | Account no longer “open” / shows visited |

---

### TC-24 Mobile ASMX — every inventory method

Walk the phone inventory screens **or** call each method. Qty must match **Warehouse Stock** / **INV Installer Report**.

| Method | Pass if |
|--------|---------|
| `GetWarehouseProductList` / `GetProductListForProject` | Products for QA project/warehouse |
| `ValidateInventoryData` | Bad serial/qty rejected |
| `AddNewProductSaveData` | New product path saves or validates |
| `ReceiveInventorySourceInfo` then `ReceiveWarehouseInventory` | Warehouse inward |
| `ReceiveUserInventory` | User stock increases |
| `RequestNewInventorySourceInfo` → `SubmitNewInventoryRequest` | Web **View Stock Request** shows it |
| `ViewNewInventoryRequest` / `ViewNewInventoryRequestSourceData` / `ViewDetailedRequest` / `UpdateNewRequest` | Request visible; update saves |
| `ViewRunningInventory` / `ViewBalanceDetailInventory` | Matches receive minus consume/return |
| `ReturnInventorySourceData` → `ReturnInventorySubmitData` | Qty returns |
| `ReceiveNotInInventoryToInventory` | Exception receive works or clear error |
| `UserSignOffRequest` | Sign-off row created |
| `ViewReconcileDetailInventory` | Reconcile view loads |
| `InventoryAutoSynch` / `InventoryAutoSynchSetFlag` | Sync flag updates |
| `SendMailForMissingInventory` | Success or mail-config blocked (not 500) |
| `ClearInventory` | Only if owner allows on **QA** data — confirm before running |

---

### TC-25 Web API (login, media, geo, audit)

| Call | Pass if |
|------|---------|
| `POST /api/login` valid JSON | Auth success / cookie or token as designed |
| `POST /api/login` bad password | Fail, no 500 |
| `UploadServiceImageFiles` / `Video` / `Audio` | File stored under `MobileUploadData`; response OK |
| `UploadCustomImageFiles` / `Audio` / `Video` | Same |
| `UploadSkipImageFiles` / `Audio` / `Video` | Same for skip |
| `UploadCustomImageFilesFromWeb` / `UploadServiceImageFilesFromWeb` | Office upload from browser works |
| `UploadAuditCustomImageFilesFromWeb` / `UploadAuditServiceImageFilesFromWeb` | Audit photos store |
| `GetLatLongForProject` / `CalPendingLatLongForProject` | Pending addresses get lat/long |
| `AllocateAuditVisit(InstallerId)` | Allocations created or “none to allocate”; auditor ≠ installer |

Local upload API setting: Web.config `webApiUploadCustomeImage` (often `https://localhost:6069/api/UploadFile/UploadCustomImageFiles`). If that port is down, mark media API **Blocked (config)**.

---

### TC-26 CMSAuditThread (mobile sync drop)

| Step | Do this | Pass if |
|------|---------|---------|
| 1 | Note `UploadedExcelData.SyncEnable` on old allocated rows | Some are 1 |
| 2 | Run scheduled `CMSAuditThread.exe` (`setMobileSyncStatus` / `PROC_MobileSync_GetAfterTop`) | Older “top” rows become `SyncEnable = 0` |
| 3 | Installer `OfflineUserDataAll` | Dropped rows no longer download |

If the console is not scheduled on this env, **Blocked (job not running)**.

---

### Screen-open checklist (every sidebar item)

Log in as Admin. Open **each** item. Page must render (grid or form), no yellow ASP.NET error.

- [ ] Dashboard  
- [ ] Project Status  
- [ ] City Logo Master  
- [ ] Form Type Master  
- [ ] Meter Make Master  
- [ ] Meter Type Master  
- [ ] Meter Size Master  
- [ ] Picture Master  
- [ ] Reason Master  
- [ ] Service Master  
- [ ] User Master  
- [ ] User Role Rights (SysAdmin only)  
- [ ] Warehouse Master  
- [ ] Category Master  
- [ ] Product Master  
- [ ] Inventory Upload  
- [ ] Inventory Receive  
- [ ] Inventory View  
- [ ] View Stock Request  
- [ ] Alert Master  
- [ ] Audit  
- [ ] Customer  
- [ ] Project  
- [ ] Installer Allocation  
- [ ] Appointment  
- [ ] Form Management  
- [ ] Time Clock Report  
- [ ] Duplicate Report  
- [ ] Export Report  
- [ ] Installer Report  
- [ ] Auditor Report  
- [ ] Skip History Report  
- [ ] Budget Status Report  
- [ ] Warehouse Stock  
- [ ] Inventory Stock  
- [ ] Movement Report  
- [ ] Inventory Transfer  
- [ ] Warehouse Transfer  
- [ ] INV Installer Report  
- [ ] Help  

Plus off-menu: Device, Country, State, City, Employee card.

---

## 5. Negative and regression checks (quick)

| # | Try | Pass if |
|---|-----|---------|
| N1 | SQL injection / script in login or project name | Rejected or encoded; no stack trace to user |
| N2 | Open `/Admin/Users` while logged out | Login page |
| N3 | Installer web user opens User Role Rights | Unauthorised (SysAdmin only in sidebar) |
| N4 | Upload 0-byte or .exe as Excel | Rejected |
| N5 | Project step 6 with unmapped required columns | Clear mapping error |
| N6 | Allocate with To date before From date | Validation |
| N7 | Two browsers, same admin, deactivate the installer in one | Mobile login fails as inactive |
| N8 | Direct URL `http://localhost:6070/Error/Index` | Error page, not crash loop |
| N9 | Idle ~30+ minutes, then click a menu or AJAX dropdown | Login page, or AJAX **401** (app may rewrite to HTTP **306**) — not a yellow screen |
| N10 | Two office users allocate the **same** address at the same time | One wins; no duplicate active `tblInstallerMapData` without warning |
| N11 | Mobile `currentDate` far in the past/future (Unix seconds) | `checkDate` / empty `UserProjectData` — not a 500 |
| N12 | Huge photo / video (tens of MB) | Saves or clear size error. `maxRequestLength` is very large; still check timeout (`executionTimeout` 1600s) |
| N13 | `POST /api/login` when `tblUsers.UserName` ≠ Identity username | Known risk: API signs in with `UserName` string; web login uses `aspnet_UserId`. Log actual result |
| N14 | Chosen dropdown empty after filter | Wait for AJAX; if session expired, 401/306 — retry login before logging a “blank dropdown” bug |

---

## 6. What you should NOT treat as a bug

These are **off** in code today (`TNMPUploadUtility` `Program.Main` and similar):

| Feature | Status |
|---------|--------|
| SFTP inbound TNMP route CSV (10124 / 10087) | Commented |
| Export CSV → SFTP | Commented |
| Email export project **10134** WITH_METER / WITHOUT_METER | Commented |
| Skip account auto-move to unallocated | Commented |
| Hangfire as the scheduler | Package only; not the live scheduler |
| Validate existing cloud URLs | Flag default **false** |
| ForgotPassword / ChangePassword on **ASMX** | Commented |

Hardcoded utility project ids (do not invent test data for these unless you are on that utility): **10087, 10122, 10124, 10134**.

---

## 7. How testers actually hit mobile / API

ASMX methods are **JSON `ScriptMethod`**. The built-in ASMX test form may not work for all methods. Use Postman or the phone app.

**UserLogin example** (body as form or JSON depending on how you call ASMX):

- `userName` = installer login id  
- `password` = usually `admin@123`  
- `currentDate` = **Unix time in seconds** (not a calendar string). Example: [https://www.unixtimestamp.com](https://www.unixtimestamp.com) → copy seconds.

`POST /api/login`:

```http
POST /api/login
Content-Type: application/json

{ "UserName": "qa.installer.YYYYMMDD", "Password": "admin@123", "RememberMe": false }
```

Pass: `{ success: true, userId, userName }`. Fail: `{ success: false, message: "Invalid username or password" }` or `"User not active or not found"`.

---

## 8. Local vs live (do not mix expectations)

| Topic | Local | Live |
|-------|--------|------|
| URL | http://localhost:6070/ | https://www.datadepotonline.com/ |
| Database | `CMSMeterLive` | `CMSMeter` |
| Photos | `MobileUploadData\` under the web project | `C:\inetpub\vhosts\datadepotonline.com\httpdocs\MobileUploadData\` |
| S3 backfill (TC-15) | Will **not** find live files on your PC | Run `TNMPUploadUtility` **on the web server** |
| Audit job (TC-14) | Only if Windows service is installed locally | Should be running as a service |
| Upload API port | Web.config may point at `https://localhost:6069/...` | Live upload API / same site |
| Dummy projects | Preferred | Only with owner approval |

---

## 9. If a test is Blocked (not a product bug)

| Symptom | First check |
|---------|-------------|
| All geocode rows empty | `googleapiKey`; network to Google |
| Forgot Password / inventory mail never arrives | SMTP keys; junk folder; mark **Blocked (mail)** |
| Mobile project list empty | Allocation **From/To includes today**; user is on the project (step 2); `currentDate` Unix is **today** |
| Photos save, no cloud URL | Allowed; TC-15 backfill. File must exist on **disk** |
| Audit never appears | Service not running, or audit % = 0, or no completed visits |
| Yellow ASP.NET error | Screenshot + URL + time → Blocker. Note `~/Error/Index` |
| Menu missing | RBAC, not a broken page — use a SysAdmin |
| Dropdown blank | Chosen + AJAX; session 401; wait for load |
| Model / site will not start | SqlServerTypes DLLs missing (section 12) |

Optional SQL (if you have read-only DB) to confirm a visit:

- Account uploaded: `UploadedExcelData` / `tblUploadedData`  
- Assigned: `tblInstallerMapData` (`Active = 1`, dates cover today)  
- Visit saved: `TblProjectFieldDatas` + `tblProjectFieldDataMasters`  
- Photo: `TblProjectFieldDataAttachments` (`strFilePath`, `strCloudUrl`)  
- User: `tblUsers.Active`, `UserName`

---

## 10. How to log a defect

Include all of this or the ticket will bounce:

1. **Environment:** Local / Live + URL  
2. **User + role** (never paste production passwords)  
3. **Menu path** (e.g. Operations → Project → Create → step 6)  
4. **Steps** numbered  
5. **Expected vs actual**  
6. **Screenshot** + browser + time  
7. If API/mobile: method name, request snippet (no secrets), response  
8. Test project name / id  

Severity guide: cannot login / cannot save visit = **Blocker**. Report wrong numbers = **Major**. Cosmetic = **Minor**.

**Evidence:** screenshot of the **full browser** (URL visible), filter values, project name, and for mobile the JSON response (passwords stripped).

---

## 11. Suggested test order (one tester, ~4 days for entire project)

| Day | Scope |
|-----|--------|
| **1** | Smoke S1–S15 + screen-open checklist + TC-01, TC-02, TC-03, TC-22 |
| **2** | TC-04, TC-18, TC-05, TC-06, TC-07, TC-08, TC-20, TC-21 |
| **3** | TC-09, TC-19, TC-10, TC-16 reports (empty/partial data OK) |
| **4** | TC-11, TC-23, TC-12, TC-25 media, TC-13, TC-24, TC-14, TC-26, TC-16 again, negatives. TC-15 only on live web server. |

Minimum ship bar: smoke + TC-01–TC-16. Entire-project sign-off: **all matrix rows ticked**, including TC-18–TC-26, screen-open checklist, and the sign-off box below.

### Sign-off (paste into the test report)

```
Environment: Local / Live
Build / version: v1.0015
Tester:
Date:
Admin user used:
QA project name / id:
Installer / auditor logins:

Smoke S1–S15:          Pass / Fail
Screen-open checklist:  Pass / Fail
TC-01 … TC-17 (core):   Pass / Fail / Blocked
TC-18 … TC-26 (full):   Pass / Fail / Blocked
Negatives N1–N14:       Pass / Fail
Open blockers:
Open majors:
Sign-off: Ready / Not ready
```

**Still not in this guide (on purpose):** Selenium/unit automation, load/performance, and the native phone APK source (test the APK against the ASMX URL; APK is not in this repo).

---

## 12. Local build (only if you test on IIS Express)

Testers who run Visual Studio:

1. Restore NuGet (`packages.config` → `packages\`).
2. `GarronT.CMS.UI\Web.config` → SQL Server catalog **CMSMeterLive**.
3. If Model will not compile, copy SqlServerTypes native DLLs from `packages\Microsoft.SqlServer.Types.*\nativeBinaries\` into `GarronT.CMS.Model\SqlServerTypes\{x64,x86}` and the same under UI.
4. Startup project **GarronT.CMS.UI**, F5 → http://localhost:6070/
5. Login: http://localhost:6070/Global/Login

Do not commit passwords, SFTP credentials, or the file-upload API token.

---

## 13. Solution map (if a test is blocked)

| Project | Tester cares because |
|---------|----------------------|
| **GarronT.CMS.UI** | All web screens, `/api/login`, `CMS_MobileService.asmx` |
| **GarronT.CMS.Model** | Database; if this is down, every screen fails |
| **CMSShedulerService** | Audit sampling every 3 hours |
| **CMSAuditThread** | Old mobile rows drop (`SyncEnable = 0`) |
| **TNMPUploadUtility** | Live S3 backfill only (`ProcessCloudUrls`) |

Config: `GarronT.CMS.UI\Web.config` (site), `TNMPUploadUtility\App.config` (utility).
