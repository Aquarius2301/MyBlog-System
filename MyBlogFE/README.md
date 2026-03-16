# MyBlogFE

MyBlogFE is the frontend Single Page Application for MyBlog. It is built with React (TypeScript) and Vite, uses Ant Design for UI and TanStack Query for data fetching/caching.

This README describes how to develop, build and deploy the frontend locally and which environment variables are supported.

## Project structure (high level)

- `src/` — source code (React components, routes, contexts, hooks, API helpers).
- `src/main.tsx` — application entry (providers: theme, auth, react-query).
- `src/api/config.api.ts` — Axios instance and API configuration (base URL uses `VITE_API_BASE_URL`).
- `vite.config.ts` — Vite configuration (aliases and server options).

## Prerequisites

- Node.js 18+ and npm (or equivalent package manager).
- A running backend API (the frontend defaults to `http://localhost:5250` if `VITE_API_BASE_URL` is not set).

Check Node version:

```powershell
node --version
```

## Available scripts

From the project root (`MyBlogFE`):

- `npm run dev` — start Vite dev server (hot reload). Default port is 5173.
- `npm run build` — TypeScript build (`tsc -b`) then `vite build` to produce production assets.
- `npm run preview` — locally preview the production build via `vite preview`.
- `npm run start` — run Vite with `--host 0.0.0.0` (useful inside containers).
- `npm run lint` — run ESLint.

Example (PowerShell):

```powershell
cd .\MyBlogFE
npm install
npm run dev
```

Open `http://localhost:5173` in your browser (or the URL printed by Vite).

## Environment variables

The frontend reads configuration from Vite environment variables (prefix `VITE_`). Key variables:

- `VITE_API_BASE_URL` — Base URL for backend API. If not set, the code falls back to `http://localhost:5250`.

Create a `.env` or `.env.local` in the `MyBlogFE` folder for local overrides, e.g.:

```
VITE_API_BASE_URL=http://localhost:5250
```

After changing `.env` values you must restart the dev server.

## API configuration

- `src/api/config.api.ts` constructs an Axios instance with `baseURL` = `import.meta.env.VITE_API_BASE_URL || "http://localhost:5250"`.
- Requests use `withCredentials: true` to send HttpOnly cookies (important for cookie-based auth flows).
- Response interceptor handles 401 by attempting a token refresh at `/api/auth/refresh` and redirecting to `/login` when refresh fails.

## Build & deploy

1. Build:

```powershell
cd .\MyBlogFE
npm run build
```

2. Preview locally:

```powershell
npm run preview
```

3. Serve static build on a web server (Nginx, static host). Vite build outputs a `dist/` folder. Configure your host to serve from that folder.

If you deploy the frontend separately from the backend, ensure the backend CORS configuration allows the frontend origin (see `MyBlogBE/WebApi/appsettings.json:BaseSettings:FrontendUrl`).

## Docker (optional)

There is a `dockerfile` in the repository for the frontend (root `MyBlogFE/dockerfile`). Typical flow:

```powershell
cd .\MyBlogFE
docker build -t myblog-fe .
docker run -p 5173:5173 -e VITE_API_BASE_URL="https://api.myblog.example" myblog-fe
```

For production you usually build the app and serve static files from an optimized server (nginx, cloud storage, or platform static hosting).

## Developer notes

- Uses TypeScript and path alias `@` configured in `vite.config.ts` to point to `src/`.
- UI library: Ant Design v6; theming handled in `src/contexts` and `src/hooks/useTheme.ts`.
- Data fetching: TanStack Query (React Query) with `QueryClient` and devtools included.
- i18n: `i18next` + `react-i18next` present for localization.

## Linting & Formatting

- ESLint is configured with `npm run lint`. Consider adding Prettier if you want consistent formatting.

---
