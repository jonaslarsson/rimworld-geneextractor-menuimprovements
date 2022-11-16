using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace ChridoGeneExtractorMenuImprovements
{
    public class Mod : Verse.Mod
    {
        public ModSettings settings;

        public Mod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("io.github.jonaslarsson.rimworld-geneextractor-menuimprovements");
            harmony.PatchAll();
        }
    }
}
