using ResoniteModLoader;
using HarmonyLib;

namespace ReArmature
{
    public class ReArmature : ResoniteMod
    {
		internal const string VERSION_CONSTANT = "2.1.6";
		public override string Name => "ReArmature";

        public override string Author => "CatShark";

        public override string Version => VERSION_CONSTANT;

        public override string Link => "https://github.com/CatSharkShin/ReArmature/";
        
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.catshark.rearmature");
            harmony.PatchAll();
        }
    }
}