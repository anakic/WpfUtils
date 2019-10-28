using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public interface INodeFactory
    {
        NodeVM GetNode(object obj);

        IEnumerable<object> GetChildObjects(object obj);
    }
}
