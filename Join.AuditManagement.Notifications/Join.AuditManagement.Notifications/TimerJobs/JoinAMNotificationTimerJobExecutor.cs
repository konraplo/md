namespace Join.AuditManagement.Notifications.TimerJobs
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Utilities;
    using System;

    /// <summary>
    /// An instance of this class is called from the timer job that handles the Join Audit Management Notification/Actions.
    /// </summary>
    public class JoinAMNotificationTimerJobExecutor
    {
        /// <summary>
        /// 30 Tage vor Ablauf des Ablaufdatums
        /// </summary>
        private const int FirstReminderDaysOffset = 30;

        /// <summary>
        /// Bei Ablauf des Ablaufdatums
        /// </summary>
        private const int SecondReminderDaysOffset = 0;

        /// <summary>
        /// 30 Tage nach Ablauf des Ablaufdatums
        /// </summary>
        private const int ThirdReminderDaysOffset = -30;

        /// <summary>
        /// resx key for first reminder notification subject
        /// </summary>
        private const string DocumentOverdueFirstReminderTitle = "DocumentOverdueFirstReminderTitle";

        /// <summary>
        /// resx key for first reminder notification body
        /// </summary>
        private const string DocumentOverdueFirstReminderBody = "DocumentOverdueFirstReminderBody";

        /// <summary>
        /// resx key for second reminder notification 
        /// </summary>
        private const string DocumentOverdueSecondReminderTitle = "DocumentOverdueSecondReminderTitle";

        /// <summary>
        /// resx key for second reminder notification body
        /// </summary>
        private const string DocumentOverdueSecondReminderBody = "DocumentOverdueSecondReminderBody";

        /// <summary>
        /// resx key for third reminder notification 
        /// </summary>
        private const string DocumentOverdueThirdReminderTitle = "DocumentOverdueThirdReminderTitle";

        /// <summary>
        /// resx key for third reminder notification body
        /// </summary>
        private const string DocumentOverdueThirdReminderBody = "DocumentOverdueThirdReminderBody";

       

        /// <summary>
        /// Execute timer job logic
        /// </summary>
        /// <param name="notificationTimerJob"></param>
        internal void Execute(JoinAMNotificationTimerJob notificationTimerJob)
        {
        }

        private static void SendTasksNotifications(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                try
                {
                    using (SPSite site = new SPSite(siteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            // 30 Tage vor Ablauf des Ablaufdatums
                            SPListItemCollection documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, SecondReminderDaysOffset);
                            string subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueFirstReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            string body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueFirstReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            //SendNotificationForTasksOwners(web, projectTasks, subject, body, 1);

                            // Bei Ablauf des Ablaufdatums
                            documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, FirstReminderDaysOffset);
                            subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            //SendNotificationForTasksOwners(web, projectTasks, subject, body, 2);

                            // 30 Tage nach Ablauf des Ablaufdatums
                            documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, ThirdReminderDaysOffset);
                            subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            //SendNotificationForTasksOwners(web, projectTasks, subject, body, 2);
                        }
                    }

                }
                catch (Exception exception)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMNotificationTimerJobExecutor).FullName, string.Format("Error while sending notifications:{0}", exception.Message));
                }
            }
        }
    }
}
