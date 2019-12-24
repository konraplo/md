namespace Join.AuditManagement.Notifications.EventReceivers
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint;
    using System;

    /// <summary>
    /// Event receivers for actions list
    /// </summary>
    public class ActionsListEventReceiver : SPItemEventReceiver
    {
        /// <inheritdoc/>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemAdded");
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
    }
}
