using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riz.Common.WPF
{
    public class RelayComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _compareFunc;
        public RelayComparer(Func<T, T, bool> compareFunc)
        {
            _compareFunc = compareFunc;
        }

        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            return _compareFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
