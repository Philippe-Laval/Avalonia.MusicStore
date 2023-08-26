using Avalonia.MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace Avalonia.MusicStore.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private string? _searchText;
    private bool _isBusy = false;
    private CancellationTokenSource? _cancellationTokenSource;
    
    public MusicStoreViewModel()
    {
        // SearchResults.Add(new AlbumViewModel());
        // SearchResults.Add(new AlbumViewModel());
        // SearchResults.Add(new AlbumViewModel());
        
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(DoSearch!);
    }
    
    private async void DoSearch(string s)
    {
        IsBusy = true;
        SearchResults.Clear();

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        
        if (!string.IsNullOrWhiteSpace(s))
        {
            var albums = await Album.SearchAsync(s);

            foreach (var album in albums)
            {
                var vm = new AlbumViewModel(album);
                SearchResults.Add(vm);
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                LoadCovers(cancellationToken);
            }
        }

        IsBusy = false;
    }
    
    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }
    
    private AlbumViewModel? _selectedAlbum;

    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

    public AlbumViewModel? SelectedAlbum
    {
        get => _selectedAlbum;
        set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
    }
    
    private async void LoadCovers(CancellationToken cancellationToken)
    {
        // Important note: this method iterates through a copy of the search results collection
        // (created by the ToList method). This is because it runs asynchronously on its own thread,
        // the original results collection could get changed at any time by another thread.
        
        foreach (var album in SearchResults.ToList())
        {
            await album.LoadCover();

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}