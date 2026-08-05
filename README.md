<<<<<<< HEAD
# Melvin Thomas Portfolio

Recruiter-focused portfolio site built with **React (Vite)** on GitHub Pages and an **ASP.NET Core 8 Web API** for contact form delivery, visitor analytics, and a private admin dashboard.

**Live site:** [https://melvinthomas-dev.github.io/Portfolio_Website/](https://melvinthomas-dev.github.io/Portfolio_Website/)

## Architecture

| Layer | Tech | Hosting |
|-------|------|---------|
| Public portfolio | React + Vite | GitHub Pages |
| API + analytics + email | ASP.NET Core 8 Web API | Render or Azure App Service |
| Database | SQLite (dev) / PostgreSQL (prod) | With backend |

## Project structure

```
├── frontend/          # React portfolio + /admin dashboard
├── backend/           # ASP.NET Core Web API
├── .github/workflows/ # GitHub Actions (Pages deploy)
└── Melvin_Thomas_D.pdf
```

## Local development

### Prerequisites

- Node.js 20+
- .NET 8 SDK (or .NET 9 SDK with roll-forward — configured in the project)

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

API runs at `http://localhost:5000`.

**Dev admin login:** username `admin`, password `admin123`

### Frontend

```bash
cd frontend
npm install
npm run dev
```

App runs at `http://localhost:5173/Portfolio_Website/`

Copy `.env.example` to `.env.development` (already configured for local API):

```
VITE_API_BASE_URL=http://localhost:5000
```

### Build verification

```bash
cd backend && dotnet build
cd ../frontend && npm run build
```

## Features

- **Public portfolio** — About, Skills, Experience, Projects, Education, Certifications, Contact (content in `frontend/src/data/resumeContent.js`)
- **Contact form** — `POST /api/contact` with validation, IP geo enrichment, traffic source classification, email via SendGrid
- **Analytics beacon** — `POST /api/analytics/visit` on page load / route change (anonymous session UUID in localStorage)
- **Admin dashboard** — `/Portfolio_Website/admin` (not linked in nav); JWT auth; visit charts + contact submissions table

## API endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/contact` | — | Submit contact form |
| POST | `/api/analytics/visit` | — | Record page visit |
| POST | `/api/auth/login` | — | Admin login → JWT |
| GET | `/api/analytics/dashboard` | Bearer JWT | Dashboard aggregates |
| GET | `/health` | — | Health check |

## Environment variables

### Backend (production)

| Variable | Required | Description |
|----------|----------|-------------|
| `DATABASE_URL` | Prod | PostgreSQL connection URL (Render/Azure) |
| `JWT_SECRET` | Yes | Min 32 characters |
| `ADMIN_USERNAME` | Yes | Admin login username |
| `ADMIN_PASSWORD` | Yes* | Plain password (or use `ADMIN_PASSWORD_HASH`) |
| `ADMIN_PASSWORD_HASH` | Yes* | BCrypt hash of admin password |
| `SENDGRID_API_KEY` | Yes | SendGrid API key for contact emails |
| `CORS_ORIGIN` | Yes | `https://melvinthomas-dev.github.io` |
| `Email__ToEmail` | No | Defaults to `melthomas220@gmail.com` |

\* Provide either `ADMIN_PASSWORD` or `ADMIN_PASSWORD_HASH`.

Optional:

| Variable | Description |
|----------|-------------|
| `GeoLocation__IpInfoToken` | ipinfo.io token (default uses free ip-api.com) |

### Frontend (GitHub Actions / build)

| Variable | Description |
|----------|-------------|
| `VITE_API_BASE_URL` | Deployed API URL (e.g. `https://your-api.onrender.com`) |

Set as a GitHub repository **Variable** or **Secret** named `VITE_API_BASE_URL`.

## Deployment

### 1. Backend — Render (recommended, free tier)

1. Create a **Web Service** from this repo; set **Root Directory** to `backend`.
2. **Build command:** `dotnet publish -c Release -o out`
3. **Start command:** `dotnet out/Portfolio.Api.dll`
4. Add a **PostgreSQL** database; copy its **Internal Database URL** to `DATABASE_URL`.
5. Set environment variables (see table above). For CORS:
   ```
   CORS_ORIGIN=https://melvinthomas-dev.github.io
   ```
   Or in Render env:
   ```
   Cors__Origins=https://melvinthomas-dev.github.io
   ```
6. Generate admin password hash:
   ```bash
   dotnet run --project backend  # then use BCrypt or set ADMIN_PASSWORD temporarily
   ```
   Easiest: set `ADMIN_PASSWORD` to your chosen password in Render env.

7. Set `Email__Provider=SendGrid` and `Email__SendGridApiKey` (or `SENDGRID_API_KEY`).

### 2. Backend — Azure App Service

1. Create an **App Service** (Linux, .NET 8).
2. Deploy from GitHub Actions or `az webapp deploy`; set app path to `backend/`.
3. Create **Azure Database for PostgreSQL** or use **Azure SQL** (update connection in `Program.cs` if using SQL Server).
4. Configure **Application Settings** with the same env vars as Render.
5. Enable **HTTPS only**; add CORS origin for GitHub Pages.

### 3. Frontend — GitHub Pages

1. Push this repo to [Portfolio_Website](https://github.com/MelvinThomas-dev/Portfolio_Website).
2. In repo **Settings → Pages**, set source to **GitHub Actions**.
3. Set repository variable `VITE_API_BASE_URL` to your deployed API URL.
4. Push to `main` — workflow `.github/workflows/deploy-frontend.yml` builds and deploys automatically.

### LinkedIn sharing URL (with UTM tracking)

```
https://melvinthomas-dev.github.io/Portfolio_Website/?utm_source=linkedin&utm_medium=social
```

## Contact links

- **Email:** melthomas220@gmail.com
- **LinkedIn:** https://linkedin.com/in/melvin-thomas-developer
- **GitHub:** https://github.com/MelvinThomas-dev

## License

Private portfolio project — © Melvin Thomas D.
