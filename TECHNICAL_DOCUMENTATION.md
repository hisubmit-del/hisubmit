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
