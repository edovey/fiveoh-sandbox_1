using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    public partial class BDSystemSetting
    {
        public const string LASTSYNC_TIMESTAMP = @"lastSync";
        public const string ARCHIVE_TIMESTAMP = @"archiveTimestamp";
        public const string SERIAL_NUMBER = @"serialNumber";
        public const string INDEX_NUMBER = @"indexNumber";
        public const string CONTROL_NUMBER = @"controlNumber";

        public static BDSystemSetting RetrieveSetting(Entities pDataContext, string pSettingName)
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

        public static BDSystemSetting RetrieveRawSetting(Entities pDataContext, string pSettingName)
        {
            BDSystemSetting setting = null;
            IQueryable<BDSystemSetting> entries = (from entry in pDataContext.BDSystemSettings
                                                   where entry.settingName == pSettingName
                                                   select entry);
            if (entries.Count<BDSystemSetting>() > 0)
            {
                setting = entries.AsQueryable().First<BDSystemSetting>();
            }

            return setting;
        }
        public static string RetrieveSettingValue(Entities pDataContext, string pSettingName)
        {
            BDSystemSetting setting = RetrieveSetting(pDataContext, pSettingName);
            return setting.settingValue;
        }

        public static void WriteSettingValue(Entities pDataContext, string pSettingName, string value)
        {
            BDSystemSetting setting = RetrieveSetting(pDataContext, pSettingName);
            setting.settingValue = value;
            pDataContext.SaveChanges();
        }

        //public static DateTime? GetTimestamp(Entities pDataContext, string pTimestampName)
        //{
        //    DateTime? result = null;
        //    BDSystemSetting entry = RetrieveSetting(pDataContext, pTimestampName);
        //    if (null != entry.settingValue)
        //    {
        //        result = DateTime.Parse(entry.settingValue);
        //    }
        //    return result;
        //}

        //public static void SaveTimestamp(Entities pDataContext, string pTimestampName, DateTime? pDateTime)
        //{
        //    BDSystemSetting entry = RetrieveSetting(pDataContext, pTimestampName);
        //    if (null == pDateTime)
        //        entry.settingValue = null;
        //    else
        //        entry.settingValue = pDateTime.Value.ToString(BDEditor.Classes.Constants.DATETIMEFORMAT);
        //    pDataContext.SaveChanges();
        //}

    }
}
