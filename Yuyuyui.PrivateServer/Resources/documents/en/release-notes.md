# Release Notes

## 3.0.0

March 19, 2023

**What's New**

* Added Battle system.
    * The Battle system includes all main story battle stages (*The Chapter of Hanayui*, *The Chapter of Yuuki Yuuna*, *The Chapter of Washio Sumi*, *The Chapter of Nogi Wakaba*, *The Chapter of Shiratori Utano*, *The Chapter of Akihara Sekka*, *The Chapter of Kohagura Natsume*, *The Chapter of Ishitsumugi*, *The Chapter of Kirameki*).
    * Choosing guest players is not supported for now.
        * However, to work around a bug on the client side, the server will generate a placeholder player for selection before entering the battle. However, this placeholder will not be reflected inside the battle.
    * The Battle system is only for experiencing the main story battle stages.
        * Battle stages will not consume player stamina.
        * Winning a battle will not increase the player's money, experience, etc.
        * The calculation of the experience and character affinity of the card is not the same as that of the official server. The calculation results in much lower values.
        * After winning a battle, the player will not receive any item rewards, including scenario rewards and drop rewards.
* Added a new config option for unlocking all difficulties.
    * When turned on, all battle stages will be shown as perfectly finished with three stars, tricking the client into unlocking higher difficulties.

**Bug Fixes**

* Fixed a problem caused by the character familiarity keep leveling up even when the level is maxed out.

## 2.0.2

February 21, 2023

**What's New**

* Renamed the HELP page to TUTORIAL page.

## 2.0.1

February 17, 2023

**Bug Fixes**

* Fixed errors caused by API access through CDN.

## 2.0.0

February 14, 2023

**What's New**

* Added Release Note page (this page).
* Added multi-language support for in-game scenarios.
    * These translations are provided through joint efforts and contributions from the community.
    * The currently supported languages include:
        * zh - Chinese
        * en - English
    * Each language has a different translation progress. The game will automatically fall back to the default language (jp - Japanese) when no translation is found.
* Updated the help page about how to create a new account.
* When new version is found, the Private Server will now refuse to start.
    * The log message when new version is found is now highlighted.
* Introduced CDN for speeding up access to the API server.

## 1.2.3

January 15, 2023

**What's New**

* Used CDN for faster content delivery.

**Bug Fixes**

* Purged the remaining code of the Account Transfer tool *Eucalyptus*.

## 1.2.1

January 13, 2023

**Bug Fixes**

* Fixed a bug where the *Infinite Items Mode* switch default value doesn't respect the actual configuration.

## 1.2.0

January 13, 2023

**What's New**

* Added *Infinite Items Mode*.

## 1.1.1

October 29, 2022

**Bug Fixes**

* Fixed a problem caused by the responded server time being after the shutdown of the official server, resulting the game client not being able to enter the game.

## 1.1.0

October 29, 2022

**What's New**

* Removed the Account Transfer tool *Eucalyptus* due to the shutdown of the official server.

## 1.0.0

October 21, 2022