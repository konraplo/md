namespace Join.AuditManagement.Notifications.TimerJobs
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Text;

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

        private static void SendActionNotifications(string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
            {
                try
                {
                    using (SPSite site = new SPSite(siteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {

                            // Bei Ablauf des Ablaufdatums
                            SPListItemCollection documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, FirstReminderDaysOffset);
                            string subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            string body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            SendNotificationForDocuments(web, documents, subject, body, 2);

                            // 30 Tage nach Ablauf des Ablaufdatums
                            documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, ThirdReminderDaysOffset);
                            subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            SendNotificationForDocuments(web, documents, subject, body, 3);
                        }
                    }

                }
                catch (Exception exception)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMNotificationTimerJobExecutor).FullName, string.Format("Error while sending notifications:{0}", exception.Message));
                }
            }
        }

        private static void SendDownloadsCenterNotifications(string siteUrl)
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

                            SendNotificationForDocuments(web, documents, subject, body, 1);

                            // Bei Ablauf des Ablaufdatums
                            documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, FirstReminderDaysOffset);
                            subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueSecondReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            SendNotificationForDocuments(web, documents, subject, body, 2);

                            // 30 Tage nach Ablauf des Ablaufdatums
                            documents = JoinAMUtilities.FindDocumentsByAblaufdatum(web, ThirdReminderDaysOffset);
                            subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);
                            body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, DocumentOverdueThirdReminderBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, web.Language);

                            SendNotificationForDocuments(web, documents, subject, body, 3);
                        }
                    }

                }
                catch (Exception exception)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMNotificationTimerJobExecutor).FullName, string.Format("Error while sending notifications:{0}", exception.Message));
                }
            }
        }

        private static void SendNotificationForDocuments(SPWeb web, SPListItemCollection documents, string mailTitle, string mailBody, int reminderCount)
        {
            SPGroup groupProcessMgmnt = web.SiteGroups.GetByName(JoinAMUtilities.GroupNames.ProcessMgmnt);
            SPGroup groupQualityMgmnt = web.SiteGroups.GetByName(JoinAMUtilities.GroupNames.QualityMgmnt);
            StringBuilder maito = new StringBuilder();
            List<int> userId = new List<int>();
            if (reminderCount == 3) //third reminder
            {
                foreach (SPUser user in groupProcessMgmnt.Users)
                {
                    if (userId.Contains(user.ID))
                    {
                        continue;
                    }
                    userId.Add(user.ID);
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        maito.Append(user.Email).Append(";");
                    }
                }

                foreach (SPUser user in groupQualityMgmnt.Users)
                {
                    if (userId.Contains(user.ID))
                    {
                        continue;
                    }
                    userId.Add(user.ID);
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        maito.Append(user.Email).Append(";");
                    }
                }
            }


            foreach (SPListItem documentItem in documents)
            {
                SPFieldUserValueCollection documentResponsible = documentItem[Fields.DocumentResponsible] as SPFieldUserValueCollection;
                if (documentResponsible == null)
                {
                    continue;
                }

                string url = Convert.ToString(documentItem[SPBuiltInFieldId.EncodedAbsUrl]);
                DateTime dueDate = Convert.ToDateTime(documentItem[Fields.DocumentDueDate]);
                foreach (SPFieldUserValue user in documentResponsible)
                {
                    if (userId.Contains(user.User.ID))
                    {
                        continue;
                    }
                    userId.Add(user.User.ID);
                    if (!string.IsNullOrEmpty(user.User.Email))
                    {
                        maito.Append(user.User.Email).Append(";");
                    }
                }

                if (reminderCount == 1) //first reminder
                {
                    mailBody = string.Format(mailBody, dueDate.ToShortDateString(), url);
                }
                else
                {
                    mailBody = string.Format(mailBody, url);
                }

                //todo: send mail
            }
        }

    }
}
