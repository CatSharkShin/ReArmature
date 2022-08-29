# ReArmature

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that adds functionality to SkinnedMeshRenderers. You can now add new clothes to your avatar even if they have new bones.


## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
2. Place [ReArmature.dll]() into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Neos logs.


## Usage
1. Import your FBX that includes your clothing(mesh and armature)
2. Drag the clothing under on avatar
3. Press "Re-Setup bones" under the "SkinnedMeshRenderer" component on your clothing!

https://user-images.githubusercontent.com/53709818/187292973-fb08753c-98d9-4baa-80da-de4a8a65a5de.mp4

**Important**
If you are in a world that you are **NOT** hosting, and the host does **NOT** have this mod, open an inspector like this to have the button:

https://user-images.githubusercontent.com/53709818/187294169-71fbf4b6-b7ca-40f3-b1d0-0ecc5ded3311.mp4


## *Details*
- The clothing should be placed above the Armature(hierarchy wise)
- New bones are not a problem, they will be duplicated and attached to your avatar
- If you have renamed bones, the above solution will solve that problem, however your already existing bones obviously wont be used.
  - So if you renamed your tail bone that had dynamicbones, there will be a new bonegroup with the new tail bone name, and it wont have Dynamic Bones
- CatShark#2783 for support and feedback
