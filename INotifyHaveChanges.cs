using System.ComponentModel;

namespace Thingie.WPF
{
	public interface INotifyHaveChanges : INotifyPropertyChanged
    {
        bool HasChanges { get; }
    }
}
