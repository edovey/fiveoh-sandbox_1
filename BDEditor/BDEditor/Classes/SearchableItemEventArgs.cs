using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
   public  class SearchableItemEventArgs : EventArgs
    {
       private Guid itemId;
       private string itemKeyName;

       /// <summary>
       /// Class to define event args for searchable item changes.  
       /// Arguments are eventually used to build the metadata record.
       /// </summary>
       /// <param name="pName"></param>
       /// <param name="pItemId"></param>
       public SearchableItemEventArgs(Guid pItemId, string pItemKeyName)
       {
           itemId = pItemId;
           itemKeyName = pItemKeyName;
       }

       /// <summary>
       /// Guid of object with searchable term or phrase 
       /// </summary>
       public Guid ItemId
       {
           get { return itemId; }
       }

       /// <summary>
       /// Class name of the item
       /// </summary>
       public string ItemKeyName
       {
           get { return itemKeyName; }
       }

    }
}
