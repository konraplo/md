namespace Join.AuditManagement.Notifications.Common
{
    using Microsoft.SharePoint;

    /// <summary>
    /// This class stores the ids of all of the used contenttypes.
    /// </summary>
    public static class ContentTypeIds
    {
        /// <summary>
        /// SPContentTypeId of the RisikoChanceMassnahmen ContentType
        /// </summary>
        public static SPContentTypeId RisikoChanceMassnahmen = new SPContentTypeId("0x0100D9009AAA078445DB9CAD017EEB2884EA0021CADF4988646B4D926500F5F9BC4AE0");

        /// <summary>
        /// SPContentTypeId of the MassnahmeausUnternehmenszielen ContentType
        /// </summary>
        public static SPContentTypeId MassnahmeausUnternehmenszielen = new SPContentTypeId("0x0100D9009AAA078445DB9CAD017EEB2884EA0021CADF4988646B4D926500F5F9BC4AE002");

        /// <summary>
        /// SPContentTypeId of the MassnahmeausPRIMA ContentType
        /// </summary>
        public static SPContentTypeId MassnahmeausPRIMA = new SPContentTypeId("0x0100D9009AAA078445DB9CAD017EEB2884EA0021CADF4988646B4D926500F5F9BC4AE001");

        /// <summary>
        /// SPContentTypeId of the Massnahme ContentType
        /// </summary>
        public static SPContentTypeId Massnahme = new SPContentTypeId("0x0100D9009AAA078445DB9CAD017EEB2884EA");
    }
}
