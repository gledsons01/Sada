namespace Sada.Application.Configuration
{
    public class CacheSettings
    {
        public const string SectionName = "CacheSettings";

        public int ProposalsAbsoluteExpirationMinutes { get; set; }
        public int InstallmentAbsoluteExpirationMinutes { get; set; }
        public int ProjectDetailsAbsoluteExpirationMinutes { get; set; }
        public int HistoryStatusAbsoluteExpirationMinutes { get; set; }
        public int ListAllProjectsAbsoluteExpirationMinutes { get; set; }
        public int HomeAbsoluteExpirationMinutes { get; set; }
        public int MonitoringDashboardAbsoluteExpirationMinutes { get; set; }
    }
}
