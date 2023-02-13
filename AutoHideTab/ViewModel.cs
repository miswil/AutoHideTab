using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace AutoHideTab
{
    internal class ViewModel : ObservableObject
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<object> Tabs { get; } = new();
        private object? selectedTab;
        public object? SelectedTab
        {
            get => this.selectedTab;
            set => this.SetProperty(ref this.selectedTab, value);
        }

        public ObservableCollection<int> Items { get; } = new(Enumerable.Range(0, 100));

        public ICommand Add1Command { get; }
        public ICommand Add2Command { get; }

        public ViewModel()
        {
            this.Add1Command = new RelayCommand(() => this.Tabs.Add(this.SelectedTab = new VM1()));
            this.Add2Command = new RelayCommand(() => this.Tabs.Add(this.SelectedTab = new VM2()));
        }
    }

    internal class VM1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<char> Items { get; } = new(Enumerable.Range(0, 52).Select(i => (char)('A' + i)));
    }

    internal class VM2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<string> Items { get; } = new(Enumerable.Range(0, 100).Select(i => $"string{i}"));
    }
}
