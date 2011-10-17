using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    public partial class BDSystemSetting
    {
        public const string LASTSYNC_TIMESTAMP = @"lastSync";

        public static BDSystemSetting GetSetting(Entities pDataContext, string pSettingName)
        {
            BDSystemSetting setting;
            IQueryable<BDSystemSetting> entries = (from entry in pDataContext.BDSystemSettings
                                                   where entry.settingName == pSettingName
                                                   select entry);
            if (entries.Count<BDSystemSetting>() <= 0)
            {
                setting = BDSystemSetting.CreateBDSystemSetting(Guid.NewGuid(), pSettingName);
                pDataContext.AddObject("BDSystemSettings", setting);
                pDataContext.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Created SystemSetting:{0}", pSettingName);
            }
            else
            {
                setting = entries.AsQueryable().First<BDSystemSetting>();
            }

            return setting;
        }

        public static DateTime? GetTimestamp(Entities pDataContext, string pTimestampName)
        {
            DateTime? result = null;
            BDSystemSetting entry = GetSetting(pDataContext, pTimestampName);
            if (null != entry.settingValue)
            {
                result = DateTime.Parse(entry.settingValue);
            }
            return result;
        }

        public static void SaveTimestamp(Entities pDataContext, string pTimestampName, DateTime? pDateTime)
        {
            BDSystemSetting entry = GetSetting(pDataContext, pTimestampName);
            if (null == pDateTime)
                entry.settingValue = null;
            else
                entry.settingValue = pDateTime.Value.ToString(BDEditor.Classes.Constants.DATETIMEFORMAT);
            pDataContext.SaveChanges();
        }

    }
}
