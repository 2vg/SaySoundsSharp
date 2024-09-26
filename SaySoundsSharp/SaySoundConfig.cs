namespace SaySoundsSharp;

public class SaySoundConfig
{

    // trigger name, sound event name
    // e.g. hey!, sayounds.hey!
    public readonly Dictionary<string, string> saySounds = new();

    public SaySoundConfig(string configPath)
    {

        if (!parseConfig(configPath))
        {
            throw new InvalidOperationException("Failed to parse config!");
        }
    }

    private bool parseConfig(string configPath)
    {
        var parser = new ConfigParser(configPath);
        var parsed = parser.GetConfigData();


        foreach (var mainSection in parsed)
        {
            foreach (var dic in mainSection.Value)
            {
                string? soundName = "";
                bool hasSoundName = dic.Value.TryGetValue("sound_trigger", out soundName);
                if (!hasSoundName || soundName == null)
                    continue;

                saySounds[dic.Key] = soundName;

            }
        }

        return true;
    }
}