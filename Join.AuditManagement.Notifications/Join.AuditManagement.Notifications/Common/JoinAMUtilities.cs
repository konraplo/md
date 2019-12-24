using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;

namespace Join.AuditManagement.Notifications.Common
{
    /// <summary>
    /// Helpermethods with solutionwide accessible methods and functions.
    /// </summary>
    public static class JoinAMUtilities
    {
        public const string JoinAMNotificationTimerJobName = "Join Audit Management Notification Timer job";
        public const string ResxForJoinAMNotifications = "$Resources:COSIntranet,{0}";
        public const string JoinAMNotificationsDefaultResourceFile = "COSIntranet";

        /// <summary>
        /// Query document by 'Ablaufdatum'
        /// </summary>
        public const string queryDocumentsByAblaufdatum =
                                   @"<Where>
                                    <Eq>
                                        <FieldRef Name='Ablaufdatum' />
                                        <Value Type='DateTime'>
                                            <Today OffsetDays='{0}' />
                                        </Value>
                                     </Eq></Where>";

        /// <summary>
        /// Groups used in AM
        /// </summary>
        public static class GroupNames
        {
            /// <summary>
            /// Name of the user group for storing members of the process management
            /// </summary>
            public const string ProcessMgmnt = "Prozessmanagament-Team";
            /// <summary>
            /// Name of the user group for storing members of the quality management
            /// </summary>
            public const string QualityMgmnt = "Qualitätsmanagement-Team";
        }

        /// <summary>
        /// Find documents in download center by 'Ablaufdatum'
        /// </summary>
        /// <param name="web">Quam web</param>
        /// <param name="offsetDays">offset in days (can be negative)</param>
        /// <returns></returns>
        public static SPListItemCollection FindDocumentsByAblaufdatum(SPWeb web, int offsetDays)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, ListUtilities.Urls.Downloadcenter));
            SPQuery query = new SPQuery();

            query.Query = string.Format(queryDocumentsByAblaufdatum, offsetDays);
            SPListItemCollection documents = list.GetItems(query);

            return documents;
        }

        /// <summary>
        /// Iterates through all site collections od the site collection and returns the URL of the web, where the Feature is activated
        /// </summary>
        /// <param name="site">SPWebApplication to search for the SiteCollection</param>
        /// <param name="featureGuid">Feature to search for</param>
        /// <returns>Url of site. Returns string.Empty if not found</returns>
        public static string FindWebUrlByFeature(SPSite site, Guid featureGuid)
        {
            if (site == null) throw new ArgumentNullException("WebApplication must be not NULL! (FindWebUrlByFeature)");

            try
            {
                foreach (SPWeb web in site.AllWebs)
                {
                    bool featureFound = (web.Features[featureGuid] != null);
                    string url = web.Url;
                    web.Dispose();
                    if (featureFound) return url;

                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMUtilities).Name, string.Format("FindWebUrlByFeature error:{0}", ex.Message));
            }

            return string.Empty;
        }

        /// <summary>
        /// Iterates through all site collections od the WebApplication and returns the ID of the Site, where Feature is activated
        /// </summary>
        /// <param name="webApp">SPWebApplication to search for the SiteCollection</param>
        /// <param name="featureGuid">Feature to search for</param>
        /// <returns>GUID of the SiteCollection. Returns Guid.Empty if not found</returns>
        public static Guid FindSiteCollIdByFeature(SPWebApplication webApp, Guid featureGuid)
        {
            if (webApp == null) throw new ArgumentNullException("WebApplication must be not NULL! (FindWebUrlByFeature)");

            Guid retval = Guid.Empty;

            try
            {
                foreach (SPSite site in webApp.Sites)
                {
                    bool featureFound = (site.RootWeb.Features[featureGuid] != null);
                    if (featureFound) return site.ID;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMUtilities).Name, string.Format("FindSiteCollIdByFeature error:{0}", ex.Message));
            }

            return retval;
        }

        /// <summary>
        /// add event receiver to spcified list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <param name="assembly"></param>
        /// <param name="className"></param>
        /// <param name="synchronous"></param>
        public static void AddListEventReceiver(SPList list, SPEventReceiverType type, string assembly, string className, bool synchronous)
        {
            using (SPSite site = new SPSite(list.ParentWeb.Site.ID))
            {
                using (SPWeb rootWeb = site.OpenWeb(list.ParentWeb.ID))
                {
                    list = rootWeb.Lists[list.ID];
                    DeleteListEventReceiver(list, type);


                    list.EventReceivers.Add(type,
                                           assembly,
                                           className);

                    if (synchronous)
                    {
                        foreach (SPEventReceiverDefinition receiver in list.EventReceivers)
                        {
                            if (receiver.Type == type)
                            {
                                receiver.Synchronization = SPEventReceiverSynchronization.Synchronous;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// remove event receiver from specified list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        public static void DeleteListEventReceiver(SPList list, SPEventReceiverType type)
        {
            foreach (SPEventReceiverDefinition evt in list.EventReceivers)
            {
                if (evt.Type == type)
                {
                    evt.Delete();
                    break;
                }
            }

            list.Update();
        }

    }
}
