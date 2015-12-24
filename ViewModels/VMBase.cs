using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thingie.WPF.ViewModels
{
    public class VMBase<TModel> : ChangeNotifierBase
        where TModel : class
    {
        public TModel Model { get; private set; }

        public VMBase(TModel model)
        {
            Model = model;
        }
    }
}
