using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace RBMAI
{
    [HarmonyPatch(typeof(TacticDefensiveEngagement))]
    internal class TacticDefensiveEngagementPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HasBattleBeenJoined")]
        private static bool PrefixHasBattleBeenJoined(TacticDefensiveEngagement __instance,ref bool __result)
        {
            var tacticDefensiveEngagementInst = Traverse.Create(__instance);
            Formation mainInfantry = tacticDefensiveEngagementInst.Field("_mainInfantry").GetValue() as Formation;
            bool hasBattleBeenJoined = tacticDefensiveEngagementInst.Field("_hasBattleBeenJoined").GetValue<bool>();
            
            __result = RBMAI.Utilities.HasBattleBeenJoined(mainInfantry, hasBattleBeenJoined);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Defend")]
        private static void PostfixDefend(TacticDefensiveEngagement __instance)
        {
            var tacticDefensiveEngagementInst = Traverse.Create(__instance);

            Formation archers = tacticDefensiveEngagementInst.Field("_archers").GetValue() as Formation;
            Formation mainInfantry = tacticDefensiveEngagementInst.Field("_mainInfantry").GetValue() as Formation;
            Formation rightCavalry = tacticDefensiveEngagementInst.Field("_rightCavalry").GetValue() as Formation;
            Formation leftCavalry = tacticDefensiveEngagementInst.Field("_leftCavalry").GetValue() as Formation;
            Formation rangedCavalry = tacticDefensiveEngagementInst.Field("_rangedCavalry").GetValue() as Formation;
            
            if (archers != null)
            {
                archers.AI.SetBehaviorWeight<BehaviorSkirmish>(0f);
                archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(0f);
                archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
                archers.AI.SetBehaviorWeight<BehaviorRegroup>(1.25f);
            }
            if (mainInfantry != null)
            {
                mainInfantry.AI.SetBehaviorWeight<BehaviorRegroup>(1.75f);
            }
            if (rightCavalry != null)
            {
                rightCavalry.AI.ResetBehaviorWeights();
                rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
            }
            if (leftCavalry != null)
            {
                leftCavalry.AI.ResetBehaviorWeights();
                leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Left;
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
        [HarmonyPatch("Engage")]
        private static void PostfixAttack(TacticDefensiveEngagement __instance)
        {
            var tacticDefensiveEngagement = Traverse.Create(__instance);
            Formation archers = tacticDefensiveEngagement.Field("_archers").GetValue() as Formation;
            Formation mainInfantry = tacticDefensiveEngagement.Field("_mainInfantry").GetValue() as Formation;
            Formation rightCavalry = tacticDefensiveEngagement.Field("_rightCavalry").GetValue() as Formation;
            Formation leftCavalry = tacticDefensiveEngagement.Field("_leftCavalry").GetValue() as Formation;
            Formation rangedCavalry = tacticDefensiveEngagement.Field("_rangedCavalry").GetValue() as Formation;
            
            if (archers != null)
            {
                archers.AI.ResetBehaviorWeights();
                archers.AI.SetBehaviorWeight<RBMBehaviorArcherSkirmish>(1f);
                archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(0f);
                archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(0f);
            }
            if (rightCavalry != null)
            {
                rightCavalry.AI.ResetBehaviorWeights();
                TacticComponent.SetDefaultBehaviorWeights(rightCavalry);
                rightCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
                rightCavalry.AI.SetBehaviorWeight<RBMBehaviorCavalryCharge>(1f);
            }
            if (leftCavalry != null)
            {
                leftCavalry.AI.ResetBehaviorWeights();
                TacticComponent.SetDefaultBehaviorWeights(leftCavalry);
                leftCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
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
    }
}