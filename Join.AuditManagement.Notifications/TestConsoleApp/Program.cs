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

        static void Main(string[] args)
        {
            TestGetDCDocu(@"http://spvm/quam/quam1");

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

        private static void GetDCDocu(SPWeb web)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, "Downloadcenter"));
            SPQuery query = new SPQuery();

            // late contracts
            query.Query = string.Format(queryDocuByAblaufdatum, -1);
            SPListItemCollection documents = list.GetItems(query);

            foreach (SPListItem docuItem in documents)
            {
                Console.WriteLine(string.Format("docu:{0}", docuItem.DisplayName));
            }
        }
    }
}
