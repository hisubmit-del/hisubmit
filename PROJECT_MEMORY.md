# HiSubmit Project Memory

> Technical reference: read [TECHNICAL_DOCUMENTATION.md](TECHNICAL_DOCUMENTATION.md)
> before making changes. This file is the chronological checkpoint and
> investigation memory; the technical document is the stable architecture and
> operating guide.

## Technical documentation checkpoint (2026-08-17)

- Created `TECHNICAL_DOCUMENTATION.md` as the stable technical reference for
  the active HiSubmit solution.
- Documented the real project structure, startup pipeline, configuration,
  database context and migrations, seeders, roles and festival-scoped
  permissions, layouts and route families, API conventions, core workflows,
  diagnostics, local execution, testing, publishing, Plesk deployment, Git
  rules, and documentation maintenance.
- This file remains the chronological memory of investigations and fixes.
  Update both documents together when architecture, deployment, security, or
  development workflow changes.
- Documentation-only change; no application code or database data was
  changed and no build was required for this checkpoint.

## Product assessment checkpoint (2026-08-17)

- Added `PRODUCT_ASSESSMENT.md` with a product evaluation and prioritized
  roadmap covering trust/security, conversion, role dashboards, commerce,
  judging, and the missing first-class residency/literary opportunity model.
- The current implementation remains festival/competition-centric. Residency,
  grant, literary-magazine, exhibition, and portfolio-review support must be
  added through an additive taxonomy and reviewed migration; do not simulate
  these workflows by adding arbitrary flags to `Festival`.
- This checkpoint begins Phase 1 of the roadmap: shared dashboard context,
  public discovery filter clarity, responsive spacing, and safe UI-only
  improvements. No database schema, payment calculation, or authorization
  rule is changed.

> این فایل حافظهٔ کاری پروژه است. قبل از هر بررسی یا تغییر، ابتدا این فایل خوانده شود.
> این سند با بررسی سورس در 2026-08-16 ایجاد شده و باید فقط با تغییرات واقعی همین پروژه به‌روزرسانی شود.

## 1. هویت و هدف پروژه

HiSubmit یک پلتفرم انگلیسی‌زبان ثبت‌نام و مدیریت مسابقه‌ها/جشنواره‌های هنری است. دامنهٔ اصلی آن شامل برگزاری جشنواره، ثبت اثر هنرمند، فروش خدمات و محصولات جشنواره، فروش بلیت و کارت ورود، تخصیص داور و داوری اثر، انتشار محتوای عمومی و مدیریت کاربران است.

بخش‌هایی که در سورس وجود دارند:

- سایت عمومی: معرفی، جشنواره‌ها، جزئیات جشنواره، آثار، اخبار، صفحات ثابت، FAQ، تبلیغات و SEO.
- حساب و هویت: ثبت‌نام، تأیید ایمیل، ورود/خروج، پروفایل، تغییر/بازیابی رمز، نقش‌ها و مجوزها.
- هنرمند/کاربر: پروژه‌ها و آثار، فایل‌ها و تصاویر، مشخصات تخصصی فیلم/موسیقی/عکاسی/فیلمنامه/VR-XR، عوامل، جوایز، ارسال اثر به جشنواره، پاسخ پرسش‌های ثبت‌نام، سبد خرید، خریدها، بلیت‌های خریداری‌شده، اعلان و گفت‌وگو.
- جشنواره: ایجاد و انتشار جشنواره، اطلاعات، برگزارکننده، تصویر/فایل/لوگو، مکان و سالن، دسته‌بندی، تمرکز/اعتبار، زمان‌بندی و مهلت‌ها، قوانین و سؤالات ثبت‌نام، کاربران فرعی و نقش‌های جشنواره، محصولات، تخفیف، پرداخت، اخبار، SEO، بلیت، فروش بلیت، داوری و بررسی/نقد.
- داوری: تعریف فرم داوری، فیلدها و دکمه‌ها، تخصیص داور به پروژه، کنترل مجوز داوری، ثبت نتیجه و نمایش میانگین/وضعیت.
- مدیر سایت: کاربران، نقش‌ها، مجوزها، جشنواره‌ها، کاتالوگ، محصولات، پرداخت و کمیسیون، درآمد، بلیت، اخبار و صفحات ثابت، FAQ، SEO، تبلیغات، تنظیمات، اعلان و چت.
- زیرساخت/جانبی: SQL Server و EF Core، Identity، JWT/کوکی، claims وابسته به جشنواره، MediatR، AutoMapper، SignalR، Hangfire، ایمیل، آپلود فایل، ذخیره‌سازی فایل، تولید PDF، QR Code، Excel، لاگ، audit trail، چندزبانه‌سازی و کامنت.

## 2. منبع حقیقت کد

### راه‌اندازی فعلی

راه‌حل اصلی ریشه `HiSubmit.sln` است و پروژهٔ وب آن `Web/Web.csproj` است. این پروژه مستقیماً به لایه‌های زیر در `hisubmit/src` ارجاع می‌دهد:

| پروژه | مسیر | مسئولیت |
|---|---|---|
| Web | `Web` | ASP.NET Core host فعلی، Razor Components/Blazor Server، API Controllers، middleware، layout و صفحات |
| Application | `hisubmit/src/Application` | use caseها، CQRS/MediatR، command/query، event، DTO/response، سرویس‌های کاربردی |
| Domain | `hisubmit/src/Domain` | موجودیت‌ها، enumها و قراردادهای دامنه |
| Infrastructure | `hisubmit/src/Infrastructure` | DbContext، EF configurations/migrations، repository، identity، payment، storage و سرویس‌های بیرونی |
| Infrastructure.Shared | `hisubmit/src/Infrastructure.Shared` | قراردادها/مدل‌های مشترک زیرساخت |
| Client.Infrastructure | `hisubmit/src/Client.Infrastructure` | managerهای سمت کلاینت برای فراخوانی API و سرویس‌های کلاینت |
| Client.SharedModels | `hisubmit/src/Client.SharedModels` | request/response، permission، constants و مدل‌های قابل اشتراک |
| Shared | `hisubmit/src/Shared` | مدل‌ها/ثابت‌های مشترک پایه |

نقطهٔ شروع فعلی `Web/Program.cs` است. در آن، Identity، دیتابیس، SignalR، Hangfire، Application/Infrastructure، Razor Components، کوکی، localization و endpointها ثبت می‌شوند. migration در مسیر اجرای برنامه اعمال می‌شود و `app.Initialize(...)` seederها را اجرا می‌کند.

### نسخه‌ها و کپی‌ها

- `Web` کد وب فعال راه‌حل ریشه است.
- `hisubmit/src` هستهٔ معماری فعال و منبع منطق دامنه/اپلیکیشن است. نسخهٔ قدیمی `hisubmit/src/Server` و `hisubmit/src/Client` در cleanup مورخ 2026-08-16 حذف شدند؛ `Client.Infrastructure` و `Client.SharedModels` همچنان فعال و مورد نیاز `Web` هستند.
- `ClientComponents`، `Hisubmit.Web`، `Backup` و `deployment` در cleanup مورخ 2026-08-16 حذف شدند؛ نسخهٔ قبل از حذف در `cleanup-recovery-20260816.zip` و tag `pre-cleanup-20260816` نگه داشته شده است.
- `Hisubmit.Web` و `ClientComponents` کپی‌های مستقل/تکراری بودند و در راه‌حل ریشهٔ فعال ارجاع نداشتند.
- `deployment` خروجی build/publish و DLLها بود و طبق تأیید کاربر روی همین سیستم برای production استفاده نمی‌شد.
- `bin` و `obj` خروجی تولیدی‌اند و در تحلیل معماری نادیده گرفته می‌شوند.
- `hisubmit/README.md` عمدتاً README قالب BlazorHero است و مستندات اختصاصی کامل HiSubmit محسوب نمی‌شود.

## 3. مدل حساب، نقش و جابه‌جایی داشبورد

نقش‌های ثابت در `hisubmit/src/Client.SharedModels/Constant/Role/RoleConstants.cs`:

- `Administrator`: مدیر کل سایت.
- `FestivalId`: مدیر/مالک جشنواره؛ جشنوارهٔ فعال در claims از `FestivalMaster.ActiveId` به دست می‌آید.
- `Artist`: نقش هنرمند.
- `Referee`: داور.

نقش‌های وابسته به جشنواره در `BlazorHeroRole` با `FestivalId` ذخیره می‌شوند. یک ایمیل/کاربر می‌تواند هم‌زمان چند نقش Identity و چند نقش جشنواره داشته باشد. ساخت claims در `Web/Handlers/CustomClaimsPrincipalFactory.cs` انجام می‌شود:

- نقش عمومی در `ClaimTypes.Role`.
- شناسهٔ جشنواره در claim نوع `FestivalId`.
- نقش محلی جشنواره در `FestivalRole`.
- مجوزهای محلی جشنواره در `FestivalPermission` به شکل JSON.
- مجوزهای عمومی در claim نوع `Permission`.

منوها و تغییر context:

- `Web/Components/Shared/NavMenu.razor(.cs)` منوی اصلی را انتخاب می‌کند.
- `Web/Components/Shared/NavMenues/AdminNavMenu.razor` منوی مدیر.
- `.../ArtistNavMenu.razor` منوی هنرمند.
- `.../FestivalNavMenu.razor` منوی مدیر جشنواره.
- `.../OtherFestivalNavMenu.razor` انتخاب/تعویض جشنواره و نقش‌های جشنواره.
- `.../RefreenavMenu.razor` منوی داور.
- `Web/Components/Shared/MainLayout.razor(.cs)` context عمومی کاربر و جشنوارهٔ انتخاب‌شده را نگه می‌دارد.
- `Web/Components/Shared/FestivalMainLayout.razor(.cs)` layout داشبورد جشنواره است.

مجوزها با policy/claim در `Web/Permission`، `hisubmit/src/Client.SharedModels/Constant/Permission/Permissions.cs` و controllerهای مربوط کنترل می‌شوند. تغییر مجوز نقش از `Identity/RolePermissions.razor` انجام می‌شود و SignalR می‌تواند session کاربران دارای آن نقش را خارج کند.

## 4. نقشهٔ صفحات و قابلیت‌ها

مسیر پایهٔ صفحات فعال: `Web/Components/Pages`.

| مسیر سورس | قابلیت |
|---|---|
| `Public` | صفحات عمومی، خانه/فهرست جشنواره، جزئیات جشنواره، اخبار، صفحات ثابت، FAQ، تبلیغ، محصول و نمایش اثر |
| `Public/Festivals` | `/festivals`، `/festivalpage/{FestivalUrl}`، اجزای معرفی جشنواره و `/tickets/{FestivalId}` |
| `Public/Projects` | `/project/{ProjectUrl}`، جزئیات اثر، فایل/تصویر/مشخصات و `RateProject.razor` برای داوری مجاز |
| `Authentication` و `Account` | ثبت‌نام، تأیید ایمیل، ورود و مدیریت حساب |
| `User` | dashboard کاربر، submissionها، چت، اعلان، محصولات خریداری‌شده، cart، بلیت و dashboard داور |
| `Project` | ایجاد/ویرایش پروژه، فهرست پروژه، فایل‌ها، تصاویر، عوامل، جوایز و مشخصات فنی |
| `Festival` | dashboard و ویرایش جشنواره، deadline/category/venue/organizer/file/image، sub-user و role، محصولات، پرداخت، خبر، SEO، review، ticket و sold-ticket |
| `Festival/Judging` | ساخت فرم داوری و فیلدهای آن |
| `Festival/JudgingProjects` و `ProjectJudgings` | تخصیص داور، دیدن جزئیات submission و ثبت نتیجهٔ داوری |
| `Festival/Submits` و `SubmissionQuestion` | ثبت‌نام اثر، دسته‌بندی مهلت، پرسش‌های فرم و نتیجهٔ نهایی |
| `Admin` | مدیریت سایت، کاتالوگ، جشنواره‌ها، اخبار/صفحات/FAQ/SEO، تبلیغات، محصولات، پرداخت، درآمد، بلیت، اعلان و چت |
| `Identity` | کاربران، نقش‌ها، user-role، role permissions و profile |
| `Utilities` | audit trail و قابلیت‌های کمکی |

## 5. API و مرزهای سرور

ریشهٔ APIها `Web/Controllers` است. ساختار قدیمی `hisubmit/src/Server/Controllers` در cleanup حذف شد.

- `Controllers/Identity`: حساب، token، user، role و role claims.
- `Controllers/Communication`: چت.
- `Controllers/Utilities`: audit، document، document type، extended attributes و preferences.
- `Controllers/v1/Admin`: dashboard مدیر، جشنوارهٔ master، اخبار، صفحات، FAQ/SEO، تبلیغات، محصولات، پرداخت، بلیت، review و chat.
- `Controllers/v1/Catalog`: art category، product، festival focus/qualifier، media right، monetary unit و sub-project type.
- `Controllers/v1/Festival`: جشنواره، فایل/venue/category/deadline، sub-user، submit، judging، project judging، news، payment، product sold، discount، ticket، sold ticket و SEO.
- `Controllers/v1/Project`: project، specification و submitted project.
- `Controllers/v1/Public`: محتوای عمومی، جشنواره، اثر، خبر، کامنت، ایمیل، تبلیغ، ticket و SEO.
- `Controllers/v1/Payments`: cart و پرداخت.
- `Controllers/v1/Referees`: dashboard داور، submissionهای قابل داوری و نتیجه.
- `Controllers/v1/Ticket`: فروش ticket/badge.
- `Controllers/v1/Users`: حساب کاربر و product sold.
- `Hubs/SignalRHub.cs`: ارتباط real-time برای پیام، اعلان، تغییر dashboard و خروج session.

## 6. مدل دامنه و دیتابیس

DbContext اصلی `hisubmit/src/Infrastructure/Contexts/BlazorHeroContext.cs` است. entityهای مهم:

- جشنواره: `Domain/Entities/Festivals` شامل Festival، FestivalMaster، FestivalSubUser، EventCategory، Deadline، Venue، ShowHall/ShowTime، تصاویر/فایل‌ها، focus/qualifier، review، submission question و judging.
- اثر/ثبت‌نام: `Domain/Entities/Projects` و `Domain/Entities/Submitter` شامل Project، ProjectFile/Image/Credit، مشخصات فیلم/موسیقی/عکاسی/فیلمنامه/VR-XR، Award، Distribution/MediaRight و Submit/SubmitAnswer/SubmitDeadlineCategory.
- داوری: `Judging` و `ProjectJudging` همراه JudgingField/Button و answer.
- پرداخت: `Domain/Entities/Payments` شامل Cart، CartItem، DiscountCode، ProductSold، FestivalPaymentInformation/Item، SiteCommission و UserSpecialPeriod.
- ticket: `Domain/Entities/Festivals/Tickets` شامل Ticket و SoldTicket؛ `SealedTickets` در DbContext فروش کارت/بلیت ثبت‌شده را نگه می‌دارد.
- کاتالوگ: Product، ProductImage، ArtCategory، FestivalFocus، FestivalQualifying و SubProjectType.
- محتوا: New، StaticPageAndFAQ، MenuItem، Comment، MetaTag.
- ارتباط/اعلان: ChatRoom، ChatMessage، ChatHistory و Notification.
- مدیریت فایل/مکان: AttachFile، Document، DocumentType، ExtendedAttribute، Country و Address.
- تبلیغ: AdvertiseBanner و AdvertiseRequest.
- audit: `AuditableContext` و `AuditTrails`.

migration اصلی فعلی در `hisubmit/src/Infrastructure/Migrations/20250704103511_Init_Db.cs` است. دیتابیس پشتیبان `hisubmi1_MainDb.bak` وجود دارد؛ قبل از restore/تغییر schema باید از وضعیت فعلی دیتابیس و connection string اطمینان گرفته شود.

## 7. رویدادها، پرداخت و اعلان

رویدادهای MediatR در `hisubmit/src/Application/Events` پیاده‌سازی شده‌اند، از جمله:

- ایجاد/انتشار/تغییر مهلت جشنواره و افزودن کاربر جشنواره.
- ثبت کاربر در جشنواره و ثبت submission.
- پرداخت cart و حذف آیتم cart.
- فروش محصول، ticket و badge.
- افزودن داور به project و ثبت نتیجهٔ داوری.
- افزودن تبلیغ، پیام chat و اعلان.

پرداخت در `Application/Services/PaymentService`، `Infrastructure/Services/PaypalService.cs` و featureهای `Application/Features/Payments` است. email templateها در `Web/Views/Emails` هستند و شامل welcome، تأیید، cart، ticket، submission، داوری، اعلان تاریخ و رویداد جشنواره‌اند.

## 8. فایل، رسانه و خروجی

- فایل‌های runtime آپلودشده: `Web/Files` با زیرشاخه‌های Festival، Advertise، Images/Products و ... .
- سرویس فایل: `hisubmit/src/Infrastructure/Services/Storage` و `UploadService`.
- فایل‌های ثابت و UI: `Web/wwwroot` شامل CSS/JS، فونت، لوگو، media، تصاویر qualifying و assetهای ticket/payment.
- PDF: `Web/Services/PdfGenerator.cs`، `Web/Services/PdfGeneratorSetUp` و `Web/Pages/PdfTemplate.cshtml`.
- QR: `QrCodeGenerator` در registration سرویس‌های Web.
- Excel: `Infrastructure/Services/ExcelService.cs` و queryهای `Export`.
- SEO: `Components/Shared/SeoTags`، featureهای `Application/Features/Seo` و controllerهای SEO.

## 9. قواعد کاری برای بررسی‌های بعدی

1. ابتدا همین فایل و `git status` خوانده شود؛ تغییرات موجود کاربر/محیط نباید overwrite شوند.
2. تحلیل سورس فقط با حذف `bin`، `obj` و فایل‌های تولیدی انجام شود.
3. مسیر پیش‌فرض تغییرات اجرایی: `Web` + لایه‌های باقی‌ماندهٔ `hisubmit/src`.
4. برای قابلیت جدید، زنجیرهٔ کامل بررسی شود: صفحه/کامپوننت ← manager/client model ← controller ← Application feature ← Domain entity ← Infrastructure/DbContext.
5. تغییر role/permission باید هم claims (`CustomClaimsPrincipalFactory`) و هم policy/permission و هم منو/layout را بررسی کند.
6. تغییرات schema باید entity، configuration، migration و seed را با هم بررسی کند؛ migration بدون تأیید وضعیت دیتابیس اجرا نشود.
7. پس از هر تغییر واقعی توسط Codex، بخش مربوط به این فایل نیز به‌روزرسانی شود؛ این فایل مرجع حافظهٔ پروژه است، نه جایگزین تست.
8. وجود قابلیت در نام فایل به معنی سالم/فعال بودن آن نیست؛ برای ادعای «فعال» بودن، مسیر اجرا و اتصال لایه‌ها نیز بررسی شود.

## 10. مواردی که هنوز باید در بررسی اجرایی تأیید شوند

- کدام‌یک از دو میزبان `Web` یا هر build منتشرشده دقیقاً روی production اجرا می‌شود.
- وضعیت واقعی connection string، schema و داده‌های production؛ از روی سورس به‌تنهایی قابل اثبات نیست.
- فعال بودن واقعی همهٔ درگاه‌ها، ارسال ایمیل، jobهای Hangfire و endpointهای public در محیط production.
- `.vs` و `.idea` عمداً حفظ شده‌اند و کد اجرایی نیستند.
- برای بازگردانی cleanup از `CLEANUP_PLAN.md`، tag `pre-cleanup-20260816` و archive `cleanup-recovery-20260816.zip` استفاده شود.

## 11. اجرای محلی فعلی

- اجرای `dotnet run` از ریشهٔ repository معتبر نیست، چون ریشه project اجرایی ندارد.
- اجرای مستقیم باید از `Web` انجام شود یا از اسکریپت ریشهٔ `run.ps1`.
- `run.ps1` ابتدا `Web` را با `--no-restore` build می‌کند و سپس از پوشهٔ `Web` اجرا می‌کند تا `appsettings.Development.json` به‌درستی خوانده شود.
- اجرای محلی فعلی به SQL Server پیش‌فرض (`.`) و دیتابیس `HiSubmitDB50` نیاز دارد.
- در 2026-08-16 دیتابیس `HiSubmitDB50` روی SQL Server محلی وجود نداشت؛ backup `hisubmi1_MainDb.bak` برای restore بررسی شده اما restore آن هنوز انجام نشده است.
## Stable local build/run notes (2026-08-16)

- `restore.ps1` is the repeatable restore entry point. It uses the local
  NuGet cache, offline certificate-revocation mode, failed-source tolerance,
  and disabled package-audit network calls.
- `global.json` targets SDK `8.0.100`, rolls forward to the latest major SDK,
  and disables prerelease SDK selection. The application projects target
  `net8.0`.
- Verification: `restore.ps1` completed successfully and
  `dotnet build HiSubmit.sln --no-restore` completed with 0 errors. The
  remaining build warnings are MediatR 11/12 dependency-constraint warnings.
- `Web/Components/Pages/Public/Festivals/Home.razor` now handles a missing
  festival API response without dereferencing a null response object.
- The application reaches startup when launched from the `Web` content root.
  The local SQL Server database `HiSubmitDB50` was restored from
  `hisubmi1_MainDb.bak` on 2026-08-16. The database is ONLINE and the
  application returned HTTP 200 for `/` during a clean startup test on port
  5121.

## Runtime bug investigation notes (2026-08-16)

- The repeated `Http Response Error` responses were initially caused by the
  restored database storing application tables in schema `hisubmi1_user`,
  while EF Core generated unqualified table names that resolved against
  `dbo` under Windows authentication. This produced errors such as
  `Invalid object name 'ArtCategories'` and `Invalid object name
  'FestivalFoci'`.
- `BlazorHeroContext.OnModelCreating` now explicitly sets
  `builder.HasDefaultSchema("hisubmi1_user")`; Identity tables remain
  explicitly mapped to the `Identity` schema.
- After rebuilding, `/api/v1/public/festival/AllArtCategory` returned HTTP
  200 from the local application, confirming that the schema mismatch for
  public catalog queries is fixed.
- The existing database contains Administrator-role users, but their
  plaintext passwords cannot be recovered from Identity password hashes.
- A local administrator test account was created and login-tested:
  email `local.admin@hisubmit.test`; password is recorded in the handoff
  response for the project owner. It is active, email-confirmed, and has
  both `Artist` and `Administrator` roles.

## Error-log investigation checkpoint (2026-08-16)

- Automatic Serilog file logging is enabled by `Web/appsettings.json` and
  writes rolling files under `Web/Logs`; the active files observed were
  `log20260816.txt`, `log20260816_001.txt`, and `log20260816_002.txt`.
- Checked through the latest observed startup entries at approximately
  `2026-08-16 06:57:24 +03:30`.
- The reported `GET /api/v1/Referee/GetAll?...PageSize=10&PageNumber=1`
  errors occurred at `06:46:57` and the related repeated requests at
  `06:47:36–06:47:58`. Their root exception was SQL Server error 208:
  `Invalid object name 'ProjectJudgings'`, caused by the old local schema
  mismatch.
- The same old run also logged missing unqualified tables including
  `Festivals`, `Notifications`, `Carts`, `UserSpecialPeriods`, `MenuItems`,
  and `MetaTags`.
- After `HasDefaultSchema("hisubmi1_user")` was built and the application was
  restarted on ports 5121–5123 at `06:55–06:57`, no newer
  `Referee/GetAll` error was found in the log files.
- The production configuration points to the remote database
  `hisubmi1_MainDb` on `31.25.90.195\MSSQLSERVER2022`, but its password is
  intentionally a placeholder (`__SET_VIA_PLESK_ENV__`). A remote read-only
  connectivity check did not complete, so downloading the real database
  requires valid credentials or a `.bak` export from the production server.

## Production database access checkpoint (2026-08-16)

- The owner supplied valid SQL authentication credentials during this
  session. They were used only in process memory/command execution and were
  not written to source files, project memory, or configuration.
- Read-only connection to
  `31.25.90.195\MSSQLSERVER2022` succeeded with certificate trust enabled
  for that connection. The server reported SQL Server `16.0.1000.6`, and
  `hisubmi1_MainDb` was confirmed `ONLINE`.
- The production database has the same three schema groups observed locally:
  `HangFire`, `hisubmi1_user`, and `Identity`.
- A full download has not yet been performed. SQL Server backup output is
  created on the remote server filesystem; completing transfer requires a
  remote `.bak` file exposed through a file-transfer/share mechanism, or a
  separately approved schema/data export workflow.

## Public bug-fix checkpoint (2026-08-16)

- The log entry at approximately `07:43:15` identified
  `GET /api/v1/public/festival/AllDeadlineEventCategory` as an AutoMapper
  projection failure: the domain stores `GoldFee`, `StudentFee`, and
  `StandardFee` as `double?`, while the command DTO used `int?`. All three
  DTO properties and the related fee editor controls now use `double?`.
- After rebuilding and restarting the application, the same endpoint returned
  HTTP 200 with a successful JSON result on `2026-08-16 08:02` (festival 9
  currently returned an empty data array rather than an exception).
- The public footer had a broken festival route (`/festival/...`) and now
  points to the actual `/festivalpage/...` route.
- The FAQ page now uses valid HTML button markup, supports Enter/search-icon
  searches, and selects the first matching answer in each category after a
  search. The previous `lass` paragraph typo was corrected.
- `/terms` now has an explicit route and a safe built-in fallback when the
  database has no enabled Terms static-page record; unrelated unknown static
  URLs still go to `/notFound`.
- The “View festival” hover animation no longer waits 500 ms/2.5 s and keeps
  its centered position during hover instead of jumping because of the global
  MudButton hover transform.
- Production secrets remain out of source control. `DEPLOYMENT.md` documents
  the `ConnectionStrings__DefaultConnection` environment setting, and
  `Web/appsettings.Production.template.json` is a non-secret template.
- Added `.github/workflows/dotnet.yml` for GitHub Actions restore/build on
  pushes and pull requests to `main`/`master`.

## Product-improvement backlog from competitor review

- Add festival trust signals: organizer verification, venue/identity evidence,
  public reviews, report-festival flow, and visible moderation status.
- Add configurable custom submission forms and custom judging forms with
  per-category criteria, weighted scores, private notes, flags, bulk
  assignment, exports, and audit history.
- Add entrant workflow improvements: reusable artist/project profiles,
  incomplete-submission checklist, saved drafts, protected screeners,
  clear submission/deadline status, waivers/discount codes, and automated
  acceptance/rejection messages.
- Add festival operations improvements: season/edition archiving, real-time
  analytics, payout reconciliation, press-kit management, and sub-user
  permissions.

## Release-candidate checkpoint (2026-08-16)

- The build failure reported by the owner was reproduced from the attached
  output. The cause was a running local `Web.exe` holding the generated DLLs,
  not a compiler error. After stopping that process,
  `dotnet build Web\Web.csproj --no-restore -p:RunAnalyzers=false` succeeded.
- A Release publish was created under the ignored `artifacts\publish-validation`
  directory and run from its own content root with `ASPNETCORE_ENVIRONMENT=Development`.
  The published output returned HTTP 200 for `/`, `/faq`, `/terms`, and
  `AllDeadlineEventCategory`.
- Visual-only styling was added to the public festival browser: a branded
  hero panel, glass search surface, card-based filter area, consistent festival
  logo cards, and responsive spacing. No business logic was changed by these
  visual styles.
- The production password was not embedded in tracked `appsettings.json`
  because that would expose a live database credential to GitHub and every
  published copy. The project remains ready for a host-side secure setting;
  explicit owner approval is required before any plaintext credential is
  written to a tracked file.

## Git and deployment policy (2026-08-16)

- Every code or configuration change must be committed with a clear message
  before the next change is started, so the project can be reverted safely.
- The current tested branch is `recovery-clean-20260816`; the latest source
  commits are `3442602` (demo seed) and `0cd4b83` (festival news filtering).
- The Plesk Git repository is connected to
  `https://github.com/hisubmit-del/hisubmit.git`, currently tracking
  `recovery-clean-20260816`, with deployment to `\httpdocs` set to Manual by
  the owner. The owner manually uploads the Release publish output and the
  host-local `appsettings.Production.json`.
- Plesk currently deploys repository files directly. It must not be treated as
  a replacement for `dotnet publish` until a Windows deployment action is
  configured and verified.
- `Web/appsettings.Production.json` is intentionally ignored by Git and must
  remain host-local. `Web/appsettings.Production.template.json` is the safe
  source-controlled template.

## Plesk compiled deployment branch (2026-08-16)

- Source branch: `recovery-clean-20260816`.
- Compiled deployment branch: `production-publish`.
- The compiled branch contains only a successful Release publish output.
- Plesk should track `production-publish` and use Manual `Deploy now`.
- The real `appsettings.Production.json` must remain host-local in
  `httpdocs`; it must never be committed to the compiled branch.
- After the news-filter fix, the Release output was rebuilt and force-updated
  on GitHub as `production-publish` commit `fd68069`. The actual production
  settings file was excluded; Plesk must retain its host-local copy.
- Because Plesk continued to show its cached old history after Pull, a new
  deployment branch `production-publish-20260816` was created from the
  compatible publish history at commit `a4a99ec`. Plesk should use this new
  branch if the old repository clone still does not refresh.
- The first UI redesign Release output is now the fast-forward child
  `7aa11f2` on `production-publish-20260816`.

## UI redesign checkpoint (2026-08-16)

- The first UI redesign phase modernizes the public header and mobile drawer
  without changing business behavior: nested link/button navigation was
  replaced with real accessible links, login/signup actions were unified,
  brand/tagline spacing was improved, and the existing color palette and
  account widgets were preserved.
- The UI change is intentionally isolated in
  `Web\Components\Shared\PublicHeader.razor` and
  `Web\wwwroot\css\site-modern.css`; the source build completed with zero
  errors.

## Local demo-data verification checkpoint (2026-08-16)

- Added `tools\seed-demo-local.sql`, an idempotent, local-only seed script for
  `HiSubmitDB50`. It is not called by application startup and does not modify
  production.
- The seed creates two demo festivals, festival/event-category deadlines,
  news, a product, a ticket with a linked venue/address, one artist project
  and submission, a referee assignment, and product/ticket purchase records.
- Demo accounts use the existing local `johndoe` password hash and therefore
  use password `123Pa$$word!` only on the local demo database. They are:
  `demo.artist@hisubmit.test`, `demo.festival@hisubmit.test`,
  `demo.referee@hisubmit.test`, `demo.productbuyer@hisubmit.test`, and
  `demo.ticketbuyer@hisubmit.test`.
- Verified locally over HTTP: home, FAQ, Terms, festival deadline categories,
  festival products, festival news, and festival tickets. The ticket seed
  must include both a venue and an address linked through `Addresses.VenueId`.
- During verification, `GET /api/v1/public/Festival/GetAllNews?FestivalId=12`
  incorrectly returned news belonging to festival 13. The public controller
  did not enable the festival-specific filter. The controller now enables that
  filter whenever `FestivalId` is present. On the rebuilt local app, festival
  12 returned only its own news, while the no-`FestivalId` public news endpoint
  continued to return global and festival news.
- The rebuilt local app also returned HTTP 200 for demo festival products,
  tickets, and deadline categories. The source build completed with zero
  errors; existing package and compiler warnings remain tracked separately.
- The static-page route bug was traced to inconsistent database links: the
  Terms record stores `terms` while the route previously queried `/terms`.
  The page now normalizes route slashes and both `/terms` and `/term` return
  HTTP 200 locally.
- The `/Account/Login` form previously passed the email directly as the
  Identity username. It now resolves the user by email first and signs in
  with the stored username. The demo credentials were verified through the
  local login API.

## Administrator access and account switching checkpoint (2026-08-16)

- The real local database confirms `amir@mohammadi.com` is active, confirmed,
  and belongs to the global `Administrator` role. No production user or
  password was changed.
- `CustomClaimsPrincipalFactory` now creates a fresh claims identity for every
  sign-in instead of reusing one identity instance between users. It also adds
  the security-stamp claim required by the revalidation provider.
- The administrator dashboard now explicitly requires the
  `Administrator` role. The local administrator account returned HTTP 200 for
  `/admin/dashboard` and HTTP 200 with `succeeded=true` from
  `/api/v1/admin/adminDashboard/GetAccountStatusCount`.
- The account switcher now exposes a distinct Administrator account alongside
  the personal account and festival accounts. Fixed Razor parameters that were
  accidentally passed as literal strings (`item.Id`, `item.AdminLogin`, and
  `FestivalId`).
- Account-selection cookies now use `Secure` only when the current request is
  HTTPS, allowing local HTTP testing while preserving secure cookies in
  production. Redirect paths are normalized to safe local absolute paths.
- Local validation command:
  `dotnet run --project .\Web\Web.csproj --launch-profile http`
  from `D:\websites\hisubmit`. The source build completed with zero errors;
  existing dependency/compiler warnings remain tracked separately.

## Festival public route and local build checkpoint (2026-08-16)

- The attached failed build was caused by a running `Web.exe` locking copied
  assemblies, not by the warning count. Stopping the exact Web process before
  rebuilding resolves `MSB3021/MSB3027` file-lock errors.
- Public festival details now accept both `/festivalpage/{FestivalUrl}` and
  `/festival/{FestivalUrl}`. Existing links using the shorter route no longer
  return 404; footer and festival-card navigation use the canonical
  `/festivalpage/{FestivalUrl}` route.
- Festival prerender state keys are now namespaced by the festival URL. This
  prevents cached images, news, products, organizers, qualifiers, or festival
  details from one festival being reused for another festival.
- The detail page now guards against a missing festival response instead of
  dereferencing `Festival` during initialization.
- Verified locally with the Aurora demo festival:
  all three route spellings (`festival`, `festivalpage`, `festivalPage`)
  returned HTTP 200 and rendered festival content.
- The build currently completes with zero errors and approximately 1,527
  existing warnings. Warning cleanup is deferred to categorized commits:
  dependency/security warnings first, then real compiler/nullability issues,
  then obsolete/unused-code warnings. No blanket suppression is allowed.

## Public festival UI redesign checkpoint (2026-08-16)

- Began the visual redesign locally while production deployment remains
  intentionally deferred.
- Modernized the public festival browser and festival detail presentation
  without changing submission, purchase, news, ticket, filter, or navigation
  handlers. Added a visual hero, featured festival cards, clearer search/filter
  hierarchy, result headings, improved festival card actions, detail-page
  heading/layout treatment, and a stronger product section.
- Changes are isolated to:
  `Web\Components\Pages\Public\Festivals\Home.razor`,
  `Web\Components\Pages\Public\Festivals\FestivalDetail.razor`, and
  `Web\wwwroot\css\site-modern.css`.
- Local build completed with 0 errors. Local HTTP checks returned 200 for `/`,
  `/festivals`, `/festival/demo-aurora-arts-film-2026`, `/terms`, and `/term`
  after starting the rebuilt application.

## Complete local QA showcase checkpoint (2026-08-17)

- The active work is now the comprehensive local data and end-to-end
  validation phase. It follows the stability, bug-fix, responsive UI, and
  adaptive project-media phases.
- Added the idempotent local-only seed script
  `tools/seed-qa-showcase-local.sql`. It targets `HiSubmitDB50` and is not
  called during application startup. It creates:
  - `qa.artist@hisubmit.test`, `qa.festival@hisubmit.test`, and
    `qa.referee@hisubmit.test`;
  - `[QA] Complete Creative Showcase 2026` with URL
    `qa-complete-creative-showcase-2026`;
  - complete festival metadata, address, venue, organizers, images, rules
    PDF, six categories, questions, fees, qualifying awards, and a
    festival-scoped referee role;
  - one complete project for every supported type: Film, Photography, Music,
    Script/ScreenWriting, VR/XR, and Art.
- All six QA projects have complete base metadata, type-specific
  specifications where supported, media files, images, credits, awards,
  distribution information, project addresses, and accepted submissions.
- The required near-term deadline is **August 19, 2026** (the current date is
  August 17, 2026), followed by September 20 and October 5 deadlines.
- Local media fixtures are under `Web/wwwroot/media/qa/`:
  `qa-flower.mp4` for video/XR and `qa-script.pdf` for document preview.
  Music uses the existing local `/media/notification.mp3`; image and art
  samples use existing local images.
- Seed execution was verified in `HiSubmitDB50`: festival ID 15, projects
  IDs 14-19, six submissions with `SubmitStatus=Paid` and
  `JudgingStatus=Selected`, and three deadlines with six fee rows each.
- Local HTTP verification after rebuild returned 200 for the QA festival,
  film project, and photography project pages. The MP4 returned
  `video/mp4` and the PDF returned `application/pdf`.
- Local credentials for this checkpoint are intentionally test-only:
  password `123Pa$$word!` for all three QA accounts. They must not be used
  or copied to production.
- The build completed with zero errors and seven remaining dependency
  vulnerability warnings. The previously observed run/build interruption was
  caused by a live `Web.exe` locking assemblies; stopping that exact process
  before rebuilding resolved it.

## Cart, festival rating, and festival browser stability checkpoint (2026-08-17)

- Root cause of the observed cart HTTP 500 was confirmed in
  `Web/Logs/log20260817.txt`: SQL `float` values in
  `SubmitDeadLineCategories.Price` were being materialized into a CLR
  `int?`, causing `System.Double` to `System.Int32` cast failures.
- The domain property, submit-category DTOs, deadline fee DTOs, and submit
  creation path now use `double`; the anonymous cart endpoint was re-tested
  and returns a controlled `User not Found` result instead of HTTP 500.
- Public festival ratings now accept one rating per festival per user or IP.
  Rating comments remain subject to the existing accepted-participant and
  two-week-after-event rule. A summary endpoint returns average, total votes,
  and whether the current visitor has rated.
- Local rating verification for festival ID 15 succeeded: first rating was
  stored, the second request from the same IP was rejected, and the summary
  returned `averageRate=4`, `totalVotes=1`, `hasRated=true`.
- Festival featured tiles and result cards are now clickable, the duplicate
  filter count was removed, filter controls have overflow-safe styling, and
  long festival names/content wrap without escaping their containers.
- The public festival heading rating is live instead of hardcoded, and empty
  rating records are excluded from the visible review list.
- Final local build completed with zero errors. Existing legacy/compiler and
  package vulnerability warnings remain outside this focused checkpoint.

## Mobile navigation, festival roles, and form reliability checkpoint (2026-08-17)

- Unified the public mobile navigation behavior with final responsive overrides.
  Authenticated mobile navigation now exposes Cart, Profile, and Advertise;
  the Advertise link is hidden from anonymous users in the public header/footer.
- Artist dashboard project cards now navigate directly to
  `/user/project/{id}` for editing.
- Festival creation now submits `AddToCurrentUser=true`, reports empty-success
  responses safely, and navigates to the festival dashboard after creation.
- Login now derives the selected-festival cookie from the newly signed-in
  principal instead of the pre-login anonymous principal, fixing festival
  accounts opening as personal accounts when the festival claim exists.
- Save/Next dialogs in both festival edit flows now safely handle Cancel/Close
  results instead of dereferencing null dialog data. Organizer forms have a
  unique `FormName`, use an explicit button action, and global dialog CSS keeps
  content scrollable within the viewport.
- Local build succeeded with zero errors on August 17, 2026. The 1,500+
  legacy analyzer/compiler warnings remain categorized technical debt and were
  not broadly changed in this focused commit.
- Local application restarted successfully at `http://127.0.0.1:5120`;
  `/` and `/advertise` returned HTTP 200. No new cart cast exception appeared
  after the fresh 16:13 start; older cast entries in `Web/Logs/log20260817.txt`
  are from before the cart type fix.

## Contextual page guidance and required-field visual language checkpoint (2026-08-17)

- Added the shared route-aware `PageUsageGuide` component under
  `Web/Components/Shared/Components/`. It gives users a concise explanation of
  the current page and its main actions, without requiring every page to copy
  the same markup.
- The guide is mounted in `MainLayout`, `PublicMainLayout`, and
  `FestivalMainLayout`, covering artist, festival, referee, administrator,
  account, shopping, tickets, news, FAQ, advertising, product and project
  workflows.
- Added a consistent responsive visual pattern in
  `Web/wwwroot/css/site-modern.css`. It wraps safely on mobile and uses the
  existing brand palette.
- Required MudBlazor input markers are now consistently red and bold. The
  existing MudBlazor required pseudo-element is styled directly, so required
  fields throughout existing forms show a red `*` without manually editing
  dozens of forms.
- Local build completed successfully with zero errors. HTTP checks returned
  200 for `/`, `/news`, `/store`, `/tickets`, `/advertise`, `/faq`,
  `/festival/demo-aurora-arts-film-2026`, and a product route. The guide was
  present on all non-home operational/public routes checked.

## Product assessment and Phase 1 UI checkpoint (2026-08-17)

- Added `PRODUCT_ASSESSMENT.md`, documenting product strengths, P0/P1/P2
  risks, the missing first-class residency/literary opportunity taxonomy, and
  the prioritized delivery roadmap.
- Added a clearer artist dashboard header and responsive profile layout in
  `Web/Components/Pages/User/Dashboard.razor`.
- Added a role-specific judging workspace header and mobile-safe wrapper in
  `Web/Components/Pages/User/Referee/Dashboard.razor`.
- Added active-filter count and one-click reset to
  `Web/Components/Pages/Public/Festivals/Home.razor` and its code-behind.
- Added responsive overflow-safe styling for dashboard introductions, filter
  actions, cards and tables in `Web/wwwroot/css/site-modern.css`.
- No database, payment, API authorization, claim, or festival-scope rule was
  changed. Existing unrelated source and local uploaded test files remain
  unstaged.
- Verification: `dotnet build .\Web\Web.csproj --no-restore
  --disable-build-servers --nologo` completed with 0 errors. Local runtime
  started on `http://localhost:5120`; `/` returned HTTP 200 and
  `POST /api/v1/public/festival/GetAll` returned HTTP 200.

## Local QA login checkpoint (2026-08-17)

- The QA accounts were verified in the local `HiSubmitDB50` database:
  `qa.artist@hisubmit.test`, `qa.festival@hisubmit.test`, and
  `qa.referee@hisubmit.test` exist, are active, email-confirmed, and have
  password hashes.
- The failed browser login was caused by the legacy `/login` component
  dereferencing a null `HttpContext` during Interactive Server initialization.
  This produced a circuit `NullReferenceException` before credentials were
  checked.
- Added null-safe `HttpContext` handling to both login components:
  `Web/Components/Pages/Authentication/Login.razor.cs` and
  `Web/Components/Account/Pages/Login.razor`.
- Verification: `/login` returned HTTP 200 and
  `POST /api/identity/token/login` returned `succeeded=true` for the local QA
  artist account. No production account or database was changed.
- Build completed with 0 errors; the existing warning backlog remains.

## Festival dashboard KPI checkpoint (2026-08-17)

- Fixed `Web/Components/Pages/Festival/Dashboard.razor.cs` so the sold-ticket
  count is assigned to `_ticketsSoldCount` instead of overwriting
  `_categoryCount`.
- This corrects the dashboard cards for Categories and Tickets without
  changing the API, database, payment logic, or festival authorization scope.
- Build verification completed with 0 errors. The existing MudBlazor analyzer,
  nullable, compiler, and package warnings remain technical debt.

## Festival account ownership and create-festival guard checkpoint (2026-08-17)

- A festival account is now single-owner: a signed-in user with the global
  `FestivalId` role cannot create a second festival.
- The rule is enforced in `AddFestivalCommandHandler`, so it also applies to
  direct API requests and cannot be bypassed by hiding the button in the UI.
- The authenticated layout now hides `Create your festival` for an existing
  festival account and shows a disabled `Festival account active` state in the
  account menu.
- `AddFestivalCommandHandler` now awaits the created-festival query, checks the
  role-operation result, and returns a controlled failure if creation did not
  produce a festival instead of dereferencing a pending task.
- Adding the existing global festival role is idempotent in
  `UserService.AddToRoleAsync`; an already assigned role is not treated as a
  creation failure.
- Festival claims now select the newest valid active festival master for the
  user and never emit a false festival id of `0`. This prevents users with
  multiple legacy masters from opening a blank or crashing festival dashboard.
- Local verification on August 17, 2026: login for
  `qa.festival@hisubmit.test` succeeded, `/festival/dashboard` returned HTTP
  200, and a direct second-festival request returned a controlled
  `succeeded=false` response with the ownership message.
- Build completed with 0 errors. The existing legacy warning backlog remains
  unchanged and is tracked separately.

## Home discovery and sponsored placement checkpoint (2026-08-18)

- Reworked the public Home/festival discovery page without changing the
  existing public header/footer or authentication and authorization flows.
- The sponsored area now queries festivals with the existing approved
  `FeeStatus.Special` value, labels the placement as sponsored, and shows a
  safe empty state linking to `/advertise`. The advertising page explains this
  placement to festival organisers.
- Added an optional `FeeStatus` filter to the existing public festival query
  path. This is read-only filtering over the existing Festival field; no
  database schema or payment calculation changed.
- Added a latest-news block after festival results using the existing public
  Content/News API and article routes.
- Improved Home card text wrapping, metadata spacing, highlighted-card colors,
  responsive action buttons, sponsored tiles, and news layout in
  `Web/wwwroot/css/site-modern.css`.
- Verified build with `dotnet build .\Web\Web.csproj --no-restore
  --disable-build-servers --nologo` (no errors; existing warning backlog
  remains).
- Started the local app with the no-build/no-restore command and verified
  `http://127.0.0.1:5120/`, `/festivals`, and `/news` all returned HTTP 200.
- Existing unrelated local changes and uploaded test files were intentionally
  not staged.

## Public discovery and festival detail resilience checkpoint (2026-08-18)

- Sponsored festival cards on the public Home page are now image/card links
  without the misleading nested “View festival” button. Missing festival and
  news images use the existing `/media/image.png` fallback and recover from
  broken image URLs.
- The public festival filter now sends nullable `FestivalType`, focus and
  category values explicitly. Search input callbacks were made explicit for
  Home, News and FAQ, so changing the text invokes the corresponding search
  flow instead of relying on a form event that could be skipped.
- Public footer placements now query approved `FeeStatus.Special` festivals
  and use a separate persisted state key (`sponsoredFestivals`) so an older
  cached regular-festival list is not shown as advertising.
- Festival detail now has safe fallback images for cover, logo, reward,
  gallery, news and organisers, loads festival files, escapes news URLs, and
  corrects visible labels such as “Timeline”, “Organizers”, “Key stats” and
  “Festival products”.
- Removed non-functional public actions: venue entries no longer navigate to
  `#`, festival email uses `mailto:`, and the inactive Reviews “Show more”
  button is not rendered.
- Corrected the festival runtime filter specification to honor nullable
  minimum/maximum runtime bounds using AND semantics.
- Added final scoped responsive CSS for Home hero sizing, long text wrapping,
  filter controls, festival cards, detail layout, gallery and timeline
  spacing. No database schema, payment, authentication, authorization or
  festival-scope rule was changed.
- Build verification: `dotnet build .\Web\Web.csproj --no-restore --nologo
  -m:1 /p:BuildInParallel=false` completed with exit code 0. The remaining
  warnings are legacy package advisories, nullable/compiler debt and the
  existing MudBlazor `MUD0002` analyzer backlog.
- GitHub prompt diagnosis: the local VS Code user setting
  `git.autofetch=true` is enabled. This is independent of `dotnet build`;
  disable Git auto-fetch in VS Code User Settings to stop background fetch
  authentication prompts. No GitHub integration was added to the project.

## Runtime, cart, authorization and UI resilience checkpoint (2026-08-18)

- Commit `99adbf6` hardened cart access and prerender states:
  - guest `GET /api/v1/cart/GetItems` now returns an empty successful cart
    instead of a misleading error response;
  - the open-cart query ignores any browser-supplied `UserId` and uses only
    the authenticated current user;
  - cart mutation/payment endpoints require authorization;
  - festival dashboard detail cards render a loading state when data is not
    available instead of dereferencing a null festival;
  - the old duplicate `favoriteFestivals` persisted-state registration was
    removed from the legacy `FavoriteFestival` component.
- Commit `6ce7427` corrected festival-scoped permission evaluation:
  - `BaseFestival` and `FestivalAuthorizeView` deserialize the per-festival
    permission dictionary and check only the selected festival;
  - malformed permission claims never grant access;
  - missing identity roles or missing festival masters no longer cause claim
    generation to throw;
  - the newest valid active festival master is selected for a festival owner.
- The current UI resilience phase is prepared for commit:
  - cart/user widgets tolerate null or empty data;
  - profile and layout name handling tolerates empty names;
  - timeline and category persisted state is keyed by FestivalId to prevent
    cross-festival state collisions;
  - public reviews, categories and timeline render explicit empty states.
- Verification:
  - `dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
    --nologo -m:1 /p:BuildInParallel=false -v:quiet` completed with exit code 0;
  - local `http://127.0.0.1:5120/`, the demo festival detail page,
    `/api/v1/cart/GetItems?UserId=`, and the public deadline endpoint all
    returned HTTP 200 during the controlled run;
  - the old log entries for `favoriteFestivals` and the dashboard null
    reference predate these fixes; port-binding errors are caused by multiple
    local Web processes, so stop the existing `Web` process before rebuilding.

## Submission, product, ticket and payment-scope checkpoint (2026-08-18)

- Hardened submission creation in `AddSubmitCommand`:
  - rejects missing project, festival or deadline input without throwing;
  - requires an authenticated user and verifies the selected project belongs
    to that user;
  - verifies every selected deadline category belongs to the requested
    festival before calculating fees;
  - removed the synchronous `.Result` call from the account-type lookup.
- Hardened festival product access:
  - product read, image, export and delete operations now receive and enforce
    the route festival ID;
  - cross-festival product IDs return a controlled not-found result;
  - product deletion cannot target a product from another festival.
- Hardened ticket access and editing:
  - venue and ticket festival ownership are checked against the route festival;
  - ticket sale dates are validated for presence and order;
  - ticket detail and delete operations enforce festival ownership;
  - show-time synchronization compares `ShowTimeId` rather than the join-row
    primary key.
- Fixed festival payment cart queries:
  - `FestivalPaymentsController` now explicitly sets the query type to
    `Festival`;
  - invalid or unsupported cart-item query types return a controlled failed
    result instead of `NullReferenceException` or `NotImplementedException`.
- Fixed the festival dashboard timeline to check `EventEndDate` before reading
  its nullable value.
- No database schema or migration was changed.

## Search interaction checkpoint (2026-08-18)

- Replaced the fire-and-forget `ReloadServerData()` calls in the operational
  search handlers with awaited asynchronous reloads.
- Normalized search input with trimming and null-safe handling before reload.
- Updated the shopping-cart search to forward its text through
  `GetAllCartsFilterDto.SearchString`, so the server-side cart filter actually
  receives the user's query.
- Updated 34 runtime search handlers across admin, festival, artist, referee,
  project, ticket, payment and public ticket areas.
- The only remaining synchronous example is commented legacy code in
  `Web/Components/Pages/Misc/DocumentStore.razor.cs`; it is not compiled or
  used at runtime.
- Verification:
  - `dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
    --nologo -m:1 /p:BuildInParallel=false` completed with 0 errors.
  - `/`, `/festivals`, `/news`, `/faq`, `/tickets` and guest cart retrieval
    returned HTTP 200 on the local server.
  - No new error marker was added to the runtime log during this smoke test;
    older historical entries remain in the rolling log.
- No database schema or migration was changed.

## Public navigation consistency checkpoint (2026-08-18)

- Added `Web/Components/Pages/Public/_Imports.razor` so all public pages
  consistently use `PublicMainLayout`, not only `/tickets`.
- This makes the public header, desktop navigation, mobile drawer, account
  actions and footer consistent across home, festivals, festival details,
  news, store, tickets, FAQ, advertise, products, projects and static pages.
- Changed public header logo references to root-relative URLs so the logo does
  not disappear on nested routes such as `/festival/{FestivalUrl}`.
- Verification:
  - build completed with 0 errors;
  - `/`, `/festivals`, `/tickets`, `/news`, `/store`, `/faq`,
    `/advertise` and the demo festival route returned HTTP 200;
  - each checked response contained the shared public layout/header/navigation;
  - no new runtime error marker was added to the latest application log.
- No database schema or migration was changed.

## Login stability checkpoint (2026-08-18)

- Root cause of the `/login` button remaining in `Processing` was identified
  in the runtime log:
  `InvalidOperationException: Headers are read-only, response has already
  started` while `PasswordSignInAsync` attempted to append the auth cookie
  from an Interactive Server circuit.
- `/login` now uses static SSR, like the `/Account` Identity pages, so the
  authentication cookie is written during the HTTP POST response.
- Login form controls now use MudBlazor static-input components and
  `LoginUser` receives the posted model through
  `[SupplyParameterFromForm(FormName = "LoginForm")]`.
- Removed the obsolete interactive loading/password-visibility state from
  this static form.
- Verification:
  - build completed with 0 errors;
  - invalid credentials returned HTTP 200 with the visible
    `Invalid login attempt` message;
  - the local test account `amir@mohammadi.com` reached `/` with HTTP 200 and
    no longer showed the sign-in button;
  - no new `Headers are read-only` or unhandled error was added after the
    fix; the two matching entries in the rolling log predate the fix.
- No database schema or migration was changed.
- Build verification after these changes completed with exit code 0. Remaining
  warnings are the existing package advisories, nullable/compiler debt and
  MudBlazor analyzer backlog.
- Local smoke checks during the controlled run:
  - `/` and the demo festival page returned HTTP 200;
  - `/api/v1/cart/GetItems?UserId=` returned HTTP 200 with a successful empty
    result;
  - the public deadline endpoint returned HTTP 200 with a successful result.
- Unrelated working-tree edits and uploaded test assets were not staged.

## Search and discount-code checkpoint (2026-08-18)

- Discount-code flow reviewed end to end:
  - festival users manage codes at `/festival/discount-codes`;
  - each code can target submissions, products, tickets/badges, or a
    combination;
  - cart calculation validates enablement, expiry, usage limit, festival
    scope and item type;
  - the selected lowest price is stored on the cart item as
    `PriceAfterDiscount` with `DiscountCodeId`;
  - final payment recalculates the total on the server.
- Fixed discount-code management search:
  - the search string is now sent to the API filter;
  - code and description are searched server-side;
  - the previous client predicate that always returned `true` was removed;
  - the page now uses the DiscountCode permission for loading and actions.
- Hardened discount-code administration:
  - null/empty code and sale-type input is rejected;
  - percentage values above 100 and invalid negative values are rejected;
  - edit, status change and delete operations require the route festival to
    match the stored code festival;
  - status responses are parsed as normal results.
- Hardened cart discount and payment:
  - discount calculation requires the current authenticated user;
  - the requested cart must belong to that user and remain unpaid;
  - duplicate/blank discount inputs are normalized;
  - usage limits count only paid cart items;
  - payment uses the server-calculated total, not a browser-provided price;
  - payment rechecks cart ownership and unpaid status before processing.
- Public festival search input now updates immediately before its debounce
  timer, reducing cases where the search box appeared unresponsive.
- Verification:
  - build completed with exit code 0 using the locked-file-safe command;
  - `/` and `/festivals` returned HTTP 200;
  - guest `/api/v1/cart/GetItems?UserId=` returned a successful empty result;
  - application startup completed on `http://localhost:5120`;
  - no new startup or request error was added to the latest runtime log after
    these tests.
- No database schema or migration was changed.

## Account selection and product opportunity checkpoint (2026-08-18)

- Fixed the distinction between a personal account and a festival account:
  - a personal selection is represented by the existing `SelectedFestivalId`
    cookie with value `0`;
  - an artist-only account defaults to the personal workspace when no
    selection cookie exists;
  - a festival owner defaults to the valid festival workspace unless the
    personal account is explicitly selected;
  - the selected account menu and festival navigation now use the same
    state, reducing cases where a festival user was shown the artist
    dashboard.
- Hardened the account-selection endpoint:
  - a non-administrator may select only the festival ID present in their main
    festival claim or in their per-festival permission claim;
  - an invalid or unauthorized festival selection returns `403`;
  - no browser-supplied festival ID is trusted without server-side claim
    validation.
- This change is cookie/claim and UI state only. It does not change the
  database schema, payment totals, judging rules, or festival ownership rule.

### Proposed interaction and revenue features (not implemented yet)

The following are product proposals for a later, explicitly approved phase.
They must not be activated by simply adding a UI button; each needs an
auditable server-side workflow, feature flag, privacy review, and pricing
decision.

1. Artist opportunity matching:
   - extract structured signals already present in a project/submission:
     medium, genre, runtime, language, country, premiere status, category,
     eligibility answers, deadline, fee and required materials;
   - match those signals against published festival, residency, grant and
     literary-magazine opportunities;
   - show explainable reasons ("matches photography category", "deadline in
     18 days", "fee within your saved budget") and a confidence level;
   - free tier: a small number of basic matches and deadline reminders;
   - paid tier: an advisory report, submission checklist, eligibility
     explanation and prioritised application plan;
   - never submit an application, answer a festival question, or claim
     eligibility automatically on behalf of an artist.

2. Festival smart selection assistant:
   - validate file presence, file type/size, required fields, date formats,
     category consistency and unanswered mandatory questions;
   - flag contradictions such as a runtime outside the selected category,
     missing premiere information, or an answer that conflicts with a
     published rule;
   - create a human-review queue with reason codes, evidence, confidence and
     an override/appeal trail;
   - do not auto-reject a submission solely because of an AI result;
   - possible monetization: a festival-plan add-on or metered review credits,
     with transparent per-submission pricing.

3. Interaction and revenue layer:
   - saved festivals, deadline alerts, personalised onboarding and
     "next best action" cards;
   - sponsored placement with clear labeling and relevance controls;
   - artist conversion analytics (views, saves, started submissions and
     completed payments);
   - festival analytics (source, category conversion, drop-off, review
     workload and sales);
   - paid advisory, promoted opportunities and festival workflow add-ons,
     without selling private artist data or hiding eligibility limitations.

4. Backend prerequisites before implementation:
   - provider interface for deterministic rules and optional AI services;
   - versioned recommendation/validation explanations and audit records;
   - feature flags, usage ledger, product/SKU and refund handling;
   - human override, appeal and notification states;
   - explicit consent, data minimisation, retention policy and access control;
   - tests proving festival-scoped isolation and no cross-festival leakage.

The next safe implementation step is a read-only, deterministic
"eligibility checklist" and explainable matching prototype over existing
data. AI-generated advice and paid checkout should follow only after the
data contract, privacy policy and pricing are approved.

## Request culture resilience checkpoint (2026-08-18)

- Runtime log review found that a client sending `Accept-Language: *` caused
  `RequestCultureMiddleware` to throw `CultureNotFoundException` and return
  HTTP 500.
- `Web/Middlewares/RequestCultureMiddleware.cs` now ignores wildcard and
  malformed culture values, supports quality parameters such as `en-US;q=0.8`,
  and falls through to the configured default culture instead of crashing.
- This is a middleware-only resilience fix; no localization resources,
  authentication, database, payment, or authorization rule changed.

## Account scope, referee audit, and warning reduction checkpoint (2026-08-18)

- Account context clarification:
  - `SelectedFestivalId=0` in the existing cookie means the personal
    workspace only; it is not an authorization grant.
  - A positive selected festival ID is accepted only when it matches the
    authenticated user's main festival claim or a server-issued
    per-festival permission claim.
  - Sensitive operations must still authorize against the current user and
    festival data. The cookie is a small browser context selector, not a
    permission database, so the model scales to many users without putting
    all memberships in the cookie.
- Referee and festival sub-user audit:
  - `FestivalSubUser` is the festival membership record and `ProjectJudging`
    is the per-project referee assignment.
  - Referee list, permission-check, detail, and result paths now require an
    authenticated current user and exclude non-active referee assignments.
  - Removed festival members are excluded from the sub-user list; adding a
    previously removed member reactivates the membership instead of creating
    a duplicate.
  - Removing a festival member marks their project assignments as removed.
  - The audit identified follow-up work: every command that accepts a route
    festival ID must perform an Application-layer ownership/permission check,
    and claims should be refreshed or revalidated after membership removal.
- Runtime log findings:
  - A wildcard `Accept-Language: *` caused a culture exception; the request
    culture middleware now ignores wildcard and malformed values.
  - A login circuit attempted to append an authentication cookie after the
    response had started. Login is intended to use the non-interactive
    Account/login path; this remains a runtime verification item after the
    next local restart.
- Safe warning cleanup completed:
  - removed unused constructor dependencies;
  - initialized generic extended-attribute DTO values;
  - fixed the missing project-file localizer injection;
  - corrected nullable return contracts for local storage/policy services;
  - hardened integer cookie/claim parsing and malformed permission JSON.
- Verification:
  - `dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
    --nologo` completed successfully;
  - final recorded result: 0 errors, 1819 warnings;
  - warning backlog is now categorized rather than hidden. The largest
    groups are legacy Razor `CS8618`, MudBlazor `MUD0002`, obsolete API
    `CS0618`, and fire-and-forget `CS4014`.
- No database schema, migration, payment total, or production data was
  changed by this checkpoint.

## Login and project-detail checkpoint (2026-08-18)

- The frozen Sign in action was traced to the legacy interactive `/login`
  component calling `PasswordSignInAsync` after the response had started.
  The runtime exception was `Headers are read-only, response has already
  started`.
- `/login` is now a compatibility alias that force-loads the static Identity
  page at `/Account/Login`. Public header links, protected-route redirects,
  and role-refresh redirects use `/Account/Login`.
- `/Account/Login` uses a native HTML submit button so ASP.NET Identity can
  issue the authentication cookie during static form processing.
- Project detail now has server-populated optional workflow data:
  artist-only festival registrations and restricted judging summaries.
  The Application handler resolves Submit and ProjectJudging relationships
  from the database and filters them by owner, administrator, active festival
  membership, or active per-submission referee assignment. Browser festival
  IDs and selected-account cookies are not authorization.
- A reusable responsive `ProjectWorkflowAccessPanel` and work-overview
  surface were added to the public project detail page. Unauthorized users
  receive empty workflow collections and do not see private panels.
- No database schema, migration, payment, or production data was changed.
- Full verification completed after stopping stale build processes:

  ```text
  dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
    -p:UseSharedCompilation=false --nologo -v:minimal
  Build succeeded
  0 errors
  ```

- The existing `Web/Logs/log20260819.txt` contains old login-circuit errors
  from before this checkpoint. After a fresh restart, the static login flow
  was verified end-to-end: `GET /Account/Login` returned HTTP 200 with a
  native POST form and anti-forgery token; a valid local test submission
  returned HTTP 302 and the log recorded `User logged in.`.
- The previous `Headers are read-only, response has already started`
  exception was caused by manually appending a selected-festival cookie after
  `PasswordSignInAsync`. That append was removed. Identity now owns the
  authentication cookie; the selected-festival cookie remains only a
  workspace selector and is never an authorization grant.
- Warning cleanup in this checkpoint was intentionally limited to safe,
  localized changes: unnecessary `async` was removed from server local
  storage methods and `FileUploader.Url` is now a normal bindable parameter.
  Broad nullable, MudBlazor migration, obsolete API, and fire-and-forget
  warnings remain for separately tested migrations.
- Latest full-output warning audit (with duplicated diagnostics counted
  separately from unique file/line/code locations) found 3,610 parsed warning
  lines and 1,579 unique locations. The largest unique groups were CS8618
  (1,050), MUD0002 (107), CS0618 (103), CS8602 (80), and CS0414 (69).
- No database schema, migration, payment, or production data changed in this
  checkpoint.

## Mobile header, category loading, and project-view access checkpoint (2026-08-19)

- Public and authenticated mobile headers now use explicit non-wrapping rows.
  The final CSS override is intentionally at the end of `site-modern.css`;
  an older responsive block was forcing desktop navigation onto small screens.
- `FestivalCategories` initializes to empty collections, reloads only when
  FestivalId changes, and handles empty/error responses safely. The
  `GetAllDeadLineEventCategory` handler validates FestivalId, uses
  `AsNoTracking`, and constrains deadline lookups to the requested festival.
- Project detail responses expose server-calculated
  `CanViewSubmittedFiles` and `CanViewPrivateContact` flags. Contact fields
  are redacted in the Application layer for festival/referee viewers, and
  the project-files query independently checks read permission.
- Artist-selectable public visibility (full, hide file/contact, password
  protected, or hidden) still needs a dedicated database field/API. Do not
  repurpose `Project.Password`, which belongs to file access.
- Verification: build succeeded with 0 errors. The local site returned HTTP
  200 on `/`, and the categories endpoint returned HTTP 200 with a controlled
  empty result for FestivalId=9. No new startup/API errors were written to
  the active log during this smoke test.

## Responsive CSS cleanup and festival-layout access checkpoint (2026-08-19)

- Removed duplicated responsive header and sponsored-festival CSS blocks from
  `Web/wwwroot/css/site-modern.css`. The shared header contract now appears
  once at the actual end of the stylesheet, so later legacy rules cannot
  override desktop/mobile visibility.
- `FestivalMainLayout` now has the same compact mobile-header pattern as the
  other layouts and opens a real temporary navigation drawer. The drawer
  starts closed; the desktop toolbar remains available at desktop breakpoints.
- `ICheckPermission.CheckReadProjectPermission` no longer treats the selected
  festival cookie/claim as authority. Festival access is resolved from
  database ownership or an active festival sub-user membership. Referee
  access is limited to active `ProjectJudging` assignments. Owner and
  administrator access remains supported.
- This checkpoint did not implement artist-selected public visibility modes;
  those still require an explicit domain/API design and controlled migration.
- Verification:
  - `dotnet build .\Web\Web.csproj --no-restore --disable-build-servers
    -p:UseSharedCompilation=false --nologo -v:q`
    - Build succeeded, 0 errors, 7 package-security warnings.
  - `git diff --check` passed for the changed files.
- Existing unrelated `Web/Permission/PermissionPolicyProvider.cs` changes and
  local uploaded test images remain unstaged and were not modified.

## Public header, project specification, and diagnostics checkpoint (2026-08-19)

- Desktop header visibility was made explicit in the final responsive CSS
  contract. Main workspace desktop rows are forced visible at the desktop
  breakpoint, while mobile rows are forced hidden; public navigation follows
  the same breakpoint contract.
- The public announcement and Advertise navigation are now role-aware:
  festival-role users see festival promotion, while artist-role users see a
  discovery link for finding festivals. Artist users are not shown the
  festival promotion CTA.
- The global page guide is intentionally hidden on public project-detail and
  public festival-detail pages because those pages already provide their own
  context and visual hierarchy.
- The public submission-category dialog now explains how to compare deadline
  date, fee tier, eligibility, and the risk of incomplete or inaccurate
  answers before adding a submission to the cart.
- The project Specification section now uses responsive cards aligned with
  the rest of the project-detail page instead of the legacy full-width red
  panel. It remains data-driven for film, photography, music, script, and
  VR/XR specifications.
- Root-level diagnostic `.log` files were moved to
  `diagnostics/logs-archive/`. Runtime Serilog output remains under
  `Web/Logs/`; the two locations have different purposes.
- Product direction for the next opportunity types is documented in
  `PRODUCT_ASSESSMENT.md`: literary competitions, literary magazines,
  residencies, grants, exhibitions, gallery open calls, and reusable event
  passes/accreditation should build on the current Festival/Submission model
  through an additive taxonomy after the current workflow is stable.

## Phase 1 regression checkpoint (2026-08-19)

- Rebuilt the active `Web\Web.csproj` after stopping stale `Web` processes.
- Build completed successfully with 0 errors. The existing warning backlog
  remains categorized in the technical documentation and was not changed in
  this test-only checkpoint.
- Local HTTP smoke tests returned 200 for `/`, `/festivals`, `/news`,
  `/store`, `/tickets`, `/faq`, and `/advertise`.
- Guest cart API `/api/v1/cart/GetItems?UserId=` returned a successful empty
  result.
- Public deadline API for `FestivalId=9` returned a successful empty result.
- The home HTML contained the public desktop navigation and expected primary
  links.
- No new runtime exception was produced by the tested requests.
- This pass did not exercise authenticated role switching, cross-festival
  denial, interactive form/modal/upload actions, payment, or visual browser
  rendering. Those remain the final Phase 1 regression items before Phase 2.
- No database schema, production data, or authorization rule was changed.

## Workflow and festival-scope audit checkpoint (2026-08-19)

- One reusable `Project` can have multiple `Submit` rows, one per festival.
- Each submission can have multiple `ProjectJudging` rows, one per referee.
- Referee assignment validates that submissions belong to the requested
  festival and selected users are active referees in that festival.
- Submission, judging-assignment, and ticket-management endpoints now require
  festival-scoped permissions.
- Referee result submission rechecks active referee membership.
- `FestivalAuthentication` no longer treats a selected festival claim as
  sufficient authority; the requested policy must be present in permissions.
- Product, ticket, and news ownership paths were reviewed without a schema
  change. Products and tickets are festival-owned; news supports festival
  news and administrator-managed global news.

## Automation controls and notification publishing checkpoint (2026-08-19)

- The legacy daily `GoNextFestivalPeriod` job clones an ended active period,
  advances its dates by one year, updates `FestivalMaster.ActiveId/ActivePeriod`,
  and publishes `CreatedFestival`. It is now opt-in through
  `Festival.EnableAutomaticPeriodCreation`, defaulting to `false`.
- A festival manager can enable or disable automatic period creation in
  Festival > Additional Settings.
- The new daily `PublishFestivalNotificationNews` Hangfire job publishes one
  public festival-scoped news item after `NotificationDate` when
  `EnableAutomaticSelectionNews` is enabled. It includes the festival
  introduction and selected/finalist/award-related submissions. Its
  date-bearing title makes the operation idempotent; empty selections are
  deferred to the next daily run.
- `EnableAutomaticSelectionNews` defaults to `true` and can be disabled by the
  festival manager in the same settings screen.
- `20260819142432_AddFestivalAutomationSettings` is a manually constrained
  migration adding only the two automation columns to `Festivals`. It has not
  been applied to any database in this checkpoint.
- Public active tickets now exclude records with no available capacity, and
  deadline email scheduling checks each nullable deadline independently.
- Store and ticket management remain structurally functional but visually
  basic. Storefront cards, reservation states, pass/accreditation metadata,
  checkout/receipt polish, and sales analytics remain future work.
- Startup initialization now applies pending EF migrations before running the
  database seeders. If migration fails, seeding is skipped and the failure is
  logged.
- The local `HiSubmitDB50` database already recorded
  `20250726170647_Change_FeePropTypeInCategoryDeadlineFeeTB`, but that
  migration identifier was missing from the active source tree. A no-op
  compatibility migration with the recorded identifier was added so EF does
  not replay the initial schema or fail on the existing `Users` table.

## Phase 1 database compatibility and API smoke checkpoint (2026-08-19)

- The restored local `HiSubmitDB50` database stores application tables and
  migration history under the `hisubmi1_user` schema. EF was initially
  checking `dbo.__EFMigrationsHistory`, which made all migrations appear
  pending and caused an attempt to recreate the existing `Identity.Users`
  table.
- `MigrationsHistoryTable("__EFMigrationsHistory", "hisubmi1_user")` is now
  configured for the application DbContext.
- The automation migration explicitly targets `hisubmi1_user.Festivals`.
- The missing recorded fee migration identifier was added as a no-op
  compatibility migration.
- Local `HiSubmitDB50` was updated successfully. Both automation columns are
  present and the history contains all eight expected migration identifiers.
- After restart, startup logged `Database migrations applied successfully`.
  `/`, `/festivals`, `/news`, `/store`, `/tickets`, `/faq`, `/advertise`,
  `/Account/Login`, `/PrivacyPolicy`, and `/terms` returned HTTP 200.
  Guest cart and the public deadline endpoint returned HTTP 200 with
  successful empty results.
- Older errors in `Web/Logs/log20260819.txt` belong to the pre-migration run.
  Authenticated role switching, cross-festival allow/deny interaction,
  form/modal/upload actions, payment, and visual browser checks remain
  unverified; Phase 1 is not closed.

## Phase 2 submission-wizard checkpoint (2026-08-19)

- The existing artist project flow is a five-panel wizard:
  information, credits, type-specific specification, awards/distribution,
  and project files.
- Added a visible progress indicator and step guidance to the wizard.
- Added stable `FormName` values to the information, credits, and
  type-specific specification forms.
- Closing/cancelling the unsaved-changes dialog now safely leaves the current
  panel unchanged instead of dereferencing a null dialog result.
- The active `Web.csproj` builds successfully with 0 errors after this change.
- This is a partial Phase 2 checkpoint, not completion: draft autosave,
  validation summary, and end-to-end interactive submission/payment checks
  remain unverified.

## Phase 2 festival-wizard checkpoint (2026-08-19)

- The festival editor is a seven-panel wizard:
  event details, contact/venue, deadlines, categories, files,
  gallery/cover, and additional settings.
- Added a shared progress indicator and step-specific guidance.
- The event-details form now has a stable unique `FormName`.
- The active `Web.csproj` builds successfully with 0 errors after this change.
- Phase 2 remains open: explicit draft recovery/autosave and complete
  interactive save/release verification are still pending.

## Phase 2 checkout and receipt checkpoint (2026-08-19)

- The shared checkout flow covers submission, ticket, badge, and product cart
  items through the existing cart/payment contracts.
- Added explicit cart loading, failed-load, and empty states.
- Cart API failures are now surfaced to the user instead of being silently
  ignored.
- Payment success navigation now requires a non-empty payment/receipt
  reference; an incomplete callback does not claim success.
- The cart summary now labels the payable amount as the checkout total.
- Build verification completed with 0 errors.
- Payment-provider success/failure, receipt download, and end-to-end checkout
  remain interactive test items; Phase 2 is not complete.

## Phase 2 festival task queue checkpoint (2026-08-19)

- Added a data-driven "Next steps" panel to the festival dashboard.
- The panel uses existing dashboard data to point to incomplete setup,
  missing categories, judging preparation, or missing festival news.
- Each task links to an existing authorized route; no API or database change
  was introduced.
- When no task is detected, the dashboard shows a clear-state message.
- Build verification completed with 0 errors.
- The task queue is a UI checkpoint; permission-denial and interactive
  completion checks remain pending.

## Phase 2 referee queue checkpoint (2026-08-19)

- The referee queue now has an explicit empty state and explains that each
  row is a festival-scoped assignment.
- The queue shows the available assignment context from the current response
  contract: project, festival, assignment date, status and average score.
- The judging dialog now displays the assigned festival name and reminds the
  reviewer that scores and answers belong only to that assignment.
- The current DTO does not contain season/period or category fields, so they
  were not inferred or fabricated.
- Build verification completed with 0 errors.
- Phase 2 remains open pending interactive referee save confirmation,
  festival-scope denial checks, autosave/recovery, and full checkout tests.

## Phase 1 and Phase 2 implementation checkpoint (2026-08-19)

- Completed the documented low-risk implementation items for the shared
  experience and completion funnels: clean cart reloads, validation summaries,
  stable form names, news preview, explicit referee save result, and
  festival-scoped claims/authorization.
- `FestivalAuthentication` now requires the requested festival and policy
  pair; a browser-supplied matching festival claim alone is not authority.
- `CustomClaimsPrincipalFactory` builds festival permission scopes from owned
  festivals, festival-manager memberships, and active referee memberships.
- Public route/API smoke checks and local migration startup checks passed.
- `dotnet build .\Web\Web.csproj --no-restore --disable-build-servers` passed
  with 0 errors and 7 package warnings. The remaining legacy warning families
  belong to the documented Phase 6 warning cleanup.
- `Web/Permission/PermissionPolicyProvider.cs` remains a pre-existing,
  unrelated local modification and is intentionally not included in the
  phase commit.
- Manual provider-payment, receipt-download, browser-visual, and real
  referee-assignment interaction remain test-session items; they are not
  falsely marked as verified by source/build checks alone.

## Phase 3 financial transparency checkpoint (2026-08-19)

- Receipt downloads are now owner-scoped for normal users and remain available
  to administrators through the admin endpoint.
- Unpaid carts cannot generate receipts.
- Downloading a receipt no longer republishes the paid-cart event, preventing
  duplicate payment notifications or downstream handlers.
- Zero-total payment with no open cart now returns a controlled failure instead
  of a null-reference error.
- No schema, payment-provider contract, or production database was changed.
- Provider payment verification, immutable transaction references, payout
  reconciliation, and income KPI trends remain the next documented items.

## Phase 3 commerce and financial reporting audit (2026-08-19)

- Discount codes are validated on the server per cart item. Festival-bound
  codes are limited to their festival and configured item types; global admin
  codes may be cross-festival. Codes are not cumulative; the best eligible
  code is selected for each item and payment recalculates the final total.
- Free festival registration uses the zero-total cart path. One submission
  can be linked to multiple categories in the same festival, while all
  payment/report/authorization queries remain scoped to the exact festival
  period.
- Receipts now carry the applied discount-code names and show discounted line
  prices where available.
- Festival financial state now counts only paid product, ticket, and
  submission records, uses discounted submission values, and exposes a
  last-30-days KPI. The admin report's submission field and upper date bound
  were corrected.
- No schema or production data was changed. Provider-side payment
  verification and immutable transaction/payout reconciliation remain
  explicitly unfinished and must not be inferred from these reports.

## Phase 3 settlement and user-finance checkpoint (2026-08-19)

- Festival finance now exposes gross income, recorded payments to the
  festival, paid Service Fee charges, and `NetSettlementDue` for the exact
  festival period. Existing `FestivalPaymentItem` records remain the source
  for manually recorded payments.
- Advertising is not yet part of settlement math: `AdvertiseRequest` has no
  festival link, price/cart item, invoice, or settlement reference. Do not
  subtract an advertising request from festival income until a documented
  financial ledger is added.
- Artist dashboard finance is based on owner-scoped paid carts and shows paid
  total, checkout count, discount savings, and spend by submission/product/
  ticket-badge/service-fee categories.
- Hi Coin remains a design proposal, not an implemented currency. Before
  enabling it, define an auditable earn/spend ledger, expiry, fraud limits,
  refund reversal, eligibility rules, and whether coins can reduce cash
  service fees.
- Future settlement work must add exact-period statements, immutable
  adjustments, monthly close/approval, payout status, reconciliation, and
  downloadable reports without combining adjacent festival periods.

## Festival account active-scope correction (2026-08-19)

- Local data for `qa.festival@hisubmit.test` contains three owned festival
  rows: festival `15` is active; festivals `16` and `18` are inactive legacy
  test rows. This is not three active festivals.
- `CustomClaimsPrincipalFactory` now includes only active owned festivals and
  active festival memberships in `FestivalPermission` and `FestivalId`
  claims. Inactive test rows are retained in the database and are not
  deleted.
- The account must log out and log in again after this change so the
  authentication cookie receives regenerated claims. Expected active festival
  after relogin: `[QA] Complete Creative Showcase 2026` (`FestivalId=15`).
- This correction preserves the documented single-owner create-festival
  guard. A new festival user was not created because the existing account
  already has one valid active festival; creating another test account would
  add duplicate data without fixing the claim scope.

## Settlement ledger foundation (2026-08-19)

- Added additive settlement tables:
  `FestivalSettlementStatements`, `SettlementAdjustments`, and
  `AdvertisingInvoices`.
- Statements are keyed by the exact `FestivalId`, `PeriodStart`, and
  `PeriodEnd`, so reports for different festival periods cannot be combined.
- Supported settlement states are Pending, Confirmed, Paid, and Disputed.
  Adjustments require a non-zero amount and reason, may include an evidence
  URL, and are blocked after confirmation or payment.
- Advertising invoices carry a festival reference and may be linked to both
  an advertising request and an exact settlement statement. Advertising
  charges are included in a newly created period statement.
- Added festival-scoped API operations for listing/creating statements,
  adding adjustments, changing status, and exporting an individual statement
  as Excel or PDF. Existing income and payment calculations remain intact.
- Migration `20260819190000_AddSettlementStatementsAndAdvertisingInvoices`
  was applied to the local `HiSubmitDB50` database. It creates only the three
  new tables and does not modify or delete existing rows.
- Added `/festival/settlements` for festival users with period selection,
  statement creation, status badges, adjacent-period comparison, adjustment
  entry, advertising-invoice totals, confirmation, and Excel/PDF download
  links. It is linked from both festival navigation variants and the payment
  information page.
- Added the admin API endpoint
  `POST /api/v1/admin/payment/AdvertisingInvoices`. It validates the festival
  and optional exact settlement link before creating an advertising invoice.
  Admin reconciliation screens and year-over-year artist finance views remain
  the next UI follow-up; the current artist dashboard still shows paid total,
  savings, count, and category chart.

## Phase 1 critical-flow verification checkpoint (2026-08-19)

- Local app was verified on `http://localhost:5120` after a successful
  no-restore build.
- Public festival deadlines, tickets, products, news, ratings, reviews, and
  art categories returned controlled HTTP 200 responses.
- Guest cart loading returns an empty successful result, and the authenticated
  QA artist cart also loads successfully.
- Festival settlement routes now require `FestivalAuthentication`;
  unauthenticated requests return 401, and a festival user requesting another
  festival is rejected.
- Ticket commands validate ticket ID, quantity, show time, commission
  configuration, and missing ticket records before accessing related data.
- `/api/*` authorization failures return HTTP 401/403 instead of redirecting
  API clients to the HTML login page.
- `ErrorHandlerMiddleware` emits `X-Correlation-ID` and includes it in
  unexpected-error responses for log correlation.
- Festival wizard and text-safety checkpoint (2026-08-20):
  - The festival editor remains a seven-step wizard, but the order is now
    identity, contact/venue, categories, deadlines/fees, files, gallery/cover,
    and additional settings. Categories are intentionally before deadlines so
    each fee schedule can be attached to an already-defined submission option.
  - The shared festival-editor guide now explains the complete order and tells
    organizers that red asterisks identify fields required for the current step.
  - The actual required fields in the event-details validator are still
    festival name, description, and rules. Release-only prerequisites such as
    logo, website, email, organizer and cover remain release checks and were
    not incorrectly made required during draft editing.
  - Festival description, awards, rules, and event-category descriptions are
    sanitized on the server to remove anchors, URL attributes, event-handler
    attributes, executable/embed blocks and similar unsafe content.
  - The rich-text editor now builds an isolated toolbar per instance and
    removes links from editors configured with `EnableLink=false`, including
    pasted content. This prevents one editor instance from re-enabling links
    in another instance.
  - No database schema or migration was required for this checkpoint.

## Festival wizard and draft-owner verification checkpoint (2026-08-19)

- `FestivalDetail` now uses `IPublicTicketManager` for the public festival
  ticket list. The previous internal ticket manager caused HTTP 401 responses
  on public festival pages.
- Owned festival records are included in the cookie claims even while they
  are draft/inactive. Active membership records remain restricted to active,
  non-removed memberships. This lets a festival owner complete the wizard
  before release without granting unrelated users access.
- `ClientAuthenticationManager` now implements selected/main/other festival
  account resolution from claims and local storage; the prior methods no
  longer throw `NotImplementedException`.
- Release prerequisites now explicitly load and validate deadlines and venues,
  and require registration opening, event start/end dates, at least one
  deadline, and a venue for offline festivals. Website and email are required
  by the contact validator and editor markers.
- Creating a category before defining deadlines is supported. Empty fee lists
  no longer call `Min()`/`Max()` or fail the category command.
- Festival organizers can be saved without an image; image upload remains
  optional and is processed only when non-empty data is supplied.
- Real local wizard verification used:
  `qa.festival.wizard@hisubmit.test` /
  `FestivalWizard#2026!`, festival `20`. Detail, contact, category, deadline,
  organizer, venue, prefix, cover upload, and release were exercised. Release
  returned success with the expected `UnderInvestigation` review state.
- No production database was changed. The local database already contains the
  test data created by the wizard verification.
- Earlier notes saying all owned drafts are filtered out are superseded by
  this checkpoint: only unrelated inactive memberships remain excluded; an
  owned draft is intentionally claimable by its owner.

## Category/deadline fee workflow checkpoint (2026-08-19)

- Categories are intentionally created before deadlines. A category may be
  saved with no fee rows while the wizard is still incomplete.
- After a deadline is created, its category links form the fee matrix. The
  festival editor shows those rows when the category is edited.
- `StandardFee = 0` means a valid free submission. `StandardFee = null` means
  the fee is not configured.
- Release validation now requires every category to be linked to at least one
  submission deadline and every linked deadline/category row to have a
  non-null standard fee. This preserves category-specific deadlines while
  preventing incomplete pricing from reaching the public site.
- The local QA festival proved both states: release was rejected while the
  fee was null, then succeeded after setting standard/student/gold fees to
  zero. No schema change was required.
