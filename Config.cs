using Rocket.API;
using System.Collections.Generic;

namespace RichChatTags
{
    public class Config : IRocketPluginConfiguration
    {
        public List<PlayerConfig> PlayerConfigs = new List<PlayerConfig>();
        public void LoadDefaults()
        {
            PlayerConfigs = new List<PlayerConfig>()
            {
                new PlayerConfig
                {
                    PlayerSteamID = "76561198161066094",
                    PlayerChatFromat = "{color=yellow}[Developer]{/color} {color=#00b2ee}%PLAYER.NAME%{/color}: %PLAYER.MESSAGE%",
                    Note = "he is mixy",
                },
            };
        }
    }
}
