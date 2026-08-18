# HiSubmit Figma Page Map

Source file: `HISUBMIT (Copy)`  
Reviewed in Edge: August 18, 2026  
Figma contains one visible page, `Page 1`, with multiple top-level frames and
dashboard groups. The design is a visual direction, not a complete state
specification: loading, empty, validation, unauthorized, payment failure and
mobile states are mostly absent and must be added during implementation.

## Page and frame inventory

| Figma title | Product area | Expected route | Purpose | Main components |
|---|---|---|---|---|
| `Home` | Public site | `/` or `/festivals` | Discover opportunities and move users toward a festival submission | Header, search, featured festival cards, festival grid, pagination, news strip, footer |
| `festival` | Public site | `/festival/{FestivalUrl}` | Present one festival and its calls, deadlines and actions | Festival hero, logo, metadata, deadline timeline, rules, awards, submit CTA, products/tickets/news |
| `merchant store` | Store/cart | `/store` and `/Product/{Id}/{Title}` | Browse products from all festivals and open product details | Search, product cards, price, festival label, image, add-to-cart CTA, pagination |
| `Project page show` | Public project | `/project/{ProjectUrl}` | Show a submitted work with its metadata and media | Media player/viewer, title, credits, project type, awards, sharing, likes |
| `news` | News/content | `/news` | Browse site and festival news in one feed | Featured story, search, cards, festival label, pagination |
| `single news` | News/content | `/new/{NewId}` or `/new/{NewId}/{Title}` | Read a complete news article | Article hero, author/date, rich content, related stories, share/like |
| `Faq page` | Public support | `/faq` | Answer common user questions | Search, accordion questions, categories, empty state |
| `sign in` | Authentication | `/login` and `/Account/Login` | Authenticate an artist, festival, referee or admin | Email/password, remember me, social placeholders, forgot password, sign-up |
| `Header` | Shared design system | Shared layouts | Consistent navigation across desktop and mobile | Brand, browse/news/store/tickets links, cart, notifications, profile, role switcher |
| `festival Dashboard Screen` | Festival management | `/festival/dashboard` | Give festival managers KPI and operational overview | Sidebar, breadcrumb, KPI cards, sales chart, activity, pending work, quick actions |
| `billing` | Admin/accounting | `/admin/income/payments`, `/commissions` | Review revenue, charges, commissions and payment status | Financial KPI cards, filters, tables, status chips, export/receipt actions |
| `user profile dashboard` | Account/artist | `/user/dashboard`, `/account` | Show personal profile, projects and account actions | Profile summary, project cards, submissions, deadlines, notifications, account menu |

## Product-area grouping

### Public site

`Home`, `festival`, `Project page show`, `Header`, `Faq page`.

The current application already has routes and API managers for these areas.
The highest-value work is visual consolidation, long-content safety, clearer
CTAs, and consistent loading/error/empty states.

### Artist dashboard and submission flow

`user profile dashboard` and the public `festival` submission CTA map to:

- `/user/dashboard`
- `/user/projects`
- `/user/project`
- `/user/project/{ProjectId}`
- `/User/Submits`
- `/user/ShoppingCart`

The existing API supports project specifications, files, submissions, cart
and payment. Autosave and a complete step-progress model are not represented
as a single shared backend contract yet.

### Festival management

`festival Dashboard Screen` maps to `/festival/dashboard` and links to the
existing festival edit, deadline, organizer, product, ticket, news, judging,
sub-user and SEO pages. Existing festival-scoped controllers and
`FestivalAuthentication` must remain the authority; browser IDs are selectors
only.

### Judging

The Figma file does not contain a dedicated referee frame. Existing product
routes remain the source of truth:

- `/User/Judges/Dashboard`
- `/Judges`
- `/JudgingDetail/{Id}`
- `/festival/judging`
- `/festival/judgingResult/{SubmitId}`

The design gap is an explicit assignment/period context, queue state, save
confirmation and conflict-of-interest messaging.

### Store, cart, payment and tickets

`merchant store` and `billing` map to:

- `/store`
- `/Product/{Id}/{Title}`
- `/user/ShoppingCart`
- `/tickets`
- `/tickets/{FestivalId}`
- `/user/tickets`
- `/festival/tickets`
- `/admin/festival/tickets`

Existing managers and controllers cover the core records. Receipt-like
breakdowns, tax/fee explanations, payment failure recovery and consistent
order state presentation need further UI work; schema changes are not needed
for the first design phase.

### Admin and accounting

`billing` maps to `/admin/dashboard`, `/admin/income/payments`,
`/commissions`, festival payment detail and admin festival/product/ticket
management. Existing APIs support the main lists and status updates. A
single reconciled income KPI contract is not yet present.

### News and content

`news` and `single news` map to `/news`, `/new/{NewId}` and festival/admin news
routes. Existing public and scoped news APIs are reusable. A distinction
between festival-local publication and promoted site-wide publication exists
as a product requirement but needs an explicit promotion field/workflow before
it can be fully implemented.

### Shared components/design system

The visual language uses:

- deep teal primary surfaces;
- coral/red accent CTAs;
- white cards with soft borders and shadows;
- compact uppercase eyebrow labels;
- rounded cards and pill status chips;
- dense but readable metadata rows;
- desktop sidebar/header with responsive mobile drawer.

The existing `site-modern.css`, `MainLayout`, `PublicMainLayout`,
`PageUsageGuide`, MudBlazor theme and shared card components can implement
this without replacing the .NET/Razor architecture.

## Reuse versus new work

### Reuse now

- `MainLayout`, `PublicMainLayout`, `FestivalMainLayout`
- `PageUsageGuide`
- MudBlazor `MudCard`, `MudGrid`, `MudSelect`, `MudDatePicker`,
  `MudProgressLinear`, `MudAlert`, `MudChip`
- existing public festival, news, product, cart, ticket and project managers
- existing role and festival-scope authorization
- current response wrappers and pagination

### New reusable components recommended

- `PublicSectionHeader`
- `OpportunityCard` with safe image fallback and long-title wrapping
- `AsyncStatePanel` for loading/empty/error/retry
- `FilterSummaryBar`
- `MoneyBreakdown` / `ReceiptSummary`
- `DashboardKpiCard`
- `MediaPreview` variants for image, video, audio, PDF and document
- `RoleScopeBadge` showing festival/season context

These should be introduced incrementally and backed by existing DTOs first.

## Missing backend/API data

1. A first-class opportunity type/capability model for residencies, grants and
   literary magazines is missing. Do not emulate it with unrelated booleans.
2. Site-wide news promotion/payment status is not a complete explicit API
   contract.
3. Dashboard trend/KPI aggregation is fragmented across several endpoints.
4. Receipt and fee/tax breakdown data is not unified across submissions,
   products and tickets.
5. Referee assignment period, conflict-of-interest and workload summary need
   explicit response fields or aggregation.
6. Autosave/draft recovery status is not a common contract for all multi-step
   forms.

## Implementation blockers and risks

- Figma has one page and many artboards, not a complete responsive/state
  specification. Mobile behavior must be designed from the desktop intent.
- Some Figma text and imagery are static examples; production data can be much
  longer. All implemented cards must wrap and clamp safely.
- Existing MudBlazor analyzer warnings are numerous. A visual pass must not
  mix a blanket warning cleanup with functional changes.
- Public cards must never infer authorization from a festival ID in the URL.
- Payment, judging, and festival management changes require endpoint-level
  regression checks and must not be UI-only.
- Existing local uncommitted uploads and unrelated project-detail changes are
  preserved and must not be included in feature commits.

## Staged implementation plan

1. Shared design tokens, public headers, section headings, cards and async
   state patterns.
2. Public Home, festival discovery, News, Store and project presentation.
3. Artist project/submission funnel and media preview.
4. Festival manager dashboard, setup and task queue.
5. Referee queue and judging context.
6. Cart/checkout/receipt, tickets and billing.
7. Admin income, content promotion, analytics and polish.
