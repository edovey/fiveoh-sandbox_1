using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    class BDSearchEntryAssociationBindingList : BindingList<BDSearchEntryAssociation>
    {
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;
        private string stringToMatch;

        public BDSearchEntryAssociationBindingList() : this(new List<BDSearchEntryAssociation>()) { }
        public BDSearchEntryAssociationBindingList(IList<BDSearchEntryAssociation> list) : base(list) { }

        #region Sort support
        public void Sort(string field, ListSortDirection direction)
        {
            if (this.Count > 0)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.Items[0]);
                PropertyDescriptor myProperty = properties.Find(field, false);
                if (myProperty != null)
                    ApplySortCore(myProperty, direction);
            }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }

        protected override void ApplySortCore(PropertyDescriptor pDescriptor, ListSortDirection pDirection)
        {
            sortProperty = pDescriptor;
            sortDirection = pDirection;
            List<BDSearchEntryAssociation> list = (List<BDSearchEntryAssociation>)Items;
            list.Sort(delegate(BDSearchEntryAssociation lhs, BDSearchEntryAssociation rhs)
            {
                if (sortProperty != null)
                {
                    isSorted = true;
                    object lhsValue = lhs == null ? null : sortProperty.GetValue(lhs);
                    object rhsValue = rhs == null ? null : sortProperty.GetValue(rhs);
                    int result = System.Collections.Comparer.Default.Compare(lhsValue, rhsValue);
                    if (sortDirection == ListSortDirection.Descending)
                    {
                        result = -result;
                    }
                    return result;
                }
                else
                {
                    isSorted = false;
                    return 0;
                }
            }
            );

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            sortDirection = ListSortDirection.Ascending;
            sortProperty = null;
        }
        #endregion

        #region Find/Search support

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        private bool findMatch(BDSearchEntryAssociation pEntry)
        {
            bool match = false;
            if (Items != null)
            {
                match = (string.Compare(stringToMatch, pEntry.displayContext, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return match;
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            int index = -1;
            stringToMatch = key as string;
            List<BDSearchEntryAssociation> items = base.Items as List<BDSearchEntryAssociation>;
            if ((items != null) && (!string.IsNullOrEmpty(stringToMatch)))
                index = items.FindIndex(this.findMatch);
            return index;
        }
        #endregion
    }
}
