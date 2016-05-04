namespace Thingie.WPF
{
	public sealed class Progress : ChangeNotifierBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        private string _subTitle;
        public string SubTitle
        {
            get { return _subTitle; }
            set { _subTitle = value; OnPropertyChanged("SubTitle"); }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set { _currentProgress = value; OnPropertyChanged("CurrentProgress"); }
        }

        private int _maxProgress;
        public int MaxProgress
        {
            get { return _maxProgress; }
            set { _maxProgress = value; OnPropertyChanged("MaxProgress"); OnPropertyChanged("IsIndeterminate"); }
        }

        public bool IsIndeterminate
        {
            get { return MaxProgress > 0; }
        }

        public Progress()
        {
            Title = "Working...";
            MaxProgress = 0;
        }
    }
}
