using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using ReactiveUI;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

internal class SettingsViewModel : ViewModelBase
{
    private WeakReference<MainWindowViewModel?> mainWindowVM = new(null);
    public SettingsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        mainWindowVM.SetTarget(mainWindowViewModel);

        interfaceLanguageSelected = Config.SupportedInterfaceLocale.IndexOf(Config.Get().General.Language);
        scenarioLanguageSelected = Config.SupportedInGameScenarioLanguage.IndexOf(Config.Get().InGame.ScenarioLanguage);
        
        canReissueCert = ProxyUtils.CertExists();
        hasNewVersion = false;
        isCheckingUpdate = false;

        newVersionInfo = new();
    }

    public SettingsViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        interfaceLanguageSelected = Config.SupportedInterfaceLocale.IndexOf(Config.Get().General.Language);
        scenarioLanguageSelected = Config.SupportedInGameScenarioLanguage.IndexOf(Config.Get().InGame.ScenarioLanguage);
        
        canReissueCert = ProxyUtils.CertExists();
        hasNewVersion = false;
        isCheckingUpdate = false;

        newVersionInfo = new();
    }

    
    public string SETTINGS_CATEGORY_GENERAL => Localization.Resources.SETTINGS_CATEGORY_GENERAL;
    public string SETTINGS_CATEGORY_IN_GAME => Localization.Resources.SETTINGS_CATEGORY_IN_GAME;
    public string SETTINGS_CATEGORY_SECURITY => Localization.Resources.SETTINGS_CATEGORY_SECURITY;
    public string SETTINGS_GENERAL_LANGUAGE => Localization.Resources.SETTINGS_GENERAL_LANGUAGE;
    public string SETTINGS_IN_GAME_LANGUAGE => Localization.Resources.SETTINGS_IN_GAME_LANGUAGE;
    public string SETTINGS_SECURITY_REISSUE_CERT => Localization.Resources.SETTINGS_SECURITY_REISSUE_CERT;
    public string SETTINGS_SECURITY_REISSUE_BUTTON => Localization.Resources.SETTINGS_SECURITY_REISSUE_BUTTON;
    public string SETTINGS_INFO_REQUIRE_RESTART => Localization.Resources.SETTINGS_INFO_REQUIRE_RESTART;
    public string SETTINGS_INFO_TRANSLATION_PROVIDER => Localization.Resources.SETTINGS_INFO_TRANSLATION_PROVIDER;
    public string SETTINGS_INFO_REISSUE_CERT => Localization.Resources.SETTINGS_INFO_REISSUE_CERT;

    public List<LanguageDisplay> InterfaceLanguages => Config.SupportedInterfaceLocale
        .Select(CultureInfo.GetCultureInfo)
        .Select(c => new LanguageDisplay(c))
        .ToList();
    public List<LanguageDisplay> ScenarioLanguages => Config.SupportedInGameScenarioLanguage
        .Select(CultureInfo.GetCultureInfo)
        .Select(c => new LanguageDisplay(c))
        .ToList();


    private int interfaceLanguageSelected;
    public int InterfaceLanguagesSelected
    {
        get => interfaceLanguageSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref interfaceLanguageSelected, value);
            Config.Get().General.Language = Config.SupportedInterfaceLocale[interfaceLanguageSelected];
            Config.Save();
        }
    }


    private int scenarioLanguageSelected;
    public int ScenarioLanguagesSelected
    {
        get => scenarioLanguageSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref scenarioLanguageSelected, value);
            Config.Get().InGame.ScenarioLanguage = Config.SupportedInGameScenarioLanguage[scenarioLanguageSelected];
            Config.Save();
        }
    }

    private bool canReissueCert;
    public bool CanReissueCert
    {
        get => canReissueCert;
        set => this.RaiseAndSetIfChanged(ref canReissueCert, value);
    }
    
    private bool hasNewVersion;
    public bool HasNewVersion
    {
        get => hasNewVersion;
        set => this.RaiseAndSetIfChanged(ref hasNewVersion, value);
    }
    
    private bool isCheckingUpdate;
    public bool IsCheckingUpdate
    {
        get => isCheckingUpdate;
        set => this.RaiseAndSetIfChanged(ref isCheckingUpdate, value);
    }

    private Update.BuildInfo newVersionInfo;
    
    public void ReissueCert()
    {
        ProxyUtils.ReissueCert();
        Refresh();
    }

    public class LanguageDisplay
    {
        public string DisplayName { get; set; }
        public string NativeName { get; set; }

        public LanguageDisplay(CultureInfo culture)
        {
            if (Equals(culture, CultureInfo.InvariantCulture))
            {
                DisplayName = Localization.Resources.LAN_DEFAULT;
                NativeName = "";
            }
            else
            {
                DisplayName = culture.DisplayName;
                NativeName = culture.NativeName;
            }
        }
    }

    public void Refresh()
    {
        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        CanReissueCert = ProxyUtils.CertExists() &&
                         mainWindowViewModel!.Status is
                             MainWindowViewModel.ServerStatus.Stopped or
                             MainWindowViewModel.ServerStatus.Updating;
    }

    public void CheckUpdate()
    {
        if (Update.IsLocalBuild) return;
        
        Utils.Log($"Checking for application update on branch {Update.LocalVersion.branch}...");
        IsCheckingUpdate = true;
        Update.Check()
            .ContinueWith(_ =>
            {
                HasNewVersion = Update.TryGetNewerVersion(out newVersionInfo);
                if (HasNewVersion)
                {
                    Utils.Log(
                        $"Found new version: commit {newVersionInfo.commit_sha[..7]} on branch {newVersionInfo.branch}");
                }

                IsCheckingUpdate = false;
            });
    }
}