using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Join.AuditManagement.Notifications.Common
{
    /// <summary>
    /// Helpermethods with solutionwide accessible methods and functions.
    /// </summary>
    public static class JoinAMUtilities
    {
        public const string JoinAMNotificationTimerJobName = "Join Audit Management Notification Timer job";
        
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
        /// Find documents in download center by 'Ablaufdatum'
        /// </summary>
        /// <param name="web">Quam web</param>
        /// <param name="offsetDays">offset in days (can be negative)</param>
        /// <returns></returns>
        public static SPListItemCollection findDocumentsByAblaufdatum(SPWeb web, int offsetDays)
        {
            SPList list = web.GetList(SPUtility.ConcatUrls(web.Url, ListUtilities.Urls.Downloadcenter));
            SPQuery query = new SPQuery();

            // late contracts
            query.Query = string.Format(queryDocumentsByAblaufdatum, offsetDays);
            SPListItemCollection documents = list.GetItems(query);

            return documents;
        }
    }
}
