namespace SaySoundsSharp;

public static class SaySoundUtil
{
    public static UserSaySoundInput processUserInput(string argString)
    {
        string soundName = argString.Replace("\"", "");
        float volume = 1.0F;
        int pitch = 100;

        // because lowest sound name with pitch is a@25,
        // if string does not have 4 length, skip it
        if (!argString.StartsWith('@') && argString.Length > 3)
        {
            string[] args = soundName.Split('@', 2);

            if (args.Length > 1 && int.TryParse(args[1], out pitch))
            {
                int[] validPitches = { 25, 50, 75, 125, 150, 175, 200, 225 };
                if (pitch != 100 && !validPitches.Contains(pitch))
                {
                    pitch = 100;
                }
            }

            soundName = args.First();
        }

        return new UserSaySoundInput(soundName, volume, pitch);
    }
}