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
    public class BDSearchEntryBindingList : BindingList<BDSearchEntry>
    {
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;
        private string stringToMatch;

        public BDSearchEntryBindingList() : this(new List<BDSearchEntry>()) { }

        public BDSearchEntryBindingList(IList<BDSearchEntry> list) : base(list) { }

        #region Sort support
        public void Sort(string field, ListSortDirection direction)
        {
            if (this.Count > 0)
            {
                PropertyDescriptorCollection properties =  TypeDescriptor.GetProperties(this.Items[0]);
                PropertyDescriptor myProperty = properties.Find(field,  false);
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
            List<BDSearchEntry> list = (List<BDSearchEntry>)Items;
            list.Sort(delegate(BDSearchEntry lhs, BDSearchEntry rhs)
            {
                if (sortProperty != null)
                {
                    isSorted = true;
                    object lhsValue = lhs == null ? null : sortProperty.GetValue(lhs);
                    object rhsValue = rhs == null ? null :  sortProperty.GetValue(rhs);
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

        private bool findMatch(BDSearchEntry pEntry)
        {
            bool match = false;
            if (Items != null)
            {
                match = (string.Compare(stringToMatch, pEntry.name, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return match;
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            int index = -1;
            stringToMatch = key as string;
            List<BDSearchEntry> items = base.Items as List<BDSearchEntry>;
            if ((items != null) && (!string.IsNullOrEmpty(stringToMatch)))
                index = items.FindIndex(this.findMatch);
            return index;
        }
        #endregion
    }
}
