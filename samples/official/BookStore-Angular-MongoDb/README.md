# BookStore Angular + MongoDB

This sample implements the ABP Book Store tutorial with an Angular UI and MongoDB persistence. The backend is under `aspnet-core/`; the Angular application is under `angular/`.

Tutorial: https://abp.io/docs/latest/tutorials/book-store?UI=NG&DB=Mongo

## Prerequisites

- .NET 10.0 SDK
- ABP CLI 3.0.10
- Node.js 22 or later
- MongoDB

## Initial Tasks

This project includes ABP Studio metadata and initial tasks under `etc/`.

- `Initialize Solution` runs `abp install-libs` in `aspnet-core/`, runs `npm install` in `angular/`, and runs `Acme.BookStore.DbMigrator` to seed MongoDB.
- `Migrate Database` runs `Acme.BookStore.DbMigrator` to seed/update MongoDB data.

In ABP Studio, open `Acme.BookStore.abpsln`, select the `Default` run profile, and run `Initialize Solution` before the first launch.

## Run the Application

After the initial task completes, start these applications from ABP Studio:

- `Acme.BookStore.HttpApi.Host` at `https://localhost:44367`
- `Acme.BookStore.Angular` at `http://localhost:4200`

Manual run commands:

```powershell
cd aspnet-core/src/Acme.BookStore.HttpApi.Host
dotnet run
```

In a second terminal:

```powershell
cd angular
.\start.ps1
```

## Manual Setup

If you are not using ABP Studio, run these commands from the project root:

```powershell
cd aspnet-core
abp install-libs
cd ../angular
npm install
cd ../aspnet-core/src/Acme.BookStore.DbMigrator
dotnet run
```
