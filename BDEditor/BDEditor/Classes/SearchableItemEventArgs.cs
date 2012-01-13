using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
   public  class SearchableItemEventArgs : EventArgs
    {
       private Guid itemId;
       private string itemEntityName;
       private Guid? displayParentId;
       private string displayParentEntityName;
       private string demographic;
       private string disease;

       /// <summary>
       /// Class to define event args for searchable item changes.  
       /// Arguments are eventually used to build the metadata record.
       /// </summary>
       /// <param name="pName"></param>
       /// <param name="pItemId"></param>
       public SearchableItemEventArgs(Guid pItemId, string pItemEntityName)
       {
           itemId = pItemId;
           itemEntityName = pItemEntityName;
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
       public string ItemEntityName
       {
           get { return itemEntityName; }
       }

       /// <summary>
       /// Guid of the entity that owns the 'page' where the searchable term will
       /// eventually be displayed
       /// </summary>
       public Guid? DisplayParentId
       {
           set { displayParentId = value; }
           get { return displayParentId; }
       }

       /// <summary>
       /// entity name of the parent of the searchable term
       /// </summary>
       public string DisplayParentEntityName
       {
           set { displayParentEntityName = value; }
           get { return displayParentEntityName; }
       }

       /// <summary>
       /// Demographic to which the searchable term applies (Adult, Paediatric, etc.)
       /// </summary>
       public string Demographic
       {
           set { demographic = value; }
           get { return Demographic; }
       }

       /// <summary>
       /// Disease that may be associated with the searchable term
       /// </summary>
       public string Disease
       {
           set { disease = value; }
           get { return disease; }
       }
    }
}
