namespace Join.AuditManagement.Notifications.Common
{
    using Microsoft.SharePoint.Administration;
    using System.Collections.Generic;

    /// <summary>
    /// Audit manamgement logger wraper
    /// </summary>
    public class Logger : SPDiagnosticsServiceBase
    {
        private static string DiagnosticAreaName = "Join.AuditManagement.Notifications";
        private static Logger _Current;

        /// <summary>
        /// Logger instance
        /// </summary>
        public static Logger Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Logger();
                }

                return _Current;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Logger() : base("Join Audit Management Logging Service", SPFarm.Local)
        {

        }

        public enum Category
        {
            Unexpected,
            High,
            Medium,
            Information
        }


        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory("Unexpected", TraceSeverity.Unexpected, EventSeverity.Error),
                    new SPDiagnosticsCategory("High", TraceSeverity.High, EventSeverity.Warning),
                    new SPDiagnosticsCategory("Medium", TraceSeverity.Medium, EventSeverity.Information),
                    new SPDiagnosticsCategory("Information", TraceSeverity.Verbose, EventSeverity.Information)
                })
            };

            return areas;
        }

        /// <summary>
        /// Wirte log message
        /// </summary>
        /// <param name="categoryName">Log category</param>
        /// <param name="source">Log source</param>
        /// <param name="errorMessage">Log message</param>
        public static void WriteLog(Category categoryName, string source, string errorMessage)
        {
            SPDiagnosticsCategory category = Logger.Current.Areas[DiagnosticAreaName].Categories[categoryName.ToString()];
            Logger.Current.WriteTrace(0, category, category.TraceSeverity, string.Concat(source, ": ", errorMessage));
        }
    }

}
