# HiSubmit product assessment and priority roadmap

Date: 2026-08-17

## Executive assessment

HiSubmit already has a strong operational foundation: public festival
discovery, artist projects and submissions, festival operations, judging,
products, tickets, payments, discounts, news, advertising, chat, notifications,
SEO, audit trails, and festival-scoped roles are represented in the active
solution.

The main product problem is not a lack of screens. It is that the current
experience presents a large set of capabilities as separate technical pages.
The user often has to infer what to do next, which records are in scope, what
has been saved, and what financial or judging state means.

The current data model is also primarily competition/festival-oriented. It
does not yet have a first-class opportunity taxonomy for:

- artist residencies and grants;
- literary magazines and open calls;
- non-competitive exhibitions, showcases, and portfolio reviews.

This should be solved additively. A new opportunity/capability model should be
introduced only after a reviewed migration and a compatibility plan for the
existing `Festival` records. Until then, UI improvements should use the
existing festival/event fields and must not pretend that a residency or
magazine workflow has already been implemented.

## Current strengths

1. Layered .NET 8 architecture with a clear Web, Application, Domain,
   Infrastructure, and shared-model split.
2. Festival-scoped claims and permissions exist in addition to global roles.
3. The platform has the core commercial primitives: carts, products, tickets,
   payments, commissions, discount codes, and sold-item records.
4. Judging has separate forms, fields, assignments, and results.
5. Public discovery already includes search, dates, deadlines, focus,
   categories, fees, ticket availability, and featured cards.
6. The shared page guide, responsive CSS corrections, and required-field
   markers provide a reusable UI foundation.

## Main product risks

### P0 - trust, security, and data correctness

- Every festival-scoped command and query must derive the effective festival
  from the authenticated context and verify access server-side. A browser
  supplied festival ID is only a selector, never an authority.
- Payment, commission, discount, ticket, and submission totals need a single
  auditable source of truth and receipt-like views.
- Judging assignments must be constrained by festival and judging period, with
  conflict-of-interest and visibility rules tested explicitly.
- Error responses should preserve correlation IDs and useful server logs
  without leaking database details to public users.

### P1 - conversion and completion

- Festival discovery needs clear opportunity cards, active-filter state,
  deadline urgency, fee transparency, and a reliable next action.
- Submission and festival creation should be staged workflows with progress,
  autosave/draft state, validation summary, and recovery after a disconnect.
- Checkout should show line items, platform/festival fees, discounts, taxes or
  payment notes where applicable, and a final receipt preview.
- Festival managers need a single workspace overview rather than navigating
  between isolated lists to understand what requires attention.

### P1 - role dashboards

- Artist: unfinished projects, upcoming deadlines, submission states, spend
  summary, and direct edit actions.
- Festival manager: entries waiting for review, judging progress, ticket/store
  sales, revenue, content publishing, and team access.
- Referee: assigned workload, remaining items, deadline, average score, and
  explicit festival/season context.
- Administrator: platform health, approvals, income, commissions, disputes,
  and anomaly signals.

### P2 - opportunity expansion

Introduce an additive `OpportunityType`/capability taxonomy with migration
support for existing festivals. Candidate types:

- Festival / competition
- Residency
- Grant / fellowship
- Literary magazine / publication
- Exhibition / showcase
- Portfolio review / workshop

Each type should enable only the relevant sections. For example, a residency
needs eligibility, accommodation, dates, funding, and portfolio requirements;
a literary magazine needs genre, simultaneous-submission policy, reading
period, rights, and response expectations. This should not be emulated by
adding unrelated booleans to `Festival`.

## Prioritized delivery plan

### Phase 1: stabilize the shared experience

1. Unify dashboard page headers, guidance, spacing, empty/loading/error states,
   and responsive behavior.
2. Improve festival search/filter interaction with active-filter summary and
   reset.
3. Standardize cards, buttons, long text wrapping, tables, and mobile drawers.
4. Add actionable notification and “next step” patterns.
5. Add regression checks for public festival discovery, cart loading, role
   switching, and festival-scope denial.

### Phase 2: improve completion funnels

1. Artist project/submission wizard with draft progress and validation summary.
2. Festival setup wizard with explicit save state and recovery.
3. Checkout and receipt view for submissions, tickets, and products.
4. Festival manager task queue and publishing preview.
5. Referee queue with assignment context and save confirmation.

### Phase 3: financial and judging transparency

1. Reconcile payment, commission, discount, and payout calculations.
2. Add immutable transaction/receipt references and exportable reports.
3. Add judging-period scope checks and audit events.
4. Add admin income and festival income KPI trends.

### Phase 4: residencies and literary opportunities

1. Design and review the additive opportunity schema.
2. Add migrations, API/application contracts, authorization rules, and seed
   data in a local database only.
3. Add type-specific forms, public cards, SEO previews, and dashboards.
4. Add tests for backward compatibility with existing festival records.

## Changes implemented in this checkpoint

- Added this product assessment and roadmap.
- Added a reusable visual treatment for dashboard context and responsive
  spacing where the current dashboards lacked it.
- Added active-filter visibility and one-click reset to public festival
  discovery.
- Preserved the existing role, permission, API, and database contracts.

These are UI/UX-only changes. No database schema, production data, payment
logic, or authorization rule was changed in this checkpoint.
