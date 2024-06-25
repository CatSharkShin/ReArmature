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
using System.Collections;
namespace ReArmature
{
    public class ReArmature : ResoniteMod
    {
        public override string Name => "ReArmature";

        public override string Author => "CatShark";

        public override string Version => "2.1.3";

        public override string Link => "https://github.com/CatSharkShin/ReArmature/";
        
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.catshark.rearmature");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(SkinnedMeshRenderer))]
        class ReplaceOldMesh
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SkinnedMeshRenderer), nameof(SkinnedMeshRenderer.BuildInspectorUI))]
            public static void Postfix(SkinnedMeshRenderer __instance, UIBuilder ui)
            {
                ui.Style.MinHeight = 24 + 36;

                ui.NestInto(ui.Empty("Re_Armature_Helper"));
                {
                    ui.HorizontalHeader(36, out RectTransform header, out RectTransform content);

                    ui.Style.MinHeight = 24;

                    ui.NestInto(header);
                    ui.Text("Re-Armature", alignment: Alignment.MiddleCenter);
                    ui.NestOut();

                    ui.NestInto(content);
                    {
                        Slot slotRefHolder = ui.Empty("Slot Reader");
                        ui.NestInto(slotRefHolder);
                        {
                            ui.HorizontalLayout(4f);
                            {
                                ReferenceField<Slot> slotField = slotRefHolder.AttachComponent<ReferenceField<Slot>>();

                                const int index = 3;
                                SyncMemberEditorBuilder.Build(
                                    slotField.GetSyncMember(index),
                                    "Armature",
                                    slotField.GetSyncMemberFieldInfo(index),
                                    ui);

                                var btn = ui.Button("Re-Setup bones");
                                btn.LocalPressed += (button, data) =>
                                {
                                    __instance.Slot.StartCoroutine(Process(button, data, __instance, slotField.Reference.Target));
                                };
                            }
                            ui.NestOut();
                        }
                        ui.NestOut();
                    }
                    ui.NestOut();
                }
                ui.NestOut();
            }

            private static IEnumerator<Context> Process(IButton button,ButtonEventData data,SkinnedMeshRenderer skmr, Slot RefArmature)
            {
                if (skmr.Mesh.Asset?.Data == null)
                {
                    button.LabelText = "Cannot setup bones without a loaded asset";
                    yield break;
                }
                MethodInfo StripEmptyBones = skmr.GetType().GetMethod("StripEmptyBones", BindingFlags.NonPublic | BindingFlags.Instance);
                StripEmptyBones.Invoke(skmr, new object[] { button, data });
                
                int elapsedUpdates = 0;
                while (button.Enabled == false || skmr.Mesh.Asset?.Data == null)
                {
                    yield return Context.WaitForNextUpdate();
                    elapsedUpdates++;
                    if (elapsedUpdates > 600)
                    {
                        button.LabelText = "Cannot setup bones without a loaded asset";
                        yield break;
                    }
                }

                if (RefArmature == null)
                {
                    Slot armature = skmr.Slot.GetObjectRoot().FindChild(s => s.Name == "Armature");
                    button.LabelText = SetupBones(skmr, armature);
                }
                else
                {
                    Slot armature = RefArmature;
                    button.LabelText = SetupBones(skmr, armature);
                }
               

                yield return Context.WaitForSeconds(5);
                button.LabelText = "Re-Setup bones";
            }
        }

        public static string SetupBones(SkinnedMeshRenderer skmr,Slot root)
        {
            MethodInfo GetBoneCandidates = skmr.GetType().GetMethod("GetBoneCandidates", BindingFlags.NonPublic | BindingFlags.Instance);
            MeshX mesh = skmr.Mesh.Asset?.Data;
            if (root == null)
            {
                return "Cannot setup bones without an Armature (not found)";
            }
            Dictionary<string, Slot> candidateDictionary = Pool.BorrowDictionary<string, Slot>();

            // Attach missing bones
            do
            {
                GetBoneCandidates.Invoke(skmr, new object[] { root, candidateDictionary});
            } while (attachMissingBones(candidateDictionary, skmr));
            GetBoneCandidates.Invoke(skmr, new object[] { root, candidateDictionary });

            // Clear old bones and add new ones from the new armature
            skmr.Bones.Clear();
            for (int i = 0; i < mesh.BoneCount; i++)
            {
                string name = mesh.GetBone(i).Name;
                candidateDictionary.TryGetValue(name, out var value);
                skmr.Bones.Add().Target = value;
            }
            Pool.Return<string, Slot>(ref candidateDictionary);

            return "Bones setup";
        }

        /// <summary>
        /// Attaches Slots it cant find in the new Armature.
        /// Attaches a duplicate.
        /// </summary>
        /// <param name="bones">Armature's bones to attach to</param>
        /// <param name="skmr">This Renderer's missing bones in the armature will be attached</param>
        private static bool attachMissingBones(Dictionary<string,Slot> bones,SkinnedMeshRenderer skmr)
        {
            for (int i = 0; i < skmr.Bones.Count; i++)
            {
                if (!bones.ContainsKey(skmr.Bones[i].Name))
                {
                    attachBoneGroup(skmr.Bones[i], bones);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the root of the bonegroup and attaches a duplicate.
        /// </summary>
        /// <param name="boneToAttach"></param>
        /// <param name="newArmatureDictionary"></param>
        private static void attachBoneGroup(Slot boneToAttach,Dictionary<string, Slot> newArmatureDictionary)
        {
            if(newArmatureDictionary.TryGetValue(boneToAttach.Parent.Name, out var value))
            {
                boneToAttach.Duplicate(value,false);
            }
            else
            {
                attachBoneGroup(boneToAttach.Parent, newArmatureDictionary);
            }
        }

    }
}