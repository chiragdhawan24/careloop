using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareLoop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCareLoopSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "diagnostic_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ordered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ordered_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diagnostic_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    medical_record_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "care_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    diagnostic_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_care_cases", x => x.id);
                    table.ForeignKey(
                        name: "fk_care_cases_diagnostic_orders_diagnostic_order_id",
                        column: x => x.diagnostic_order_id,
                        principalTable: "diagnostic_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_care_cases_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "care_case_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_care_case_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_care_case_events_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clinical_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clinical_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_clinical_reviews_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    method = table.Column<int>(type: "integer", nullable: false),
                    attempted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    successful = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contact_attempts_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diagnostic_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    diagnostic_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diagnostic_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_diagnostic_results_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_diagnostic_results_diagnostic_orders_diagnostic_order_id",
                        column: x => x.diagnostic_order_id,
                        principalTable: "diagnostic_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "escalations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    escalated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_escalations", x => x.id);
                    table.ForeignKey(
                        name: "fk_escalations_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follow_up_actions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    assigned_to = table.Column<Guid>(type: "uuid", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    care_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_follow_up_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_follow_up_actions_care_cases_care_case_id",
                        column: x => x.care_case_id,
                        principalTable: "care_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_care_case_events_care_case_id",
                table: "care_case_events",
                column: "care_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_care_cases_diagnostic_order_id",
                table: "care_cases",
                column: "diagnostic_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_care_cases_patient_id",
                table: "care_cases",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_care_cases_status_created_at",
                table: "care_cases",
                columns: new[] { "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_clinical_reviews_care_case_id",
                table: "clinical_reviews",
                column: "care_case_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contact_attempts_care_case_id",
                table: "contact_attempts",
                column: "care_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_diagnostic_orders_ordered_at",
                table: "diagnostic_orders",
                column: "ordered_at");

            migrationBuilder.CreateIndex(
                name: "ix_diagnostic_results_care_case_id",
                table: "diagnostic_results",
                column: "care_case_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_diagnostic_results_diagnostic_order_id",
                table: "diagnostic_results",
                column: "diagnostic_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_escalations_care_case_id",
                table: "escalations",
                column: "care_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_follow_up_actions_care_case_id",
                table: "follow_up_actions",
                column: "care_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_follow_up_actions_status_due_at",
                table: "follow_up_actions",
                columns: new[] { "status", "due_at" });

            migrationBuilder.CreateIndex(
                name: "ix_patients_medical_record_number",
                table: "patients",
                column: "medical_record_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "care_case_events");

            migrationBuilder.DropTable(
                name: "clinical_reviews");

            migrationBuilder.DropTable(
                name: "contact_attempts");

            migrationBuilder.DropTable(
                name: "diagnostic_results");

            migrationBuilder.DropTable(
                name: "escalations");

            migrationBuilder.DropTable(
                name: "follow_up_actions");

            migrationBuilder.DropTable(
                name: "care_cases");

            migrationBuilder.DropTable(
                name: "diagnostic_orders");

            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
