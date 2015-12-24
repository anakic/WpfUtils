using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Thingie.WPF.MarkupExtensions
{
    public class AttirbuteValueExtension : MarkupExtension
    {
        public Type AttributeType { get; set; }

        public string AttributeProperty { get; set; }

        public Type TargetType { get; set; }
        public object Target { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object val;

            Type type = null;
            if (Target != null)
            {
                type = Target.GetType();
            }
            else if (TargetType != null)
            {
                type = TargetType;
            }

            if ((type == null) || (AttributeType == null) || (AttributeProperty == null))
                val = null;
            else
            {
                Attribute att = (Attribute)TargetType.GetCustomAttributes(true).Where(a => a.GetType() == AttributeType).SingleOrDefault();
                if (att == null)
                    val = null;
                else
                    val = att.GetType().GetProperty(AttributeProperty).GetValue(att, null);
            }
            return val;
        }
    }
}
