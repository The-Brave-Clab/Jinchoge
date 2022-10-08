using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;
using Yuyuyui.PrivateServer.GUI.Views.HelpSubViews;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class HelpViewModel : ViewModelBase
{
    public ObservableCollection<HelpButtonDataTemplate> HelpTopics { get; }

    private bool isRootPage = true;

    public bool IsRootPage
    {
        get => isRootPage;
        set => this.RaiseAndSetIfChanged(ref isRootPage, value);
    }

    private HelpSubViewBase? currentPage = null;

    public HelpSubViewBase CurrentPage
    {
        get => currentPage!;
        set => this.RaiseAndSetIfChanged(ref currentPage, value);
    }

    private Stack<HelpSubViewBase> pageStack;

    public HelpViewModel()
    {
        pageStack = new Stack<HelpSubViewBase>();

        IsRootPage = true;
        HelpTopics = new ObservableCollection<HelpButtonDataTemplate>
        {
            new() { page = typeof(GeneralInfoView), ButtonText = "What is this project?", helpVM = new(this) },
            new() { page = typeof(PrivateServerView), ButtonText = "How to Use Private Server", helpVM = new(this) },
        };
    }

    public void PushPage(Type page)
    {
        if (!page.IsSubclassOf(typeof(HelpSubViewBase))) return;

        var newPage = (HelpSubViewBase?)Activator.CreateInstance(page)!;
        newPage.HelpViewModel = this;

        pageStack.Push(newPage);

        CurrentPage = newPage;
        IsRootPage = pageStack.Count == 0;
    }

    public void PopPage()
    {
        if (pageStack.Count == 0) return;

        pageStack.Pop();

        if (pageStack.Count != 0)
            CurrentPage = pageStack.Peek();
        
        IsRootPage = pageStack.Count == 0;
    }

    public class HelpButtonDataTemplate
    {
        public WeakReference<HelpViewModel?> helpVM = new (null);
        public Type page = typeof(object);

        public string ButtonText { get; set; } = "";

        public void HelpNavigationButtonClick()
        {
            if (helpVM.TryGetTarget(out var helpViewModel))
            {
                helpViewModel.PushPage(page);
            }
        }
    }
}