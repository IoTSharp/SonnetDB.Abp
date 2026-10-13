# BookStore MVC + EF Core

> 本仓库已将这个官方 SQL Server 示例纳入为 **SonnetDB 适配示例 S01（A06）**，并加入根 SonnetDB.Abp.slnx。Web 与 DbMigrator 可通过 Database:Provider=SonnetDB 使用本仓库适配层；公开修复包已固定，标准初始迁移、账户 HTTP 与 Passkey 存储回归通过。其余模块、升级恢复及实际运行范围以 [DEMO.md](DEMO.md) 为准，第一批 13 个例子的迁移顺序见 [示例索引](../README.md)。下文保留官方原始运行说明作为来源记录。

This sample implements the ABP Book Store tutorial with an MVC/Razor Pages UI and Entity Framework Core. It is a layered ABP application with the usual `Domain`, `Application`, `EntityFrameworkCore`, `HttpApi`, `Web`, and `DbMigrator` projects.

Tutorial: https://abp.io/docs/latest/tutorials/book-store?UI=MVC&DB=EF

## Prerequisites

- .NET 10.0 SDK
- ABP CLI 3.0.10
- Node.js 22 or later
- SQL Server

## Initial Tasks

This project includes ABP Studio metadata and initial tasks under `etc/`.

- `Initialize Solution` runs `abp install-libs`, runs `Acme.BookStore.DbMigrator`, and creates the local OpenIddict development certificate.
- `Migrate Database` runs `Acme.BookStore.DbMigrator` to apply migrations and seed data.

In ABP Studio, open `Acme.BookStore.abpsln`, select the `Default` run profile, and run `Initialize Solution` before the first launch.

## Run the Application

After the initial task completes, start `Acme.BookStore.Web` from ABP Studio or run it manually:

```powershell
cd src/Acme.BookStore.Web
dotnet run
```

The MVC application runs at `https://localhost:44339`.

## Manual Setup

If you are not using ABP Studio, run these commands from the project root:

```powershell
abp install-libs
cd src/Acme.BookStore.DbMigrator
dotnet run
cd ../Acme.BookStore.Web
dotnet dev-certs https -v -ep openiddict.pfx -p 69d2731d-0125-4f47-9dbe-ce30ae744c46
dotnet run
```
