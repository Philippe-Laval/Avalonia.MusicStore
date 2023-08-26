using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows.Input;
using Avalonia.MusicStore.Models;
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
            if (result != null)
            {
                Albums.Add(result);
                await result.SaveToDiskAsync();
            }
        });
        
        RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }
    
    
    public ICommand BuyMusicCommand { get; }
    
    public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
    
    public ObservableCollection<AlbumViewModel> Albums { get; } = new();
    
    private async void LoadAlbums()
    {
        var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

        // After this all the album view models are added to the observable collection -
        // this will instantly update the UI with the text data for the albums.
        
        foreach (var album in albums)
        {
            Albums.Add(album);
        }

        // You will notice that after the JSON album files are loaded, the second loop loads the cover art image files.
        // This provides your user with visual feedback as quickly as possible (in the form of album tiles with text
        // and the placeholder music note icon) about what albums are in the collection.
        // The cover art is then loaded asynchronously.
        // This ensures that the app remains responsive during the image loading process. 
        
        foreach (var album in Albums.ToList())
        {
            await album.LoadCover();
        }
    }
}