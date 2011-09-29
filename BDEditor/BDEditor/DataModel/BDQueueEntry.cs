using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.DataModel
{
    /// <summary>
    /// Extension of generated BDQueueEntry
    /// </summary>
    public partial class BDQueueEntry
    {
        public static BDQueueEntry GetQueueEntryWithId(Guid pQueueEntryId)
        {
            BDQueueEntry queueEntry;
            using (BDEditor.DataModel.Entities context = new BDEditor.DataModel.Entities())
            {
                IQueryable<BDQueueEntry> queueEntries = (from bdQueueEntries in context.BDQueueEntries
                                                         where bdQueueEntries.uuid == pQueueEntryId
                                                         select bdQueueEntries);
                queueEntry = queueEntries.AsQueryable().First<BDQueueEntry>();
            }
            return queueEntry;
        }
    }
}
