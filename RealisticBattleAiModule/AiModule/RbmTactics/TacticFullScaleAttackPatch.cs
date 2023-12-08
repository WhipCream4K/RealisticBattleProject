using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.Core.ItemObject;

namespace RBMAI.AiModule.RbmTactics
{
    [HarmonyPatch(typeof(TacticFullScaleAttack))]
    internal class TacticFullScaleAttackPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Advance")]
        private static void PostfixAdvance(TacticFullScaleAttack __instance)
        {
            var tacticFullScaleAttack = Traverse.Create(__instance);
            Formation archers = tacticFullScaleAttack.Field("_archers").GetValue<Formation>();
            Formation mainInfantry = tacticFullScaleAttack.Field("_mainInfantry").GetValue<Formation>();
            Formation rightCavalry = tacticFullScaleAttack.Field("_rightCavalry").GetValue<Formation>();
            Formation leftCavalry = tacticFullScaleAttack.Field("_leftCavalry").GetValue<Formation>();
            Formation rangedCavalry = tacticFullScaleAttack.Field("_rangedCavalry").GetValue<Formation>();
            
            if (mainInfantry != null)
            {
                mainInfantry.AI.SetBehaviorWeight<BehaviorRegroup>(1.75f);
            }
            if (archers != null)
            {
                archers.AI.SetBehaviorWeight<BehaviorSkirmish>(0f);
                archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(0f);
                archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
                archers.AI.SetBehaviorWeight<BehaviorRegroup>(1.25f);
            }
            if (rightCavalry != null)
            {
                rightCavalry.AI.ResetBehaviorWeights();
                rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
                rightCavalry.AI.SetBehaviorWeight<RBMBehaviorForwardSkirmish>(1f).FlankSide = FormationAI.BehaviorSide.Right;
            }
            if (leftCavalry != null)
            {
                leftCavalry.AI.ResetBehaviorWeights();
                leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Left;
                leftCavalry.AI.SetBehaviorWeight<RBMBehaviorForwardSkirmish>(1f).FlankSide = FormationAI.BehaviorSide.Left;
            }
            if (rangedCavalry != null)
            {
                rangedCavalry.AI.ResetBehaviorWeights();
                TacticComponent.SetDefaultBehaviorWeights(rangedCavalry);
                rangedCavalry.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
                rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Attack")]
        private static void PostfixAttack(TacticFullScaleAttack __instance)
        {
            var tacticFullScaleAttack = Traverse.Create(__instance);
            
            Formation archers = tacticFullScaleAttack.Field("_archers").GetValue<Formation>();
            Formation mainInfantry = tacticFullScaleAttack.Field("_mainInfantry").GetValue<Formation>();
            Formation rightCavalry = tacticFullScaleAttack.Field("_rightCavalry").GetValue<Formation>();
            Formation leftCavalry = tacticFullScaleAttack.Field("_leftCavalry").GetValue<Formation>();
            Formation rangedCavalry = tacticFullScaleAttack.Field("_rangedCavalry").GetValue<Formation>();
            
            if (archers != null)
            {
                archers.AI.ResetBehaviorWeights();
                archers.AI.AddAiBehavior(new RBMBehaviorArcherSkirmish(archers));
                archers.AI.SetBehaviorWeight<RBMBehaviorArcherSkirmish>(1f);
                archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(0f);
                archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(0f);
            }
            if (rightCavalry != null)
            {
                rightCavalry.AI.ResetBehaviorWeights();
                rightCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
                rightCavalry.AI.SetBehaviorWeight<BehaviorCharge>(1f);
                rightCavalry.AI.SetBehaviorWeight<RBMBehaviorCavalryCharge>(1f);
            }
            if (leftCavalry != null)
            {
                leftCavalry.AI.ResetBehaviorWeights();
                leftCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
                leftCavalry.AI.SetBehaviorWeight<BehaviorCharge>(1f);
                leftCavalry.AI.SetBehaviorWeight<RBMBehaviorCavalryCharge>(1f);
            }
            if (rangedCavalry != null)
            {
                rangedCavalry.AI.ResetBehaviorWeights();
                TacticComponent.SetDefaultBehaviorWeights(rangedCavalry);
                rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
            }
            RBMAI.Utilities.FixCharge(ref mainInfantry);
        }

        [HarmonyPostfix]
        [HarmonyPatch("HasBattleBeenJoined")]
        private static void PostfixHasBattleBeenJoined(TacticFullScaleAttack __instance, ref bool __result)
        {
            var tacticFullScaleAttack = Traverse.Create(__instance);
            Formation mainInfantry = tacticFullScaleAttack.Field("_mainInfantry").GetValue<Formation>();
            bool hasBattleBeenJoined = tacticFullScaleAttack.Field("_hasBattleBeenJoined").GetValue<bool>();
            __result = RBMAI.Utilities.HasBattleBeenJoined(mainInfantry, hasBattleBeenJoined);
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetTacticWeight")]
        private static void PostfixGetAiWeight(TacticFullScaleAttack __instance, ref float __result)
        {
            Team currentTeam = __instance.Team;
            if (currentTeam.Side == BattleSideEnum.Defender)
            {
                if (currentTeam.QuerySystem.InfantryRatio > 0.9f)
                {
                    __result = 100f;
                }
            }
            if (float.IsNaN(__result))
            {
                __result = 0.01f;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ManageFormationCounts")]
        private static void PostfixManageFormationCounts(TacticFullScaleAttack __instance)
        {
            var tacticFullScaleAttack = Traverse.Create(__instance);
            Formation leftCavalry = tacticFullScaleAttack.Field("_leftCavalry").GetValue<Formation>();
            Formation rightCavalry = tacticFullScaleAttack.Field("_rightCavalry").GetValue<Formation>();
            
            if (leftCavalry != null && rightCavalry != null && leftCavalry.IsAIControlled && rightCavalry.IsAIControlled)
            {
                List<Agent> mountedSkirmishersList = new List<Agent>();
                List<Agent> mountedMeleeList = new List<Agent>();
                leftCavalry.ApplyActionOnEachUnitViaBackupList(delegate (Agent agent)
                {
                    bool ismountedSkrimisher = false;
                    for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
                    {
                        if (agent.Equipment != null && !agent.Equipment[equipmentIndex].IsEmpty)
                        {
                            if (agent.Equipment[equipmentIndex].Item.Type == ItemTypeEnum.Thrown && agent.MountAgent != null)
                            {
                                ismountedSkrimisher = true;
                                break;
                            }
                        }
                    }
                    if (ismountedSkrimisher)
                    {
                        mountedSkirmishersList.Add(agent);
                    }
                    else
                    {
                        mountedMeleeList.Add(agent);
                    }
                });

                rightCavalry.ApplyActionOnEachUnitViaBackupList(delegate (Agent agent)
                {
                    bool ismountedSkrimisher = false;
                    for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
                    {
                        if (agent.Equipment != null && !agent.Equipment[equipmentIndex].IsEmpty)
                        {
                            if (agent.Equipment[equipmentIndex].Item.Type == ItemTypeEnum.Thrown && agent.MountAgent != null)
                            {
                                ismountedSkrimisher = true;
                                break;
                            }
                        }
                    }
                    if (ismountedSkrimisher)
                    {
                        mountedSkirmishersList.Add(agent);
                    }
                    else
                    {
                        mountedMeleeList.Add(agent);
                    }
                });
                int j = 0;
                int cavalryCount = leftCavalry.CountOfUnits + rightCavalry.CountOfUnits;
                foreach (Agent agent in mountedSkirmishersList.ToList())
                {
                    if (j < cavalryCount / 2)
                    {
                        agent.Formation = leftCavalry;
                    }
                    else
                    {
                        agent.Formation = rightCavalry;
                    }
                    j++;
                }
                foreach (Agent agent in mountedMeleeList.ToList())
                {
                    if (j < cavalryCount / 2)
                    {
                        agent.Formation = leftCavalry;
                    }
                    else
                    {
                        agent.Formation = rightCavalry;
                    }
                    j++;
                }
            }
        }
    }
}