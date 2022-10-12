using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ReactiveUI;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

internal class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
        interfaceLanguageSelected = Config.SupportedInterfaceLocale.IndexOf(Config.Get().General.Language);
        scenarioLanguageSelected = Config.SupportedInGameScenarioLanguage.IndexOf(Config.Get().InGame.ScenarioLanguage);
    }
    
    public string SETTINGS_CATEGORY_GENERAL => Localization.Resources.SETTINGS_CATEGORY_GENERAL;
    public string SETTINGS_CATEGORY_IN_GAME => Localization.Resources.SETTINGS_CATEGORY_IN_GAME;
    public string SETTINGS_GENERAL_LANGUAGE => Localization.Resources.SETTINGS_GENERAL_LANGUAGE;
    public string SETTINGS_IN_GAME_LANGUAGE => Localization.Resources.SETTINGS_IN_GAME_LANGUAGE;
    public string SETTINGS_INFO_REQUIRE_RESTART => Localization.Resources.SETTINGS_INFO_REQUIRE_RESTART;
    public string SETTINGS_INFO_TRANSLATION_PROVIDER => Localization.Resources.SETTINGS_INFO_TRANSLATION_PROVIDER;

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
}