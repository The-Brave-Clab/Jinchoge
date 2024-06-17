using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        updateChannelSelected = Config.SupportedUpdateChannel.IndexOf(Config.Get().General.UpdateBranch);
        scenarioLanguageSelected = Config.SupportedInGameScenarioLanguage.IndexOf(Config.Get().InGame.ScenarioLanguage);
        
        canReissueCert = ProxyUtils.CertExists();
        autoCheckUpdate = Config.Get().General.AutoCheckUpdate;
        allowCheckUpdate = !Update.LocalVersion.is_local_build;
        allowDownloadUpdate = false;
        privateServerRunning = false;
        updateStatus = "";
        infiniteItems = Config.Get().InGame.InfiniteItems;
        useOnlineDecryption = Config.Get().Security.UseOnlineDecryption;

        hasNewUpdate = false;
        newVersionInfo = new();
    }

    public SettingsViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        interfaceLanguageSelected = 0;
        updateChannelSelected = 0;
        scenarioLanguageSelected = 0;
        
        canReissueCert = false;
        autoCheckUpdate = true;
        allowCheckUpdate = false;
        allowDownloadUpdate = false;
        privateServerRunning = false;
        updateStatus = "";
        infiniteItems = false;
        useOnlineDecryption = false;

        hasNewUpdate = false;
        newVersionInfo = new();
    }

    
    public string SETTINGS_CATEGORY_GENERAL => Resources.SETTINGS_CATEGORY_GENERAL;
    public string SETTINGS_CATEGORY_IN_GAME => Resources.SETTINGS_CATEGORY_IN_GAME;
    public string SETTINGS_CATEGORY_SECURITY => Resources.SETTINGS_CATEGORY_SECURITY;
    public string SETTINGS_GENERAL_LANGUAGE => Resources.SETTINGS_GENERAL_LANGUAGE;
    public string SETTINGS_GENERAL_AUTO_UPDATE => Resources.SETTINGS_GENERAL_AUTO_UPDATE;
    public string SETTINGS_GENERAL_UPDATE_CHANNEL => Resources.SETTINGS_GENERAL_UPDATE_CHANNEL;
    public string SETTINGS_GENERAL_CHECK_UPDATE => Resources.SETTINGS_GENERAL_CHECK_UPDATE;
    public string SETTINGS_GENERAL_CHECK_UPDATE_BUTTON => Resources.SETTINGS_GENERAL_CHECK_UPDATE_BUTTON;
    public string SETTINGS_GENERAL_UPDATE_NOW_BUTTON => Resources.SETTINGS_GENERAL_UPDATE_NOW_BUTTON;
    public string SETTINGS_IN_GAME_LANGUAGE => Resources.SETTINGS_IN_GAME_LANGUAGE;
    public string SETTINGS_IN_GAME_INFINITE_ITEMS => Resources.SETTINGS_IN_GAME_INFINITE_ITEMS;
    public string SETTINGS_IN_GAME_UNLOCK_ALL_DIFFICULTIES => Resources.SETTINGS_IN_GAME_UNLOCK_ALL_DIFFICULTIES;
    public string SETTINGS_SECURITY_REISSUE_CERT => Resources.SETTINGS_SECURITY_REISSUE_CERT;
    public string SETTINGS_SECURITY_REISSUE_BUTTON => Resources.SETTINGS_SECURITY_REISSUE_BUTTON;
    public string SETTINGS_SECURITY_ONLINE_DECRYPTION => Resources.SETTINGS_SECURITY_ONLINE_DECRYPTION;
    public string SETTINGS_INFO_REQUIRE_RESTART => Resources.SETTINGS_INFO_REQUIRE_RESTART;
    public string SETTINGS_INFO_TRANSLATION_PROVIDER => Resources.SETTINGS_INFO_TRANSLATION_PROVIDER;
    public string SETTINGS_INFO_REISSUE_CERT => Resources.SETTINGS_INFO_REISSUE_CERT;
    public string SETTINGS_INFO_UNLOCK_ALL_DIFFICULTIES => Resources.SETTINGS_INFO_UNLOCK_ALL_DIFFICULTIES;

    public List<LanguageDisplay> InterfaceLanguages => Config.SupportedInterfaceLocale
        .Select(CultureInfo.GetCultureInfo)
        .Select(c => new LanguageDisplay(c))
        .ToList();
    public List<string> AvailableBranches => Config.SupportedUpdateChannel;
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

    private int updateChannelSelected;
    public int UpdateChannelSelected
    {
        get => updateChannelSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref updateChannelSelected, value);
            Config.Get().General.UpdateBranch = AvailableBranches[updateChannelSelected];
            Config.Save();
        }
    }

    private bool autoCheckUpdate;
    public bool AutoCheckUpdate
    {
        get => autoCheckUpdate;
        set
        {
            this.RaiseAndSetIfChanged(ref autoCheckUpdate, value);
            Config.Get().General.AutoCheckUpdate = value;
            Config.Save();
        }
    }

    private bool infiniteItems;
    public bool InfiniteItems
    {
        get => infiniteItems;
        set
        {
            this.RaiseAndSetIfChanged(ref infiniteItems, value);
            Config.Get().InGame.InfiniteItems = value;
            Config.Save();
        }
    }

    private bool unlockAllDifficulties;
    public bool UnlockAllDifficulties
    {
        get => unlockAllDifficulties;
        set
        {
            this.RaiseAndSetIfChanged(ref unlockAllDifficulties, value);
            Config.Get().InGame.UnlockAllDifficulties = value;
            Config.Save();
        }
    }

    public bool useOnlineDecryption;
    public bool UseOnlineDecryption
    {
        get => false;
        set
        {
            this.RaiseAndSetIfChanged(ref useOnlineDecryption, false);
            Config.Get().Security.UseOnlineDecryption = false;
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

    private bool privateServerRunning;
    public bool PrivateServerRunning
    {
        get => privateServerRunning;
        set => this.RaiseAndSetIfChanged(ref privateServerRunning, value);
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
        PrivateServerRunning = mainWindowViewModel!.Status == MainWindowViewModel.ServerStatus.Started;
        CanReissueCert = ProxyUtils.CertExists() &&
                         mainWindowViewModel!.Status is
                             MainWindowViewModel.ServerStatus.Stopped or
                             MainWindowViewModel.ServerStatus.Updating;
    }

    public void CheckUpdate()
    {
        if (Update.LocalVersion.is_local_build) return;
        
        Utils.Log(string.Format(Resources.LOG_UPDATE_CHECKING, Config.Get().General.UpdateBranch));
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
                    Utils.LogWarning(
                        string.Format(Resources.LOG_UPDATE_FOUND, newVersionInfo.commit_sha[..7],
                            newVersionInfo.branch, Resources.NAV_BUTTON_SETTINGS));
                    Utils.LogWarning(Resources.LOG_UPDATE_RESTRICTION);
                    UpdateStatus = Resources.SETTINGS_GENERAL_CHECK_UPDATE_TEXT_FOUND;
                }
                else
                {
                    Utils.Log(Resources.LOG_UPDATE_NOT_FOUND);
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

        string defaultFile = $"{Update.LocalVersion.runtime_id}";
        string fileName = $"{defaultFile}.zip";
        if (newVersionInfo.artifacts != null && newVersionInfo.artifacts.ContainsKey(defaultFile))
            fileName = newVersionInfo.artifacts[defaultFile];
            
        string url = $"{Update.BASE_URL}/{newVersionInfo.branch}/{newVersionInfo.ci_run}/{fileName}";

        string extension = Path.GetExtension(fileName).Replace(".", "");
        
        SaveFileDialog saveFileBox = new SaveFileDialog
        {
            Title = Resources.SETTINGS_DIALOG_SAVE_TITLE,
            InitialFileName = fileName,
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new List<string> { extension },
                    Name = Resources.SETTINGS_DIALOG_SAVE_FILE_TYPE
                }
            },
            DefaultExtension = extension,
        };

        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        mainWindowViewModel!.TryGetWindow(out var mainWindow);

        var localFileName = await saveFileBox.ShowAsync(mainWindow!);
        if (string.IsNullOrEmpty(localFileName))
        {
            AllowCheckUpdate = true;
            AllowDownloadUpdate = hasNewUpdate;
            return;
        }

        ToolbarViewModel toolbarVM = (ToolbarViewModel)mainWindow!.BottomToolBar.DataContext!;

        try
        {

            await using FileStream fs = new FileStream(localFileName, FileMode.Create, FileAccess.Write,
                FileShare.None);

            toolbarVM.ClearProgressBar();
            toolbarVM.IsProgressBarVisible = true;
            toolbarVM.ShowProgressText = true;
            toolbarVM.IsProgressIndeterminate = false;
            toolbarVM.ProgressBarText = fileName;
            await PrivateServer.HttpClient.DownloadAsync(url, fs, new Progress<float>(
                progress => { toolbarVM.ToolbarProgress = progress * 100; }));

            Utils.Log(string.Format(Resources.LOG_UPDATE_DOWNLOADED, localFileName));
        }
        catch (Exception e)
        {
            Utils.LogError(e.Message);
        }
        finally
        {
            toolbarVM.ClearProgressBar();

            AllowCheckUpdate = true;
            AllowDownloadUpdate = hasNewUpdate;
        }
    }
}