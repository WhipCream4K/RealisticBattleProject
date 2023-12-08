using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace RBMAI.AiModule.RbmTactics
{
    [HarmonyPatch(typeof(TacticDefensiveLine))]
    internal class TacticDefensiveLinePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HasBattleBeenJoined")]
        private static bool PrefixHasBattleBeenJoined(TacticDefensiveLine __instance,ref bool __result)
        {
            var tacticDefensiveLine = Traverse.Create(__instance);
            Formation mainInfantry = tacticDefensiveLine.Field("_mainInfantry").GetValue<Formation>();
            bool hasBattleBeenJoined = tacticDefensiveLine.Field("_hasBattleBeenJoined").GetValue<bool>();
            
            __result = RBMAI.Utilities.HasBattleBeenJoined(mainInfantry, hasBattleBeenJoined);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Defend")]
        private static void PostfixDefend(
            TacticDefensiveLine __instance)
        {
            var tacticDefensiveLine = Traverse.Create(__instance);
            Formation archers = tacticDefensiveLine.Field("_archers").GetValue<Formation>();
            Formation mainInfantry = tacticDefensiveLine.Field("_mainInfantry").GetValue<Formation>();
            Formation rightCavalry = tacticDefensiveLine.Field("_rightCavalry").GetValue<Formation>();
            Formation leftCavalry = tacticDefensiveLine.Field("_leftCavalry").GetValue<Formation>();
            Formation rangedCavalry = tacticDefensiveLine.Field("_rangedCavalry").GetValue<Formation>();
            
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
        private static void PostfixEngage(TacticDefensiveLine __instance)
        {
            var tacticDefensiveLine = Traverse.Create(__instance);
            Formation archers = tacticDefensiveLine.Field("_archers").GetValue<Formation>();
            Formation mainInfantry = tacticDefensiveLine.Field("_mainInfantry").GetValue<Formation>();
            Formation rightCavalry = tacticDefensiveLine.Field("_rightCavalry").GetValue<Formation>();
            Formation leftCavalry = tacticDefensiveLine.Field("_leftCavalry").GetValue<Formation>();
            Formation rangedCavalry = tacticDefensiveLine.Field("_rangedCavalry").GetValue<Formation>();

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
    }
}