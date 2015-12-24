using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riz.Common.WPF
{
    public class FlatHierarchy<T> : List<T>
    {
        public FlatHierarchy(T element, Func<T, IEnumerable<T>> getChildFunc)
        {
            AddElements(element, getChildFunc);
        }

        public FlatHierarchy(IEnumerable<T> rootElements, Func<T, IEnumerable<T>> getChildFunc)
        {
            foreach (T element in rootElements)
                AddElements(element, getChildFunc);
        }

        private void AddElements(T element, Func<T, IEnumerable<T>> getChildFunc)
        {
            this.Add(element);
            IEnumerable<T> children = getChildFunc(element);
            if (children != null)
            {
                foreach (T child in children)
                {
                    AddElements(child, getChildFunc);
                }
            }
        }
    }
}
