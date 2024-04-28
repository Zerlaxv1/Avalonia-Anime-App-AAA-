using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.SimpleRouter;

namespace Avalonia_RandomAnimeTorrentApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase content = default!;

        public MainWindowViewModel(HistoryRouter<ViewModelBase> router)
        {
            router.CurrentViewModelChanged += viewModel => Content = viewModel;

            router.GoTo<SearchAndInfoViewModel>();
        }
    }
}