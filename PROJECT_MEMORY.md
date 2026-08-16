# HiSubmit Project Memory

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
- The current tested branch is `recovery-clean-20260816`; its latest published
  commit is `3a9c107`.
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
