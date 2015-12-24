using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Thingie.WPF
{
    public interface INotifyHaveChanges : INotifyPropertyChanged
    {
        bool HasChanges { get; }
    }
}
