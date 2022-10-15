using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Yuyuyui.PrivateServer.GUI.Views;

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

    public static Dictionary<Type, string> ButtonInfo => new()
    {
        { typeof(IntroductionView),              Localization.Resources.PS_HELP_INTRO },
        { typeof(PrivateServerView),             Localization.Resources.PS_HELP_PS },
        { typeof(AccountTransferView),           Localization.Resources.PS_HELP_AT },
        { typeof(ConnectToProxyView),            Localization.Resources.PS_HELP_PROXY },
        { typeof(InstallCertificateIosView),     Localization.Resources.PS_HELP_CERT_IOS },
        { typeof(InstallCertificateAndroidView), Localization.Resources.PS_HELP_CERT_ANDROID },
    };

    public HelpViewModel()
    {
        pageStack = new Stack<HelpSubViewBase>();

        IsRootPage = true;
        HelpTopics = new ObservableCollection<HelpButtonDataTemplate>(ButtonInfo
            .Select(i => new HelpButtonDataTemplate()
            {
                helpVM = new(this),
                page = i.Key,
                ButtonText = i.Value
            }));
    }

    public void PushPage(Type page)
    {
        if (!page.IsSubclassOf(typeof(HelpSubViewBase))) return;

        var newPage = pageStack.FirstOrDefault(p => p.GetType() == page);

        if (newPage == null)
        {
            newPage = (HelpSubViewBase?)Activator.CreateInstance(page)!;
            newPage.HelpViewModel = this;

            var viewModelType = Type.GetType(page.FullName?.Replace("View", "ViewModel") ?? "") ??
                                typeof(HelpSubViewModelBase);

            var vm = (HelpSubViewModelBase?)Activator.CreateInstance(viewModelType);
            if (vm != null)
            {
                vm.HelpViewModel = this;
                newPage.DataContext = vm;
            }
        }

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

    public string PS_HELP_BACK_BUTTON => Localization.Resources.PS_HELP_BACK_BUTTON;
}