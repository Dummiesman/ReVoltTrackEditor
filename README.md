# ReVoltTrackEditor
A re-make of the Re-Volt Track Editor in Unity 2020.3.21f1

## Running
This requires a copy of the original Track Editor files to use (wavs, bitmaps, and trackunit.rtu). Open `editor_path.txt` and set this to the path where your editor files exist, from there it will just run.

You can obtain these files [here](https://www.mediafire.com/file/6rimevzh3xljmzn/editor_files.zip/file) (Note that these files come from the original editor and are not covered under the LICENSE)

You can only export levels on desktop platforms if the editor detects it's installed within a Re-Volt installation, like so:

Games\Re-Volt  
\-- editor  
\--- trackunit.rtu  
\--- etc  
\-- levels 

Exporting levels to Android works by automatically detecting an RVGL installation.

## Platform Support
The editor has been tested on Windows, Linux, and Android. It should also work on Mac. There is currently no Android UI and to use it on Android you must use a gamepad.

## Builds
Pre-built binaries are available on the [Releases](https://github.com/Dummiesman/ReVoltTrackEditor/releases) page
