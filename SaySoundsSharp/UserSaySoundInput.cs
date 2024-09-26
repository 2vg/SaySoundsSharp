namespace SaySoundsSharp;

public class UserSaySoundInput
{

    public readonly string soundName;
    public readonly float volume;
    public readonly int pitch;

    public UserSaySoundInput(string soundName, float volume = 1.0F, int pitch = 100)
    {
        this.soundName = soundName;
        this.pitch = pitch;

        if (volume > 1.0F)
        {
            this.volume = 1.0F;
        }
        else if (volume < 0.0F)
        {
            this.volume = 0.0F;
        }
        else
        {
            this.volume = volume;
        }
    }
}