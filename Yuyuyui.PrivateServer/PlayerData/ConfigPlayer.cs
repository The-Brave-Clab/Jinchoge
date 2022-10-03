using System.Collections.Generic;
using System.Linq;

namespace Yuyuyui.PrivateServer
{
    public static class ConfigPlayer
    {
        public static readonly Dictionary<string, Dictionary<string, PlayerProfile>> All = new();

        public const string Language = "LAN";
        public const string Chinese = "ZH";
        public const string English = "EN";

        public static void Initialize()
        {
            // Create dummy data
            if (!Card.Exists(1)) // a dummy card
            {
                Card dummyCard = Card.DefaultYuuna();
                dummyCard.id = 1;
                dummyCard.Save();
            }

            if (!Unit.Exists(1)) // a dummy unit
            {
                Unit dummyUnit = Card.Load(1).CreateUnit();
                dummyUnit.id = 1;
                dummyUnit.Save();
            }
            
            RegisterConfigPlayer("101", Language, Chinese, "Language: Chinese");
            RegisterConfigPlayer("102", Language, English, "Language: English");
        }

        private static PlayerProfile RegisterConfigPlayer(string code, string type, string name, string description)
        {
            string playerCode = new string('0', 10 - code.Length) + code;
            string playerUUID = new string('0', 64 - code.Length) + code;

            PlayerProfile configPlayer;
                
            if (!PlayerProfile.Exists(playerCode))
            {
                configPlayer = new()
                {
                    id = new()
                    {
                        code = playerCode,
                        uuid = playerUUID
                    },
                    profile = new()
                    {
                        nickname = name,
                        comment = description
                    }
                };
                
                configPlayer.Save();
            }
            else
            {
                configPlayer = PlayerProfile.Load(playerCode);
            }

            if (!All.ContainsKey(type))
            {
                All.Add(type, new Dictionary<string, PlayerProfile>());
            }
            
            All[type].Add(name, configPlayer);

            return configPlayer;
        }

        public static bool IsConfigPlayer(string code)
        {
            return All.Any(type => type.Value.Any(config => config.Value.id.code == code));
        }

        public static bool IsConfigPlayer(this PlayerProfile player)
        {
            return IsConfigPlayer(player.id.code);
        }
    }
}