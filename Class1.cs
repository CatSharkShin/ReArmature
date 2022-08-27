using NeosModLoader;
using HarmonyLib;
using FrooxEngine;
using FrooxEngine.UIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkinnedMeshRenderer = FrooxEngine.SkinnedMeshRenderer;
using System.Collections.Generic;
using BaseX;
using FrooxEngine.CommonAvatar;
using FrooxEngine.UIX;

namespace ReArmature
{
    public class ReArmature : NeosMod
    {
        public override string Name => "ReArmature";

        public override string Author => "CatShark";

        public override string Version => "1.0";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.catshark.rearmature");
            harmony.PatchAll();
        }
        public static Slot findSlotByName(Slot slot, string name)
        {
            Slot root = slot.GetObjectRoot();
            Slot armature = root.FindChild(s => s.Name == name);
            return armature;
        }

        public static void SetupBones(SkinnedMeshRenderer skmr,Slot root)
        {
            MeshX val = skmr.Mesh.Asset?.Data;
            if (val == null)
            {
                throw new Exception("Cannot setup bones without a loaded asset");
            }

            Dictionary<string, Slot> candidateDictionary = Pool.BorrowDictionary<string, Slot>();
            GetBoneCandidates(root, candidateDictionary);

            List<Slot> oldBones = new List<Slot>(skmr.Bones);
            skmr.Bones.Clear();

            for (int i = 0; i < val.BoneCount; i++)
            {
                string name = val.GetBone(i).Name;
                bool found = candidateDictionary.TryGetValue(name, out var value);
                if (!found)
                {
                    Slot oldbone = oldBones.First(x => x.Name == name);
                    attachBoneGroup(candidateDictionary, oldbone);
                    value = oldbone;
                }
                skmr.Bones.Add().Target = value;
            }

            Pool.Return<string, Slot>(ref candidateDictionary);
        }
        private static void attachBoneGroup(Dictionary<string, Slot> newArmatureDictionary,Slot boneFromNewGroup)
        {
            if(newArmatureDictionary.TryGetValue(boneFromNewGroup.Parent.Name, out var value))
            {
                boneFromNewGroup.SetParent(value,false);
            }
            else
            {
                attachBoneGroup(newArmatureDictionary, boneFromNewGroup.Parent);
            }
            
        }
        private static void GetBoneCandidates(Slot root, Dictionary<string, Slot> candidates)
        {
            if (root.GetComponent<MeshRenderer>() != null)
            {
                return;
            }

            if (!candidates.ContainsKey(root.Name))
            {
                candidates.Add(root.Name, root);
            }
            else
            {
                UniLog.Warning("Duplicate slot name that's a potential joint: " + root.Name, false);
            }

            IEnumerator<Slot> enumerator = root.Children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Slot current = enumerator.Current;
                    GetBoneCandidates(current, candidates);
                }
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
        }

        [HarmonyPatch(typeof(SkinnedMeshRenderer))]
        class ReplaceOldMesh
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(SkinnedMeshRenderer.BuildInspectorUI))]
            public static void Postfix(UIBuilder ui, SkinnedMeshRenderer __instance)
            {
                var btn = ui.Button("Re-Setup bones");
                btn.LocalPressed += (button, data) => {
                    Slot armature = findSlotByName(__instance.Slot, "Armature");
                    SetupBones(__instance, armature);
                };
            }
        }
    }
}