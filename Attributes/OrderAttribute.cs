using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Attributes
{
    public class OrderAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}
