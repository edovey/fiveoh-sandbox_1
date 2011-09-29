using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated class BDTherapy
    /// </summary>
    public partial class BDTherapy
    {
        /// <summary>
        /// Extended Create method that sets created date and schema version
        /// </summary>
        /// <returns></returns>
        public static BDTherapy CreateTherapy()
        {
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                BDTherapy therapy = CreateBDTherapy(Guid.NewGuid(), false);
                therapy.createdBy = Guid.Empty;
                therapy.createdDate = DateTime.Now;
                therapy.schemaVersion = 0;

                context.AddObject("BDTherapies", therapy);

                return therapy;
            }
        }
    }
}
