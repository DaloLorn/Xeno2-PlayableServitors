using Artitas;
using Artitas.Utils;
using Common;
using Common.Boards;
using Common.Boards.System;
using Common.Content;
using Common.Mechanics.Factions;
using Common.RPG;
using Common.Util;
using EnumT.Extensions;
using System;
using UnityEngine;
using Xenonauts;
using Xenonauts.Common.Systems;
using Xenonauts.GroundCombat;
using Xenonauts.GroundCombat.Abilities;
using Xenonauts.GroundCombat.Abilities.Shoot;
using Xenonauts.GroundCombat.Boards.Natures;
using Xenonauts.GroundCombat.Systems;
using Xenonauts.GroundCombat.Utils;
using Xenonauts.UI;

#nullable disable
namespace X2UnificationWar.Abilities
{
  public class MedisprayAbility : ItemAbility
  {
    public static AbilityDefinitionBuilder Create(
      string name,
      AssetReference<CrosshairBehaviour> crosshair,
      int absoluteCost,
      float percentageCost,
      int range,
      AssetReference<GameObject> projectilePrefab,
      AssetReference<AudioClip> soundFX,
      float fxDuration,
      float importanceScale)
    {
      MedisprayAbility medisprayAbility = new();
      return new AbilityDefinitionBuilder()
      {
        ID = name,
        Name = name,
        Variables = {
          {
            GroundCombatConstants.KEY_ABSOLUTE_COST_TIME_UNITS,
             new XRange( absoluteCost)
          },
          {
            GroundCombatConstants.KEY_PERCENTAGE_COST_TIME_UNITS,
             new XRange(percentageCost)
          },
          {
            GroundCombatConstants.KEY_IMPORTANCE_SCALE,
             new XRange(importanceScale)
          },
              {
                  GroundCombatConstants.KEY_RANGE,
                  new XRange(range)
              },
        },
        Metadata = {
          {
            GroundCombatConstants.META_REQUIRES_AMMO,
             false
          },
          {
            GroundCombatConstants.META_CROSSHAIR,
             crosshair ?? AssetReference.Asset<CrosshairBehaviour>("ui/crosshair/crosshair_heal", (UnsafeOptional<Enum>)  GameScreens.GroundCombat)
          },
          {
            GroundCombatConstants.META_CONFLICT_TYPE,
             typeof (MedisprayConflict)
          },
          {
            GroundCombatConstants.META_CONFLICT_FACTORY,
             new AbilityFactory.ConflictFactory(MedisprayConflict.Generate)
          },
          {
            GroundCombatConstants.META_USER_INTERACTIVE,
             true
          },
              {
                  GroundCombatConstants.META_DELAY_TO_DESTROY_PROJECTILE,
                  fxDuration
              },
              {
                  GroundCombatConstants.META_FX_PREFAB,
                  projectilePrefab
              },
          {
            GroundCombatConstants.META_DOES_NOT_CAUSE_OVERWATCH,
             true
          },
          {
            GroundCombatConstants.META_AUDIOCLIP,
             soundFX
          }
        },
        CastPrecondition = (ability, weapon, rawConflict) => rawConflict is MedisprayConflict medisprayConflict1 && (double)medisprayConflict1.AttackerAvailableTU >= (double)medisprayConflict1.ComputeTimeUnits(),
        TargetPrecondition = (ability, weapon, target, rawConflict) =>
        {
            MedisprayConflict medisprayConflict2 = rawConflict as MedisprayConflict;
            if (!medisprayConflict2.IsTargetValid(target) || !medisprayConflict2.TargetIsWithinRange)
                return false;
            GCBoardSystem system = target.World.GetSystem<GCBoardSystem>();
            return medisprayConflict2.Target.IsFriendlyTo(medisprayConflict2.Attacker) && ((!target.HitPoints().AtMaximum() ? 1 : 0) | (!target.HasStun() ? 0 : (!target.Stun().AtMinimum() ? 1 : 0)) | (BleedingSystem.IsCombatantBleeding(target) ? 1 : 0)) != 0;
        },
        Effects = {
          medisprayAbility.AnimateUse()
        }
      };
    }

    public override void Apply(AbilityState state)
    {
      MedisprayConflict conflict = state.GetConflict() as MedisprayConflict;
      if (!conflict.Computed)
        conflict.Compute();
        Entity attacker = conflict.Attacker;
      attacker.DeltaTimeUnits(-conflict.TimeUnits);
      Entity target = conflict.Target;
      float maximum = target.HitPoints().DeltaToMaximum();
      int deltaStun = target.HasStun() ? -target.Stun().DeltaToMinimum().Ceil() : 0;
      DamageSystem.AdjustHpStunAndBleed(state.Target, maximum, deltaStun, DataLoadedEnums.DamageType.Heal(), conflict);
      
            AddressComponent addressComponent = attacker.Address();
            AddressComponent targetAddress = target.Address();
            Vector3 vector3 = Vector3.up * addressComponent.value.boardParam.slotHeight / 2f;
            Vector3 projectileStart = attacker.Transformation().position + vector3;
            Vector3 projectileEnd = targetAddress.value.position + vector3;
              float meta4 = state.GetMeta<float>(GroundCombatConstants.META_DELAY_TO_DESTROY_PROJECTILE);
              AssetReference<GameObject> meta5 = state.GetMeta<AssetReference<GameObject>>(GroundCombatConstants.META_FX_PREFAB);
              World world = state.Source.World;
              if (SightSystem.IsAddressVisibleForAnyLocalPlayer(world, (Address)targetAddress))
                world.CreateEntity().AddTransformation(projectileStart).AddPrototype(meta5, false, false, true).AddDeleteInSeconds(meta4).GameObject().value.GetComponent<IProjectile>().Spawn(projectileStart, projectileEnd);


      AssetReference<AudioClip> meta = state.GetMeta<AssetReference<AudioClip>>(GroundCombatConstants.META_AUDIOCLIP, null);
      if (meta != null)
        XenonautsAudioSystem.PlayOneShot(meta, GroundCombatConstants.Audio.Materials.COMBATANT, (Optional<Vector3>) state.Target.Transformation().position);
      else
                Log.WarnFormat("[heal] AudioClip not set in {0} for entity {1}", nameof(MedisprayAbility), state.Source);
    }
  }
}
