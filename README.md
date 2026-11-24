<img width="772" height="82" alt="Medal_vofTKuYY6y" src="https://github.com/user-attachments/assets/b146a2dd-3ea0-4e03-8232-3cc150da02ef" /># ReArmature

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com) that adds functionality to SkinnedMeshRenderers. You can now add new clothes to your avatar even if they have new bones.


## Installation
### ResoniteModLoader
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [ReArmature.dll]() into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
### Bepis
[![Thunderstore Badge](https://modding.resonite.net/assets/available-on-thunderstore.svg)](https://thunderstore.io/c/resonite/p/catshark/rearmature/)

1. Install [BepisLoader](https://github.com/ResoniteModding/BepisLoader) for Resonite.
2. Download the latest release ZIP file (e.g., `CatSharkShin-ReArmature-1.0.0.zip`) from the [Releases](https://github.com/CatSharkShin/ReArmature/releases) page.
3. Extract the ZIP and copy the `plugins` folder to your BepInEx folder in your Resonite installation directory:
   - **Default location:** `C:\Program Files (x86)\Steam\steamapps\common\Resonite\BepInEx\`
4. Start the game. If you want to verify that the mod is working you can check your BepInEx logs.

## Usage
input your **_target_ armature** on your **_source's_ skinned mesh renderer**, and press "Re-Setup bones"
<img width="768" height="80" alt="Medal_oAlwhILRJy" src="https://github.com/user-attachments/assets/2a35b557-e55d-4f79-9753-5edb0d37a52b" />
### Detailed:
here you can see i have input the target avatars armature into the "Armature" input field, on the clothing:
<img width="2019" height="1205" alt="Medal_AEvVgH3fVC" src="https://github.com/user-attachments/assets/a6e3b77d-b4b2-4edb-b5ff-7a83eb3c3986" />
after pressing "Re-Setup bones", the mesh starts following the target avatar:
<img width="2015" height="1207" alt="Medal_LOjW9U8uH8" src="https://github.com/user-attachments/assets/3975a906-bcca-47ec-b615-f7ba291c8d5e" />
to finish, i can drag the clothing onto my target avatar:
<img width="2023" height="1210" alt="Medal_qog483YdCN" src="https://github.com/user-attachments/assets/b428ec54-7912-4fb5-814e-996677d1aeea" />

## *Details*
- You can use it without providing an Armature, if you drag the clothing onto the avatar first (needs to be on the same hierarchical level as the Armature on the target)
- New bones are not a problem, they will be duplicated and attached to your avatar
- If you have renamed bones, the above solution will solve that problem, however your already existing bones obviously wont be used.
  - So if you renamed your tail bone that had dynamicbones, there will be a new bonegroup with the new tail bone name, and it wont have Dynamic Bones
- kiba.dog on Discord for support and feedback
