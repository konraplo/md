using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Join.AuditManagement.Notifications.Common;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Join.AuditManagement.Notifications.Features.JoinActionsTracking
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("0937fd64-cfcd-41b1-9dba-4faec8fc1d27")]
    public class JoinActionsTrackingEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {

                    SPWeb web = properties.Feature.Parent as SPWeb;
                    this.AddErToLists(web);
                });
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, this.GetType().Name, string.Format("Error while activating Feature:{0}", ex.Message));
                throw;
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        /// <summary>
        /// Add event receivers to list
        /// </summary>
        /// <param name="web"></param>
        private void AddErToLists(SPWeb web)
        {
            string actionsUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl.TrimEnd('/'), ListUtilities.Urls.Actions);
            SPList actionsList = web.GetList(actionsUrl);

            JoinAMUtilities.AddListEventReceiver(actionsList, SPEventReceiverType.ItemAdded, Assembly.GetExecutingAssembly().FullName, "Join.AuditManagement.Notifications.EventReceivers.ActionsListEventReceiver", false);
            JoinAMUtilities.AddListEventReceiver(actionsList, SPEventReceiverType.ItemUpdated, Assembly.GetExecutingAssembly().FullName, "Join.AuditManagement.Notifications.EventReceivers.ActionsListEventReceiver", false);
            JoinAMUtilities.AddListEventReceiver(actionsList, SPEventReceiverType.ItemUpdating, Assembly.GetExecutingAssembly().FullName, "Join.AuditManagement.Notifications.EventReceivers.ActionsListEventReceiver", true);

        }
    }
}
