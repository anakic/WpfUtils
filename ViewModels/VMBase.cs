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
