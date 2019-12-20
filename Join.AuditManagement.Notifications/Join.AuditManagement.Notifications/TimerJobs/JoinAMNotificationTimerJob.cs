namespace Join.AuditManagement.Notifications.TimerJobs
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint.Administration;
    using System;

    /// <summary>
    /// This job definition represents the Timer job responsible for the join audit management notifications
    /// </summary>
    public class JoinAMNotificationTimerJob : SPJobDefinition
    {
        /// <summary>
        /// Empty CTOR
        /// </summary>
        public JoinAMNotificationTimerJob() : base()
        {

        }

        /// <summary>
        /// Unused CTOR
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <param name="service">The Service</param>
        /// <param name="server">The server</param>
        /// <param name="targetType">SPJobLockType</param>
        public JoinAMNotificationTimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType) : base(jobName, service, server, targetType)
        {

        }

        /// <summary>
        /// Unused CTOR
        /// </summary>
        /// <param name="jobName">Name of the job</param>
        /// <param name="webApplication">WebApplication object</param>
        public JoinAMNotificationTimerJob(string jobName, SPWebApplication webApplication) : base(jobName, webApplication, null, SPJobLockType.Job)
        {
            this.Title = JoinAMUtilities.JoinAMNotificationTimerJobName;
        }

        /// <summary>
        /// Execute-Method.
        /// </summary>
        /// <param name="targetInstanceId">ID of the job instance</param>
        public override void Execute(Guid targetInstanceId)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "Entered Executemethod.");
            JoinAMNotificationTimerJobExecutor executer = new JoinAMNotificationTimerJobExecutor();
            executer.Execute(this);
        }
    }
}
