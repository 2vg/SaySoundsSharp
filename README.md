# SaySoundsSharp

CounterStrikeSharp implementation of **SaySounds**.

I have made improvements to the original implementation.

## Usage

clone repository and compile it, put plugin into your cssharp"s plugins directory.

then put `saysounds.txt` into `<plugins directory>/config`.

`saysounds.txt` format example is:

```txt
"SaySounds"
{
    "<sound trigger string>"
    {
        "sound_trigger"	"<saysounds.sound_event_name>"
    }
}
```

**You can change the only the parts enclosed in `<>`, Don't change `"sound_trigger"`.**

for example paths,

* plugin: `game/csgo/addons/counterstrikesharp/plugins/SaySoundsSharp/SaySoundsSharp.dll`
* txt: `game/csgo/addons/counterstrikesharp/plugins/SaySoundsSharp/config/saysounds.txt`

Your SaySounds Workshop addon is a must have for all players(also yourself).

Check out [Source2ZE/MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager) to automate it without subscribing.

## What is **SaySounds** ?

SaySounds is a feature that plays a corresponding sound when a player says a specific word in chat.

For example, when a player says `rtv` in chat, the sound corresponding to the word `rtv` will be played to all players.

Most of the players who like this feature love to spam it, since the sound can be layered with any number of multiple sounds.

## How to create sounds

Sounds used in SaySounds must be workshop addons.

The add-on must include a `vsnd` file with each sound file compiled into it, and a `vsndevts` file with their paths and settings.

First, create a new workshop add-on and place your SaySounds sound files in the sounds directory.
You can then create the `vsndevts` file using scripts/generate_sndevts.py.

The script has settings for each path, so edit it according to your environment.

`counter-strike global offensive/content/csgo_addons` is the path of the workshop add-on you created, but if `vsnd` and `vsndevts` have been successfully compiled, the path of `counter-strike global offensive/game/csgo_addons` There should be `vsnd_c` and `vsndevts_c` files within the add-on in your path.

If for some reason `vsndevts` does not compile, you will need to manually investigate the error using the resource compiler.
