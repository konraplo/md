using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Join.AuditManagement.Notifications.Common;
using Join.AuditManagement.Notifications.TimerJobs;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Join.AuditManagement.Notifications.Features.JoinAMTimerjobs
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("5ba95959-1713-4fb4-b994-c61446f9e106")]
    public class JoinAMTimerjobsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {

                    SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                    this.RemoveTimmerJobs(parentWebApp);
                    this.SetupTimerJobs(parentWebApp);
                });
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, this.GetType().Name, string.Format("Error wgile activating Feature:{0}", ex.Message));
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

        private void RemoveTimmerJobs(SPWebApplication webApp)
        {
            foreach (SPJobDefinition spJobDefinition in webApp.JobDefinitions)
            {
                if (spJobDefinition.Name == JoinAMUtilities.JoinAMNotificationTimerJobName)
                {
                    Logger.WriteLog(Logger.Category.Information, this.GetType().Name, string.Format("delete:{0}", spJobDefinition.Name));
                    spJobDefinition.Delete();
                }
            }
        }

        /// <summary>
        /// This method initialize all timer jobs necessary for the solution
        /// </summary>
        /// <param name="webApp"></param>
        private void SetupTimerJobs(SPWebApplication webApp)
        {
            // notification timer job
            Logger.WriteLog(Logger.Category.Information, this.GetType().Name, string.Format("set up:{0}", JoinAMUtilities.JoinAMNotificationTimerJobName));
            SPJobDefinition job = new JoinAMNotificationTimerJob(JoinAMUtilities.JoinAMNotificationTimerJobName, webApp);

            SPDailySchedule schedule = new SPDailySchedule();
            schedule.BeginSecond = 0;
            schedule.EndSecond = 0;
            schedule.BeginHour = 23;
            schedule.EndHour = 23;
            schedule.BeginMinute = 0;
            schedule.EndMinute = 30;

            job.Schedule = schedule;
            job.Update();
        }
    }
}
