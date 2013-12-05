using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Amazon.SimpleDB.Model;

namespace BDEditor.Classes
{
    public class RepositoryControlNumber
    {
        public const string REPOSITORY_CONTROL_NUMBER_DOMAIN = @"bd_ControlNumber";

#if DEBUG
        public const string ENVIRONMENT_CONTEXT = @"Development";
#else
         public const string ENVIRONMENT_CONTEXT = @"Production";
#endif

        public const int ENTITY_SCHEMAVERSION = 1;
        public const Int32 SERIAL_NUMBER_BASE = 10000;
        public const Int32 SERIAL_NUMBER_UNDEFINED = 0;
        public const Int32 INDEX_NUMBER_BASE = 0;

        public const string SCHEMA_VERSION = @"cn_schema_version";
        public const string UUID = @"cn_uuid";
        public const string CONTROL_NUMBER = @"cn_control_number";
        public const string SERIAL_NUMBER = @"cn_serial_number";
        public const string INDEX_NUMBER = @"cn_index_number";
        public const string USERNAME = @"cn_username";
        public const string CONTENT_DATE = @"cn_content_date";
        public const string INDEX_DATE = @"cn_index_date";
        public const string COMMENT = @"cn_comment";
        public const string BUCKET_NAME = @"cn_bucket_name";
        public const string ARCHIVE_NAME = @"cn_archive_name";
        public const string APP_VERSION = @"cn_app_version";
        public const string MACHINE_NAME = @"cn_machine_name";
        public const string MACHINE_PATH = @"cn_machine_path";
        public const string MACHINE_FILENAME = @"cn_machine_filename";

        public Guid uuid;
        public string environment;
        public Int32 serialNumber;
        public Int32 indexNumber;
        public string userName;
        public DateTime? contentDate;
        public DateTime? indexDate;
        public string comment;
        public string bucketName;
        public string archiveName;
        public string appVersion;
        public string machineName;
        public string machinePath;
        public string machineFilename;


        public void increment(Boolean incSerialNumber, Boolean incIndexNumber)
        {
            if (incSerialNumber) this.serialNumber++;
            if (incIndexNumber) this.indexNumber++;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(RepositoryControlNumber.REPOSITORY_CONTROL_NUMBER_DOMAIN).WithItemName(ControlNumberText);
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.ENVIRONMENT_CONTEXT).WithValue(environment).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.SCHEMA_VERSION).WithValue(string.Format(@"{0}", ENTITY_SCHEMAVERSION)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.USERNAME).WithValue(userName ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.CONTENT_DATE).WithValue((null == contentDate) ? string.Empty : contentDate.Value.ToString(BDConstants.DATETIMEFORMAT_ZULU)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.INDEX_DATE).WithValue((null == indexDate) ? string.Empty : indexDate.Value.ToString(BDConstants.DATETIMEFORMAT_ZULU)).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.COMMENT).WithValue(comment ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.BUCKET_NAME).WithValue(bucketName ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.ARCHIVE_NAME).WithValue(archiveName ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.APP_VERSION).WithValue(appVersion ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.MACHINE_NAME).WithValue(machineName ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.MACHINE_PATH).WithValue(machinePath ?? string.Empty).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.MACHINE_FILENAME).WithValue(machineFilename ?? string.Empty).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.SERIAL_NUMBER).WithValue(SerialNumberString(serialNumber)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.INDEX_NUMBER).WithValue(IndexNumberString(indexNumber)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(RepositoryControlNumber.CONTROL_NUMBER).WithValue(ControlNumberText).WithReplace(true));

            return putAttributeRequest;
        }

        public string ControlNumberText
        {
            get { return ControlNumberString(this.serialNumber, this.indexNumber); }
        }

        public string ContentDateText
        {
            get { return (contentDate.HasValue) ? contentDate.Value.ToString("MMMM dd, yyyy") : @""; }    
        }

        public string IndexDateText
        {
            get { return (indexDate.HasValue) ? indexDate.Value.ToString("MMMM dd, yyyy") : @""; }
        }

        public static string SerialNumberString(Int32 pSerialNumber)
        {
            return string.Format(@"{0:00000}", pSerialNumber);
        }

        public static string IndexNumberString(Int32 pIndexNumber)
        {
            return string.Format(@"{0:000}", pIndexNumber);
        }

        public static string ControlNumberString(Int32 pSerialNumber, Int32 pIndexNumber)
        {
            return string.Format(@"{0}.{1}", SerialNumberString(pSerialNumber), IndexNumberString(pIndexNumber));
        }

        public static RepositoryControlNumber Create(Int32 pSerialNumber, Int32 pIndexNumber)
        {
            RepositoryControlNumber cn = new RepositoryControlNumber
                {
                    uuid = Guid.NewGuid(),
                    environment = RepositoryControlNumber.ENVIRONMENT_CONTEXT,
                    serialNumber = pSerialNumber,
                    indexNumber = pIndexNumber,
                    contentDate = DateTime.Now,
                    indexDate = null
                };

            return cn;
        }

        public static RepositoryControlNumber LoadFromSdbItem(Amazon.SimpleDB.Model.Item item)
        {
            RepositoryControlNumber cn = null;
            if (null != item)
            {
                cn = RepositoryControlNumber.Create(0, 0);

                foreach (Amazon.SimpleDB.Model.Attribute attribute in item.Attribute)
                {
                    switch (attribute.Name)
                    {
                        case UUID:
                            cn.uuid = Guid.Parse(attribute.Value);
                            break;
                        case SCHEMA_VERSION:
                            //do nothing
                            break;
                        case USERNAME:
                            cn.userName = attribute.Value;
                            break;
                        case CONTENT_DATE:
                            cn.contentDate = DateTime.ParseExact(attribute.Value, BDConstants.DATETIMEFORMAT_ZULU, CultureInfo.CurrentCulture);
                            break;
                        case INDEX_DATE:
                            if (attribute.IsSetValue() && (!String.IsNullOrEmpty(attribute.Value)) )
                            {
                                cn.indexDate = DateTime.ParseExact(attribute.Value, BDConstants.DATETIMEFORMAT_ZULU, CultureInfo.CurrentCulture);
                            }
                            else
                            {
                                cn.indexDate = null;
                            }
                            break;
                        case COMMENT:
                            cn.comment = attribute.Value;
                            break;
                        case BUCKET_NAME:
                            cn.bucketName = attribute.Value;
                            break;
                        case ARCHIVE_NAME:
                            cn.archiveName = attribute.Value;
                            break;
                        case APP_VERSION:
                            cn.appVersion = attribute.Value;
                            break;
                        case MACHINE_NAME:
                            cn.machineName = attribute.Value;
                            break;
                        case MACHINE_FILENAME:
                            cn.machineFilename = attribute.Value;
                            break;
                        case MACHINE_PATH:
                            cn.machinePath = attribute.Value;
                            break;
                        case SERIAL_NUMBER:
                            cn.serialNumber = Int32.Parse(attribute.Value);
                            break;
                        case INDEX_NUMBER:
                            cn.indexNumber = Int32.Parse(attribute.Value);
                            break;
                        case CONTROL_NUMBER:
                            // do nothing
                            break;
                    }
                }
            }
            return cn;
        }
    }
}
