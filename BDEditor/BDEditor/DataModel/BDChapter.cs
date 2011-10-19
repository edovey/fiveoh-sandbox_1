using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;
using System.Data;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDChapter
    /// </summary>
    public partial class BDChapter
    {
        public const string AWS_DOMAIN = @"bd_chapters";
        public const string ENTITYNAME = @"BDChapters";
        public const string ENTITYNAME_FRIENDLY = @"Chapter";

        private const string UUID = @"ch_uuid";
        private const string SCHEMAVERSION = @"ch_schemaversion";
        private const string CREATEDBY = @"ch_createdby";
        private const string CREATEDDATE = @"ch_createddate";
        private const string MODIFIEDBY = @"ch_modifiedby";
        private const string MODIFIEDDATE = @"ch_modifieddate";
        private const string NAME = @"ch_name";
        private const string DEPRECATED = @"ch_deprecated";
        private const string DISPLAYORDER = @"ch_displayorder";

        /// <summary>
        /// Extended Create method that sets the created date and the schema version
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public static BDChapter CreateChapter(Entities pContext)
        {
            BDChapter chapter = CreateBDChapter(Guid.NewGuid(), false);
            chapter.createdBy = Guid.Empty;
            chapter.createdDate = DateTime.Now;
            chapter.schemaVersion = 0;
            chapter.displayOrder = -1;
            chapter.name = string.Empty;

            pContext.AddObject("BDChapters", chapter);

            return chapter;
        }

        /// <summary>
        /// Extended Save method that sets the modified date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pChapter"></param>
        public static void SaveChapter(Entities pContext, BDChapter pChapter)
        {
            if (pChapter.EntityState != EntityState.Unchanged)
            {
                //pChapter.modifiedBy = Guid.Empty;
                //pChapter.modifiedDate = DateTime.Now;
                System.Diagnostics.Debug.WriteLine(@"Chapter Save");
                pContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get the Chapter with the specified ID
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pChapterId"></param>
        /// <returns>BDChapter object</returns>
        public static BDChapter GetChapterWithId(Entities pContext, Guid pChapterId)
        {
            BDChapter chapter = null;

            if(null != pChapterId)
            {
                IQueryable<BDChapter> entryList = (from entries in pContext.BDChapters
                                                  where entries.uuid == pChapterId
                                                  select entries);
                if (entryList.Count<BDChapter>() > 0)
                    chapter = entryList.AsQueryable().First<BDChapter>();
            }
            return chapter;
        }

        public static List<BDChapter> GetAll(Entities pContext)
        {
            List<BDChapter> entryList = new List<BDChapter>();
            IQueryable<BDChapter> entries = (from entry in pContext.BDChapters
                                             orderby entry.displayOrder
                                             select entry);
            if (entries.Count() > 0)
                entryList = entries.ToList<BDChapter>();
            return entryList;
        }

        protected override void OnPropertyChanged(string property)
        {
            switch (property)
            {
                case "createdBy":
                case "createdDate":
                case "modifiedBy":
                case "modifiedDate":
                    break;
                default:
                    {
                        modifiedBy = Guid.Empty;
                        modifiedDate = DateTime.Now;
                    }
                    break;
            }

            base.OnPropertyChanged(property);
        }
        #region Repository

        /// <summary>
        /// Retrieve all entries changed since a given date
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pUpdateDateTime">Null date will return all records</param>
        /// <returns>List of entries. Empty list if none found.</returns>
        public static List<BDChapter> GetEntriesUpdatedSince(Entities pContext, DateTime? pUpdateDateTime)
        {
            List<BDChapter> entryList = new List<BDChapter>();
            IQueryable<BDChapter> entries;

            if (null == pUpdateDateTime)
            {
                entries = (from entry in pContext.BDChapters
                           select entry);
            }
            else
            {
                entries = (from entry in pContext.BDChapters
                           where entry.modifiedDate > pUpdateDateTime.Value
                           select entry);
            }
            if (entries.Count() > 0)
                entryList = entries.ToList<BDChapter>();
            return entryList;
        }

        public static SyncInfo SyncInfo()
        {
            return new SyncInfo(AWS_DOMAIN, MODIFIEDDATE);
        }

        /// <summary>
        /// Create or update an existing BDSection from attributes in a dictionary. Saves the entry.
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pAttributeDictionary"></param>
        /// <returns>Uuid of the created/updated entry</returns>
        public static Guid? LoadFromAttributes(Entities pDataContext, AttributeDictionary pAttributeDictionary, bool pSaveChanges)
        {
            Guid uuid = Guid.Parse(pAttributeDictionary[UUID]);
            bool deprecated = bool.Parse(pAttributeDictionary[DEPRECATED]);
            BDChapter entry = BDChapter.GetChapterWithId(pDataContext, uuid);
            if (null == entry)
            {
                entry = BDChapter.CreateBDChapter(uuid, deprecated);
                pDataContext.AddObject("BDChapters", entry);
            }

            short schemaVersion = short.Parse(pAttributeDictionary[SCHEMAVERSION]);
            entry.schemaVersion = schemaVersion;
            short displayOrder = (null == pAttributeDictionary[DISPLAYORDER]) ? (short)-1 : short.Parse(pAttributeDictionary[DISPLAYORDER]);
            entry.displayOrder = displayOrder;
            entry.createdBy = Guid.Parse(pAttributeDictionary[CREATEDBY]);
            entry.createdDate = DateTime.Parse(pAttributeDictionary[CREATEDDATE]);
            entry.modifiedBy = Guid.Parse(pAttributeDictionary[MODIFIEDBY]);
            entry.modifiedDate = DateTime.Parse(pAttributeDictionary[MODIFIEDDATE]);
            entry.name = pAttributeDictionary[NAME];

            if (pSaveChanges)
                pDataContext.SaveChanges();

            return uuid;
        }

        public PutAttributesRequest PutAttributes()
        {
            PutAttributesRequest putAttributeRequest = new PutAttributesRequest().WithDomainName(AWS_DOMAIN).WithItemName(this.uuid.ToString().ToUpper());
            List<ReplaceableAttribute> attributeList = putAttributeRequest.Attribute;
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.UUID).WithValue(uuid.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.SCHEMAVERSION).WithValue(string.Format(@"{0}", schemaVersion)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.DISPLAYORDER).WithValue(string.Format(@"{0}", displayOrder)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.CREATEDBY).WithValue((null == createdBy) ? Guid.Empty.ToString() : createdBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.CREATEDDATE).WithValue((null == createdDate) ? string.Empty : createdDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.MODIFIEDBY).WithValue((null == modifiedBy) ? Guid.Empty.ToString() : modifiedBy.ToString().ToUpper()).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.MODIFIEDDATE).WithValue((null == modifiedDate) ? string.Empty : modifiedDate.Value.ToString(Constants.DATETIMEFORMAT)).WithReplace(true));
            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.DEPRECATED).WithValue(deprecated.ToString()).WithReplace(true));

            attributeList.Add(new ReplaceableAttribute().WithName(BDChapter.NAME).WithValue(name).WithReplace(true));

            return putAttributeRequest;
        }
        #endregion

    }
}
