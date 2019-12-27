using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        private const string queryLateContracts =
                                   @"<Where>
                                     <And>
                                      <Lt>
                                        <FieldRef Name='ChangeContractWarnDate' />
                                        <Value Type='DateTime'>
                                          <Today/>
                                        </Value>
                                      </Lt>
                                      <Eq>
                                        <FieldRef Name='ChangeContractContractStatus' />
                                        <Value Type='Text'>Active</Value>
                                      </Eq>
                                    </And>
                                   </Where>";

        private const string queryDocuByAblaufdatum =
                                   @"<Where>
                                    <Eq>
                                        <FieldRef Name='Ablaufdatum' />
                                        <Value Type='DateTime'>
                                            <Today OffsetDays='{0}' />
                                        </Value>
                                     </Eq></Where>";

        private const string DocumentOverdueFirstReminderBody = @"Sehr geehrte KollegInnen,<br/> 
 
                                                                    ein Dokument, für das Sie verantwortlich sind, verliert in 30 Tagen seine Gültigkeit.<br/> 
                                                                    Wir bitten um Prüfung und Bearbeitung bis zum angegebenen Zeitpunkt:<br/>
                                                                    {0} <br/><br/>
                                                                    <a href='{1}'>Link zum Dokument</a>";
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

        static void Main(string[] args)
        {
            TestGetActions(@"http://spvm/quam/quam1");

        }

        private static void TestGetActions(string siteUrl)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    GetActions(web);
                }
            }
        }
        private static void TestGetDCDocu(string siteUrl)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    GetDCDocu(web);
                }
            }
        }

        private static void TestSendNotificationForDocuments(string siteUrl)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SendNotificationForDocuments(web, GetDCDocu(web),"", DocumentOverdueFirstReminderBody, 1); ;
                }
            }
        }

        private static SPListItemCollection GetDCDocu(SPWeb web)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, "Downloadcenter"));
            SPQuery query = new SPQuery();

            // late contracts
            query.Query = string.Format(queryDocuByAblaufdatum, 0);
            SPListItemCollection documents = list.GetItems(query);

            foreach (SPListItem docuItem in documents)
            {
                Console.WriteLine(string.Format("docu:{0}", docuItem.DisplayName));
            }

            return documents;
        }

        private static SPListItemCollection GetActions(SPWeb web)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, "Lists/Actions"));
            SPQuery query = new SPQuery();

            // late contracts
            query.Query = string.Format(queryActionsByAblaufdatum, -1);
            SPListItemCollection actions = list.GetItems(query);

            foreach (SPListItem actionItem in actions)
            {
                Console.WriteLine(string.Format("title:{0}, ct:{1}", actionItem.Title, actionItem.ContentType.Parent.Name));
            }

            return actions;
        }

        private static void SendNotificationForDocuments(SPWeb web, SPListItemCollection documents, string mailTitle, string mailBody, int reminderCount)
        {
            SPGroup groupProcessMgmnt = web.SiteGroups.GetByName("Prozessmanagament-Team");
            SPGroup groupQualityMgmnt = web.SiteGroups.GetByName("Qualitätsmanagement-Team");
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
                SPFieldUserValueCollection documentResponsible = documentItem["Dokumentenverantwortlicher"] as SPFieldUserValueCollection;
                if (documentResponsible == null)
                {
                      continue;
                }
               
                string url = Convert.ToString(documentItem[SPBuiltInFieldId.EncodedAbsUrl]);
                DateTime dueDate = Convert.ToDateTime(documentItem["Ablaufdatum"]);
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

        /// <summary>
        /// check if specified group exsits
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SPGroup GetGroupBy(SPGroupCollection groups, string name)
        {
            if (string.IsNullOrEmpty(name) ||
                (name.Length > 255) ||
                (groups == null) ||
                (groups.Count == 0))
                return null;

            foreach (SPGroup group in groups)
            {
                if (group.Name.Equals(name))
                {
                    return group;
                }
            }

            return null;
        }
    }
}
