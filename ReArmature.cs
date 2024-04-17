using ResoniteModLoader;
using HarmonyLib;
using FrooxEngine;
using FrooxEngine.UIX;
using System;
using System.Collections.Generic;
using SkinnedMeshRenderer = FrooxEngine.SkinnedMeshRenderer;
using System.Reflection;
using Elements.Assets;
using Elements.Core;
namespace ReArmature
{
    public class ReArmature : ResoniteMod
    {
        public override string Name => "ReArmature";

        public override string Author => "CatShark";

        public override string Version => "2.1.1";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.catshark.rearmature");
            harmony.PatchAll();
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
                    Slot armature = findSlotByName(__instance.Slot.GetObjectRoot(), "Armature");
                    SetupBones(__instance, armature);
                };
            }
        }

        public static Slot findSlotByName(Slot slot, string name)
        {
            Slot armature = slot.FindChild(s => s.Name == name);
            return armature;
        }

        public static void SetupBones(SkinnedMeshRenderer skmr,Slot root)
        {
            MethodInfo GetBoneCandidates = skmr.GetType().GetMethod("GetBoneCandidates", BindingFlags.NonPublic | BindingFlags.Instance);

            MeshX val = skmr.Mesh.Asset?.Data;
            if (val == null)
            {
                throw new Exception("Cannot setup bones without a loaded asset");
            }
            Dictionary<string, Slot> candidateDictionary = Pool.BorrowDictionary<string, Slot>();
            val.StripEmptyBones();
            do
            {
                GetBoneCandidates.Invoke(skmr, new object[] { root, candidateDictionary});
            } while (!attachMissingBones(candidateDictionary, skmr));
            GetBoneCandidates.Invoke(skmr, new object[] { root, candidateDictionary });

            skmr.Bones.Clear();
            for (int i = 0; i < val.BoneCount; i++)
            {
                string name = val.GetBone(i).Name;
                candidateDictionary.TryGetValue(name, out var value);
                skmr.Bones.Add().Target = value;
            }
            Pool.Return<string, Slot>(ref candidateDictionary);
        }

        /// <summary>
        /// Attaches BoneGroups it can't find in the bones. returns if there was a bonegroup to attach.
        /// Attaches a duplicate.
        /// </summary>
        /// <param name="bones">Armature's bones to attach to</param>
        /// <param name="skmr">These Renderers missing bones in the armature will be attached</param>
        private static bool attachMissingBones(Dictionary<string,Slot> bones,SkinnedMeshRenderer skmr)
        {
            for (int i = 0; i < skmr.Bones.Count; i++)
            {
                string name = skmr.Bones[i].Name;
                if (!bones.ContainsKey(name))
                {
                    attachBoneGroup(skmr.Bones[i], bones);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the root of the bonegroup and attaches a duplicate.
        /// </summary>
        /// <param name="boneRootToAttach"></param>
        /// <param name="newArmatureDictionary"></param>
        private static void attachBoneGroup(Slot boneRootToAttach,Dictionary<string, Slot> newArmatureDictionary)
        {
            if(newArmatureDictionary.TryGetValue(boneRootToAttach.Parent.Name, out var value))
            {
                boneRootToAttach.Duplicate(value,false);
            }
            else
            {
                attachBoneGroup(boneRootToAttach.Parent, newArmatureDictionary);
            }
            
        }

    }
}