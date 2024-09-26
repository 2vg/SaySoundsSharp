using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace SaySoundsSharp;


// Code from Metapyziks. Edit by faketuna.
// https://gist.github.com/Metapyziks/88745748d90f8955e60b9831a7214cd2
public static class EmitSoundExtension
{
    // Search string with "DeathCry" and the function using this argument is EmitSoundParams.
    // Windows
    // private static MemoryFunctionVoid<CBaseEntity, string, int, float, float> CBaseEntity_EmitSoundParamsFunc = new("48 8B C4 48 89 58 10 48 89 70 18 55 57 41 56 48 8D A8 08 FF FF FF");
    // Linux
    private static MemoryFunctionVoid<CBaseEntity, string, int, float, float> CBaseEntity_EmitSoundParamsFunc = new("48 B8 ? ? ? ? ? ? ? ? 55 48 89 E5 41 55 41 54 49 89 FC 53 48 89 F3");

    // Windows
    // private static MemoryFunctionVoid<RecipientFilter, uint, EmitSound_t> CBaseEntity_EmitSoundFilterFunc = new("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 48 89 7C 24 20 41 56 48 83 EC 30 48 8B EA")
    // Linux
    // private static MemoryFunctionVoid<RecipientFilter, uint, EmitSound_t> CBaseEntity_EmitSoundFilterFunc = new("55 48 89 E5 41 56 49 89 D6 41 55 41 89 F5 41 54 48 8D 35 ? ? ? ?");

    /*
     * Thanks oylsister: https://discord.com/channels/1160907911501991946/1173099041735835719/1285969901970002032
     */
    /*
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SndOpEventGuid_t
    {
        public uint m_nGuid;
        public ulong m_hStackHash;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EmitSound_t
    {
        public EmitSound_t()
        {
            Channel = 0;
            SoundName = "";
            Volume = 1f;
            SoundLevel = soundlevel_t.SNDLVL_NONE;
            Flags = 0;
            Pitch = 100;
            Origin = new();
            SoundTime = 0;
            SoundDuration = 0;
            EmitCloseCaption = true;
            WarnOnMissingCloseCaption = false;
            WarnOnDirectWaveReference = false;
            SpeakerEntity = 0;
            UtlVecSoundOrigin = new();
            ForceGuid = 0;
            SpeakerGender = gender_t.GENDER_NONE;
        }

        public int Channel;
        public string SoundName; // Note: Pointer to a C-style string (const char*)
        public float Volume;
        public soundlevel_t SoundLevel; // Assuming soundlevel_t is an enum or similar
        public int Flags;
        public int Pitch;
        public Vector Origin; // Pointer to a Vector (assuming Vector is another struct)
        public float SoundTime;
        public float SoundDuration; // Pointer to a float (duration)
        public bool EmitCloseCaption;
        public bool WarnOnMissingCloseCaption;
        public bool WarnOnDirectWaveReference;
        public uint SpeakerEntity;
        public CUtlVector UtlVecSoundOrigin;
        public uint ForceGuid;
        public gender_t SpeakerGender;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CUtlMemory
    {
        public unsafe nint* m_pMemory;
        public int m_nAllocationCount;
        public int m_nGrowSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CUtlVector
    {
        public unsafe nint this[int index]
        {
            get => m_Memory.m_pMemory[index];
            set => m_Memory.m_pMemory[index] = value;
        }

        public int m_iSize;
        public CUtlMemory m_Memory;

        public nint Element(int index) => this[index];
    }

    public enum gender_t : ushort
    {
        GENDER_NONE = 0x0,
        GENDER_MALE = 0x1,
        GENDER_FEMALE = 0x2,
        GENDER_NAMVET = 0x3,
        GENDER_TEENGIRL = 0x4,
        GENDER_BIKER = 0x5,
        GENDER_MANAGER = 0x6,
        GENDER_GAMBLER = 0x7,
        GENDER_PRODUCER = 0x8,
        GENDER_COACH = 0x9,
        GENDER_MECHANIC = 0xA,
        GENDER_CEDA = 0xB,
        GENDER_CRAWLER = 0xC,
        GENDER_UNDISTRACTABLE = 0xD,
        GENDER_FALLEN = 0xE,
        GENDER_RIOT_CONTROL = 0xF,
        GENDER_CLOWN = 0x10,
        GENDER_JIMMY = 0x11,
        GENDER_HOSPITAL_PATIENT = 0x12,
        GENDER_BRIDE = 0x13,
        GENDER_LAST = 0x14,
    };
    */

    [ThreadStatic]
    private static IReadOnlyDictionary<string, float>? CurrentParameters;

    /// <summary>
    /// Emit a sound event by name (e.g., "Weapon_AK47.Single").
    /// TODO: parameters passed in here only seem to work for sound events shipped with the game, not workshop ones.
    /// </summary>
    public static void EmitSound(this CBaseEntity entity, string soundName, IReadOnlyDictionary<string, float>? parameters = null)
    {
        if (!entity.IsValid)
        {
            throw new ArgumentException("Entity is not valid.");
        }

        try
        {
            // We call CBaseEntity::EmitSoundParams,
            // which calls a method that returns an ID that we can use
            // to modify the playing sound.

            CurrentParameters = parameters;

            // Pitch, volume etc aren't actually used here
            CBaseEntity_EmitSoundParamsFunc.Invoke(entity, soundName, 100, 1f, 0f);
        }
        finally
        {
            CurrentParameters = null;
        }
    }

    public static void EmitSoundWithPitch(this CBaseEntity entity, string soundName, int pitch = 100)
    {
        if (!entity.IsValid)
        {
            throw new ArgumentException("Entity is not valid.");
        }

        if (pitch != 100)
        {
            soundName += $".p{pitch}";
        };

        try
        {
            Console.WriteLine($"[SaySoundsSharp] played {soundName}");
            CBaseEntity_EmitSoundParamsFunc.Invoke(entity, soundName, 100, 1f, 0f);
        }
        finally
        {
            CurrentParameters = null;
        }
    }

    /*
    public static void EmitSoundFilter(this CCSPlayerController client, string soundName, int nPitch = 100, float flVolume = 1.0f)
    {
        if (!client.IsValid)
        {
            throw new ArgumentException("Entity is not valid.");
        }

        var filter = new RecipientFilter
        {
            client.Slot
        };
        var soundParams = new EmitSound_t
        {
            SoundName = soundName,
            Pitch = nPitch,
            Volume = flVolume
        };

        try
        {
            CBaseEntity_EmitSoundFilterFunc.Invoke(filter, client.Index, soundParams);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to call EmitSoundFilter. error: {ex}");
        }
    }

    public static void EmitSoundToAll(this CCSPlayerController client, string soundName, int nPitch = 100, float flVolume = 1.0f)
    {
        if (!client.IsValid)
        {
            throw new ArgumentException("Entity is not valid.");
        }

        var filter = new RecipientFilter();
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            if (!cl.IsValid || cl.IsBot || cl.IsHLTV)
                continue;
            filter.Add(cl.Slot);
        }
        var soundParams = new EmitSound_t
        {
            SoundName = soundName,
            Pitch = nPitch,
            Volume = flVolume
        };

        try
        {
            CBaseEntity_EmitSoundFilterFunc.Invoke(filter, client.Index, soundParams);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to call EmitSoundFilter. error: {ex}");
        }
    }
    */
}