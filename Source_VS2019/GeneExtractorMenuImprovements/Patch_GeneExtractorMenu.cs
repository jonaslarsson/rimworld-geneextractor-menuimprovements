using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace GeneExtractorMenuImprovements
{
	[HarmonyPatch(typeof(Building_GeneExtractor), nameof(Building_GeneExtractor.GetGizmos))]
	class Patch_Building_GeneExtractor_GetGizmos_patch
    {
		static void Postfix(Building_GeneExtractor __instance, ref IEnumerable<Gizmo> __result)
        {
			List<Gizmo> updatedGizmos = new List<Gizmo>();
			List<Gizmo> gizmos = new List<Gizmo>(__result);
			foreach (Gizmo gizmo in gizmos)
			{
				Command_Action command_Action = gizmo as Command_Action;
				if (command_Action != null && command_Action.defaultLabel == "InsertPerson".Translate() + "...")
				{
					command_Action.action = delegate ()
					{
						List<FloatMenuOption> list = new List<FloatMenuOption>();
						foreach (Pawn pawn2 in (__instance as Thing).Map.mapPawns.AllPawnsSpawned)
						{
							Pawn pawn = pawn2;
							AcceptanceReport acceptanceReport = __instance.CanAcceptPawn(pawn2);
							if (!acceptanceReport.Accepted)
							{
								if (!acceptanceReport.Reason.NullOrEmpty())
								{
									list.Add(new FloatMenuOption(pawn2.LabelShortCap + ": " + acceptanceReport.Reason, null, pawn,
										Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
								}
							}
							else
							{
								string label = pawn2.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap;

								if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating, false))
								{
									label += " (fatal)";
								}

								list.Add(new FloatMenuOption(label, delegate ()
								{
									var selectPawn = __instance.GetType().GetMethod("SelectPawn", BindingFlags.NonPublic | BindingFlags.Instance);
									object[] parameters = new object[] { pawn };
									selectPawn.Invoke(__instance, parameters);
								}, pawn, Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
							}
						}
						if (!list.Any<FloatMenuOption>())
						{
							list.Add(new FloatMenuOption("NoExtractablePawns".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
						Find.WindowStack.Add(new FloatMenu(list));
					};
					updatedGizmos.Add(gizmo);

					// Add action for prisoners
					Command_Action command_ActionSlaves = new Command_Action();
					command_ActionSlaves.defaultLabel = "Safely insert prisoner...";
					command_ActionSlaves.defaultDesc = "InsertPersonGeneExtractorDesc".Translate();
					command_ActionSlaves.icon = command_Action.icon;
					command_ActionSlaves.action = delegate ()
					{
						List<FloatMenuOption> list = new List<FloatMenuOption>();
						foreach (Pawn pawn2 in (__instance as Thing).Map.mapPawns.AllPawnsSpawned)
						{
							Pawn pawn = pawn2;
							AcceptanceReport acceptanceReport = __instance.CanAcceptPawn(pawn2);
							if (!acceptanceReport.Accepted) continue;
							if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating, false)) continue;
							if (pawn.IsPrisoner == false) continue;

							list.Add(new FloatMenuOption(pawn2.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap, delegate ()
							{
								var selectPawn = __instance.GetType().GetMethod("SelectPawn", BindingFlags.NonPublic | BindingFlags.Instance);
								object[] parameters = new object[] { pawn };
								selectPawn.Invoke(__instance, parameters);
							}, pawn, Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
						if (!list.Any<FloatMenuOption>())
						{
							list.Add(new FloatMenuOption("NoExtractablePawns".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
						Find.WindowStack.Add(new FloatMenu(list));
					};
					updatedGizmos.Add(command_ActionSlaves);
				}
				else
				{
					updatedGizmos.Add(gizmo);
				}
			}
			__result = updatedGizmos;
		}
	}
}
	