using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.BookStore.SonnetDB.Migrations.Migrations;

/// <summary>为官方 BookStore 完整模型创建 SonnetDB 初始架构。</summary>
public partial class InitialSonnetDB : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AbpAuditLogExcelFiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                FileName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpAuditLogExcelFiles", x => x.Id);
                table.CheckConstraint("CK_AbpAuditLogExcelFiles_FileName_MaxLength", "\"FileName\" IS NULL OR char_length(\"FileName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpAuditLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                ApplicationName = table.Column<string>(type: "STRING", maxLength: 96, nullable: true),
                UserId = table.Column<Guid>(type: "STRING", nullable: true),
                UserName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                TenantName = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ImpersonatorUserId = table.Column<Guid>(type: "STRING", nullable: true),
                ImpersonatorUserName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                ImpersonatorTenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ImpersonatorTenantName = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ExecutionTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ExecutionDuration = table.Column<int>(type: "INT", nullable: false),
                ClientIpAddress = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ClientName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                ClientId = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                CorrelationId = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                BrowserInfo = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                HttpMethod = table.Column<string>(type: "STRING", maxLength: 16, nullable: true),
                Url = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                Exceptions = table.Column<string>(type: "STRING", nullable: true),
                Comments = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                HttpStatusCode = table.Column<int>(type: "INT", nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                table.CheckConstraint("CK_AbpAuditLogs_ApplicationName_MaxLength", "\"ApplicationName\" IS NULL OR char_length(\"ApplicationName\") <= 96");
                table.CheckConstraint("CK_AbpAuditLogs_BrowserInfo_MaxLength", "\"BrowserInfo\" IS NULL OR char_length(\"BrowserInfo\") <= 512");
                table.CheckConstraint("CK_AbpAuditLogs_ClientId_MaxLength", "\"ClientId\" IS NULL OR char_length(\"ClientId\") <= 64");
                table.CheckConstraint("CK_AbpAuditLogs_ClientIpAddress_MaxLength", "\"ClientIpAddress\" IS NULL OR char_length(\"ClientIpAddress\") <= 64");
                table.CheckConstraint("CK_AbpAuditLogs_ClientName_MaxLength", "\"ClientName\" IS NULL OR char_length(\"ClientName\") <= 128");
                table.CheckConstraint("CK_AbpAuditLogs_Comments_MaxLength", "\"Comments\" IS NULL OR char_length(\"Comments\") <= 256");
                table.CheckConstraint("CK_AbpAuditLogs_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpAuditLogs_CorrelationId_MaxLength", "\"CorrelationId\" IS NULL OR char_length(\"CorrelationId\") <= 64");
                table.CheckConstraint("CK_AbpAuditLogs_HttpMethod_MaxLength", "\"HttpMethod\" IS NULL OR char_length(\"HttpMethod\") <= 16");
                table.CheckConstraint("CK_AbpAuditLogs_ImpersonatorTenantName_MaxLength", "\"ImpersonatorTenantName\" IS NULL OR char_length(\"ImpersonatorTenantName\") <= 64");
                table.CheckConstraint("CK_AbpAuditLogs_ImpersonatorUserName_MaxLength", "\"ImpersonatorUserName\" IS NULL OR char_length(\"ImpersonatorUserName\") <= 256");
                table.CheckConstraint("CK_AbpAuditLogs_TenantName_MaxLength", "\"TenantName\" IS NULL OR char_length(\"TenantName\") <= 64");
                table.CheckConstraint("CK_AbpAuditLogs_Url_MaxLength", "\"Url\" IS NULL OR char_length(\"Url\") <= 256");
                table.CheckConstraint("CK_AbpAuditLogs_UserName_MaxLength", "\"UserName\" IS NULL OR char_length(\"UserName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpBackgroundJobs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                ApplicationName = table.Column<string>(type: "STRING", maxLength: 96, nullable: true),
                JobName = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                JobArgs = table.Column<string>(type: "STRING", maxLength: 1048576, nullable: false),
                TryCount = table.Column<short>(type: "INT", nullable: false, defaultValue: (short)0),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                NextTryTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                LastTryTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                IsAbandoned = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                CompletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                Priority = table.Column<byte>(type: "INT", nullable: false, defaultValue: (byte)15),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                table.CheckConstraint("CK_AbpBackgroundJobs_ApplicationName_MaxLength", "\"ApplicationName\" IS NULL OR char_length(\"ApplicationName\") <= 96");
                table.CheckConstraint("CK_AbpBackgroundJobs_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpBackgroundJobs_JobArgs_MaxLength", "\"JobArgs\" IS NULL OR char_length(\"JobArgs\") <= 1048576");
                table.CheckConstraint("CK_AbpBackgroundJobs_JobName_MaxLength", "\"JobName\" IS NULL OR char_length(\"JobName\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpClaimTypes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                Required = table.Column<bool>(type: "BOOL", nullable: false),
                IsStatic = table.Column<bool>(type: "BOOL", nullable: false),
                Regex = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                RegexDescription = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                Description = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                ValueType = table.Column<int>(type: "INT", nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpClaimTypes", x => x.Id);
                table.CheckConstraint("CK_AbpClaimTypes_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpClaimTypes_Description_MaxLength", "\"Description\" IS NULL OR char_length(\"Description\") <= 256");
                table.CheckConstraint("CK_AbpClaimTypes_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 256");
                table.CheckConstraint("CK_AbpClaimTypes_Regex_MaxLength", "\"Regex\" IS NULL OR char_length(\"Regex\") <= 512");
                table.CheckConstraint("CK_AbpClaimTypes_RegexDescription_MaxLength", "\"RegexDescription\" IS NULL OR char_length(\"RegexDescription\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpFeatureGroups",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                table.CheckConstraint("CK_AbpFeatureGroups_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 256");
                table.CheckConstraint("CK_AbpFeatureGroups_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpFeatures",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                GroupName = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                ParentName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                DefaultValue = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                IsVisibleToClients = table.Column<bool>(type: "BOOL", nullable: false),
                IsAvailableToHost = table.Column<bool>(type: "BOOL", nullable: false),
                AllowedProviders = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                ValueType = table.Column<string>(type: "STRING", maxLength: 2048, nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                table.CheckConstraint("CK_AbpFeatures_AllowedProviders_MaxLength", "\"AllowedProviders\" IS NULL OR char_length(\"AllowedProviders\") <= 256");
                table.CheckConstraint("CK_AbpFeatures_DefaultValue_MaxLength", "\"DefaultValue\" IS NULL OR char_length(\"DefaultValue\") <= 256");
                table.CheckConstraint("CK_AbpFeatures_Description_MaxLength", "\"Description\" IS NULL OR char_length(\"Description\") <= 256");
                table.CheckConstraint("CK_AbpFeatures_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 256");
                table.CheckConstraint("CK_AbpFeatures_GroupName_MaxLength", "\"GroupName\" IS NULL OR char_length(\"GroupName\") <= 128");
                table.CheckConstraint("CK_AbpFeatures_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpFeatures_ParentName_MaxLength", "\"ParentName\" IS NULL OR char_length(\"ParentName\") <= 128");
                table.CheckConstraint("CK_AbpFeatures_ValueType_MaxLength", "\"ValueType\" IS NULL OR char_length(\"ValueType\") <= 2048");
            });

        migrationBuilder.CreateTable(
            name: "AbpFeatureValues",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                ProviderName = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ProviderKey = table.Column<string>(type: "STRING", maxLength: 64, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpFeatureValues", x => x.Id);
                table.CheckConstraint("CK_AbpFeatureValues_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpFeatureValues_ProviderKey_MaxLength", "\"ProviderKey\" IS NULL OR char_length(\"ProviderKey\") <= 64");
                table.CheckConstraint("CK_AbpFeatureValues_ProviderName_MaxLength", "\"ProviderName\" IS NULL OR char_length(\"ProviderName\") <= 64");
                table.CheckConstraint("CK_AbpFeatureValues_Value_MaxLength", "\"Value\" IS NULL OR char_length(\"Value\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpLinkUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                SourceUserId = table.Column<Guid>(type: "STRING", nullable: false),
                SourceTenantId = table.Column<Guid>(type: "STRING", nullable: true),
                TargetUserId = table.Column<Guid>(type: "STRING", nullable: false),
                TargetTenantId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpLinkUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AbpOrganizationUnits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ParentId = table.Column<Guid>(type: "STRING", nullable: true),
                Code = table.Column<string>(type: "STRING", maxLength: 95, nullable: false),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                EntityVersion = table.Column<int>(type: "INT", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                table.CheckConstraint("CK_AbpOrganizationUnits_Code_MaxLength", "\"Code\" IS NULL OR char_length(\"Code\") <= 95");
                table.CheckConstraint("CK_AbpOrganizationUnits_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpOrganizationUnits_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 128");
                table.ForeignKey(
                    name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                    column: x => x.ParentId,
                    principalTable: "AbpOrganizationUnits",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "AbpPermissionGrants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                ProviderName = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                ProviderKey = table.Column<string>(type: "STRING", maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpPermissionGrants", x => x.Id);
                table.CheckConstraint("CK_AbpPermissionGrants_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpPermissionGrants_ProviderKey_MaxLength", "\"ProviderKey\" IS NULL OR char_length(\"ProviderKey\") <= 64");
                table.CheckConstraint("CK_AbpPermissionGrants_ProviderName_MaxLength", "\"ProviderName\" IS NULL OR char_length(\"ProviderName\") <= 64");
            });

        migrationBuilder.CreateTable(
            name: "AbpPermissionGroups",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                table.CheckConstraint("CK_AbpPermissionGroups_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 256");
                table.CheckConstraint("CK_AbpPermissionGroups_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpPermissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                GroupName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                ResourceName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                ManagementPermissionName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                ParentName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                IsEnabled = table.Column<bool>(type: "BOOL", nullable: false),
                MultiTenancySide = table.Column<byte>(type: "INT", nullable: false),
                Providers = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                StateCheckers = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                table.CheckConstraint("CK_AbpPermissions_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 256");
                table.CheckConstraint("CK_AbpPermissions_GroupName_MaxLength", "\"GroupName\" IS NULL OR char_length(\"GroupName\") <= 128");
                table.CheckConstraint("CK_AbpPermissions_ManagementPermissionName_MaxLength", "\"ManagementPermissionName\" IS NULL OR char_length(\"ManagementPermissionName\") <= 128");
                table.CheckConstraint("CK_AbpPermissions_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpPermissions_ParentName_MaxLength", "\"ParentName\" IS NULL OR char_length(\"ParentName\") <= 128");
                table.CheckConstraint("CK_AbpPermissions_Providers_MaxLength", "\"Providers\" IS NULL OR char_length(\"Providers\") <= 128");
                table.CheckConstraint("CK_AbpPermissions_ResourceName_MaxLength", "\"ResourceName\" IS NULL OR char_length(\"ResourceName\") <= 256");
                table.CheckConstraint("CK_AbpPermissions_StateCheckers_MaxLength", "\"StateCheckers\" IS NULL OR char_length(\"StateCheckers\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpResourcePermissionGrants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                ProviderName = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                ProviderKey = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                ResourceName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                ResourceKey = table.Column<string>(type: "STRING", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpResourcePermissionGrants", x => x.Id);
                table.CheckConstraint("CK_AbpResourcePermissionGrants_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpResourcePermissionGrants_ProviderKey_MaxLength", "\"ProviderKey\" IS NULL OR char_length(\"ProviderKey\") <= 64");
                table.CheckConstraint("CK_AbpResourcePermissionGrants_ProviderName_MaxLength", "\"ProviderName\" IS NULL OR char_length(\"ProviderName\") <= 64");
                table.CheckConstraint("CK_AbpResourcePermissionGrants_ResourceKey_MaxLength", "\"ResourceKey\" IS NULL OR char_length(\"ResourceKey\") <= 256");
                table.CheckConstraint("CK_AbpResourcePermissionGrants_ResourceName_MaxLength", "\"ResourceName\" IS NULL OR char_length(\"ResourceName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpRoles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                Name = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                NormalizedName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                IsDefault = table.Column<bool>(type: "BOOL", nullable: false),
                IsStatic = table.Column<bool>(type: "BOOL", nullable: false),
                IsPublic = table.Column<bool>(type: "BOOL", nullable: false),
                EntityVersion = table.Column<int>(type: "INT", nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpRoles", x => x.Id);
                table.CheckConstraint("CK_AbpRoles_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpRoles_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 256");
                table.CheckConstraint("CK_AbpRoles_NormalizedName_MaxLength", "\"NormalizedName\" IS NULL OR char_length(\"NormalizedName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpSecurityLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ApplicationName = table.Column<string>(type: "STRING", maxLength: 96, nullable: true),
                Identity = table.Column<string>(type: "STRING", maxLength: 96, nullable: true),
                Action = table.Column<string>(type: "STRING", maxLength: 96, nullable: true),
                UserId = table.Column<Guid>(type: "STRING", nullable: true),
                UserName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                TenantName = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ClientId = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                CorrelationId = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ClientIpAddress = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                BrowserInfo = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpSecurityLogs", x => x.Id);
                table.CheckConstraint("CK_AbpSecurityLogs_Action_MaxLength", "\"Action\" IS NULL OR char_length(\"Action\") <= 96");
                table.CheckConstraint("CK_AbpSecurityLogs_ApplicationName_MaxLength", "\"ApplicationName\" IS NULL OR char_length(\"ApplicationName\") <= 96");
                table.CheckConstraint("CK_AbpSecurityLogs_BrowserInfo_MaxLength", "\"BrowserInfo\" IS NULL OR char_length(\"BrowserInfo\") <= 512");
                table.CheckConstraint("CK_AbpSecurityLogs_ClientId_MaxLength", "\"ClientId\" IS NULL OR char_length(\"ClientId\") <= 64");
                table.CheckConstraint("CK_AbpSecurityLogs_ClientIpAddress_MaxLength", "\"ClientIpAddress\" IS NULL OR char_length(\"ClientIpAddress\") <= 64");
                table.CheckConstraint("CK_AbpSecurityLogs_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpSecurityLogs_CorrelationId_MaxLength", "\"CorrelationId\" IS NULL OR char_length(\"CorrelationId\") <= 64");
                table.CheckConstraint("CK_AbpSecurityLogs_Identity_MaxLength", "\"Identity\" IS NULL OR char_length(\"Identity\") <= 96");
                table.CheckConstraint("CK_AbpSecurityLogs_TenantName_MaxLength", "\"TenantName\" IS NULL OR char_length(\"TenantName\") <= 64");
                table.CheckConstraint("CK_AbpSecurityLogs_UserName_MaxLength", "\"UserName\" IS NULL OR char_length(\"UserName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AbpSessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                SessionId = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                Device = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                DeviceInfo = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                ClientId = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                IpAddresses = table.Column<string>(type: "STRING", maxLength: 2048, nullable: true),
                SignedIn = table.Column<DateTime>(type: "DATETIME", nullable: false),
                LastAccessed = table.Column<DateTime>(type: "DATETIME", nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpSessions", x => x.Id);
                table.CheckConstraint("CK_AbpSessions_ClientId_MaxLength", "\"ClientId\" IS NULL OR char_length(\"ClientId\") <= 64");
                table.CheckConstraint("CK_AbpSessions_Device_MaxLength", "\"Device\" IS NULL OR char_length(\"Device\") <= 64");
                table.CheckConstraint("CK_AbpSessions_DeviceInfo_MaxLength", "\"DeviceInfo\" IS NULL OR char_length(\"DeviceInfo\") <= 256");
                table.CheckConstraint("CK_AbpSessions_IpAddresses_MaxLength", "\"IpAddresses\" IS NULL OR char_length(\"IpAddresses\") <= 2048");
                table.CheckConstraint("CK_AbpSessions_SessionId_MaxLength", "\"SessionId\" IS NULL OR char_length(\"SessionId\") <= 128");
            });

        migrationBuilder.CreateTable(
            name: "AbpSettingDefinitions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                DisplayName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                DefaultValue = table.Column<string>(type: "STRING", maxLength: 2048, nullable: true),
                IsVisibleToClients = table.Column<bool>(type: "BOOL", nullable: false),
                Providers = table.Column<string>(type: "STRING", maxLength: 1024, nullable: true),
                IsInherited = table.Column<bool>(type: "BOOL", nullable: false),
                IsEncrypted = table.Column<bool>(type: "BOOL", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpSettingDefinitions", x => x.Id);
                table.CheckConstraint("CK_AbpSettingDefinitions_DefaultValue_MaxLength", "\"DefaultValue\" IS NULL OR char_length(\"DefaultValue\") <= 2048");
                table.CheckConstraint("CK_AbpSettingDefinitions_Description_MaxLength", "\"Description\" IS NULL OR char_length(\"Description\") <= 512");
                table.CheckConstraint("CK_AbpSettingDefinitions_DisplayName_MaxLength", "\"DisplayName\" IS NULL OR char_length(\"DisplayName\") <= 256");
                table.CheckConstraint("CK_AbpSettingDefinitions_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpSettingDefinitions_Providers_MaxLength", "\"Providers\" IS NULL OR char_length(\"Providers\") <= 1024");
            });

        migrationBuilder.CreateTable(
            name: "AbpSettings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "STRING", maxLength: 2048, nullable: false),
                ProviderName = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                ProviderKey = table.Column<string>(type: "STRING", maxLength: 64, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpSettings", x => x.Id);
                table.CheckConstraint("CK_AbpSettings_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.CheckConstraint("CK_AbpSettings_ProviderKey_MaxLength", "\"ProviderKey\" IS NULL OR char_length(\"ProviderKey\") <= 64");
                table.CheckConstraint("CK_AbpSettings_ProviderName_MaxLength", "\"ProviderName\" IS NULL OR char_length(\"ProviderName\") <= 64");
                table.CheckConstraint("CK_AbpSettings_Value_MaxLength", "\"Value\" IS NULL OR char_length(\"Value\") <= 2048");
            });

        migrationBuilder.CreateTable(
            name: "AbpTenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                NormalizedName = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                EntityVersion = table.Column<int>(type: "INT", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpTenants", x => x.Id);
                table.CheckConstraint("CK_AbpTenants_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpTenants_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 64");
                table.CheckConstraint("CK_AbpTenants_NormalizedName_MaxLength", "\"NormalizedName\" IS NULL OR char_length(\"NormalizedName\") <= 64");
            });

        migrationBuilder.CreateTable(
            name: "AbpUserDelegations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                SourceUserId = table.Column<Guid>(type: "STRING", nullable: false),
                TargetUserId = table.Column<Guid>(type: "STRING", nullable: false),
                StartTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                EndTime = table.Column<DateTime>(type: "DATETIME", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserDelegations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AbpUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                UserName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                NormalizedUserName = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                Surname = table.Column<string>(type: "STRING", maxLength: 64, nullable: true),
                Email = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                NormalizedEmail = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                EmailConfirmed = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                PasswordHash = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                SecurityStamp = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                IsExternal = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                PhoneNumber = table.Column<string>(type: "STRING", maxLength: 16, nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                IsActive = table.Column<bool>(type: "BOOL", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "DATETIME", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                AccessFailedCount = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "BOOL", nullable: false),
                EntityVersion = table.Column<int>(type: "INT", nullable: false),
                LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "DATETIME", nullable: true),
                LastSignInTime = table.Column<DateTimeOffset>(type: "DATETIME", nullable: true),
                Leaved = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUsers", x => x.Id);
                table.CheckConstraint("CK_AbpUsers_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AbpUsers_Email_MaxLength", "\"Email\" IS NULL OR char_length(\"Email\") <= 256");
                table.CheckConstraint("CK_AbpUsers_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 64");
                table.CheckConstraint("CK_AbpUsers_NormalizedEmail_MaxLength", "\"NormalizedEmail\" IS NULL OR char_length(\"NormalizedEmail\") <= 256");
                table.CheckConstraint("CK_AbpUsers_NormalizedUserName_MaxLength", "\"NormalizedUserName\" IS NULL OR char_length(\"NormalizedUserName\") <= 256");
                table.CheckConstraint("CK_AbpUsers_PasswordHash_MaxLength", "\"PasswordHash\" IS NULL OR char_length(\"PasswordHash\") <= 256");
                table.CheckConstraint("CK_AbpUsers_PhoneNumber_MaxLength", "\"PhoneNumber\" IS NULL OR char_length(\"PhoneNumber\") <= 16");
                table.CheckConstraint("CK_AbpUsers_SecurityStamp_MaxLength", "\"SecurityStamp\" IS NULL OR char_length(\"SecurityStamp\") <= 256");
                table.CheckConstraint("CK_AbpUsers_Surname_MaxLength", "\"Surname\" IS NULL OR char_length(\"Surname\") <= 64");
                table.CheckConstraint("CK_AbpUsers_UserName_MaxLength", "\"UserName\" IS NULL OR char_length(\"UserName\") <= 256");
            });

        migrationBuilder.CreateTable(
            name: "AppAuthors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                BirthDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ShortBio = table.Column<string>(type: "STRING", nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppAuthors", x => x.Id);
                table.CheckConstraint("CK_AppAuthors_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AppAuthors_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 64");
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictApplications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                ApplicationType = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                ClientId = table.Column<string>(type: "STRING", maxLength: 100, nullable: true),
                ClientSecret = table.Column<string>(type: "STRING", nullable: true),
                ClientType = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                ConsentType = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                DisplayName = table.Column<string>(type: "STRING", nullable: true),
                DisplayNames = table.Column<string>(type: "STRING", nullable: true),
                JsonWebKeySet = table.Column<string>(type: "STRING", nullable: true),
                Permissions = table.Column<string>(type: "STRING", nullable: true),
                PostLogoutRedirectUris = table.Column<string>(type: "STRING", nullable: true),
                Properties = table.Column<string>(type: "STRING", nullable: true),
                RedirectUris = table.Column<string>(type: "STRING", nullable: true),
                Requirements = table.Column<string>(type: "STRING", nullable: true),
                Settings = table.Column<string>(type: "STRING", nullable: true),
                FrontChannelLogoutUri = table.Column<string>(type: "STRING", nullable: true),
                ClientUri = table.Column<string>(type: "STRING", nullable: true),
                LogoUri = table.Column<string>(type: "STRING", nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                table.CheckConstraint("CK_OpenIddictApplications_ApplicationType_MaxLength", "\"ApplicationType\" IS NULL OR char_length(\"ApplicationType\") <= 50");
                table.CheckConstraint("CK_OpenIddictApplications_ClientId_MaxLength", "\"ClientId\" IS NULL OR char_length(\"ClientId\") <= 100");
                table.CheckConstraint("CK_OpenIddictApplications_ClientType_MaxLength", "\"ClientType\" IS NULL OR char_length(\"ClientType\") <= 50");
                table.CheckConstraint("CK_OpenIddictApplications_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_OpenIddictApplications_ConsentType_MaxLength", "\"ConsentType\" IS NULL OR char_length(\"ConsentType\") <= 50");
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictScopes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Description = table.Column<string>(type: "STRING", nullable: true),
                Descriptions = table.Column<string>(type: "STRING", nullable: true),
                DisplayName = table.Column<string>(type: "STRING", nullable: true),
                DisplayNames = table.Column<string>(type: "STRING", nullable: true),
                Name = table.Column<string>(type: "STRING", maxLength: 200, nullable: true),
                Properties = table.Column<string>(type: "STRING", nullable: true),
                Resources = table.Column<string>(type: "STRING", nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true),
                IsDeleted = table.Column<bool>(type: "BOOL", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "STRING", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                table.CheckConstraint("CK_OpenIddictScopes_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_OpenIddictScopes_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 200");
            });

        migrationBuilder.CreateTable(
            name: "AbpAuditLogActions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                AuditLogId = table.Column<Guid>(type: "STRING", nullable: false),
                ServiceName = table.Column<string>(type: "STRING", maxLength: 256, nullable: true),
                MethodName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                Parameters = table.Column<string>(type: "STRING", maxLength: 2000, nullable: true),
                ExecutionTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ExecutionDuration = table.Column<int>(type: "INT", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpAuditLogActions", x => x.Id);
                table.CheckConstraint("CK_AbpAuditLogActions_MethodName_MaxLength", "\"MethodName\" IS NULL OR char_length(\"MethodName\") <= 128");
                table.CheckConstraint("CK_AbpAuditLogActions_Parameters_MaxLength", "\"Parameters\" IS NULL OR char_length(\"Parameters\") <= 2000");
                table.CheckConstraint("CK_AbpAuditLogActions_ServiceName_MaxLength", "\"ServiceName\" IS NULL OR char_length(\"ServiceName\") <= 256");
                table.ForeignKey(
                    name: "FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId",
                    column: x => x.AuditLogId,
                    principalTable: "AbpAuditLogs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpEntityChanges",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                AuditLogId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ChangeTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                ChangeType = table.Column<byte>(type: "INT", nullable: false),
                EntityTenantId = table.Column<Guid>(type: "STRING", nullable: true),
                EntityId = table.Column<string>(type: "STRING", maxLength: 128, nullable: true),
                EntityTypeFullName = table.Column<string>(type: "STRING", maxLength: 512, nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                table.CheckConstraint("CK_AbpEntityChanges_EntityId_MaxLength", "\"EntityId\" IS NULL OR char_length(\"EntityId\") <= 128");
                table.CheckConstraint("CK_AbpEntityChanges_EntityTypeFullName_MaxLength", "\"EntityTypeFullName\" IS NULL OR char_length(\"EntityTypeFullName\") <= 512");
                table.ForeignKey(
                    name: "FK_AbpEntityChanges_AbpAuditLogs_AuditLogId",
                    column: x => x.AuditLogId,
                    principalTable: "AbpAuditLogs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpOrganizationUnitRoles",
            columns: table => new
            {
                RoleId = table.Column<Guid>(type: "STRING", nullable: false),
                OrganizationUnitId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId",
                    column: x => x.OrganizationUnitId,
                    principalTable: "AbpOrganizationUnits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AbpRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpRoleClaims",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                RoleId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ClaimType = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                ClaimValue = table.Column<string>(type: "STRING", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                table.CheckConstraint("CK_AbpRoleClaims_ClaimType_MaxLength", "\"ClaimType\" IS NULL OR char_length(\"ClaimType\") <= 256");
                table.CheckConstraint("CK_AbpRoleClaims_ClaimValue_MaxLength", "\"ClaimValue\" IS NULL OR char_length(\"ClaimValue\") <= 1024");
                table.ForeignKey(
                    name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AbpRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpTenantConnectionStrings",
            columns: table => new
            {
                TenantId = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                Value = table.Column<string>(type: "STRING", maxLength: 1024, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpTenantConnectionStrings", x => new { x.TenantId, x.Name });
                table.CheckConstraint("CK_AbpTenantConnectionStrings_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 64");
                table.CheckConstraint("CK_AbpTenantConnectionStrings_Value_MaxLength", "\"Value\" IS NULL OR char_length(\"Value\") <= 1024");
                table.ForeignKey(
                    name: "FK_AbpTenantConnectionStrings_AbpTenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "AbpTenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserClaims",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ClaimType = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                ClaimValue = table.Column<string>(type: "STRING", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                table.CheckConstraint("CK_AbpUserClaims_ClaimType_MaxLength", "\"ClaimType\" IS NULL OR char_length(\"ClaimType\") <= 256");
                table.CheckConstraint("CK_AbpUserClaims_ClaimValue_MaxLength", "\"ClaimValue\" IS NULL OR char_length(\"ClaimValue\") <= 1024");
                table.ForeignKey(
                    name: "FK_AbpUserClaims_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserLogins",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                LoginProvider = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                ProviderKey = table.Column<string>(type: "STRING", maxLength: 196, nullable: false),
                ProviderDisplayName = table.Column<string>(type: "STRING", maxLength: 128, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserLogins", x => new { x.UserId, x.LoginProvider });
                table.CheckConstraint("CK_AbpUserLogins_LoginProvider_MaxLength", "\"LoginProvider\" IS NULL OR char_length(\"LoginProvider\") <= 64");
                table.CheckConstraint("CK_AbpUserLogins_ProviderDisplayName_MaxLength", "\"ProviderDisplayName\" IS NULL OR char_length(\"ProviderDisplayName\") <= 128");
                table.CheckConstraint("CK_AbpUserLogins_ProviderKey_MaxLength", "\"ProviderKey\" IS NULL OR char_length(\"ProviderKey\") <= 196");
                table.ForeignKey(
                    name: "FK_AbpUserLogins_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserOrganizationUnits",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                OrganizationUnitId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
                table.ForeignKey(
                    name: "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationUnitId",
                    column: x => x.OrganizationUnitId,
                    principalTable: "AbpOrganizationUnits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserPasskeys",
            columns: table => new
            {
                CredentialId = table.Column<byte[]>(type: "BLOB", maxLength: 1024, nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                Data = table.Column<string>(type: "JSON", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserPasskeys", x => x.CredentialId);
                table.ForeignKey(
                    name: "FK_AbpUserPasskeys_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserPasswordHistories",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                Password = table.Column<string>(type: "STRING", maxLength: 256, nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "DATETIME", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserPasswordHistories", x => new { x.UserId, x.Password });
                table.CheckConstraint("CK_AbpUserPasswordHistories_Password_MaxLength", "\"Password\" IS NULL OR char_length(\"Password\") <= 256");
                table.ForeignKey(
                    name: "FK_AbpUserPasswordHistories_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserRoles",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                RoleId = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AbpUserRoles_AbpRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AbpRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AbpUserRoles_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AbpUserTokens",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "STRING", nullable: false),
                LoginProvider = table.Column<string>(type: "STRING", maxLength: 64, nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                Value = table.Column<string>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.CheckConstraint("CK_AbpUserTokens_LoginProvider_MaxLength", "\"LoginProvider\" IS NULL OR char_length(\"LoginProvider\") <= 64");
                table.CheckConstraint("CK_AbpUserTokens_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.ForeignKey(
                    name: "FK_AbpUserTokens_AbpUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AbpUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AppBooks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                Name = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                Type = table.Column<int>(type: "INT", nullable: false),
                PublishDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                Price = table.Column<float>(type: "FLOAT", nullable: false),
                AuthorId = table.Column<Guid>(type: "STRING", nullable: false),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                CreatorId = table.Column<Guid>(type: "STRING", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                LastModifierId = table.Column<Guid>(type: "STRING", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppBooks", x => x.Id);
                table.CheckConstraint("CK_AppBooks_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_AppBooks_Name_MaxLength", "\"Name\" IS NULL OR char_length(\"Name\") <= 128");
                table.ForeignKey(
                    name: "FK_AppBooks_AppAuthors_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AppAuthors",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictAuthorizations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                ApplicationId = table.Column<Guid>(type: "STRING", nullable: true),
                CreationDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                Properties = table.Column<string>(type: "STRING", nullable: true),
                Scopes = table.Column<string>(type: "STRING", nullable: true),
                Status = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "STRING", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                table.CheckConstraint("CK_OpenIddictAuthorizations_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_OpenIddictAuthorizations_Status_MaxLength", "\"Status\" IS NULL OR char_length(\"Status\") <= 50");
                table.CheckConstraint("CK_OpenIddictAuthorizations_Subject_MaxLength", "\"Subject\" IS NULL OR char_length(\"Subject\") <= 400");
                table.CheckConstraint("CK_OpenIddictAuthorizations_Type_MaxLength", "\"Type\" IS NULL OR char_length(\"Type\") <= 50");
                table.ForeignKey(
                    name: "FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId",
                    column: x => x.ApplicationId,
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "AbpEntityPropertyChanges",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                TenantId = table.Column<Guid>(type: "STRING", nullable: true),
                EntityChangeId = table.Column<Guid>(type: "STRING", nullable: false),
                NewValue = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                OriginalValue = table.Column<string>(type: "STRING", maxLength: 512, nullable: true),
                PropertyName = table.Column<string>(type: "STRING", maxLength: 128, nullable: false),
                PropertyTypeFullName = table.Column<string>(type: "STRING", maxLength: 512, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                table.CheckConstraint("CK_AbpEntityPropertyChanges_NewValue_MaxLength", "\"NewValue\" IS NULL OR char_length(\"NewValue\") <= 512");
                table.CheckConstraint("CK_AbpEntityPropertyChanges_OriginalValue_MaxLength", "\"OriginalValue\" IS NULL OR char_length(\"OriginalValue\") <= 512");
                table.CheckConstraint("CK_AbpEntityPropertyChanges_PropertyName_MaxLength", "\"PropertyName\" IS NULL OR char_length(\"PropertyName\") <= 128");
                table.CheckConstraint("CK_AbpEntityPropertyChanges_PropertyTypeFullName_MaxLength", "\"PropertyTypeFullName\" IS NULL OR char_length(\"PropertyTypeFullName\") <= 512");
                table.ForeignKey(
                    name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                    column: x => x.EntityChangeId,
                    principalTable: "AbpEntityChanges",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictTokens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "STRING", nullable: false),
                ApplicationId = table.Column<Guid>(type: "STRING", nullable: true),
                AuthorizationId = table.Column<Guid>(type: "STRING", nullable: true),
                CreationDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                ExpirationDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                Payload = table.Column<string>(type: "STRING", nullable: true),
                Properties = table.Column<string>(type: "STRING", nullable: true),
                RedemptionDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                ReferenceId = table.Column<string>(type: "STRING", maxLength: 100, nullable: true),
                Status = table.Column<string>(type: "STRING", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "STRING", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "STRING", maxLength: 150, nullable: true),
                ExtraProperties = table.Column<string>(type: "STRING", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "STRING", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                table.CheckConstraint("CK_OpenIddictTokens_ConcurrencyStamp_MaxLength", "\"ConcurrencyStamp\" IS NULL OR char_length(\"ConcurrencyStamp\") <= 40");
                table.CheckConstraint("CK_OpenIddictTokens_ReferenceId_MaxLength", "\"ReferenceId\" IS NULL OR char_length(\"ReferenceId\") <= 100");
                table.CheckConstraint("CK_OpenIddictTokens_Status_MaxLength", "\"Status\" IS NULL OR char_length(\"Status\") <= 50");
                table.CheckConstraint("CK_OpenIddictTokens_Subject_MaxLength", "\"Subject\" IS NULL OR char_length(\"Subject\") <= 400");
                table.CheckConstraint("CK_OpenIddictTokens_Type_MaxLength", "\"Type\" IS NULL OR char_length(\"Type\") <= 150");
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                    column: x => x.ApplicationId,
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                    column: x => x.AuthorizationId,
                    principalTable: "OpenIddictAuthorizations",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_AbpAuditLogActions_AuditLogId",
            table: "AbpAuditLogActions",
            column: "AuditLogId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_ExecutionTime",
            table: "AbpAuditLogActions",
            columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
            table: "AbpAuditLogs",
            columns: new[] { "TenantId", "ExecutionTime" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpAuditLogs_TenantId_UserId_ExecutionTime",
            table: "AbpAuditLogs",
            columns: new[] { "TenantId", "UserId", "ExecutionTime" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpBackgroundJobs_ApplicationName_CompletionTime_IsAbandoned_NextTryTime",
            table: "AbpBackgroundJobs",
            columns: new[] { "ApplicationName", "CompletionTime", "IsAbandoned", "NextTryTime" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpEntityChanges_AuditLogId",
            table: "AbpEntityChanges",
            column: "AuditLogId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId",
            table: "AbpEntityChanges",
            columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpEntityPropertyChanges_EntityChangeId",
            table: "AbpEntityPropertyChanges",
            column: "EntityChangeId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpFeatureGroups_Name",
            table: "AbpFeatureGroups",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpFeatures_GroupName",
            table: "AbpFeatures",
            column: "GroupName");

        migrationBuilder.CreateIndex(
            name: "IX_AbpFeatures_Name",
            table: "AbpFeatures",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpFeatureValues_Name_ProviderName_ProviderKey",
            table: "AbpFeatureValues",
            columns: new[] { "Name", "ProviderName", "ProviderKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId",
            table: "AbpLinkUsers",
            columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId",
            table: "AbpOrganizationUnitRoles",
            columns: new[] { "RoleId", "OrganizationUnitId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpOrganizationUnits_Code",
            table: "AbpOrganizationUnits",
            column: "Code");

        migrationBuilder.CreateIndex(
            name: "IX_AbpOrganizationUnits_ParentId",
            table: "AbpOrganizationUnits",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
            table: "AbpPermissionGrants",
            columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpPermissionGroups_Name",
            table: "AbpPermissionGroups",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpPermissions_GroupName",
            table: "AbpPermissions",
            column: "GroupName");

        migrationBuilder.CreateIndex(
            name: "IX_AbpPermissions_ResourceName_Name",
            table: "AbpPermissions",
            columns: new[] { "ResourceName", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpResourcePermissionGrants_TenantId_Name_ResourceName_ResourceKey_ProviderName_ProviderKey",
            table: "AbpResourcePermissionGrants",
            columns: new[] { "TenantId", "Name", "ResourceName", "ResourceKey", "ProviderName", "ProviderKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpRoleClaims_RoleId",
            table: "AbpRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpRoles_NormalizedName",
            table: "AbpRoles",
            column: "NormalizedName");

        migrationBuilder.CreateIndex(
            name: "IX_AbpSecurityLogs_TenantId_Action",
            table: "AbpSecurityLogs",
            columns: new[] { "TenantId", "Action" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpSecurityLogs_TenantId_ApplicationName",
            table: "AbpSecurityLogs",
            columns: new[] { "TenantId", "ApplicationName" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpSecurityLogs_TenantId_Identity",
            table: "AbpSecurityLogs",
            columns: new[] { "TenantId", "Identity" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpSecurityLogs_TenantId_UserId",
            table: "AbpSecurityLogs",
            columns: new[] { "TenantId", "UserId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpSessions_Device",
            table: "AbpSessions",
            column: "Device");

        migrationBuilder.CreateIndex(
            name: "IX_AbpSessions_SessionId",
            table: "AbpSessions",
            column: "SessionId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpSessions_TenantId_UserId",
            table: "AbpSessions",
            columns: new[] { "TenantId", "UserId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpSettingDefinitions_Name",
            table: "AbpSettingDefinitions",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpSettings_Name_ProviderName_ProviderKey",
            table: "AbpSettings",
            columns: new[] { "Name", "ProviderName", "ProviderKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AbpTenants_Name",
            table: "AbpTenants",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_AbpTenants_NormalizedName",
            table: "AbpTenants",
            column: "NormalizedName");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUserClaims_UserId",
            table: "AbpUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUserLogins_LoginProvider_ProviderKey",
            table: "AbpUserLogins",
            columns: new[] { "LoginProvider", "ProviderKey" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId",
            table: "AbpUserOrganizationUnits",
            columns: new[] { "UserId", "OrganizationUnitId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpUserPasskeys_UserId",
            table: "AbpUserPasskeys",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUserRoles_RoleId_UserId",
            table: "AbpUserRoles",
            columns: new[] { "RoleId", "UserId" });

        migrationBuilder.CreateIndex(
            name: "IX_AbpUsers_Email",
            table: "AbpUsers",
            column: "Email");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUsers_NormalizedEmail",
            table: "AbpUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUsers_NormalizedUserName",
            table: "AbpUsers",
            column: "NormalizedUserName");

        migrationBuilder.CreateIndex(
            name: "IX_AbpUsers_UserName",
            table: "AbpUsers",
            column: "UserName");

        migrationBuilder.CreateIndex(
            name: "IX_AppAuthors_Name",
            table: "AppAuthors",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_AppBooks_AuthorId",
            table: "AppBooks",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictApplications_ClientId",
            table: "OpenIddictApplications",
            column: "ClientId");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
            table: "OpenIddictAuthorizations",
            columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictScopes_Name",
            table: "OpenIddictScopes",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
            table: "OpenIddictTokens",
            columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_AuthorizationId",
            table: "OpenIddictTokens",
            column: "AuthorizationId");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ReferenceId",
            table: "OpenIddictTokens",
            column: "ReferenceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AbpAuditLogActions");

        migrationBuilder.DropTable(
            name: "AbpAuditLogExcelFiles");

        migrationBuilder.DropTable(
            name: "AbpBackgroundJobs");

        migrationBuilder.DropTable(
            name: "AbpClaimTypes");

        migrationBuilder.DropTable(
            name: "AbpEntityPropertyChanges");

        migrationBuilder.DropTable(
            name: "AbpFeatureGroups");

        migrationBuilder.DropTable(
            name: "AbpFeatures");

        migrationBuilder.DropTable(
            name: "AbpFeatureValues");

        migrationBuilder.DropTable(
            name: "AbpLinkUsers");

        migrationBuilder.DropTable(
            name: "AbpOrganizationUnitRoles");

        migrationBuilder.DropTable(
            name: "AbpPermissionGrants");

        migrationBuilder.DropTable(
            name: "AbpPermissionGroups");

        migrationBuilder.DropTable(
            name: "AbpPermissions");

        migrationBuilder.DropTable(
            name: "AbpResourcePermissionGrants");

        migrationBuilder.DropTable(
            name: "AbpRoleClaims");

        migrationBuilder.DropTable(
            name: "AbpSecurityLogs");

        migrationBuilder.DropTable(
            name: "AbpSessions");

        migrationBuilder.DropTable(
            name: "AbpSettingDefinitions");

        migrationBuilder.DropTable(
            name: "AbpSettings");

        migrationBuilder.DropTable(
            name: "AbpTenantConnectionStrings");

        migrationBuilder.DropTable(
            name: "AbpUserClaims");

        migrationBuilder.DropTable(
            name: "AbpUserDelegations");

        migrationBuilder.DropTable(
            name: "AbpUserLogins");

        migrationBuilder.DropTable(
            name: "AbpUserOrganizationUnits");

        migrationBuilder.DropTable(
            name: "AbpUserPasskeys");

        migrationBuilder.DropTable(
            name: "AbpUserPasswordHistories");

        migrationBuilder.DropTable(
            name: "AbpUserRoles");

        migrationBuilder.DropTable(
            name: "AbpUserTokens");

        migrationBuilder.DropTable(
            name: "AppBooks");

        migrationBuilder.DropTable(
            name: "OpenIddictScopes");

        migrationBuilder.DropTable(
            name: "OpenIddictTokens");

        migrationBuilder.DropTable(
            name: "AbpEntityChanges");

        migrationBuilder.DropTable(
            name: "AbpTenants");

        migrationBuilder.DropTable(
            name: "AbpOrganizationUnits");

        migrationBuilder.DropTable(
            name: "AbpRoles");

        migrationBuilder.DropTable(
            name: "AbpUsers");

        migrationBuilder.DropTable(
            name: "AppAuthors");

        migrationBuilder.DropTable(
            name: "OpenIddictAuthorizations");

        migrationBuilder.DropTable(
            name: "AbpAuditLogs");

        migrationBuilder.DropTable(
            name: "OpenIddictApplications");
    }
}
