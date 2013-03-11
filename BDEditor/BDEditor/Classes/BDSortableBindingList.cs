using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace BDEditor.Classes
{
    public class BDSortableBindingList<T> : BindingList<T>
    {
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;

        public BDSortableBindingList() : base() { }

        public BDSortableBindingList(IList<T> list) : base(list)
        {
        }

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
            List<T> list = (List<T>)Items;
            list.Sort(delegate(T lhs, T rhs)
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
    }
}
