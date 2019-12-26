namespace Join.AuditManagement.Notifications.EventReceivers
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Utilities;
    using System;

    /// <summary>
    /// Event receivers for actions list
    /// </summary>
    public class ActionsListEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// resx key for action added notification subject
        /// </summary>
        private const string ActionAddedNotificationTitle = "ActionAddedNotificationTitle";

        /// <summary>
        /// resx key for action added notification body
        /// </summary>
        private const string ActionAddedNotificationBody = "ActionAddedNotificationBody";

        /// <inheritdoc/>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemAdded");
            try
            {
                this.SendNotificationForActionAdded(properties.ListItem);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, this.GetType().Name, string.Format("Error while ItemAdded id:{0} error:{1}", properties.ListItemId, ex.Message));
                throw;
            }
        }

        /// <inheritdoc/>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemUpdated");
        }

        /// <inheritdoc/>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemUpdating");
            string actionStatusOld = Convert.ToString(properties.BeforeProperties[Fields.ActionStatus]);
            string actionStatusNew = Convert.ToString(properties.AfterProperties[Fields.ActionStatus]);

            // check status
            if (!string.IsNullOrEmpty(actionStatusNew) && !actionStatusNew.Equals(actionStatusOld))
            {
                try
                {
                    //todo: check status and send notification
                    switch (actionStatusNew)
                    {
                        case ActionStatus.Implemented:
                            break;
                        case ActionStatus.Completed:
                            break;
                        case ActionStatus.Canceled:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, this.GetType().Name, string.Format("Error while ItemUpdating id:{0} error:{1}", properties.ListItemId, ex.Message));
                    throw;
                }
            }
        }

        private void SendNotificationForActionAdded(SPListItem actionItem)
        {
            string actionResponsible = Convert.ToString(actionItem[Fields.ActionResponsible]);
            if (!string.IsNullOrEmpty(actionResponsible))
            {
                SPFieldUserValue user = new SPFieldUserValue(actionItem.Web, actionResponsible);
                if (!string.IsNullOrEmpty(user.User.Email))
                {
                    // send notification
                    Logger.WriteLog(Logger.Category.Information, typeof(ActionsListEventReceiver).FullName, string.Format("send action added notification to :{0}", user.User.Email));
                    string subject = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, ActionAddedNotificationTitle), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, actionItem.Web.Language);
                    string body = SPUtility.GetLocalizedString(string.Format(JoinAMUtilities.ResxForJoinAMNotifications, ActionAddedNotificationBody), JoinAMUtilities.JoinAMNotificationsDefaultResourceFile, actionItem.Web.Language);
                    string url = Convert.ToString(actionItem[SPBuiltInFieldId.EncodedAbsUrl]);
                    DateTime dueDate = Convert.ToDateTime(actionItem[Fields.ActionPlannedRealisationDate]);

                    JoinAMUtilities.SendEmail(actionItem.Web, user.User.Email, string.Format(body, dueDate.ToShortDateString(), url), subject);

                }
            }
        }
    }
}
