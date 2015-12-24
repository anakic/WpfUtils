using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Riz.Common.WPF.Attributes;

namespace Riz.Common.WPF
{
    public interface IViewFactory
    {
        object GetView(object obj);
        object GetView(string context, object obj);
    }

    public class AttributeBasedViewFactory:IViewFactory
    {
        #region IViewFactory Members

        public object GetView(object obj)
        {
            return GetView(null, obj);
        }

        public object GetView(string context, object obj)
        {
            ViewAttribute att = obj.GetType().GetCustomAttributes(true).OfType<ViewAttribute>().Where(va => va.Context == context).SingleOrDefault();
            return Activator.CreateInstance(att.TypeOfView, att.ViewConstructorParams);
        }

        #endregion
    }
}
