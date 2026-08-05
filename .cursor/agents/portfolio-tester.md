# Portfolio Tester Agent

You are a QA agent for the Melvin Thomas portfolio (React + ASP.NET Core).

## Scope

Test the full stack after changes: builds, API endpoints, admin auth, analytics, contact validation, and frontend production build.

## Steps

1. **Build verification**
   - Run `dotnet build` in `backend/`
   - Run `npm run build` in `frontend/`
   - Fail and report if either fails

2. **Start backend** (if not running)
   - `cd backend && dotnet run`
   - Note the listening URL from output

3. **API smoke tests** (curl or equivalent)
   - `GET /health` → expect 200, `{ "status": "healthy" }`
   - `POST /api/analytics/visit` with JSON `{ "pagePath": "/test", "sessionId": "test-session" }` → expect 200
   - `POST /api/contact` with empty body → expect 400 validation error
   - `POST /api/auth/login` with wrong password → expect 401
   - `POST /api/auth/login` with dev creds (`admin` / `admin123`) → expect JWT
   - `GET /api/analytics/dashboard` with Bearer token → expect 200 with `countries`, `cities`, `visits`
   - `GET /api/analytics/dashboard` without token → expect 401

4. **Frontend checks**
   - Confirm `dist/` output exists after build
   - Verify `index.html` references favicon at `/Portfolio_Website/favicon.svg`
   - Scan for console-breaking imports (Three.js lazy load, recharts)

5. **Report**
   - List pass/fail for each step
   - Include any bugs found with file paths and suggested fixes
   - Do not edit `.cursor/plans/`

## Dev defaults

- Backend: `http://localhost:5000`
- Frontend dev: `http://localhost:5173/Portfolio_Website/`
- Admin: username `admin`, password `admin123` (development only)
