using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.BookStore.SonnetDB.Migrations.Migrations;

/// <inheritdoc />
public partial class NativeDateTimeTicks5 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "RedemptionDate",
            table: "OpenIddictTokens",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExpirationDate",
            table: "OpenIddictTokens",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationDate",
            table: "OpenIddictTokens",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "OpenIddictScopes",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "OpenIddictScopes",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "OpenIddictScopes",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationDate",
            table: "OpenIddictAuthorizations",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "OpenIddictApplications",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "OpenIddictApplications",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "OpenIddictApplications",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "PublishDate",
            table: "AppBooks",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AppBooks",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AppBooks",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AppAuthors",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AppAuthors",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AppAuthors",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "BirthDate",
            table: "AppAuthors",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LockoutEnd",
            table: "AbpUsers",
            type: "DATETIMEOFFSET",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LastSignInTime",
            table: "AbpUsers",
            type: "DATETIMEOFFSET",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LastPasswordChangeTime",
            table: "AbpUsers",
            type: "DATETIMEOFFSET",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpUsers",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpUsers",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpUsers",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "AbpUserPasswordHistories",
            type: "DATETIMEOFFSET",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpUserOrganizationUnits",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "StartTime",
            table: "AbpUserDelegations",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "EndTime",
            table: "AbpUserDelegations",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpTenants",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpTenants",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpTenants",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "SignedIn",
            table: "AbpSessions",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastAccessed",
            table: "AbpSessions",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpSecurityLogs",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpRoles",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpOrganizationUnitRoles",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "ChangeTime",
            table: "AbpEntityChanges",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpClaimTypes",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "NextTryTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastTryTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CompletionTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExecutionTime",
            table: "AbpAuditLogs",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpAuditLogExcelFiles",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExecutionTime",
            table: "AbpAuditLogActions",
            type: "DATETIME2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "RedemptionDate",
            table: "OpenIddictTokens",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExpirationDate",
            table: "OpenIddictTokens",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationDate",
            table: "OpenIddictTokens",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "OpenIddictScopes",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "OpenIddictScopes",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "OpenIddictScopes",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationDate",
            table: "OpenIddictAuthorizations",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "OpenIddictApplications",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "OpenIddictApplications",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "OpenIddictApplications",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "PublishDate",
            table: "AppBooks",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AppBooks",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AppBooks",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AppAuthors",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AppAuthors",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AppAuthors",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "BirthDate",
            table: "AppAuthors",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LockoutEnd",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIMEOFFSET",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LastSignInTime",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIMEOFFSET",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "LastPasswordChangeTime",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIMEOFFSET",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpUsers",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "AbpUserPasswordHistories",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTimeOffset),
            oldType: "DATETIMEOFFSET");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpUserOrganizationUnits",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "StartTime",
            table: "AbpUserDelegations",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "EndTime",
            table: "AbpUserDelegations",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpTenants",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpTenants",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpTenants",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "SignedIn",
            table: "AbpSessions",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastAccessed",
            table: "AbpSessions",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpSecurityLogs",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpRoles",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastModificationTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "DeletionTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpOrganizationUnits",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpOrganizationUnitRoles",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "ChangeTime",
            table: "AbpEntityChanges",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpClaimTypes",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "NextTryTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "LastTryTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CompletionTime",
            table: "AbpBackgroundJobs",
            type: "DATETIME",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExecutionTime",
            table: "AbpAuditLogs",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreationTime",
            table: "AbpAuditLogExcelFiles",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");

        migrationBuilder.AlterColumn<DateTime>(
            name: "ExecutionTime",
            table: "AbpAuditLogActions",
            type: "DATETIME",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "DATETIME2");
    }
}
