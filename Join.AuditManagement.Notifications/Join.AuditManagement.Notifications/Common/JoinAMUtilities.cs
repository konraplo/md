﻿using Microsoft.SharePoint;
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
        public const string ResxForJoinAMNotifications = "$Resources:Join.AuditManagement.Notifications,{0}";
        public const string JoinAMNotificationsDefaultResourceFile = "Join.AuditManagement.Notifications";
        public static Guid JoinActionsTrackingFeatureId = Guid.Parse("9f96eb6f-c06d-4d6b-a8f5-393eebb2abaf");

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
        /// Query opened actions by 'LintraAmPlannedDateOfRealisation'
        /// </summary>
        public const string queryActionsByAblaufdatum =
                                   @"<Where>
                                    <And>
                                      <Eq>
										  <FieldRef Name='StatusderMassnahme' />
										  <Value Type='Text'>offen</Value>
									  </Eq>
                                     <Eq>
                                        <FieldRef Name='LintraAmPlannedDateOfRealisation' />
                                        <Value Type='DateTime'>
                                            <Today OffsetDays='{0}' />
                                        </Value>
                                     </Eq>
                                    </And></Where>";

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
        /// Find actions by 'Ablaufdatum'
        /// </summary>
        /// <param name="web">Quam web</param>
        /// <param name="offsetDays">offset in days (can be negative)</param>
        /// <returns></returns>
        public static SPListItemCollection FindOpenActionsByAblaufdatum(SPWeb web, int offsetDays)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, ListUtilities.Urls.Actions));
            SPQuery query = new SPQuery();

            query.Query = string.Format(queryActionsByAblaufdatum, offsetDays);
            SPListItemCollection actions = list.GetItems(query);

            return actions;
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

        /// <summary>
        /// Send eamil from sharepoint to user.
        /// </summary>
        /// <param name="pWeb"></param>
        /// <param name="pTo"></param>
        /// <param name="pBody"></param>
        /// <param name="pSubject"></param>
        /// <returns>true if mail was successfully send</returns>
        public static bool SendEmail(SPWeb pWeb, string pTo, string pBody, string pSubject)
        {
            if (pWeb == null)
            {
                throw new ArgumentNullException("pWeb");
            }
            if (string.IsNullOrEmpty(pTo))
            {
                throw new ArgumentNullException("pTo");
            }

            System.Collections.Specialized.StringDictionary messageHeaders = new System.Collections.Specialized.StringDictionary();
            //Get the “from email address” from “Outgoing e-mail settings”
            string from = pWeb.Site.WebApplication.OutboundMailSenderAddress;
            messageHeaders.Add("from", from);
            messageHeaders.Add("to", pTo);
            messageHeaders.Add("subject", pSubject);
            messageHeaders.Add("content-type", "text/html");

            bool isOK = false;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                try
                {
                    isOK = SPUtility.SendEmail(pWeb, messageHeaders, pBody);
                    if (isOK)
                    {
                        Logger.WriteLog(Logger.Category.Information, typeof(JoinAMUtilities).FullName, "Email sent.");
                    }
                    else
                    {
                        Logger.WriteLog(Logger.Category.Information, typeof(JoinAMUtilities).FullName, "Email not sent.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMUtilities).FullName, string.Format("Problem with send email '{0}' to user '{1}' with error communicate '{2}' ", pSubject, pTo, ex.Message));
                    throw new InvalidOperationException(string.Format("Problem with send email '{0}' to user '{1}' with error communicate '{2}' ", pSubject, pTo, ex.Message));
                }
            });

            return isOK;
        }

        /// <summary>
        /// Iterates through all site collections od the WebApplication and returns the ID of the Site, where the "JoinActionsTracking"-Feature is activated
        /// </summary>
        /// <param name="webApp">SPWebApplication to search for the SiteCollection</param>
        /// <returns>url of site. Returns string.Empty if not found</returns>
        public static string FindJoinActionsTrackingSiteUrl(SPWebApplication webApp)
        {
            if (webApp == null) throw new ArgumentNullException("WebApplication must be not NULL! (FindJoinActionsTrackingSiteUrl)");

            Guid siteId = FindSiteCollIdByFeature(webApp, JoinActionsTrackingFeatureId);
            if (!siteId.Equals(Guid.Empty))
            {
                try
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        return site.RootWeb.Url;
                    }

                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.Category.Unexpected, typeof(JoinAMUtilities).Name, string.Format("FindJoinActionsTrackingSiteUrl error:{0}", ex.Message));
                }
            }

            return string.Empty;
        }
    }
}
