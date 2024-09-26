using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace SaySoundsSharp;

public class SaySoundsSharp : BasePlugin
{

    public override string ModuleName => "SaySoundsSharp";

    public override string ModuleVersion => "0.0.1";

    public override string ModuleDescription => "CounterStrikeSharp implementation of SaySounds";

    public override string ModuleAuthor => "faketuna, uru";

    private SaySoundConfig saySoundConfig = default!;
    private string CHAT_PREFIX = $" {ChatColors.Purple}[SaySounds]{ChatColors.Default}";

    private FakeConVar<string> soundPath = new("saysounds_sound_path", "Sound path of say sound", "soundevents/soundevents_xxsaysounds.vsndevts");

    private const string saySoundMessageFormat = "%player% played %soundname%";

    public override void Load(bool hotReload)
    {
        AddCommandListener("say", CommandListener_Say);
        AddCommandListener("say_team", CommandListener_SayTeam);
        saySoundConfig = new SaySoundConfig(this.ModuleDirectory + "/config/saysounds.txt");
        RegisterListener<Listeners.OnServerPrecacheResources>((ResourceManifest res) =>
        {
            res.AddResource(soundPath.Value);
        });

        AddCommand("css_sss", "Finds a SaySounds that contains the specified argument.", CommandSaySoundSearch);
    }

    private HookResult CommandListener_Say(CCSPlayerController? client, CommandInfo commandInfo)
    {
        if (client == null)
            return HookResult.Continue;

        UserSaySoundInput saySound = SaySoundUtil.processUserInput(commandInfo.ArgString);

        string? sound = saySoundConfig!.saySounds!.GetValueOrDefault(saySound.soundName, null);

        if (sound == null)
            return HookResult.Continue;

        playSaySound(client, saySound, sound);

        printSaySoundNotification(client, saySound.soundName);
        return HookResult.Handled;
    }

    private HookResult CommandListener_SayTeam(CCSPlayerController? client, CommandInfo commandInfo)
    {
        if (client == null)
            return HookResult.Continue;


        UserSaySoundInput saySound = SaySoundUtil.processUserInput(commandInfo.ArgString);

        string? sound = saySoundConfig!.saySounds!.GetValueOrDefault(saySound.soundName, null);

        if (sound == null)
            return HookResult.Continue;

        playSaySound(client, saySound, sound);

        printSaySoundNotification(client, saySound.soundName);
        return HookResult.Handled;
    }

    private void CommandSaySoundSearch(CCSPlayerController? client, CommandInfo commandInfo)
    {
        if (client == null)
            return;

        string arg1 = commandInfo.GetArg(1);


        if (arg1.Equals("", StringComparison.OrdinalIgnoreCase))
        {
            client.PrintToChat($"{CHAT_PREFIX} usage: css_sss <sound name>");
            return;
        }

        if (arg1.Length < 3)
        {
            client.PrintToChat($"{CHAT_PREFIX} You need specify at least 3 characters!");
            return;
        }

        List<string> searchResult = new();

        foreach (var sound in saySoundConfig.saySounds)
        {
            if (sound.Key.Contains(arg1, StringComparison.OrdinalIgnoreCase))
                searchResult.Add(sound.Key);
        }


        if (searchResult.Count() == 0)
        {
            client.PrintToChat($"{CHAT_PREFIX} No sounds found with {arg1}.");
            return;
        }

        client.PrintToChat($"{CHAT_PREFIX} {searchResult.Count()} sounds found. See client console to full list");
        foreach (string sound in searchResult)
        {
            client.PrintToConsole(sound);
        }

    }

    // currently, support only EmitSoundWithPitch
    private void playSaySound(CCSPlayerController client, UserSaySoundInput saySound, string soundName)
    {
        client.EmitSoundWithPitch(soundName, saySound.pitch);
    }

    private void printSaySoundNotification(CCSPlayerController client, string soundName)
    {
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            if (!cl.IsValid || cl.IsBot || cl.IsHLTV)
                continue;

            cl.PrintToChat(" " + saySoundMessageFormat.Replace("%player%", $"{ChatColors.LightPurple}{client.PlayerName}{ChatColors.Default}").Replace("%soundname%", $"{ChatColors.Lime}{soundName}{ChatColors.Default}"));
        }
    }
}