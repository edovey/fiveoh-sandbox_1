using System;
using System.Collections.Generic;
using System.Data;
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
        public static BDQueueEntry GetQueueEntryWithId(Entities pContext, Guid pQueueEntryId)
        {
            BDQueueEntry queueEntry;
            IQueryable<BDQueueEntry> queueEntries = (from bdQueueEntries in pContext.BDQueueEntries
                                                        where bdQueueEntries.uuid == pQueueEntryId
                                                        select bdQueueEntries);
            queueEntry = queueEntries.AsQueryable().First<BDQueueEntry>();
            return queueEntry;
        }
    }
}
