using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
    class MultiChoicePropertyProxy : ChoicePropertyProxy
    {
        public MultiChoicePropertyProxy(IEnumerable choices) : base (choices)
        {

        }
    }
}
