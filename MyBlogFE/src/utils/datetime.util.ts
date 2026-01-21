/**
 * GitHub Copilot
 *
 * Utility functions to parse and format ISO-like datetimes such as:
 * "2025-11-23T08:18:30.7320749"
 *
 * Exports:
 * - toDate(iso): Date | null
 * - formatDate(iso): "DD-MM-YYYY" | null
 * - formatDateTime(iso, opts): "DD-MM-YYYY HH:mm[:ss]" | null
 */

function pad(n: number, width = 2) {
  const s = String(n);
  return s.length >= width ? s : "0".repeat(width - s.length) + s;
}

function normalizeFractionalSeconds(iso: string) {
  // Reduce fractional seconds to milliseconds (3 digits) if there are more
  return iso.replace(/(\.\d{3})\d+/, "$1");
}

function tryCreateDate(iso: string): Date | null {
  if (!iso || typeof iso !== "string") return null;
  let d = new Date(iso);
  if (!isFinite(d.getTime())) {
    // Try truncating fractional seconds to milliseconds
    const truncated = normalizeFractionalSeconds(iso);
    d = new Date(truncated);
  }
  if (!isFinite(d.getTime())) {
    // As a last attempt, append Z to treat as UTC (only if no offset present)
    if (!/[zZ]|[+\-]\d{2}:\d{2}$/.test(iso)) {
      d = new Date(iso + "Z");
    }
  }
  return isFinite(d.getTime()) ? d : null;
}

/**
 * Convert ISO string to a Date object (or null if invalid).
 */
export function toDate(iso: string): Date | null {
  return tryCreateDate(iso);
}

/**
 * Add Z to ISO string if missing and parse as UTC.
 */
function parseUtcIso(iso: string): Date | null {
  if (!iso) return null;
  // Add Z to understand as UTC
  if (!iso.endsWith("Z")) iso = iso + "Z";
  const d = new Date(iso);
  if (isNaN(d.getTime())) return null;
  return d;
}

/**
 * Format ISO string to "DD-MM-YYYY".
 * Returns null if input is not a valid date.
 */
export function formatDate(iso: string): string | null {
  const d = parseUtcIso(iso);
  if (!d) return null;

  const day = pad(d.getDate());
  const month = pad(d.getMonth() + 1);
  const year = d.getFullYear();

  return `${day}-${month}-${year}`;
}

/**
 * Format ISO string to "DD-MM-YYYY HH:mm" or "DD-MM-YYYY HH:mm:ss".
 * Options:
 *  - withSeconds: include seconds (default: false)
 *  - tz: "local" (default) or "utc" - choose which timezone to format in
 */
export function formatDateTime(iso: string): string | null {
  const d = parseUtcIso(iso);
  if (!d) return null;

  const day = pad(d.getDate());
  const month = pad(d.getMonth() + 1);
  const year = d.getFullYear();
  const hour = pad(d.getHours());
  const minute = pad(d.getMinutes());

  return `${day}-${month}-${year} ${hour}:${minute}`;
}

// export function formatDate(iso: string): string | null {
//   const d = tryCreateDate(iso);
//   if (!d) return null;
//   const day = pad(d.getDate());
//   const month = pad(d.getMonth() + 1);
//   const year = d.getFullYear();
//   return `${day}-${month}-${year}`;
// }

// export function formatDateTime(
//   iso: string,
//   opts?: { withSeconds?: boolean; tz?: "local" | "utc" }
// ): string | null {
//   const d = tryCreateDate(iso);
//   if (!d) return null;
//   const withSeconds = opts?.withSeconds ?? false;
//   const tz = opts?.tz ?? "local";

//   const get = (fnLocal: string, fnUtc: string) =>
//     tz === "utc" ? (d as any)[fnUtc]() : (d as any)[fnLocal]();

//   const day = pad(get("getDate", "getUTCDate"));
//   const month = pad(get("getMonth", "getUTCMonth") + 1);
//   const year = get("getFullYear", "getUTCFullYear");
//   const hour = pad(get("getHours", "getUTCHours"));
//   const minute = pad(get("getMinutes", "getUTCMinutes"));
//   const second = pad(get("getSeconds", "getUTCSeconds"));

//   return withSeconds
//     ? `${day}-${month}-${year} ${hour}:${minute}:${second}`
//     : `${day}-${month}-${year} ${hour}:${minute}`;
// }

/**
 * Get today's date in "YYYY-MM-DD" format.
 */
export function getToday() {
  const now = new Date();
  const year = now.getFullYear();
  const month = pad(now.getMonth() + 1);
  const day = pad(now.getDate());
  return `${year}-${month}-${day}`;
}

/**
 * Get the latest valid birth date (13 years ago from today) in "YYYY-MM-DD" format.
 */
export function getValidBirthDate() {
  const now = new Date();
  const year = now.getFullYear() - 13;
  const month = pad(now.getMonth() + 1);
  const day = pad(now.getDate());
  return `${year}-${month}-${day}`;
}

/*
 Example usage:
 const iso = "2025-11-23T08:18:30.7320749";
 toDate(iso); // Date object or null
 formatDate(iso); // "23-11-2025"
 formatDateTime(iso); // "23-11-2025 08:18"
 formatDateTime(iso, { withSeconds: true }); // "23-11-2025 08:18:30"
 formatDateTime(iso, { tz: 'utc' }); // use UTC components
*/
