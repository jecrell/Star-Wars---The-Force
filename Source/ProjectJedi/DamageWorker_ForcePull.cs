using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ProjectJedi
{
    public class DamageWorker_ForcePull : DamageWorker_ForceLeveled
    {
        public override void ApprenticeEffect(Thing target)
        {
            if (target != null && target.def != null)
            {
                //Pull haulable things to us
                if (target.def.EverHaulable && (!(target is Pawn)))
                {
                    FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                    flyingObject.Launch(Caster, Caster, target);
                }
            }
        }
        public override void AdeptEffect(Thing target)
        {
            if (target != null && target.def != null)
            {
                //Pull haulable things to us
                if (target.def.EverHaulable && (!(target is Pawn)))
                {
                    //If it's equippable or wearable, equip/wear it right away.
                    if (target.def.equipmentType == EquipmentType.Primary || target.def.IsApparel)
                    {
                        FlyingObject_Equipable flyingObject = (FlyingObject_Equipable)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject_Equipable"), target.Position, target.Map);
                        flyingObject.Launch(Caster, Caster, target);
                    }
                    //Or don't equip it~~
                    else
                    {
                        FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                        flyingObject.Launch(Caster, Caster, target);
                    }
                }
            }
        }
        public override void MasterEffect(Thing target)
        {
            //Pull haulable things to us
            if (target.def.EverHaulable && (!(target is Pawn)))
            {
                //If it's equippable, equip it right away.
                if (target.def.equipmentType == EquipmentType.Primary || target.def.IsApparel)
                {
                    FlyingObject_Equipable flyingObject = (FlyingObject_Equipable)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject_Equipable"), target.Position, target.Map);
                    flyingObject.Launch(Caster, Caster, target);
                }
                //Or don't equip it~~
                else
                {
                    FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                    flyingObject.Launch(Caster, Caster, target);
                }
            }
            else if (target is Pawn)
            {
                Pawn pawnTarget = target as Pawn;
                if (pawnTarget != null)
                {
                    if (pawnTarget.equipment != null)
                    {
                        if (pawnTarget.equipment.Primary != null)
                        {
                            ThingWithComps droppedEquip = null;
                            pawnTarget.equipment.TryDropEquipment(pawnTarget.equipment.Primary, out droppedEquip, pawnTarget.Position.RandomAdjacentCell8Way(), false);
                            if (droppedEquip != null)
                            {
                                if (pawnTarget.RaceProps.Humanlike) pawnTarget.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("PJ_ThoughtPull"), null);
                                FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                                flyingObject.Launch(Caster, Caster, droppedEquip);
                            }
                        }
                        else
                        {
                            if (pawnTarget.RaceProps.Humanlike) pawnTarget.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("PJ_ThoughtPull"), null);
                            FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                            flyingObject.Launch(Caster, Caster, pawnTarget);    
                        }
                    }
                    else
                    {
                        if (pawnTarget.RaceProps.Humanlike) pawnTarget.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("PJ_ThoughtPull"), null);
                        FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("PJ_PFlyingObject"), target.Position, target.Map);
                        flyingObject.Launch(Caster, Caster, pawnTarget);
                    }
                }
            }
        }
    }
}
