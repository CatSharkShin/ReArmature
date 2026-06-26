using BepInEx;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using HarmonyLib;

namespace ReArmature
{
    [ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
    [BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class ReArmature : BasePlugin
    {
        internal const string VERSION_CONSTANT = "2.1.6";

        public override void Load()
        {
            HarmonyInstance.PatchAll();
        }
    }
}