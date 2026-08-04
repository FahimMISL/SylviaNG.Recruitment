# MinIO File Storage Migration (F1 + F2)

## What

Swaps local-disk file storage for MinIO (S3-compatible object storage) behind a config switch,
with local disk remaining the committed default.

- **F1 - core swap.** `MinioFileStorageService`/`MinioApplicationCvStorageService` implement the
  existing `IFileStorageService`/`IApplicationCvStorageService` interfaces. `FileStorage:Provider`
  ("Local" default, or "Minio") picks which implementation resolves, via the same factory-switch
  pattern as `ShortlistScoring:Provider`. New `FilesController` (`GET
  recruitment/files/download?key=...`) proxies/streams stored files regardless of provider -
  MinIO objects aren't reachable via `app.UseStaticFiles()` the way local-disk files were.
- **F2 - `ExportRequest` blob migration.** `ExportRequest.Content` (`byte[]` bytea column) replaced
  with `ExportRequest.ContentObjectKey` (`string?`) - the generated export file now lives in
  object storage like everything else, not inline in Postgres.
- **F3 (backfill of already-uploaded local files into MinIO) was scoped but skipped** - not
  worth building for a couple of demo files; re-upload through the UI once if ever needed.

## Why

`ExportRequest`'s own doc comment flagged the DB-blob approach as a stopgap pending "a separate,
deferred piece of work" (the MinIO swap). Requested now as the next scoped piece of work.

## Design notes

- **Zero frontend changes.** Every consuming Angular component already builds file URLs as
  `${Base_URL}${path}` (`environment.ts`). F1 only changes what value the backend puts into that
  `path`/`DownloadUrl`/`GeneratedPdfPath` field - from a raw wwwroot-relative path to
  `recruitment/files/download?key=<encoded key>` (built by the new
  `Application/Common/Helpers/FileUrlBuilder.BuildDownloadUrl`) - so the existing concatenation
  produces the correct proxy URL automatically. Applied at ~15 response-DTO/mapper boundaries; the
  underlying entity/DB value always stays the raw object key, never the built URL.
- **`FilesController` has no `[Authorize]`/needs `[AllowAnonymous]`.** Found live: a global
  `AuthorizeFilter` (`Program.cs`) requires auth on every controller by default. Plain `<img
  src>`/`<a href>` browser-native loads (profile photos, document links) don't carry the Angular
  `HttpClient` interceptor's bearer token, so an authorized proxy 401s on every such load. Matches
  the old `app.UseStaticFiles()` behavior it replaces, which was also unauthenticated.
- **No `Content-Disposition` on the proxy endpoint.** Initially returned `File(stream, contentType,
  filename)`, which sets `Content-Disposition: attachment` - browsers then refuse to render the
  response inline, breaking every `<img src>` (`net::ERR_BLOCKED_BY_ORB`). Switched to the
  two-arg `File(stream, contentType)` overload (no header at all), matching what
  `app.UseStaticFiles()` always did.
- **One bucket (`sylviang-recruitment`), two key prefixes.** `MinioFileStorageService` writes under
  `job-postings/{subFolder}/...`, `MinioApplicationCvStorageService` under
  `applications/{subFolder}/...` - mirrors how the two Local implementations already use sibling
  wwwroot folders rather than sibling storage roots. `IApplicationCvStorageService` gained a new
  `OpenReadAsync` method (previously `SaveAsync`/`DeleteAsync` only) so the proxy can serve CV keys
  too, via a `FileNotFoundException`-triggered fallback from `IFileStorageService`.
  `IExportRequestService`'s reuses `IFileStorageService` under an `export-requests` subFolder
  rather than introducing a third storage interface - conceptually "generated documents," same
  category as offer/medical letters.
- **`MinioBucketInitializer`** (`IHostedService`, registered only when the Minio provider is
  active) idempotently creates the bucket on startup - MinIO does not auto-create on first
  `PutObject` the way some S3-compatible providers do.
- **Retention sweep now deletes the object, not just the DB row.** `ExportRequestWorker.SweepExpiredAsync`
  converted from `static` to an instance method (needs `_logger`) - loops expired rows, calls
  `fileStorageService.DeleteAsync` per object (log-and-continue on failure so one bad delete
  doesn't abort the batch), then deletes the DB rows. Without this, MinIO would accumulate
  orphaned objects forever once `Content` moved out of the DB.
- **Bonus perf fix, no code change:** `ExportRequestRepository.GetPagedAsync` (list/history
  endpoint) previously fetched full entities including the `Content` bytea column with no
  projection, even though the response mapper never included it. Dropping the column fixes this
  automatically.
- **MinIO runs in Docker**, same `docker inspect`-else-`docker run` pattern as Postgres/Keycloak in
  `start.bat`/`stop.bat` (`sylviang-minio-dev`, ports 9000 API / 9001 console,
  minioadmin/minioadmin123 dev creds).
