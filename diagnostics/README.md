# HiSubmit diagnostics

This directory is for local investigation artifacts that should not become
application content or production data.

- `logs-archive/` contains root-level build, smoke-test, and diagnostic logs
  moved out of the repository root on 2026-08-19.
- Runtime application logs are written separately under `Web/Logs/` by
  Serilog.
- Diagnostic logs are ignored by Git. Do not place secrets, database backups,
  uploaded user files, or production credentials here.
