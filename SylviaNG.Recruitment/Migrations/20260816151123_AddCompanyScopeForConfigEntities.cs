using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyScopeForConfigEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "WaiverRules",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "TalentPoolCandidates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "SpecialCategories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ShortlistFilters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ShortlistFilterCriteria",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Scorecards",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ScorecardCriteria",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "SavedSearches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ReferralSources",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "QuestionGroups",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "PreBoardingSubmissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "PreBoardingNominees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "PipelineStages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "NotificationTemplateVersions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "NotificationTemplates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "NotificationLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "InterviewVenues",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "InterviewRooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "HiringPipelines",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamVenues",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamRooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamQuestions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamQuestionOptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "EventTemplateMappings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "DocumentTemplateVersions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "DocumentTemplates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "AutoShortlistRuns",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "AutoShortlistResults",
                type: "bigint",
                nullable: true);

            // Backfill: these tables never had a CompanyId before this migration, so every
            // existing row is assigned to the primary/first company (id 1) - same convention
            // AddCompanyMultiTenancy used for JobPostings/JobApplications/Interviews/UserAccounts.
            migrationBuilder.Sql("UPDATE \"WaiverRules\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"TalentPoolCandidates\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"SpecialCategories\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ShortlistFilters\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ShortlistFilterCriteria\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"Scorecards\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ScorecardCriteria\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"SavedSearches\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ReferralSources\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"QuestionGroups\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"PreBoardingSubmissions\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"PreBoardingNominees\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"PipelineStages\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"NotificationTemplateVersions\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"NotificationTemplates\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"NotificationLogs\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"InterviewVenues\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"InterviewRooms\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"HiringPipelines\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExamVenues\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExamRooms\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExamQuestions\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExamQuestionOptions\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"EventTemplateMappings\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"DocumentTemplateVersions\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"DocumentTemplates\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"AutoShortlistRuns\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"AutoShortlistResults\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");

            migrationBuilder.CreateIndex(name: "IX_WaiverRules_CompanyId", table: "WaiverRules", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_TalentPoolCandidates_CompanyId", table: "TalentPoolCandidates", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_SpecialCategories_CompanyId", table: "SpecialCategories", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ShortlistFilters_CompanyId", table: "ShortlistFilters", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ShortlistFilterCriteria_CompanyId", table: "ShortlistFilterCriteria", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_Scorecards_CompanyId", table: "Scorecards", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ScorecardCriteria_CompanyId", table: "ScorecardCriteria", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_SavedSearches_CompanyId", table: "SavedSearches", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ReferralSources_CompanyId", table: "ReferralSources", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_QuestionGroups_CompanyId", table: "QuestionGroups", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_PreBoardingSubmissions_CompanyId", table: "PreBoardingSubmissions", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_PreBoardingNominees_CompanyId", table: "PreBoardingNominees", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_PipelineStages_CompanyId", table: "PipelineStages", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_NotificationTemplateVersions_CompanyId", table: "NotificationTemplateVersions", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_NotificationTemplates_CompanyId", table: "NotificationTemplates", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_NotificationLogs_CompanyId", table: "NotificationLogs", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_InterviewVenues_CompanyId", table: "InterviewVenues", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_InterviewRooms_CompanyId", table: "InterviewRooms", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_HiringPipelines_CompanyId", table: "HiringPipelines", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ExamVenues_CompanyId", table: "ExamVenues", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ExamRooms_CompanyId", table: "ExamRooms", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ExamQuestions_CompanyId", table: "ExamQuestions", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_ExamQuestionOptions_CompanyId", table: "ExamQuestionOptions", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_EventTemplateMappings_CompanyId", table: "EventTemplateMappings", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_DocumentTemplateVersions_CompanyId", table: "DocumentTemplateVersions", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_DocumentTemplates_CompanyId", table: "DocumentTemplates", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_AutoShortlistRuns_CompanyId", table: "AutoShortlistRuns", column: "CompanyId");
            migrationBuilder.CreateIndex(name: "IX_AutoShortlistResults_CompanyId", table: "AutoShortlistResults", column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_WaiverRules_CompanyId", table: "WaiverRules");
            migrationBuilder.DropIndex(name: "IX_TalentPoolCandidates_CompanyId", table: "TalentPoolCandidates");
            migrationBuilder.DropIndex(name: "IX_SpecialCategories_CompanyId", table: "SpecialCategories");
            migrationBuilder.DropIndex(name: "IX_ShortlistFilters_CompanyId", table: "ShortlistFilters");
            migrationBuilder.DropIndex(name: "IX_ShortlistFilterCriteria_CompanyId", table: "ShortlistFilterCriteria");
            migrationBuilder.DropIndex(name: "IX_Scorecards_CompanyId", table: "Scorecards");
            migrationBuilder.DropIndex(name: "IX_ScorecardCriteria_CompanyId", table: "ScorecardCriteria");
            migrationBuilder.DropIndex(name: "IX_SavedSearches_CompanyId", table: "SavedSearches");
            migrationBuilder.DropIndex(name: "IX_ReferralSources_CompanyId", table: "ReferralSources");
            migrationBuilder.DropIndex(name: "IX_QuestionGroups_CompanyId", table: "QuestionGroups");
            migrationBuilder.DropIndex(name: "IX_PreBoardingSubmissions_CompanyId", table: "PreBoardingSubmissions");
            migrationBuilder.DropIndex(name: "IX_PreBoardingNominees_CompanyId", table: "PreBoardingNominees");
            migrationBuilder.DropIndex(name: "IX_PipelineStages_CompanyId", table: "PipelineStages");
            migrationBuilder.DropIndex(name: "IX_NotificationTemplateVersions_CompanyId", table: "NotificationTemplateVersions");
            migrationBuilder.DropIndex(name: "IX_NotificationTemplates_CompanyId", table: "NotificationTemplates");
            migrationBuilder.DropIndex(name: "IX_NotificationLogs_CompanyId", table: "NotificationLogs");
            migrationBuilder.DropIndex(name: "IX_InterviewVenues_CompanyId", table: "InterviewVenues");
            migrationBuilder.DropIndex(name: "IX_InterviewRooms_CompanyId", table: "InterviewRooms");
            migrationBuilder.DropIndex(name: "IX_HiringPipelines_CompanyId", table: "HiringPipelines");
            migrationBuilder.DropIndex(name: "IX_ExamVenues_CompanyId", table: "ExamVenues");
            migrationBuilder.DropIndex(name: "IX_ExamRooms_CompanyId", table: "ExamRooms");
            migrationBuilder.DropIndex(name: "IX_ExamQuestions_CompanyId", table: "ExamQuestions");
            migrationBuilder.DropIndex(name: "IX_ExamQuestionOptions_CompanyId", table: "ExamQuestionOptions");
            migrationBuilder.DropIndex(name: "IX_EventTemplateMappings_CompanyId", table: "EventTemplateMappings");
            migrationBuilder.DropIndex(name: "IX_DocumentTemplateVersions_CompanyId", table: "DocumentTemplateVersions");
            migrationBuilder.DropIndex(name: "IX_DocumentTemplates_CompanyId", table: "DocumentTemplates");
            migrationBuilder.DropIndex(name: "IX_AutoShortlistRuns_CompanyId", table: "AutoShortlistRuns");
            migrationBuilder.DropIndex(name: "IX_AutoShortlistResults_CompanyId", table: "AutoShortlistResults");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "WaiverRules");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TalentPoolCandidates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SpecialCategories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ShortlistFilters");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ShortlistFilterCriteria");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Scorecards");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ScorecardCriteria");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SavedSearches");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ReferralSources");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "QuestionGroups");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PreBoardingSubmissions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PreBoardingNominees");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "NotificationTemplateVersions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "NotificationLogs");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InterviewVenues");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InterviewRooms");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "HiringPipelines");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamVenues");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamRooms");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamQuestionOptions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "EventTemplateMappings");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DocumentTemplateVersions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DocumentTemplates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AutoShortlistRuns");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AutoShortlistResults");
        }
    }
}
