# HiSubmit Cleanup Plan

تاریخ بررسی: 2026-08-16

## نتیجهٔ build قبل از cleanup

راه‌حل ریشه `HiSubmit.sln` با دستور `dotnet build HiSubmit.sln --no-restore` با موفقیت build شد:

- خطا: 0
- warning: 11 (همگی dependency/security warning موجود قبل از cleanup)

## منبع کد فعال

راه‌حل فعال فقط این پروژه‌ها را دارد:

- `Web/Web.csproj`
- پروژه‌های `hisubmit/src/Application`
- `hisubmit/src/Client.Infrastructure`
- `hisubmit/src/Client.SharedModels`
- `hisubmit/src/Domain`
- `hisubmit/src/Infrastructure.Shared`
- `hisubmit/src/Infrastructure`
- `hisubmit/src/Shared`

## کاندیدهای حذف با ریسک پایین

این موارد در راه‌حل فعال استفاده نمی‌شوند و نقش خروجی/پشتیبان/فایل محیط توسعه دارند:

- `Backup/`
- `deployment/` — خروجی publish، حدود 219 MB
- `UpgradeLog.htm`
- `tore --staged deployment`
- `.vs/`
- `.idea/`

حذف `deployment/` فقط در صورتی انجام می‌شود که این پوشه محل اجرای production روی همین ماشین نباشد. چون deployment ممکن است برای rollback عملیاتی لازم باشد، قبل از حذف باید archive بازیابی تهیه شود.

## کاندیدهای حذف مشروط

این‌ها پروژه‌های سورسی مستقل‌اند و فقط به دلیل خارج بودن از راه‌حل فعال حذف نمی‌شوند:

- `Hisubmit.Web/` — برنامهٔ جدا با `ApplicationDbContext` و Identity مستقل؛ 618 فایل.
- `ClientComponents/` — پروژهٔ مستقل کلاینت؛ 825 فایل و namespaceهای `ClientComponents`.
- `Testcli/` — پروژهٔ آزمایشی کوچک؛ 5 فایل.
- `MyDarkThem/` — افزونهٔ Visual Studio؛ 3 فایل.
- `ServerClientShared/` — قرارداد کوچک مستقل؛ 2 فایل.

## مواردی که فعلاً نباید حذف شوند

- `hisubmit/src/Server/`
- `hisubmit/src/Client/`
- `hisubmit/HiSubmit.sln`
- `hisubmit/docker-compose.yml`
- `hisubmit/src/Server/Dockerfile`

این‌ها یک مسیر مستقل قدیمی/Docker دارند و هنوز نمی‌توان فقط با نبودن در solution ریشه ثابت کرد که هیچ محیطی از آن‌ها استفاده نمی‌کند.

## نقطهٔ بازگشت

قبل از حذف:

1. `git tag pre-cleanup-20260816` روی آخرین commit موجود ساخته می‌شود.
2. برای فایل‌های tracked، بازیابی از git ممکن است.
3. برای فایل‌های untracked مانند `Backup/` و `PROJECT_MEMORY.md`، archive یا manifest جدا ثبت می‌شود.
4. فهرست حذف، hash و زمان عملیات در همین فایل ثبت خواهد شد.

## نتیجهٔ اجرا

کاربر تأیید کرد که `deployment/` روی همین سیستم برای production استفاده نمی‌شود، پوشه‌های مستقل خارج از solution ریشه استفاده نمی‌شوند و نسخهٔ قدیمی `hisubmit/src/Server` و `hisubmit/src/Client` باید حذف شود.

در 2026-08-16 موارد زیر حذف شدند:

- `Backup/`
- `deployment/`
- `Hisubmit.Web/`
- `ClientComponents/`
- `Testcli/`
- `MyDarkThem/`
- `ServerClientShared/`
- `UpgradeLog.htm`
- `tore --staged deployment`
- `hisubmit/src/Server/`
- `hisubmit/src/Client/`
- `hisubmit/HiSubmit.sln`
- `hisubmit/docker-compose.yml`
- `hisubmit/docker-compose.dcproj`

`.vs/` و `.idea/` عمداً حفظ شدند چون تنظیمات محیط توسعه‌اند و حذفشان برای داشتن یک solution فعال ضروری نیست.

## بازیابی

- tag بازیابی: `pre-cleanup-20260816`
- archive بازیابی: `cleanup-recovery-20260816.zip`
- archive با موفقیت خوانده و بررسی شد: 3572 entry و حدود 434MB محتوای uncompressed.

برای بازگردانی فایل‌های حذف‌شده، archive را در ریشهٔ پروژه extract کن. برای نسخهٔ tracked قبل از cleanup نیز tag بالا روی commit قبلی قرار دارد.
