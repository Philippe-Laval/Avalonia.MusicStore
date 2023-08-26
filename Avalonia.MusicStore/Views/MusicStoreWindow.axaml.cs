using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.MusicStore.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Avalonia.MusicStore.Views;

public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
{
    public MusicStoreWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(action => action(ViewModel!.BuyMusicCommand.Subscribe(Close)));
    }
}