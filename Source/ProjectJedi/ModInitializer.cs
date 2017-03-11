using AbilityUser;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ProjectJedi
{ 
    /*
     * Special thanks to Erdelf for helping me create and utilize these injectors.
     * -Jecrell 
    */
    [StaticConstructorOnStartup]
    public class ModInitializer : MonoBehaviour
    {
        public static string ModTitle = "ProjectJedi";

        static ModInitializer()
        {
            GameObject initializer = new UnityEngine.GameObject(ModTitle);
            initializer.AddComponent<ModInitializer>();
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)initializer);
        }

        protected float reinjectTime = 0;
        int lastTicks;

        public void FixedUpdate()
        {
            try
            {
                if (Find.TickManager != null)
                {
                    if (Find.TickManager.TicksGame > lastTicks + 200)
                    {
                        lastTicks = Find.TickManager.TicksGame;
                        reinjectTime -= Time.fixedDeltaTime;
                        if (reinjectTime <= 0)
                        {
                            reinjectTime = 0;
                            if (Find.Maps != null)
                            {
                                Find.Maps.ForEach(delegate (Map map)
                                {
                                    List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where((Pawn p) => p.story != null).ToList();
                                    pawns.Where((Pawn p) => p.Name != null && p.TryGetComp<CompForceUser>() == null &&
                                            (p.story.traits.HasTrait(ProjectJediDefOf.PJ_JediTrait) ||
                                            p.story.traits.HasTrait(ProjectJediDefOf.PJ_SithTrait))).ToList().ForEach(
                                        delegate (Pawn p)
                                        {
                                            Log.Message("CompForceUser Added");
                                            CompForceUser pca = new CompForceUser();
                                            pca.parent = p;
                                            p.AllComps.Add(pca);

                                        });
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

        }
    }
}