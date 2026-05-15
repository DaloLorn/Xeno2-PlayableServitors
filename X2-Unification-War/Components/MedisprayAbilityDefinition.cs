using Artitas.Utils;
using Common.Content;
using Common.RPG;
using System;
using UnityEngine;
using X2UnificationWar.Abilities;
using Xenonauts;
using Xenonauts.GroundCombat.Components;
using Xenonauts.UI;

#nullable disable
namespace X2UnificationWar.Components
{
  public class MedisprayAbilityDefinition : TargetAbilityDefinition
  {
    public static readonly AssetReference<CrosshairBehaviour> CROSSHAIR = AssetReference.Asset<CrosshairBehaviour>("ui/crosshairs/crosshair_heal", (UnsafeOptional<Enum>)GameScreens.GroundCombat);

    public int Range { get; set; }

    public int FXDuration { get; set; }

    public AssetReference<AudioClip> SoundFX { get; set; }

    public AssetReference<GameObject> LineProjectilePrefab { get; set; }

    public MedisprayAbilityDefinition() => Crosshair = CROSSHAIR;

    public override AbilityDefinitionBuilder BuildAbilityDefinition()
    {
      return MedisprayAbility.Create(Name, Crosshair, AbsoluteCostTimeUnits, PercentageCostTimeUnits, Range, LineProjectilePrefab, SoundFX, FXDuration, ImportanceScale);
    }

    protected bool Equals(MedisprayAbilityDefinition other)
    {
      return Equals((TargetAbilityDefinition) other) && Equals(SoundFX, other.SoundFX);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != GetType()) && Equals((MedisprayAbilityDefinition) obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() * 397 ^ (SoundFX != null ? SoundFX.GetHashCode() : 0);
    }
  }
}
