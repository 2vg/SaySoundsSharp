
#
# Generates sndevts and saysounds.txt files from the directory of mp3 files.
#
# Original Author: Spitice
#
# This script traverses your specified saysound directory and collects all .mp3
# files found. It generates a sndevts file that also contains all available
# pitch variations.
#
# Don't bother with the other modifications such as durations, reverb, 3D, and
# Doppler effect for now. Also note that duration is apparently already
# implemented by the new saysound plugin so they don't need to be included in
# the sndevts file anymore.
#
#
# HOW TO USE:
#
# 1. Fill the five file/directory path entries
#    1. cs2Dir
#    2. addonName
#    3. saysoundDir
#    4. outputSndevtsFile (I used a temporary directory for testing purposes.
#       Please change the directory for your own)
#    5. outputSSListFile
# 2. Run the python script
# 3. Copy/move the output sndevts/sslist file to their appropriate location
#
#
# !! CAUTION !! This script does NOT comply with the saysound name escaping rule
# of the new saysound plugin! So, some entries that contain "!" might not be
# played.
#
# Also, the sslist file is customized with the modified SaySoundsSharp so it is
# also not compatible with the new saysound plugin.
#

#%%
import os
from pathlib import Path

cs2Dir = "C:\\program files (x86)\\steam\\steamapps\\common\\counter-strike global offensive"
addonName = "saysounds"
# "sounds" directory name will be automatically prepended to this directory
saysoundDir = "xx_saysounds"
# Place it under "<addon content>/soundevents" directory
outputSndevtsFile = "C:\\program files (x86)\\steam\\steamapps\\common\\counter-strike global offensive\\content\\csgo_addons\\saysounds\\soundevents\\soundevents_xxsaysounds.vsndevts"
# Copy to "csgo/addons/counterstrikesharp/plugins/SaySoundsSharp/config/saysounds.txt"
outputSSListFile = "C:\\program files (x86)\\steam\\steamapps\\common\\counter-strike global offensive\\content\\csgo_addons\\saysounds\\saysounds.txt"

# if want faster compile, put empty array into these.
availablePitches = range(25, 226, 25) # 25, 50, 75...225
availableDurations = []#[2, 5, 10]
availableAuxPitches = []#[50, 200]

cs2Path = Path(cs2Dir)

def escapeSaysoundName(ssName):
    ssName = ssName.replace("!", "_EXCL_")
    ssName = ssName.replace("-", "_MIN_")
    ssName = ssName.replace("+", "_PLS_")
    ssName = ssName.replace(" ", "_SPC_")
    ssName = ssName.replace("~", "_TILD_")
    ssName = ssName.replace("^", "_CARET_")
    ssName = ssName.replace("(", "_LPAR_")
    ssName = ssName.replace(")", "_RPAR_")
    return ssName

def unescapeSaysoundName(ssName):
    ssName = ssName.replace("_EXCL_", "!")
    ssName = ssName.replace("_MIN_", "-")
    ssName = ssName.replace("_PLS_", "+")
    ssName = ssName.replace("_SPC_", " ")
    ssName = ssName.replace("_TILD_", "~")
    ssName = ssName.replace("_CARET_", "^")
    ssName = ssName.replace("_LPAR_", "(")
    ssName = ssName.replace("_RPAR_", ")")
    return ssName

def getAllSaysounds():
    saysounds = {}
    addonRootPath = cs2Path / "content/csgo_addons" / addonName
    saysoundPath = addonRootPath / "sounds" / saysoundDir
    for (currentDir, dirs, filenames) in os.walk(saysoundPath):
        currentDirRelPath = Path(currentDir).relative_to(addonRootPath)
        for filename in filenames:
            filenamePath = Path(filename)
            if (filenamePath.suffix != ".wav" and filenamePath.suffix != ".mp3"):
                continue

            saysoundName = filenamePath.stem
            vsndPath = (currentDirRelPath / filename).with_suffix(".vsnd")
            #print(str(saysoundName) + ": " + str(vsndPath))

            saysoundName = saysoundName.lower()
            saysoundName = escapeSaysoundName(saysoundName)

            if saysoundName in saysounds:
                print("Identical saysound name entries found")
                print("  - saysound name: " + saysoundName)
                print("  - Picked  vsnd: " + saysounds[saysoundName] )
                print("  - Ignored vsnd: " + vsndPath.as_posix())
                continue

            saysounds[saysoundName] = str(vsndPath.as_posix())
    return saysounds

saysounds = getAllSaysounds()

# %%
def pitchSuffix(pitch):
    return ".p" + str(pitch)

def durationSuffix(duration):
    return ".d{0:02}".format(duration)

def generateSoundevents(saysounds):
    out = []
    def write(text):
        out.append(text)

    write("<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->")
    write("{")
    write("  ss_base = {")
    #write('    type = "csgo_mega"')
    write('    type = "csgo_music"')
    write('    volume = 1.000000')
    write('    volume_convar = "snd_menumusic_volume"')
    write("  }")

    for pitch in availablePitches:
        if pitch == 100:
            continue
        write("  ss_base" + pitchSuffix(pitch) + " = {")
        write('    base = "ss_base"')
        write("    pitch = " + str(pitch / 100))
        write("  }")

    for duration in availableDurations:
        durSuffix = durationSuffix(duration)
        write("  ss_base" + durSuffix + " = {")
        write('    base = "ss_base"')
        write("    use_time_volume_mapping_curve = true")
        write("    time_volume_mapping_curve =")
        write("    [")
        write("      [0.0, 1.0, 0, 0, 2.0, 3.0],")
        write("      [{}, 1.0, 0, 0, 2.0, 3.0],".format(duration * 0.1 - 0.0001))
        write("      [{}, 0.0, 0, 0, 2.0, 3.0],".format(duration * 0.1))
        write("    ]")
        write("  }")

        for pitch in availableAuxPitches:
            write("  ss_base" + durSuffix + pitchSuffix(pitch) + " = {" )
            write('    base = "ss_base' + durSuffix + '"')
            write("    pitch = " + str(pitch / 100))
            write("  }")

    #write("  ss_base.reverb = {")
    #write('    base = "ss_base"')
    #write('    mixgroup = "World"')
    #write("    reverb_wet = 1.0")
    #write("    reverb_source_wet = 1.0")
    #write("    override_dsp_preset = true")
    #write('    dsp_preset = "reverb_24_largeBathroom"')
    #write("  }")

    #write("  ss_base.3d = {")
    #write('    base = "ss_base"')
    #write('    mixgroup = "World"')
    #write("    distance_volume_mapping_curve =")
    #write("    [")
    #write("      [0.0, 1.0, 0.0, 0.0, 2.0, 3.0],")
    #write("      [200.0, 1.0, 0.0, 0.0, 2.0, 3.0],")
    #write("      [1500.0, 0.01, 0.0, 0.0, 2.0, 3.0],")
    #write("      [5000.0, 0, 0.0, 0.0, 2.0, 3.0],")
    #write("    ]")
    #write("  }")

    #write("  ss_base.3dd = {")
    #write('    base = "saydound_base.3d"')
    #write("    use_doppler = true")
    #write("    doppler_factor = 100")
    #write("    doppler_factor_receding = 100")
    #write("  }")

    for pitch in availableAuxPitches:
        write("  ss_base.reverb" + pitchSuffix(pitch) + " = {" )
        write('    base = "ss_base.reverb"')
        write("    pitch = " + str(pitch / 100))
        write("  }")
        write("  ss_base.3d" + pitchSuffix(pitch) + " = {" )
        write('    base = "ss_base.3d"')
        write("    pitch = " + str(pitch / 100))
        write("  }")
        write("  ss_base.3dd" + pitchSuffix(pitch) + " = {" )
        write('    base = "ss_base.3dd"')
        write("    pitch = " + str(pitch / 100))
        write("  }")

    def writeSaysoundVariation(saysoundName, suffix, vsndPath):
        write("  saysounds.{}{} = {{".format(saysoundName, suffix))
        write('    base = "ss_base{}"'.format(suffix))
        #write('    vsnd_files_track_01 = "{}"'.format(vsndPath))
        write('    vsnd_files = "{}"'.format(vsndPath))
        write("  }")

    for saysoundName, vsndPath in saysounds.items():
        writeSaysoundVariation(saysoundName, "", vsndPath)

        for pitch in availablePitches:
            if pitch == 100:
                continue
            writeSaysoundVariation(saysoundName, pitchSuffix(pitch), vsndPath)
        
        for duration in availableDurations:
            writeSaysoundVariation(saysoundName, durationSuffix(duration), vsndPath)
            for pitch in availableAuxPitches:
                writeSaysoundVariation(saysoundName, durationSuffix(duration) + pitchSuffix(pitch), vsndPath)

        # writeSaysoundVariation(saysoundName, ".reverb", vsndPath)
        # writeSaysoundVariation(saysoundName, ".3d", vsndPath)
        # writeSaysoundVariation(saysoundName, ".3dd", vsndPath)
        # for pitch in availableAuxPitches:
        #     writeSaysoundVariation(saysoundName, ".reverb" + pitchSuffix(pitch), vsndPath)
        #     writeSaysoundVariation(saysoundName, ".3d" + pitchSuffix(pitch), vsndPath)
        #     writeSaysoundVariation(saysoundName, ".3dd" + pitchSuffix(pitch), vsndPath)

    write('}')

    return "\n".join(out)

soundeventTxt = generateSoundevents(saysounds)
with open(outputSndevtsFile, "w") as f:
    f.write(soundeventTxt)

with open(outputSSListFile, "w") as f:
    f.write('"SaySounds"\n')
    f.write('{\n')
    for ssName in saysounds.keys():
        f.write("        \"{}\"\n".format(unescapeSaysoundName(ssName)))
        f.write('        {\n')
        f.write("                \"sound_trigger\" \"saysounds.{}\"\n".format(ssName))
        f.write('        }\n')
    f.write('}\n')

# %%
