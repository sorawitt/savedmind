# SavedMind

[![CI](https://github.com/sorawitt/savedmind/actions/workflows/ci.yml/badge.svg)](https://github.com/sorawitt/savedmind/actions/workflows/ci.yml)

Personal bookmark manager that actually helps you find stuff later.

## The Problem

You save hundreds of articles, tweets, and videos. When you need them? Gone. Can't remember the title, forgot which folder, keyword search fails because you only remember "that React article with the cool diagram."

## The Solution

Save with one click. Search with natural language. AI handles the organizing.

## How It Works

1. **Save** - Browser extension, one click, no folders or tags required
2. **Process** - System extracts content and understands what it's about
3. **Find** - Search like you think: "React article with diagram about performance"

## Tech Stack

- **Backend**: .NET 10, PostgreSQL, pgvector
- **Frontend**: Preact, Tailwind CSS
- **Extension**: Plasmo (Chrome)
- **AI**: OpenAI embeddings for semantic search

## Project Structure

```
savedmind/
├── backend/           # .NET 10 API (Clean Architecture)
│   └── src/
│       ├── SavedMind.Api/
│       ├── SavedMind.Application/
│       ├── SavedMind.Domain/
│       └── SavedMind.Infrastructure/
├── frontend/          # Preact + Vite + Tailwind
├── extension/         # Plasmo Chrome extension
├── shared/            # Shared TypeScript types
└── docker-compose.yml # PostgreSQL + pgvector
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Bun](https://bun.sh/) (JavaScript runtime & package manager)
- [Docker](https://www.docker.com/)

### Quick Start

```bash
# Clone
git clone https://github.com/YOUR_USERNAME/savedmind.git
cd savedmind

# Install root dependencies (husky + commitlint)
bun install

# Start database
docker-compose up -d

# Backend
cd backend && dotnet run --project src/SavedMind.Api

# Frontend (new terminal)
cd frontend && bun install && bun run dev

# Extension (new terminal)
cd extension && bun install && bun run dev
```

### Development URLs

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000
- **Database**: localhost:5432

## Commit Convention

We use [Conventional Commits](https://www.conventionalcommits.org/). Commits are validated by commitlint via husky.

```
<type>(<scope>): <subject>

# Examples:
feat(api): add bookmark search endpoint
fix(extension): resolve popup not closing
docs: update README with setup instructions
chore: update dependencies
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `ci`, `revert`

## Status

MVP in development. See [Projects](../../projects) for current progress.

## License

MIT
