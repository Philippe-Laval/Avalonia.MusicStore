using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using ReactiveUI;

namespace Avalonia.MusicStore.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        // BuyMusicCommand = ReactiveCommand.Create(() =>
        // {
        //     // Code here will be executed when the button is clicked.
        //     Debug.WriteLine("test");
        // });
        
        BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new MusicStoreViewModel();

            var result = await ShowDialog.Handle(store);
        });
    }
    
    
    public ICommand BuyMusicCommand { get; }
    
    public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
    
    
    
}