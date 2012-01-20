using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;

using BDEditor.Classes;

namespace BDEditor.DataModel
{
    public partial class BDObjectAssociation
    {
        public const string AWS_PROD_DOMAIN = @"bd_1_objectAssociation";
        public const string AWS_DEV_DOMAIN = @"bd_dev_1_objectAssociation";

#if DEBUG
        public const string AWS_DOMAIN = AWS_DEV_DOMAIN;
#else
        public const string AWS_DOMAIN = AWS_PROD_DOMAIN;
#endif
        public const string ENTITYNAME = @"BDObjectAssociations";
        public const string ENTITYNAME_FRIENDLY = @"ObjectAssociations";
        public const string KEY_NAME = @"BDObjectAssociation";
        public const int ENTITY_SCHEMAVERSION = 0;

        private const string UUID = @"oa_uuid";
        private const string OBJECTID = @"oa_objectId";

        private const string OBJECTKEYNAME = @"oa_objectKeyName";
        private const string CHILDKEYNAME = @"oa_childKeyName";

        public static BDObjectAssociation CreateObjectAssociation(Entities pContext, Guid pObjectId, string pObjectKeyName, string pChildKeyName)
        {
            BDObjectAssociation association = CreateBDObjectAssociation(Guid.NewGuid());
            association.objectId = pObjectId;
            association.objectKeyName = pObjectKeyName;
            association.childKeyName = pChildKeyName;

            pContext.AddObject(ENTITYNAME, association);

            return association;
        }


    }
}
