# HiSubmit Technical Documentation

This is the technical reference for the active HiSubmit codebase. Read this
document before changing code, and read `PROJECT_MEMORY.md` afterwards for
recent fixes, decisions, test results, and known issues.

The product assessment and prioritized roadmap are maintained in
`PRODUCT_ASSESSMENT.md`. It records the current product strengths, risks,
opportunity gap for residencies/literary magazines, and the staged delivery
plan. Read it before starting a cross-cutting UI or workflow change.

## 1. Project identity and source of truth

HiSubmit is an English-language online platform for artistic festivals and
competitions. It supports public visitors, artists, festival teams, referees,
administrators, commerce, ticketing, submissions, judging, news, advertising,
chat, notifications, SEO, and static content.

The active source of truth is:

```text
D:\websites\hisubmit
```

The active executable project is:

```text
Web\Web.csproj
```

The solution file is:

```text
HiSubmit.sln
```

The current source branch is `recovery-clean-20260816`. The GitHub remote is
the `hisubmit-del/hisubmit` repository. The latest documented source
checkpoint is `7b62d78`.

Do not use old copies such as `Web-nested-recovery-20260816`,
`Web-origin-main-20260816`, or `Web-stable-inner-copy-20260816` as development
sources. They are recovery references, not active projects.

## 2. Architecture at a glance

The application is an ASP.NET Core .NET 8 application using Razor Components
with Interactive Server rendering. It has a layered structure:

```text
Web
 ├─ UI: Razor pages, layouts, shared components, navigation
 ├─ HTTP: API controllers, middleware, filters, authentication
 ├─ Runtime: Program.cs, SignalR hub, Hangfire, static files
 └─ Client access: managers and services that call the local API

HiSubmit/src
 ├─ Domain              entities, enums, contracts, domain model
 ├─ Application         CQRS/MediatR requests, handlers, DTOs, validators
 ├─ Infrastructure      EF Core, SQL Server, repositories, identity, seeders
 ├─ Infrastructure.Shared mail and shared infrastructure services
 ├─ Client.Infrastructure API managers and client-side service abstractions
 ├─ Client.SharedModels requests, responses, constants, permissions, validators
 └─ Shared              cross-layer shared models and constants
```

### Project responsibilities

| Project | Path | Responsibility |
|---|---|---|
| Web | `Web` | Executable host, Razor UI, controllers, middleware, layouts, API composition |
| Application | `HiSubmit/src/Application` | Use cases, MediatR commands/queries, handlers, application services, DTOs |
| Domain | `HiSubmit/src/Domain` | Entities, enums, domain contracts, auditable base classes |
| Infrastructure | `HiSubmit/src/Infrastructure` | `BlazorHeroContext`, EF Core mappings/migrations, repositories, Identity, storage |
| Infrastructure.Shared | `HiSubmit/src/Infrastructure.Shared` | Shared mail and infrastructure services |
| Client.Infrastructure | `HiSubmit/src/Client.Infrastructure` | HTTP/API managers and client service implementations |
| Client.SharedModels | `HiSubmit/src/Client.SharedModels` | Requests, responses, wrappers, permissions, constants, validators |
| Shared | `HiSubmit/src/Shared` | Models and constants shared by several layers |

The project targets `net8.0`. MudBlazor is the main UI component library.
MediatR is the application request pipeline. Entity Framework Core with SQL
Server is the persistence layer. SignalR handles live notifications and chat.
Hangfire handles background and recurring jobs.

## 3. Where to start a change

Use this sequence for a normal feature or bug fix:

1. Read `PROJECT_MEMORY.md` and this document.
2. Reproduce the problem locally and inspect `Web\Logs`.
3. Find the UI route under `Web\Components\Pages`.
4. Find the manager/service called by that page under `Web` or
   `HiSubmit/src/Client.Infrastructure`.
5. Find the controller endpoint under `Web\Controllers`.
6. Find the request, handler, validator, and response under
   `HiSubmit/src/Application` or `HiSubmit/src/Client.SharedModels`.
7. Find the entity, EF mapping, or repository under
   `HiSubmit/src/Domain` and `HiSubmit/src/Infrastructure`.
8. Check authorization at both UI and API levels.
9. Change the smallest correct layer or set of layers.
10. Build, run the affected workflow, update the memory checkpoint, commit,
    and push.

Do not fix a server-side data or authorization problem only with CSS or a UI
guard. Do not fix a UI-only problem by changing database data.

## 4. Runtime composition

The application startup is `Web\Program.cs`. It registers:

- CORS and localization.
- SQL Server through `AddDatabase`.
- Identity and cookie authentication.
- Authorization and dynamic permission policies.
- MediatR application handlers and AutoMapper.
- Repositories, application services, upload/storage services, mail, PDF,
  QR-code generation, Excel services, and payment services.
- Hangfire and its SQL Server storage.
- SignalR.
- Razor Components with Interactive Server rendering.
- API controllers, Razor Pages, static files, antiforgery, and identity
  endpoints.

The request pipeline includes:

```text
CORS
 → exception handling
 → HTTPS redirection
 → ErrorHandlerMiddleware
 → static files and /Files
 → localization
 → routing
 → authentication
 → authorization
 → CheckLogoutUser
 → Hangfire dashboard
 → controllers, Razor Components, SignalR, Identity endpoints
```

The SignalR hub is:

```text
/signalRHub
```

The Hangfire dashboard is:

```text
/jobs
```

Access to Hangfire should be treated as administrator-only even though the
dashboard configuration also controls read-only behavior.

## 5. Configuration and environments

Configuration files:

```text
Web\appsettings.json
Web\appsettings.Development.json
Web\appsettings.Production.template.json
Web\appsettings.Production.json   # host/local secret file; do not commit
```

Development uses the local SQL Server database `HiSubmitDB50` through
Windows authentication in `appsettings.Development.json`.

Production uses the real SQL Server database configured by the hosting
installation. The production connection string and passwords must not be
written into this documentation, a template, a commit, or a GitHub issue.
On the Windows host, keep the real `appsettings.Production.json` in the
application directory and set `ASPNETCORE_ENVIRONMENT=Production` as required
by the hosting panel.

Important configuration keys:

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection |
| `AppConfiguration:Secret` | application secret |
| `SiteURLConfiguration:BaseUrl` | generated links and site URLs |
| `MailConfiguration` | SMTP sender and mail server |
| `PayPal` | payment provider configuration |
| `Serilog` | file and console logging |

Never copy production secrets into local test fixtures. When a configuration
problem is suspected, verify the active environment and the effective
configuration file before changing code.

## 6. Database and persistence

The EF Core context is:

```text
HiSubmit/src/Infrastructure/Contexts/BlazorHeroContext.cs
```

It uses the `hisubmi1_user` default schema for application tables and maps
Identity tables under the `Identity` schema. The context applies mappings
from `HiSubmit/src/Infrastructure/Configurations`.

### Main data groups

The context includes entities for:

- Identity: users, roles, role claims, user roles, tokens, profile data.
- Festivals: festivals, masters, organizers, files, images, addresses, venues,
  halls, deadlines, event categories, art categories, focuses, qualifying
  awards, submission questions, festival users and roles.
- Projects: projects, files, images, credits, people, awards, distribution,
  media rights, and type-specific film, music, script, photography and VR/XR
  specifications.
- Submissions: submissions, deadline/category fees, answers, statuses,
  selection and judging data.
- Judging: judging forms, fields, buttons, project judging assignments and
  referee results.
- Commerce: carts, cart items, products, product images, sold products,
  discount codes, festival payment items, commissions and payment information.
- Tickets: tickets, venues/show halls, sold tickets and entry badges.
- Content: news, comments, static pages, FAQs, menu items and SEO metadata.
- Communication: chat rooms, chat messages, chat history and notifications.
- Advertising: advertising requests, banners and attached files.
- Audit and extended attributes.

### Migrations and seeders

EF migrations are under:

```text
HiSubmit/src/Infrastructure/Migrations
```

The application calls the registered `IDatabaseSeeder` services during startup
through `app.Initialize(...)`. The seeder creates or updates baseline roles,
permissions, catalog values, commissions, the default administrator, a basic
artist account, and referee roles where needed.

The current `Program.cs` does not contain an explicit
`Database.Migrate()` call. Therefore, do not assume that a new migration is
automatically applied merely because the application starts. Before applying a
schema migration:

1. Back up the target database.
2. Inspect the migration and model snapshot.
3. Apply it deliberately using the approved SQL/EF deployment process.
4. Test the affected workflow against a restored copy first.

Local-only showcase data is inserted by scripts under `tools`, especially:

```text
tools\seed-qa-showcase-local.sql
tools\seed-demo-local.sql
```

These scripts must not be run against production without an explicit,
reviewed migration/data plan.

## 7. Identity, roles, permissions, and context switching

Role constants are defined in:

```text
HiSubmit/src/Client.SharedModels/Constant/Role/RoleConstants.cs
```

The main roles are:

| Role | Meaning |
|---|---|
| `Administrator` | site-wide administrator |
| `Artist` | artist/submitter workspace |
| `FestivalId` | festival owner/manager context |
| `Referee` | referee/judging context |

A single email can have several roles. Festival-scoped role records carry a
`FestivalId`, so a user can have different permissions for different
festivals or judging seasons.

Claims are created by:

```text
Web/Handlers/CustomClaimsPrincipalFactory.cs
```

Important claims include:

- `ClaimTypes.Role`: global role.
- `FestivalId`: active festival identifier.
- `FestivalRole`: festival-scoped role.
- `FestivalPermission`: JSON permissions keyed by festival ID.
- `Permission`: global permission.

Selected-festival storage keys are defined in
`Client.SharedModels/Constant/Storage/StorageConstants.cs`, including
`selected-festival-id` and `admin-selected-festival-id`.

Permission names are defined in:

```text
HiSubmit/src/Client.SharedModels/Constant/Permission/Permissions.cs
```

Dynamic permission policy code is under:

```text
Web/Permission
```

Festival-specific authorization is additionally checked by
`Web/Filters/FestivalAuthentication.cs` and the `BaseFestivalController`
route convention. When changing a festival workflow, verify:

1. The page visibility rule.
2. The manager/API call.
3. The controller authorization.
4. The selected festival ID.
5. The festival permission claim.
6. Cross-festival denial.

Role and permission changes can invalidate active sessions through SignalR.
Do not bypass these checks by trusting a festival ID sent by the browser.

Account context switching is implemented with the existing cookies:

- `SelectedFestivalId=0` explicitly selects the personal/artist workspace.
- A positive `SelectedFestivalId` selects a festival only after the
  `/Account/Manage/select-account` endpoint validates it against the user's
  main `FestivalId` claim or `FestivalPermission` claim.
- An absent selection cookie means the user's normal default: the active
  festival workspace for a festival owner, or the personal workspace for an
  artist-only account.
- Administrators use the separate
  `/Account/Manage/admin-login-to-festival` flow, which is restricted to the
  administrator role.

When changing account switching, test both the visible dashboard/menu and the
server-side authorization. A festival account must not become an artist
dashboard merely because the user selected the personal context, and a
browser-supplied festival ID outside the user's claims must return `403`.

## 8. UI and route organization

The route host is:

```text
Web/Components/Routes.razor
```

The default layout is `MainLayout`. Public, festival and not-found layouts
are selected by individual pages or the router.

### Layouts

| Layout | Responsibility |
|---|---|
| `Web/Components/Shared/MainLayout.razor` | authenticated/general UI, account controls, navigation |
| `Web/Components/Shared/PublicMainLayout.razor` | public header, public body and footer |
| `Web/Components/Shared/FestivalMainLayout.razor` | festival workspace |
| `Web/Components/Shared/NotFoundLayout.razor` | not-found page |
| `Web/Components/Pages/Admin/_AdminLayout.razor` | administrator wrapper over `MainLayout` |
| `Web/Components/Account/Shared/AccountLayout.razor` | Identity account pages |

Navigation menus are under:

```text
Web/Components/Shared/NavMenues
Web/Components/Shared/NavMenu.razor
Web/Components/Shared/MainNavMenu.razor
```

The shared contextual help component is:

```text
Web/Components/Shared/Components/PageUsageGuide.razor
```

It is mounted by the main layouts and maps routes to concise user guidance.
Required MudBlazor markers are styled globally in:

```text
Web/wwwroot/css/site-modern.css
```

When creating a new operational page, add its route guidance and use the
existing layout/component patterns before inventing a new page-level design.

### Main page families

Public:

```text
/                         festival browser/home
/festivals                festival browser
/festival/{FestivalUrl}  festival detail
/project/{ProjectUrl}     public work detail
/news                     news index
/new/{NewId}              news article
/store                    public store
/Product/{Id}/{Title}     product detail
/tickets                  public tickets
/faq                      FAQ
/advertise                advertising request
```

Artist/user:

```text
/user/dashboard
/user/projects
/user/project
/User/Submits
/user/ShoppingCart
/user/tickets
/user/purchasedProducts
/Chat
/user/notifications
```

Festival:

```text
/festival/dashboard
/festival/edit
/festival/submits
/festival/judging
/festival/judgingResult/{SubmitId}
/festival/products
/festival/products/add
/festival/tickets
/festival/venues
/festival/news
/festival/new
/festival/question
/festival/subuser/*
/festival/payment-information
/festival/discount-codes
/festival/Chat
```

Referee:

```text
/Judges
/User/Judges/Dashboard
/JudgingDetail/{Id}
```

Administrator:

```text
/admin/dashboard
/identity/users
/identity/roles
/identity/role-permissions/{Id}
/admin/festivals
/admin/news
/admin/faq
/admin/advertises
/admin/banners
/admin/income/payments
/admin/festival/products
/admin/festival/tickets
/audit-trails
```

The full route inventory is always discoverable with:

```powershell
rg -n "^@page " .\Web\Components\Pages .\Web\Components\Account -g "*.razor"
```

## 9. API organization

Base controllers are:

```text
Web/Controllers/BaseApiController.cs
Web/Controllers/BaseFestivalController.cs
Web/Controllers/BaseAdminController.cs
```

The normal API route pattern is:

```text
api/v{version}/[controller]/...
```

Public API pattern:

```text
api/v{version}/public/[controller]/...
```

Administrator API pattern:

```text
api/v{version}/admin/[controller]/...
```

Representative endpoint families:

| Area | Source |
|---|---|
| Public festivals, deadlines, reviews, likes, products, news | `Controllers/v1/Public/FestivalController.cs` |
| Public content, FAQ, comments, likes | `Controllers/v1/Public/ContentController.cs`, `CommentController.cs` |
| Project CRUD, media, specifications, credits, awards | `Controllers/v1/Project` |
| Submissions and reviews | `ProjectSubmittedController.cs`, `Controllers/v1/Festival/FestivalSubmitsController.cs` |
| Cart and payment flow | `Controllers/v1/Payments/CartController.cs` |
| Tickets and sold tickets | `Controllers/v1/Public/TicketController.cs`, `Controllers/v1/Festival/TicketController.cs` |
| Referees and judging | `Controllers/v1/Referees`, `Controllers/v1/Festival/Judging*` |
| Festival setup | `Controllers/v1/Festival/FestivalController.cs` and related controllers |
| Admin content, users, payments, advertising | `Controllers/v1/Admin` and `Controllers/Identity` |

Most responses use the shared wrapper with `Succeeded`, `Data`, and
`Messages`. A browser message such as `An unexpected error occurred while
processing your request` is only the outer symptom. Inspect the server log,
the controller result, and the Application handler before changing the UI.

## 10. Application request flow

The common request path is:

```text
Razor page
 → Client.Infrastructure manager/service
 → HttpClient with cookie/interceptor
 → API controller
 → MediatR command/query
 → Application handler
 → repository/EF Core context
 → Result<T> response
 → UI state update
```

For a new use case, prefer the existing CQRS structure:

```text
HiSubmit/src/Application/Features/<Area>/<Commands|Queries>/<UseCase>
```

Put request/response contracts in the shared model locations used by the
neighboring feature. Add validation in the corresponding
`Client.SharedModels/Validators` location when validation is shared with the
client.

## 11. Core business workflows

### Artist submission

1. Artist creates a project.
2. Project base information and type-specific specification are completed.
3. Images, files, credits, awards and distribution information are added.
4. Artist selects a festival category/deadline.
5. Festival questions are answered.
6. Fees are added to the cart.
7. Payment or zero-payment completion creates/updates the submission.
8. Festival staff and authorized referees process the submission.

Supported project types include film, photography, music, script/screenwriting,
VR/XR, and visual art. Public project detail components select media viewers
according to the stored file type.

### Festival management

Festival users create or edit festival details, contacts, organizers, images,
venues, deadlines, categories, fees, questions, rules, awards, products,
tickets, news, SEO, team users, permissions, payments, judging settings and
submission decisions.

All festival actions must remain scoped to the selected festival and the
user's permissions for that festival.

### Judging

Festival managers define judging fields and buttons, assign referees to the
appropriate festival/season and project, and review submitted scores/results.
Referees must only see assigned and authorized projects.

### Commerce and tickets

Products, tickets, badges and submission fees use the cart/payment path.
Before changing price types, inspect both SQL column types and CLR properties;
the project previously had a SQL `float` to CLR `int` materialization failure
in deadline fee data.

### Content, comments, likes and advertising

Festival news is managed in the festival workspace and may be surfaced in the
public news area according to the existing publication rules. Comments, likes,
ratings and advertising requests each have their own API/controller paths and
authorization or anti-abuse rules. Do not assume that a visible button means
the corresponding server operation is authorized.

## 12. Logging and diagnostics

Serilog writes rolling files under:

```text
Web/Logs
```

Typical files are `logYYYYMMDD.txt` and older checkpoint logs. Runtime-created
`Logs` and `Files` directories must be writable by the application identity.

When diagnosing an HTTP 500:

1. Record the exact URL, method, query/body, user role, and selected festival.
2. Reproduce once locally.
3. Inspect the newest `Web/Logs/log*.txt` entries by timestamp.
4. Search for the endpoint name, exception type, inner exception, SQL column,
   and user/festival ID.
5. Trace the endpoint to its handler and database query.
6. Retest anonymous, artist, festival, referee, and administrator paths as
   appropriate.
7. Add the result and timestamp to `PROJECT_MEMORY.md`.

Do not expose production credentials, tokens, personal data, or full sensitive
request payloads in commits or issue reports.

## 13. Local development

From the repository root:

```powershell
cd D:\websites\hisubmit
dotnet build .\Web\Web.csproj --no-restore
dotnet run --project .\Web\Web.csproj --launch-profile http
```

Open:

```text
http://127.0.0.1:5120
```

The repository helper script builds and runs the Development environment:

```powershell
.\run.ps1 -Profile http
```

If assemblies are locked by a previous `Web` process:

```powershell
Get-Process Web -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
dotnet run --project .\Web\Web.csproj --launch-profile http --no-build
```

For package restoration when NuGet is reachable or packages are already
cached:

```powershell
.\restore.ps1
```

The project has a large legacy warning backlog. Treat warnings separately from
errors, and only fix warnings after checking package compatibility and runtime
impact.

## 14. Testing checklist

At minimum, after a change:

- `git diff --check`
- build `Web\Web.csproj`
- open `/`, `/news`, `/store`, `/tickets`, `/faq`
- open the affected authenticated page with the correct role
- test both allowed and denied role/festival access
- inspect logs for new exceptions
- test mobile navigation if layout or responsive CSS changed
- test upload, form submit, modal close/cancel and navigation when relevant
- verify no unrelated files are staged

`RequestCultureMiddleware` treats query-string and `Accept-Language` values
as untrusted input. Wildcards, quality suffixes, unsupported cultures, and
malformed values must fall back to the configured application culture and must
never produce a request-level HTTP 500.

For database or payment changes, also test:

- a clean local database or restored test database,
- an existing record,
- empty and populated lists,
- anonymous and authenticated requests,
- decimal/float/int boundaries,
- failed and successful payment paths.

## 15. Publish and deployment

Source code and publish output are separate:

```powershell
dotnet publish .\Web\Web.csproj --configuration Release --output .\artifacts\publish
```

The source branch contains the project files. The `production-publish` branch
is intended for compiled publish output consumed by Plesk. The host-side
production configuration file must remain in the application directory.

Recommended release sequence:

1. Build and test the source branch locally.
2. Commit and push source changes.
3. Create a clean Release publish.
4. Replace the publish branch contents with the new publish output.
5. Commit and push the publish branch.
6. In Plesk, select the exact branch containing the new publish commit.
7. Use manual `Deploy now`.
8. Restart the Windows application pool/site if required.
9. Check the site, logs, database connectivity and one authenticated workflow.

Plesk may continue tracking an older date-named publish branch. Always verify
the selected branch before Pull/Deploy. Do not delete the host's writable
`Files`, `Logs`, or production configuration data while replacing binaries.

## 16. Git and change-management rules

Every coherent change must have its own commit. Use descriptive messages:

```powershell
git status
git diff --check
git add <only-intended-files>
git commit -m "Short description of the change"
git push origin recovery-clean-20260816
```

Do not stage:

- `bin` or `obj` build output,
- local logs,
- local profile pictures or uploaded test files,
- production secrets,
- unrelated changes made during manual QA.

Before a risky change, create a recoverable commit or tag. Never use
`git reset --hard` or broad checkout commands to discard work without explicit
approval.

## 17. Documentation maintenance

`TECHNICAL_DOCUMENTATION.md` describes the stable architecture and working
procedures. Update it when a structural rule, route family, deployment method,
security model, database strategy, or developer workflow changes.

`PROJECT_MEMORY.md` is the chronological project memory. Add a dated checkpoint
after each meaningful investigation or implementation containing:

- the problem or goal,
- files/components changed,
- root cause,
- behavior now verified,
- build/test result,
- remaining risks or follow-up work,
- commit hash when available.

Keep secrets and user passwords out of both documents. If the two documents
disagree, verify the source code and configuration first, then update both
documents in the same commit.

## 18. Product/UI checkpoint

The current product assessment and roadmap are in
`PRODUCT_ASSESSMENT.md`. The first Phase 1 implementation added dashboard
context improvements, mobile-safe spacing, and active-filter/reset feedback to
public festival discovery. It intentionally did not change database schema,
payment calculations, or authorization behavior.

## 19. Account context and festival-scoped authorization

The selected-account cookie is only a workspace selector:

- `SelectedFestivalId=0` means the personal workspace.
- A positive value means a selected festival workspace.
- The server accepts a positive value only when it is present in the
  authenticated user's server-issued festival claims.
- The cookie must never be used as proof that a user owns or can manage a
  festival.

For production-scale use, keep the cookie small and non-sensitive. Claims may
help render navigation quickly, but commands and sensitive queries must
recheck the current user, the festival membership/owner record, and the
required scoped permission in the Application layer. Membership removal,
role changes, and security-stamp refresh must invalidate stale access.

The festival sub-user model is:

```text
Festival
  └── FestivalSubUser (festival membership, role/referee marker, removed flag)
        └── ProjectJudging (specific submission assignment for a referee)
```

Referee access is assignment-scoped: a referee may see and score only active
assignments belonging to that referee, and only while the assignment is in the
active status. Festival sub-user screens must exclude removed memberships and
must reactivate an existing removed membership rather than insert duplicates.

Remaining security audit items:

- add a shared Application-layer festival-access service for all commands and
  queries that accept a festival ID from a route or request;
- validate removal of a project referee assignment against its festival and
  current festival manager;
- refresh/revalidate scoped claims after a membership is removed;
- verify that admin “login to festival” is always distinguished from a normal
  festival account.

## 20. Build-warning policy and current baseline

Warnings are tracked by category and are not suppressed globally:

| Category | Meaning | Policy |
|---|---|---|
| `CS8618` / nullable warnings | legacy DTOs and Razor-generated state | fix by component/feature group with tests |
| `MUD0002` | old MudBlazor attributes/components | migrate in small UI batches |
| `CS0618` | obsolete MudBlazor/API calls | migrate dialog/form call sites incrementally |
| `CS4014` | background email, notification, PDF, or job work | do not blindly await; preserve intended delivery semantics |
| `NU1902` / `NU1903` | vulnerable package versions | upgrade one package family at a time and test payment/email/mapping |
| `CA1416` | Windows-only QR implementation | document hosting requirement or replace with a cross-platform encoder |

The latest full build baseline on 2026-08-18 is:

```text
dotnet build .\Web\Web.csproj --no-restore --disable-build-servers --nologo
Build succeeded
0 errors
1819 warnings
```

The warning count must be measured from a full build, not an incremental build
that may omit unchanged projects. Each warning-reduction commit must record
the before/after count and the affected category.

## 21. Runtime log workflow

The application writes runtime errors under `Web/Logs`. For every reported
HTTP 500 or frozen action:

1. stop the current local `Web` process before rebuilding;
2. reproduce the exact route/action once;
3. inspect the newest log file by timestamp;
4. identify the exception, endpoint, user scope, and festival scope;
5. trace the endpoint through controller → Application handler → repository;
6. retest anonymous, artist, festival manager, referee, sub-user, and admin
   paths as applicable;
7. record the result in `PROJECT_MEMORY.md`.

Never commit local logs, uploaded QA files, profile pictures, credentials, or
production database settings.

## 22. Authentication and project-detail access contract

### Authentication entry point

The canonical login route is `/Account/Login`. It is statically rendered so
ASP.NET Identity can append the authentication cookie before the response is
committed. The legacy `/login` route is retained only as a compatibility
alias and force-loads `/Account/Login`; it must not contain a second
interactive `PasswordSignInAsync` implementation.

Links and protected-route redirects should target `/Account/Login`. When the
navigation originates from an interactive component, use a full reload so the
static Identity form is rendered in a fresh HTTP response.

### Project-detail visibility

`GetProjectDetailQueryHandler` is the server authority for workflow visibility:

```text
Public viewer
  -> public project fields and media only

Project owner / artist
  -> public fields
  -> registrations for their own project

Active referee
  -> public fields
  -> active ProjectJudging assignments for that referee

Festival owner or active festival sub-user
  -> public fields
  -> rows belonging to festivals they manage

Administrator
  -> authorized workflow rows for the requested project
```

The project URL and project ID are lookup inputs only. The handler resolves
the project and its Submit/ProjectJudging relationships from the database,
checks the current authenticated user, and filters the response. A selected
festival cookie or browser-supplied festival ID is never proof of access.

The response contract exposes `CanViewFestivalRegistrations`,
`FestivalRegistrations`, `CanViewJudgingDetails`, and `JudgingAssignments`.
Unauthorized users receive empty collections. The UI component
`ProjectWorkflowAccessPanel.razor` renders each section only when the server
sets its corresponding flag.

## 23. Login verification and warning-audit policy

The supported local login entry point is `/Account/Login`. The legacy
`/login` route is a compatibility redirect to that static Identity form. The
form uses a native HTML submit button because the account endpoint is
processed before an interactive Blazor circuit exists. After
`PasswordSignInAsync` succeeds, the handler must not append another
authentication or selected-festival cookie; the response may already be
committed. Identity owns the authentication cookie, while a selected-festival
cookie is only a workspace selector. Every protected request and command must
still resolve the current user and verify database-backed role, membership,
festival, and assignment scope.

A local smoke test on 2026-08-19 verified:

- `GET /Account/Login` returned HTTP 200 with a native POST form and
  anti-forgery token.
- A valid local test login returned HTTP 302 and logged `User logged in.`.
- No new response-started exception was produced after removing the duplicate
  cookie append.

The warning audit is intentionally tracked by unique file/line/code
locations, because the build output repeats diagnostics for project
references. The latest audit produced 3,610 parsed warning lines and 1,579
unique locations. The main groups were CS8618 (nullable initialization),
MUD0002 (MudBlazor component parameters), CS0618 (obsolete APIs), CS8602
(nullable dereference), and CS0414 (unused fields). Safe localized fixes have
already removed unnecessary `async` methods in server local storage and the
custom `FileUploader.Url` parameter setter. The remaining groups require
feature-by-feature tests and must not be mass-suppressed.

## 24. Current cross-cutting safeguards (2026-08-19)

The mobile header has two implementations: `PublicHeader.razor` for the
public layout and the responsive bar in `MainLayout.razor` for workspace
pages. Both use explicit flex rows and grouped action elements. The final
responsive rules in `Web/wwwroot/css/site-modern.css` must remain last so
legacy Bootstrap/MudBlazor display utilities cannot re-enable the desktop
header on mobile.

Festival category/deadline data is loaded by
`FestivalCategories.razor.cs` and
`GetAllDeadLineEventCategoryQuery`. The handler must scope every deadline
lookup by `FestivalId`; a browser-supplied ID is input only and never grants
festival access. The component is resilient to empty responses and avoids
duplicate calls for the same festival.

Project viewing is role-scoped in the Application layer. Owners and
administrators may receive private contact information. Festival owners,
festival sub-users, and assigned referees receive only the registrations and
judging records allowed by server-side membership/assignment checks.
`GetAllProjectFilesQuery` independently calls `ICheckPermission`, preventing
a direct file API request from bypassing the project page. A future artist
visibility feature requires an explicit domain field and migration;
`Project.Password` must not be reused because it is associated with file
access.

Checkpoint verification on 2026-08-19: the application built with zero
errors, `/` returned HTTP 200, and
`/api/v1/public/festival/AllDeadlineEventCategory?FestivalId=9...` returned
HTTP 200 with an empty result rather than an unhandled exception. The active
log had no new startup/API exception during this test.

## 25. Responsive CSS and festival-layout checkpoint (2026-08-19)

`Web/wwwroot/css/site-modern.css` contains one final responsive header
contract at the end of the file. Earlier duplicate rules for public header
visibility, sponsored festival tiles, and mobile breakpoints were removed.
When adding responsive rules, extend that contract or place a clearly scoped
component rule before it; do not append another competing global override.

`FestivalMainLayout.razor` uses:

```text
festival-main-appbar
  -> festival-mobile-header-row
  -> festival-desktop-header-row
  -> festival-mobile-drawer (temporary, overlay)
```

The mobile row is visible below the 992px breakpoint and the desktop row is
visible at or above it. `_drawerOpen` is initialized closed. The drawer
reuses `NavMenu` and therefore keeps the existing festival-account navigation
instead of introducing a second permission model.

Project-detail authorization is enforced in the Application layer by
`ICheckPermission` and `GetProjectDetailQueryHandler`:

- the project owner and administrator can read the project;
- festival owners and non-removed festival sub-users can read submissions
  belonging to festivals they manage;
- referees can read only submissions with an active `ProjectJudging`
  assignment for their user;
- the selected festival cookie/claim and a browser-supplied FestivalId are
  selectors only, never authority;
- the project-file query repeats the permission check so a direct file API
  call cannot bypass the project page.

The artist-controlled public visibility modes requested in the product
roadmap are intentionally not implemented yet. They need an explicit
domain/API field and migration; `Project.Password` must not be repurposed.

Verification on 2026-08-19:

```text
dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
  -p:UseSharedCompilation=false --nologo -v:q
Build succeeded
0 errors
7 warnings (package vulnerability advisories)
```

The remaining warnings are package-security advisories for AutoMapper,
System.Linq.Dynamic.Core, MailKit, and MimeKit. They are tracked separately
from the functional/layout checkpoint and should be upgraded with compatibility
testing rather than hidden or mass-suppressed.

## 26. Public header, project specification, and diagnostics (2026-08-19)

The last CSS block in `Web/wwwroot/css/site-modern.css` is the responsive
header contract. It explicitly controls all three header families:

- `MainLayout`: `main-mobile-header-row` versus
  `main-desktop-header-row`;
- `PublicHeader`: public desktop navigation versus the mobile menu button;
- `FestivalMainLayout`: festival mobile row versus desktop toolbar.

If a desktop header disappears, inspect this contract and the CSS viewport
breakpoint before changing Razor markup. Do not add another competing global
override earlier in the stylesheet.

The public header uses role-aware calls to action. `FestivalRole` receives the
festival advertising CTA; `ArtistRole` receives a festival-discovery CTA.
This is a presentation rule only. The `/advertise` endpoint must continue to
enforce authorization server-side.

`PageUsageGuide` intentionally returns no guide for `/project/{ProjectUrl}`
and public festival-detail routes. The detail pages have their own headings,
tabs, deadlines, and action surfaces, so a second global guide would add
visual noise.

The submission-category dialog (`FestivalCategorySelected.razor`) explains
how to compare deadline, fee, eligibility, and form-answer accuracy. It does
not promise that an earlier deadline is always cheaper; the actual fee
returned by the festival remains authoritative.

The project Specification surface is styled by `site-modern.css` as a
responsive card grid. The underlying type-specific components remain the
source of displayed fields, so this change does not alter the project
response contract or database schema.

Diagnostics policy:

- Application runtime logs: `Web/Logs/`, configured by Serilog.
- Local build and investigation logs: `diagnostics/logs-archive/`.
- Root-level generated logs should not be recreated; use the diagnostics
  folder for redirected build/test output.
- Neither location may contain secrets, connection strings, production
  database backups, or user-uploaded files.

Opportunity-type roadmap:

The current domain is Festival-centric. The next model should add an
`OpportunityType` taxonomy rather than duplicating Festival entities:

| Opportunity | Required first-class data |
|---|---|
| Literary competition | manuscript/poetry categories, word/page limits, language, rights, blind-review option |
| Literary magazine | issue/volume, reading period, simultaneous-submission policy, contributor payment, publication rights |
| Residency | duration, dates, location/remote mode, accommodation, stipend, discipline, portfolio/statement questions |
| Grant | funding amount, budget, eligibility, milestones, reporting requirements |
| Exhibition/gallery open call | medium, dimensions, installation, insurance, sale commission, delivery/collection |
| Event pass/accreditation | access scope, validity window, venue/program permissions, quotas, identity verification |

This remains a planning checkpoint. No schema migration is introduced here.

## 28. Workflow and festival-scope audit (2026-08-19)

The implemented relationship is:

`Project` -> many `Submit` rows -> one festival per submission, and each
`Submit` -> many `ProjectJudging` rows -> one referee per assignment.
Therefore one work may enter several festivals and be assigned to several
referees, while every assignment remains tied to its submission's festival.

Verified and reinforced:

- Festival submission list/detail/form-answer endpoints require
  `Permissions.Submits.View`.
- Referee assignment requires `Permissions.Submits.AddToReferee` and validates
  all submission IDs against the requested festival.
- Selected referees must be active, non-removed `FestivalSubUser` records with
  `IsReferee=true` for that festival.
- Project-judging listing requires `Permissions.Judging.View`.
- Ticket list/detail require `Permissions.Ticket.View`; create/update/delete
  require `Permissions.Ticket.Edit`.
- A judging result is rejected after referee membership is removed.
- `FestivalAuthentication` no longer treats a selected festival claim as
  sufficient authority; the policy must be present in the festival permission
  claim. Administrator bypass remains intentional.

Product and content review:

- `Product` is owned by one festival and its update/delete paths check the
  festival boundary.
- `Ticket` is linked through `Venue`; ticket commands validate the venue and
  ticket relationship.
- `New` supports nullable `FestivalId`: festival news is festival-scoped,
  while administrator-created site news can be global.

Build validation after this audit: `Web.csproj` completed with 0 errors.
The existing warning backlog remains tracked separately. No database migration
was introduced.

## 27. Phase 1 regression checkpoint (2026-08-19)

The first regression pass for shared-experience stabilization was completed
against the local Development server after stopping stale `Web` processes and
building the active `Web\Web.csproj`.

Verified:

- The locked-file-safe build completed with 0 errors.
- `/`, `/festivals`, `/news`, `/store`, `/tickets`, `/faq`, and `/advertise`
  returned HTTP 200.
- Guest cart endpoint `/api/v1/cart/GetItems?UserId=` returned HTTP 200 with
  a successful empty result.
- Public deadline endpoint
  `/api/v1/public/festival/AllDeadlineEventCategory?FestivalId=9&TakeCurrentDeadLine=False&SpecfyWithProject=False`
  returned HTTP 200 with a successful empty result.
- The home response contained the public desktop navigation and the expected
  Browse festivals, News, Store, and Tickets links.
- No new runtime exception was produced by these requests.

This is an HTTP/DOM smoke pass, not a replacement for interactive browser
verification. Authenticated role switching, festival-scope allow/deny cases,
form/modal/upload actions, payment, and visual desktop/mobile inspection
remain before Phase 1 can be closed. Phase 2 must not start until those checks
are completed or a specific blocker is recorded.

## Automation and scheduled festival workflows (2026-08-19)

### Legacy next-period job

`HiSubmit/src/Application/Jobs/Daily/Festivals/GoToNextPeriodOfFestival.cs`
registers the daily Hangfire job `GoNextFestivalPeriod`. For an active period
whose `EventEndDate` has passed, it creates the next period, advances date
fields by one year, deactivates the old period, updates the festival master
pointer, and publishes `CreatedFestival`.

This operation is opt-in per festival through
`Festival.EnableAutomaticPeriodCreation`, defaulting to `false`. The control is
exposed in Festival > Additional Settings. The legacy clone behavior should
receive a preview/deep-copy test before production activation.

### Automatic official-selection news

`HiSubmit/src/Application/Jobs/Daily/Festivals/PublishFestivalNotificationNews.cs`
registers `PublishFestivalNotificationNews` as a daily Hangfire job. It
selects public festivals whose `NotificationDate` has arrived and whose
`EnableAutomaticSelectionNews` setting is enabled. It publishes one
festival-scoped `New` item containing the festival description and
submissions with a selection-related judging status. The title includes the
notification date to prevent duplicates without another column. If no selected
works exist, publication is deferred to the next daily run.

The manager can disable this workflow in Festival > Additional Settings. The
job is registered at startup together with the legacy period job; it does not
run database migrations automatically.

### Migration and deployment safety

`20260819142432_AddFestivalAutomationSettings` is a manually constrained
migration that adds only:

- `Festivals.EnableAutomaticPeriodCreation` (`bit`, default `false`)
- `Festivals.EnableAutomaticSelectionNews` (`bit`, default `true`)

It has not been applied by this checkpoint. Back up the target database and
inspect the connection string before applying it. The controlled local command
is:

```powershell
dotnet ef database update `
  --project .\hisubmit\src\Infrastructure\Infrastructure.csproj `
  --startup-project .\Web\Web.csproj
```

Production deployment must include the migration code and apply it through the
approved database process. Copying publish files or deploying a Git commit
alone must not be assumed to update the database.

The current startup initializer now calls `BlazorHeroContext.Database.Migrate()`
before database seeding. This supports a deliberately reset empty database:
the application applies the committed migrations first and then seeds the
required baseline roles/catalog data. A migration failure is logged and
seeding is skipped. This behavior must still be verified on the hosting
provider before treating it as a production database migration policy.

The restored local `HiSubmitDB50` database already contained the fee-type
migration identifier `20250726170647_Change_FeePropTypeInCategoryDeadlineFeeTB`,
while the active source tree only contained the older differently named fee
migration file. A no-op compatibility migration with the recorded identifier
is now present. It records the existing database state without replaying the
fee alteration and prevents EF from attempting to recreate the initial
`Users` table.

The restored database keeps application tables and migration history in the
`hisubmi1_user` schema. The DbContext therefore configures
`MigrationsHistoryTable("__EFMigrationsHistory", "hisubmi1_user")`, and the
automation migration explicitly names that schema. The local database update
completed successfully; both automation columns and all eight expected
migration identifiers were verified.

Phase 1 smoke verification after restart:

- Public routes `/`, `/festivals`, `/news`, `/store`, `/tickets`, `/faq`,
  `/advertise`, `/Account/Login`, `/PrivacyPolicy`, and `/terms`: HTTP 200.
- Guest cart API: HTTP 200 with a successful empty result.
- Public deadline API for FestivalId 9: HTTP 200 with a successful empty
  result.
- Startup log: `Database migrations applied successfully`.

This does not close Phase 1. Authenticated role switching, cross-festival
allow/deny interaction, form/modal/upload actions, payment paths, and visual
browser checks are still unverified.

### Phase 2 submission wizard checkpoint

The current artist project editor already uses five panels: project
information, credits, type-specific specification, awards/distribution, and
project files. The first Phase 2 implementation adds a shared progress
indicator and step guidance without changing the project or submission API.
The editor forms now have stable unique `FormName` values, and a cancelled or
closed unsaved-changes dialog is treated as a no-op.

Build verification after the change completed with 0 errors. This checkpoint
does not claim the complete Phase 2 submission wizard: draft autosave,
validation summary, and interactive end-to-end submission/payment checks are
still required by the roadmap.

The festival editor follows the same pattern with seven panels: event details,
contact/venue, deadlines, categories, files, gallery/cover, and additional
settings. It now exposes shared progress and step guidance, and its event
details form has a stable unique `FormName`. Build verification completed with
0 errors. Explicit draft recovery/autosave and full interactive save/release
verification remain pending.

### Phase 2 checkout and receipt checkpoint

The existing cart is the shared checkout surface for submissions, tickets,
badges, and products. It now has explicit loading, failed-load, and empty
states, reports cart API failures, and only redirects to the payment result
when the payment callback contains a valid reference. The payable amount is
labelled as the checkout total. No payment or receipt API contract was
changed. Build verification completed with 0 errors; provider success/failure,
receipt download, and full interactive checkout remain unverified.

### Phase 2 festival task queue checkpoint

The festival dashboard now includes a "Next steps" panel derived from its
existing festival, category, submission, review, and news counts. It links to
the existing edit, judging, and news routes and displays a clear state when no
immediate task is detected. No API, database, or permission contract was
changed. Build verification completed with 0 errors; interactive completion
and denial checks remain pending.

### Phase 2 referee queue checkpoint

The referee queue now has an explicit empty state, shows the available
festival-scoped assignment context, and explains the confidentiality boundary
to the reviewer. The judging dialog receives and displays the assigned
festival name. The current response DTO does not contain a judging
period/season or category, so those fields were not inferred. Build
verification completed with 0 errors. Interactive save confirmation,
festival-scope denial, autosave and full checkout tests remain pending.

### Commerce audit checkpoint

Products are festival-owned and tickets are linked through festival-owned
venues. Public active tickets now exclude sold-out records
(`AvailableCapacity <= 0`). The current store/ticket UI remains a basic
management form and list; richer storefront, reservation/stock states,
event-pass/accreditation metadata, receipt/checkout improvements, and income
analytics are planned but not yet implemented.

### Phase 1 and Phase 2 verification checkpoint (2026-08-19)

- The shared cart now reloads open and paid items from a clean state after
  initialization, deletion, payment, and zero-total completion; this prevents
  paid rows from being duplicated in the client view.
- Project and festival editor forms now expose validation summaries and stable
  unique form names across the project specifications and festival setup
  panels.
- Festival news editing now includes a local preview state before publishing.
- Referee assignment selection is festival-specific when an artist's project
  has more than one active judging assignment. A successful judging result
  returns an explicit dialog result and the parent refreshes the assignment
  state.
- Festival authorization no longer treats a matching `FestivalId` claim as
  sufficient authority. The permission claim must match both the requested
  festival and the required policy. Claims are built for owned, manager, and
  active referee-member festival scopes.
- Local smoke checks after the latest restart returned HTTP 200 for the public
  pages, login page, privacy/terms pages, guest cart, and public deadline API.
- A clean build of `Web.csproj` completed with 0 errors and 7 package warnings.
  The remaining legacy nullable/MudBlazor analyzer warnings are tracked under
  Phase 6 and were not mixed into this workflow checkpoint.
- The latest application log contains successful startup/migration entries and
  no new exception after the latest restart. Earlier invalid-column entries
  are retained as historical evidence from before the local migration ran.

Phase 1 and Phase 2 implementation checkpoints are complete for the changes
listed above. Full interactive payment-provider, receipt-download, browser
visual, and real referee-submit tests still require a manual local test
session with valid transaction and assignment data; they are not claimed as
automatically verified by this checkpoint.

## Phase 3 financial and judging transparency checkpoint (2026-08-19)

- `DownloadCartFactorCommand` now checks the current user against the cart
  owner, while preserving administrator access through the admin controller.
- Receipt generation now rejects unpaid carts and is read-only: it no longer
  republishes `CartPaidedEvent` when a user downloads an existing receipt.
- `PaidZeroCartCommand` now returns a controlled failure when no open cart
  exists, preventing a null-reference 500 response.
- These changes preserve the existing API and database contracts; no
  migration is required.
- Local build completed with 0 errors. Public route and guest-cart smoke
  checks returned HTTP 200, and the latest restart/migration log contains no
  new exception after the test run.
- Provider-side payment verification remains an explicit pre-existing TODO.
  Immutable transaction/receipt storage, payout reconciliation, and admin/
  festival KPI trend reports remain the next financial work items and are not
  claimed complete by this checkpoint.

## Phase 3 commerce and reporting audit checkpoint (2026-08-19)

- Discount validation is server-side and item-scoped. Festival codes require
  the exact festival and configured cart item type; global administrator codes
  are the only cross-festival case. Multiple submitted codes are not stacked;
  the lowest eligible resulting price is selected per item and payment
  recalculates the total again on the server.
- A zero-price festival submission completes through the existing zero-total
  cart path. Multiple categories for one festival are represented by the
  submission's category associations, while payment and authorization remain
  bound to the exact festival period.
- Cart item DTOs and both receipt templates now include applied discount-code
  names. Receipt line items display `PriceAfterDiscount` when it exists.
- Festival payment state now excludes unpaid product/ticket/submission rows,
  uses discounted submission prices, and exposes a last-30-days income KPI.
  The query filters by the selected `FestivalId`, so previous and next
  periods are not combined.
- The admin site-purchase query now assigns submission income correctly and
  applies the maximum date filter in the correct direction.
- No migration was added. External payment-provider verification remains a
  known pre-existing TODO; financial reports should not be described as
  provider-settled until that integration is implemented.

## Phase 3 settlement and user-finance checkpoint (2026-08-19)

- `GetFestivalPaymentStateQuery` now reports `Income`,
  `AdminPayment`, `SiteCharges`, and `NetSettlementDue`. `SiteCharges`
  currently means paid `CartItemType.ServiceFee` rows linked to submissions
  in the exact festival period. It does not include advertising because the
  existing `AdvertiseRequest` has no festival or billing reference.
- The existing `FestivalPaymentItem` remains a manually recorded payment
  record (`Amount`, `PaidDate`, `TrackNumber`, `Type`). It is visible in the
  existing festival payment-item list, but it is not yet an immutable
  settlement ledger or monthly close.
- The artist dashboard reads the authenticated user's paid carts through the
  existing owner-scoped cart query and presents total paid amount, discount
  savings, checkout count, and spend by item type. No new endpoint or schema
  was required.
- The platform should not introduce Hi Coin as a direct price mutation yet.
  A safe design requires a coin ledger, earn-event idempotency, expiry,
  refund/reversal, per-product eligibility, and audit history before it can
  reduce a service fee or festival charge.
- Recommended settlement sequence: introduce an immutable ledger, link every
  charge/payment/adjustment to exact `FestivalId` and period, add monthly
  statement approval, then add exports and payout/reconciliation states.

## Festival account active-scope correction (2026-08-19)

- `qa.festival@hisubmit.test` has one active owned festival (`15`) and two
  inactive legacy test festival rows (`16`, `18`) in the local database.
- `Web/Handlers/CustomClaimsPrincipalFactory.cs` now filters owned and member
  festivals by `Festival.IsActive` before building festival permission and
  current-festival claims. This prevents an inactive row from becoming the
  selected festival and avoids festival-scope 401 responses caused by stale
  or invalid active IDs.
- Existing inactive rows are retained for audit/test history; no data deletion
  or migration was performed. Users must sign out and sign in again to
  regenerate the authentication cookie claims.

## Settlement ledger and period reporting (2026-08-19)

The existing `FestivalPaymentItem` remains the record of payments made to a
festival. A new additive ledger layer now provides period-specific statements:

- `FestivalSettlementStatement`: exact festival and period, gross income,
  service charges, advertising charges, payments to the festival, net amount,
  approval/payment metadata, and status.
- `SettlementAdjustment`: signed adjustment with mandatory reason and optional
  evidence URL. Confirmed and paid statements cannot receive new adjustments.
- `AdvertisingInvoice`: invoice number, amount, dates, festival, optional
  advertising request, and optional settlement statement link.

New festival-scoped routes are exposed under
`api/v1/FestivalPayments/{festivalId}/SettlementStatements` for listing,
creation, adjustment, status changes, and `export?format=excel|pdf`. The
controller still uses the existing festival authorization pipeline; the
browser-supplied festival ID is a selector and is not sufficient to bypass
festival permissions.

Migration `20260819190000_AddSettlementStatementsAndAdvertisingInvoices` was
applied locally to `HiSubmitDB50`. It is additive and does not alter existing
commerce, submission, ticket, or payment rows. The festival UI route
`/festival/settlements` now exposes period selection, statement creation,
status badges, adjacent-period comparison, adjustment entry,
advertising-invoice totals, confirmation, and Excel/PDF download links. It is
linked from both festival navigation variants. The admin payment API also
accepts `POST /api/v1/admin/payment/AdvertisingInvoices`, validating the
festival and optional exact settlement link before creating an advertising
invoice. Admin reconciliation screens and year-over-year artist finance views
remain separate follow-up work; the current artist dashboard still provides
paid total, savings, count, and category chart.

## Phase 1 critical-flow verification checkpoint (2026-08-19)

The local app was verified on `http://localhost:5120` after a successful
no-restore build. Read-only checks for public festival deadlines, tickets,
products, news, ratings, reviews, and art categories returned controlled HTTP
200 responses. Guest cart loading returned an empty successful result, while
the authenticated QA artist cart also loaded successfully.

Festival settlement endpoints now have an explicit
`FestivalAuthentication` policy. Unauthenticated requests return HTTP 401,
and a festival user requesting another festival is rejected. Ticket commands
validate ticket ID, quantity, show time, commission configuration, and missing
ticket records before accessing related entities.

Cookie authentication now returns HTTP 401/403 for `/api/*` authorization
failures instead of redirecting API clients to the HTML login page.
`ErrorHandlerMiddleware` emits `X-Correlation-ID` and includes the same value
in unexpected-error responses, allowing a failed request to be traced through
server logs.
- Festival wizard and rich-text safety checkpoint (2026-08-20)

  The festival editor route (`/festival/edit`) uses the existing seven
  components and no schema change. Its user-facing order is:

  1. Event details
  2. Contact and venue
  3. Event categories and submission questions
  4. Dates, deadlines and fees
  5. Files
  6. Gallery and cover
  7. Additional settings

  The parent tab navigation and save guards were updated together, including
  the modified-form prompt. Category records are saved by their inline editor;
  the parent does not invent a second category save operation.

  Required-field semantics remain based on the existing validator rather than
  UI assumptions: event name, description and rules are required for this
  section. Logo, email, website, prefix, organizer, notification date,
  categories and cover are validated by the existing release command and are
  presented as release prerequisites.

  `HiSubmit.Application.Services.Text.HtmlTextSanitizer` is applied by the
  festival detail and event-category command handlers. It preserves ordinary
  rich text but removes anchor tags, `href`/`src`, inline event handlers, and
  executable/embed blocks. `Web/wwwroot/js/richtext.js` applies the same
  no-link policy in editors with `EnableLink=false`, including pasted HTML.

  This is a presentation/validation hardening change only; it does not change
  the database model or migration history.

## Festival wizard and draft-owner verification checkpoint (2026-08-19)

The public festival detail page now loads tickets through
`IPublicTicketManager` and the public ticket route. This removes the previous
401 caused by calling the festival-owner ticket route from a public page.

Cookie claims now include festivals owned by the current user even when those
festivals are still drafts. Active festival memberships continue to require
an active, non-removed membership. `ClientAuthenticationManager` also resolves
main, selected, other, and personal account scopes from claims/local storage.
Users must log out and sign in again after deployment to receive regenerated
claims.

Release validation now loads deadlines and venues and requires registration
opening, event start/end, at least one submission deadline, and an offline
venue. Contact validation and editor markers require a valid website and
email. Categories may be created before deadline fee rows; an empty fee list
is handled safely. Organizer creation accepts a missing optional image.

The local end-to-end wizard test used festival owner
`qa.festival.wizard@hisubmit.test` with festival `20`: detail, contact,
category, deadline, organizer, venue, tracking prefix, cover upload, and
release all completed. Release returned the expected successful
`UnderInvestigation` review state. The test used only local `HiSubmitDB50`;
no production data was changed.

### Category and deadline fee sequencing

The wizard keeps categories before deadlines because a fee belongs to the
relationship between a category and a deadline, not to either record alone.
The supported workflow is:

1. Save one or more categories without fee rows.
2. Define deadlines and choose which categories each deadline applies to.
3. Reopen each category and complete the generated fee matrix.
4. Use `0` for a free submission; leave no required fee as `null`.
5. Release only after every category has an applicable deadline and every
   applicable row has a standard fee.

The release command now loads `DeadlineEventCategories` and enforces these
rules server-side. The category editor displays an explanatory alert while
the matrix is incomplete. The local QA festival was rejected with a null fee
and accepted after all three fee fields were set to zero.

### Wizard save behavior and fee table

Several wizard forms previously combined a form submit handler with an
explicit click handler, which could send duplicate requests and destabilize
an Interactive Server circuit. Those duplicate triggers are removed.
Changing a wizard tab or pressing a step Next now saves the active section
once and remains on that section if validation fails.

The active wizard section is autosaved once per minute. The save is
server-side, festival-scoped, protected against overlapping requests, and
reported with a snackbar. This is draft persistence; it does not release or
publish a festival.

The category list now maps and returns its `DeadlineEventCategories`,
including deadline date/name and Standard/Student/Gold fees. The editor
displays each deadline as a readable card with a readiness badge and a
required Standard Fee label. Categories and deadlines remain separate because
the fee matrix is created from their relationship.

### Files & Media wizard step

The festival editor now combines the previous `Files` and `Gallery & Cover`
tabs into one `Files & Media` step. This is a presentation-level composition:
`FestivalFile` still owns the file list and its add/edit/delete modal, while
`FestivalImages` still owns cover/gallery upload state and its upload API.
`FestivalFilesAndMedia` coordinates the two components and exposes one shared
navigation/save bar. The child navigation bars are disabled only when rendered
inside the wrapper.

The wizard therefore has six steps:

1. Event Details
2. Contact And Venue
3. Categories
4. Deadlines
5. Files & Media
6. Additional Setting

No database schema or API contract changed. The wrapper saves only pending
cover/gallery changes; file rows remain persisted by their existing modal.

### Required festival setup and listing URL

The festival detail step requires a logo and at least one event type before a
normal save/next operation succeeds. The release command repeats the event
type check server-side, together with the existing logo and cover checks.

The listing URL is assigned during the first additional-settings save. The
server normalizes it to a lowercase hyphenated slug and resolves collisions
with a numeric suffix. The UI permits editing while the database value is
empty; after the first successful save it disables the field. A later attempt
to change to another festival's URL returns a validation failure. No schema
change or unique-index migration is required for this behavior.

Festival category/focus join updates use `ArtCategoryId` and `FestivalFocusId`
when comparing existing links. The category table displays deadline names and
fee readiness so a saved category does not appear as an empty or unreadable
row.

### Form visual consistency

The modern workspace CSS now scopes readable foreground colors to form cards,
marks required fields in red, reduces checkbox stack spacing, and prevents
category/fee content from being clipped or visually washed out. This is a
presentation-only change and does not alter permissions or data contracts.

### Festival detail update safety and public header contract

`POST /api/v1/festival/{festivalId}/UpdateDetail` accepts edits without a new
logo upload. The application handler must treat `UploadRequest` as optional;
the UI initializes upload request objects after mapping an existing festival,
but the server remains authoritative and null-safe.

The public header uses the same navigation items for desktop and mobile. On
desktop, the header is a bounded flex row with a shrinkable brand, an
overflow-contained navigation region, and non-shrinkable account actions. At
narrow desktop widths, navigation scrolls within its region instead of
covering the profile/account controls. The mobile drawer continues to render
the same primary links and account actions.

### Global UI rhythm and readability

The final block of `Web/wwwroot/css/site-modern.css` is the global visual
contract for the main, public, festival, and dashboard layouts. It provides
consistent text contrast, internal padding for cards/dialogs/alerts, input
and control spacing, readable line-height, compact checkbox/switch labels,
table cell padding, and safe wrapping for long labels and values.

This block is intentionally at the true end of the stylesheet because the
project contains older page-specific CSS. It is presentation-only and does
not change APIs, permissions, persistence, or business rules.
## Judging scope and result-state checkpoint (2026-08-20)

Judging scope is bound to the exact submission festival period:

```text
Festival period -> Submit.FestivalId -> ProjectJudging -> referee
```

The referee read paths require an active assignment for the current user and
an active, non-removed `FestivalSubUser` with `IsReferee=true` for that same
festival. The next-period background job clones the festival record but does
not clone `ProjectJudging` rows, so prior-period referee access is not carried
into the new period.

The final-result endpoint is authenticated and the Application handler permits
only the administrator, festival/master owner, or active non-referee festival
manager. Referees can submit their assigned judging form but cannot overwrite
the festival's final result. Positive judging outcomes require a paid or
in-consideration submission state. Withdrawal is authenticated and limited to
the project owner or administrator.

Referee assignment accepts explicit submission IDs and optional
`DeadlineEventCategoryIds`. Category links are resolved only through deadlines
belonging to the requested festival, after which the normal submission and
active-referee checks run. This is additive and requires no migration.

`SubmitStatus` is the registration/payment lifecycle, while `JudgingStatus` is
the judging/selection outcome. They are intentionally kept separate and
positive judging outcomes are rejected for unpaid or incomplete registrations.

The current schema has no persisted automatic-selection criteria, rule
version, flag, explanation, or audit-decision model. A festival-configurable
automatic-selection panel therefore remains a documented contract/schema item;
no speculative database change was introduced.

### Payment page readability checkpoint (2026-08-20)

`Web/Components/Pages/User/ShoppingCart/ShoppingCart.razor` keeps the existing
cart and payment flow but presents each festival group as a bounded card. The
card header includes the festival name, submission categories, item count, and
festival subtotal. Submission rows expose the project title, category, and
tracking code where available.

The related rules in `Web/wwwroot/css/site-modern.css` use a fluid table width,
wrapping for long values, a bounded table wrapper, and mobile-friendly column
sizes. This prevents cart content from escaping the viewport without changing
payment amounts, discount validation, or persistence behavior.

Build verification after the change completed with 0 errors. A populated-cart
browser visual check remains dependent on a local authenticated account with
an active cart containing submission/category data; the available referee
session currently exposes an empty cart.

The payment result route `/user/payment-result/{PaymentId}` now acts as a
receipt handoff page. It shows the payment reference, confirms that no further
payment action is required, and links the user to the cart/receipts area and
personal dashboard. This is UI-only and does not invoke payment processing or
publish a payment event.

### PayPal server verification checkpoint (2026-08-20)

The paid-cart command now verifies a PayPal order server-side before marking
the open cart as paid. Verification checks the configured PayPal environment,
completed order status, exact configured currency, exact recalculated cart
amount, returned order ID, and the PayPal custom cart ID. A previously recorded
paid order or payment ID is rejected. Verification failures return a controlled
failure and leave the cart unpaid.

The PayPal API base URL is selected by `PayPal:Environment` (`Sandbox` by
default, `Live` for production), and `PayPal:Currency` defaults to `USD`.
PayPal credentials remain configuration secrets. Zero-total payment has its
separate path and is not sent to PayPal verification.

The referee dashboard and queue now use a correct empty-state condition:
`RatedProject + NotRatedProject == 0`. They also make the active
festival-scoped assignment context explicit in the page guidance and table.
This is a presentation correction; the existing server-side assignment and
authorization checks remain unchanged.

### Local database storage maintenance checkpoint (2026-08-20)

The local SQL Server database was measured before maintenance:

```text
Database allocation: 528 MB
Application data:     72 MB
Transaction log:     456 MB
```

The large footprint was therefore transaction-log allocation, not table data.
`database/maintenance/Optimize-HiSubmitDatabase.sql` was added as a repeatable
maintenance script. It refreshes table statistics, rebuilds or reorganizes
existing indexes when fragmentation and page-count thresholds justify it,
normalizes log growth to fixed 64 MB increments, and shrinks a mostly-unused
log to approximately 128 MB.

The script was executed against the local `HiSubmitDB50` database. The result
was 208 MB allocated (`72 MB` data and `136 MB` log). No application rows,
audit rows, or Hangfire rows were deleted. Production execution still requires
an approved backup and maintenance window.
