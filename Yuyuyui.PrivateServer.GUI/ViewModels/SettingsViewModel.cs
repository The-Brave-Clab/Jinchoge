using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Yuyuyui.PrivateServer.Localization;

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
        allowCheckUpdate = !Update.LocalVersion.is_local_build;
        allowDownloadUpdate = false;
        updateStatus = "";

        hasNewUpdate = false;
        newVersionInfo = new();
    }

    public SettingsViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        interfaceLanguageSelected = Config.SupportedInterfaceLocale.IndexOf(Config.Get().General.Language);
        scenarioLanguageSelected = Config.SupportedInGameScenarioLanguage.IndexOf(Config.Get().InGame.ScenarioLanguage);
        
        canReissueCert = ProxyUtils.CertExists();
        allowCheckUpdate = false;
        updateStatus = "";

        hasNewUpdate = false;
        newVersionInfo = new();
    }

    
    public string SETTINGS_CATEGORY_GENERAL => Resources.SETTINGS_CATEGORY_GENERAL;
    public string SETTINGS_CATEGORY_IN_GAME => Resources.SETTINGS_CATEGORY_IN_GAME;
    public string SETTINGS_CATEGORY_SECURITY => Resources.SETTINGS_CATEGORY_SECURITY;
    public string SETTINGS_GENERAL_LANGUAGE => Resources.SETTINGS_GENERAL_LANGUAGE;
    public string SETTINGS_GENERAL_CHECK_UPDATE => Resources.SETTINGS_GENERAL_CHECK_UPDATE;
    public string SETTINGS_GENERAL_CHECK_UPDATE_BUTTON => Resources.SETTINGS_GENERAL_CHECK_UPDATE_BUTTON;
    public string SETTINGS_GENERAL_UPDATE_NOW_BUTTON => Resources.SETTINGS_GENERAL_UPDATE_NOW_BUTTON;
    public string SETTINGS_IN_GAME_LANGUAGE => Resources.SETTINGS_IN_GAME_LANGUAGE;
    public string SETTINGS_SECURITY_REISSUE_CERT => Resources.SETTINGS_SECURITY_REISSUE_CERT;
    public string SETTINGS_SECURITY_REISSUE_BUTTON => Resources.SETTINGS_SECURITY_REISSUE_BUTTON;
    public string SETTINGS_INFO_REQUIRE_RESTART => Resources.SETTINGS_INFO_REQUIRE_RESTART;
    public string SETTINGS_INFO_TRANSLATION_PROVIDER => Resources.SETTINGS_INFO_TRANSLATION_PROVIDER;
    public string SETTINGS_INFO_REISSUE_CERT => Resources.SETTINGS_INFO_REISSUE_CERT;

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
    
    private bool allowDownloadUpdate;
    public bool AllowDownloadUpdate
    {
        get => allowDownloadUpdate;
        set => this.RaiseAndSetIfChanged(ref allowDownloadUpdate, value);
    }
    
    private bool allowCheckUpdate;
    public bool AllowCheckUpdate
    {
        get => allowCheckUpdate;
        set => this.RaiseAndSetIfChanged(ref allowCheckUpdate, value);
    }
    
    private string updateStatus;
    public string UpdateStatus
    {
        get => updateStatus;
        set => this.RaiseAndSetIfChanged(ref updateStatus, value);
    }

    private bool hasNewUpdate;
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
                DisplayName = Resources.LAN_DEFAULT;
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
        if (Update.LocalVersion.is_local_build) return;
        
        Utils.Log($"Checking for application update on branch {Update.LocalVersion.version_info.branch}...");
        UpdateStatus = Resources.SETTINGS_GENERAL_CHECK_UPDATE_TEXT_CHECKING;
        AllowCheckUpdate = false;
        AllowDownloadUpdate = false;
        hasNewUpdate = false;
        Update.Check()
            .ContinueWith(_ =>
            {
                hasNewUpdate = Update.TryGetNewerVersion(out newVersionInfo);
                if (hasNewUpdate)
                {
                    Utils.Log(
                        $"Found new version: commit {newVersionInfo.commit_sha[..7]} on branch {newVersionInfo.branch}");
                    UpdateStatus = Resources.SETTINGS_GENERAL_CHECK_UPDATE_TEXT_FOUND;
                }
                else
                {
                    Utils.Log($"No new version found.");
                    UpdateStatus = Resources.SETTINGS_GENERAL_CHECK_UPDATE_TEXT_NOT_FOUND;
                }

                AllowCheckUpdate = true;
                AllowDownloadUpdate = hasNewUpdate;
            });
    }

    public async void DownloadUpdate()
    {
        if (Update.LocalVersion.is_local_build) return;

        AllowDownloadUpdate = false;
        AllowCheckUpdate = false;

        string fileName = $"{Update.LocalVersion.framework}-{Update.LocalVersion.runtime_id}.zip";
        string url = $"{Update.BASE_URL}/{newVersionInfo.branch}/{newVersionInfo.ci_run}/{fileName}";
        
        SaveFileDialog saveFileBox = new SaveFileDialog
        {
            Title = "Save Update File As...",
            InitialFileName = fileName,
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new List<string> { "zip" },
                    Name = "Update File"
                }
            },
            DefaultExtension = "zip",
        };

        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        mainWindowViewModel!.TryGetWindow(out var mainWindow);

        var localFileName = await saveFileBox.ShowAsync(mainWindow!);
        if (string.IsNullOrEmpty(localFileName)) return;

        ToolbarViewModel toolbarVM = (ToolbarViewModel)mainWindow!.BottomToolBar.DataContext!;

        await using FileStream fs = new FileStream(localFileName, FileMode.Create, FileAccess.Write, FileShare.None);

        toolbarVM.ClearProgressBar();
        toolbarVM.IsProgressBarVisible = true;
        toolbarVM.ShowProgressText = true;
        toolbarVM.IsProgressIndeterminate = false;
        toolbarVM.ProgressBarText = fileName;
        await PrivateServer.HttpClient.DownloadAsync(url, fs, new Progress<float>(
            progress => { toolbarVM.ToolbarProgress = progress * 100; }));

        toolbarVM.ClearProgressBar();

        AllowCheckUpdate = true;
        AllowDownloadUpdate = hasNewUpdate;
        
        Utils.Log($"Downloaded new update file at {localFileName}");
    }
}