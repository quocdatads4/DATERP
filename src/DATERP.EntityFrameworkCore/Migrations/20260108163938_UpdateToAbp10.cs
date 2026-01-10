using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATERP.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToAbp10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceInfo",
                table: "AbpSessions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            // Defensive SQL for Postgres
            migrationBuilder.Sql("ALTER TABLE \"AbpRoles\" ADD COLUMN IF NOT EXISTS \"CreationTime\" timestamp without time zone NOT NULL DEFAULT '0001-01-01 00:00:00';");
            migrationBuilder.Sql("ALTER TABLE \"AbpClaimTypes\" ADD COLUMN IF NOT EXISTS \"CreationTime\" timestamp without time zone NOT NULL DEFAULT '0001-01-01 00:00:00';");
            migrationBuilder.Sql("ALTER TABLE \"AbpBackgroundJobs\" ADD COLUMN IF NOT EXISTS \"ApplicationName\" character varying(96) NULL;");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename  = 'AbpAuditLogExcelFiles') THEN
                        CREATE TABLE ""AbpAuditLogExcelFiles"" (
                            ""Id"" uuid NOT NULL CONSTRAINT ""PK_AbpAuditLogExcelFiles"" PRIMARY KEY,
                            ""TenantId"" uuid NULL,
                            ""FileName"" character varying(256) NULL,
                            ""CreationTime"" timestamp without time zone NOT NULL,
                            ""CreatorId"" uuid NULL
                        );
                    END IF;
                END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogExcelFiles");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AbpRoles");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AbpClaimTypes");

            migrationBuilder.DropColumn(
                name: "ApplicationName",
                table: "AbpBackgroundJobs");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceInfo",
                table: "AbpSessions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
