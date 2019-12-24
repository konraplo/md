namespace Join.AuditManagement.Notifications.EventReceivers
{
    using Join.AuditManagement.Notifications.Common;
    using Microsoft.SharePoint;

    /// <summary>
    /// Event receivers for actions list
    /// </summary>
    public class ActionsListEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// Ein Element wurde hinzugefügt..
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemAdded");
        }

        /// <summary>
        /// Ein Element wurde aktualisiert..
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, "ItemUpdated");
        }
    }
}
