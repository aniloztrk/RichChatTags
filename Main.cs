using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RichChatTags
{
    public class Main : RocketPlugin<Config>
    {
        public List<UnturnedPlayer> MutedPlayers = new List<UnturnedPlayer>();
        protected override void Load()
        {
            UnturnedPlayerEvents.OnPlayerChatted += PlayerChatted;
            Console.WriteLine("#=> Mixy.RichChatTags Loaded!", Console.ForegroundColor = ConsoleColor.Cyan);
            Console.ResetColor();
        }
        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerChatted -= PlayerChatted;
            Console.WriteLine("#=> Mixy.RichChatTags: If you need help. => https://discord.gg/yzsVPAmqhy", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();
        }
        [RocketCommand("mute", "", "", AllowedCaller.Both)]
        [RocketCommandPermission("RCT.mute")]
        public void Mute(IRocketPlayer caller, string[] args)
        {
            var target = UnturnedPlayer.FromName(args[0]);
            if (args.Count() > 0 && args[0].Count() > 1)
            {
                if (target != null)
                {
                    if (!MutedPlayers.Contains(target))
                    {
                        MutedPlayers.Add(target);
                        UnturnedChat.Say(target, "You are muted.", Color.red);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Target is already muted.", Color.red);
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, "Target not found.", Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(caller, "Wrong usage.", Color.red);
            }
        }
        [RocketCommand("unmute", "", "", AllowedCaller.Both)]
        [RocketCommandPermission("RCT.mute")]
        public void Unmute(IRocketPlayer caller, string[] args)
        {
            var target = UnturnedPlayer.FromName(args[0]);
            if (args.Count() > 0 && args[0].Count() > 1)
            {
                if (target != null)
                {
                    if (MutedPlayers.Contains(target))
                    {
                        MutedPlayers.Remove(target);
                        UnturnedChat.Say(target, "You are unmuted.", Color.cyan);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Target is already unmuted.", Color.red);
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, "Target not found.", Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(caller, "Wrong usage.", Color.red);
            }
        }
        private void PlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            var RCT = Configuration.Instance.PlayerConfigs.FirstOrDefault((PlayerConfig p) => p.PlayerSteamID == player.CSteamID.ToString());
            if (!message.StartsWith("/"))
            {
                if (RCT != null)
                {
                    var img = player.SteamProfile.AvatarIcon.ToString();
                    cancel = true;
                    if (!MutedPlayers.Contains(player))
                    {
                        if (!player.IsAdmin) message = message.Replace(">", "");
                        string playerMessage = RCT.PlayerChatFromat.Replace('{', '<').Replace('}', '>').Replace("%PLAYER.NAME%", player.CharacterName).Replace("%PLAYER.MESSAGE%", message);
                        if (chatMode == EChatMode.GLOBAL)
                        {
                            ChatManager.serverSendMessage(playerMessage, color, default, default, 0, img, true);
                        }
                        else if (chatMode == EChatMode.LOCAL)
                        { 
                            foreach (UnturnedPlayer unturnedPlayer in Provider.clients.Select(p => UnturnedPlayer.FromSteamPlayer(p)))
                            {
                                if (Vector3.Distance(player.Position, unturnedPlayer.Position) < 35)
                                {
                                    ChatManager.serverSendMessage("[A]" + " " +  playerMessage, color, unturnedPlayer.SteamPlayer(), unturnedPlayer.SteamPlayer(), EChatMode.LOCAL, img, true);
                                }
                            }
                        }
                        else if (chatMode == EChatMode.GROUP)
                        {
                            foreach (UnturnedPlayer unturnedPlayer in Provider.clients.Select(p => UnturnedPlayer.FromSteamPlayer(p)))
                            {
                                if (player.Player.quests.groupID == unturnedPlayer.Player.quests.groupID)
                                {
                                    ChatManager.serverSendMessage("[G]" + " " + playerMessage, color, unturnedPlayer.SteamPlayer(), unturnedPlayer.SteamPlayer(), EChatMode.GROUP, img, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        cancel = true;
                        return;
                    }
                }
                else
                {
                    if (!MutedPlayers.Contains(player))
                    {
                        cancel = false;
                    }
                    else
                    {
                        cancel = true;
                    }
                }
            }
            else
            {
                cancel = true;
                return;
            }
        }
    }
}
