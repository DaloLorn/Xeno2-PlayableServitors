using Artitas;
using Artitas.Utils;
using Common;
using Common.Boards;
using Common.Input;
using Common.RPG;
using Common.Util;
using log4net;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Xenonauts.GroundCombat;
using Xenonauts.GroundCombat.Components;
using Xenonauts.GroundCombat.Factories;
using Xenonauts.GroundCombat.Picking;
using Xenonauts.GroundCombat.Systems;

#nullable disable
namespace X2UnificationWar.Abilities
{
  public class MedisprayConflict : BaseConflict
  {
    private static readonly ILog Log = ArtitasLogger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly bool IsWarnEnabled = Log.IsWarnEnabled;
    private static readonly bool IsInfoEnabled = Log.IsInfoEnabled;
    public override BaseConflict Clone()
    {
            MedisprayConflict medikitConflict = new()
            {
                _ability = _ability,
                Attacker = Attacker,
                Target = Target,
                World = World,
                _abilityKey = _abilityKey,
                AbilitySource = AbilitySource,
                AttackerAddress = AttackerAddress,
                TrueChanceToHit = TrueChanceToHit,
                PerceivedChanceToHit = PerceivedChanceToHit,
                Board = Board,
                Computed = Computed,
                Cover = Cover,
                Damage = Damage,
                CoverFinder = CoverFinder,
                SightSystem = SightSystem,
                TargetAddress = TargetAddress,
                AttackerVisionAngle = AttackerVisionAngle,
                AttackerVisionRange = AttackerVisionRange,
                AttackerVisionVector = AttackerVisionVector,
                SightPrecondition = SightPrecondition,
                TimeUnits = TimeUnits,
                LastErrorMessage = LastErrorMessage,
                TargetIsCrouching = TargetIsCrouching,
                TargetIsWithinRange = TargetIsWithinRange,
                TargetSightPreconditionsAreMet = TargetSightPreconditionsAreMet,
                AngleForArc = AngleForArc,
                AttackerReference = AttackerReference,
                TargetReference = TargetReference,
                AbilitySourceReference = AbilitySourceReference
            };
            return medikitConflict;
    }

    private MedisprayConflict()
    {
    }

    public MedisprayConflict(
      World world,
      SightSystem system,
      Board board,
      InterveningCoverFinder intervening,
      AdjacentCoverFinder targetAdjacentCoverFinder,
      AdjacentCoverFinder attackerAdjacentCoverFinder,
      InterveningUnitFinder interveningUnitFinder,
      AtmosphericCoverFinder atmosphericCoverFinder)
      : base(world, system, board, intervening, targetAdjacentCoverFinder, attackerAdjacentCoverFinder, interveningUnitFinder, atmosphericCoverFinder)
    {
    }

    public override void ComputeChanceToHit()
    {
      PerceivedChanceToHit = TrueChanceToHit = !TargetIsWithinRange || !TargetSightPreconditionsAreMet ? 0.0f : 100f;
    }

    public override bool ComputeTargetIsWithinRange()
    {
      Ability ability = AbilitySource.Abilities().FindAbility(_abilityKey);
        var range = ability.GetVariable(GroundCombatConstants.KEY_RANGE).Value;
      return TargetIsWithinRange = TargetAddress == AttackerAddress || (double) AttackerAddress.EuclidTileSqrDistanceTo(TargetAddress) <= (double) range * (double) range;
    }

    public override void ResolveTarget(
      MouseHoverReport hoverReport,
      GCPickingReport pickReport,
      out Entity targetEntity,
      out Address targetAddress,
      out GCPickingReport.Context target)
    {
      if (pickReport.PickedEntity != null && IsTargetValid(pickReport.PickedEntity))
      {
        targetEntity = pickReport.PickedEntity;
        targetAddress = pickReport.PickedAddress;
        target = pickReport.CurrentContext;
      }
      else if (pickReport.BoardTileAddress.isValid)
      {
        Optional<Entity> optional = Board.GetSlot(pickReport.BoardTileAddress).GetEntities(IsTargetValid).OrderBy(c => (int)c.LifeStatus().value).FirstOrOptional();
        if (optional.IsSet)
        {
          targetEntity = optional.Value;
          targetAddress = (Address)targetEntity.Address();
          target = GCPickingReport.Context.Target;
        }
        else
        {
          targetEntity = null;
          targetAddress = pickReport.BoardTileAddress;
          target = GCPickingReport.Context.Move;
        }
      }
      else
        base.ResolveTarget(hoverReport, pickReport, out targetEntity, out targetAddress, out target);
    }

    public override bool IsTargetValid(Entity candidateTarget)
    {
      return candidateTarget != null && GroundCombatArchetypes.Combatant.Accepts(candidateTarget) && candidateTarget.GCCombatantMeta().CanBeHealed() && !candidateTarget.LifeStatus().IsDead();
    }

    public override bool IsProperlySetup()
    {
      int num = !base.IsProperlySetup() || !AttackerAddress.isValid ? 0 : (TargetAddress.isValid ? 1 : 0);
      if (num != 0)
        return num != 0;
      if (!AttackerAddress.isValid)
        LastErrorMessage += ", AttackerAddress is not valid";
      if (TargetAddress.isValid)
        return num != 0;
      LastErrorMessage += ", TargetAddress is not valid";
      return num != 0;
    }

    public override bool CancelForceFireWhenCyclingAbility() => true;

    protected override void OnCompute()
    {
      ComputeTargetIsWithinRange();
      ComputeTargetIsVisible();
      ComputeChanceToHit();
            _ = (double)ComputeTimeUnits();
        }

    public static BaseConflict Generate(
      [Combatant] Entity attacker,
      [Combatant] Entity target,
      [Firearm] Entity abilitySource,
      string abilityKey)
    {
      GCBoardSystem system1 = attacker.World.GetSystem<GCBoardSystem>();
      SightSystem system2 = attacker.World.GetSystem<SightSystem>();
      MedisprayConflict medikitConflict = new(system1.World, system2, system1.Board, system1.InterveningCover, system1.TargetAdjacentCover, system1.AttackerAdjacentCover, system1.InterveningUnit, system1.AtmosphericCover);
      Entity attacker1 = attacker;
      Optional<float> optional = (Optional<float>) 360f;
      Optional<Address> address = new();
      Optional<float> visionAngle = optional;
      Optional<float> visionRange = new();
      Optional<Vector3> visionDirection = new();
      Optional<SightPrecondition> sightPrecondition = new();
      medikitConflict.SetAttacker(attacker1, address, visionAngle, visionRange, visionDirection, sightPrecondition);
      medikitConflict.SetSource(abilitySource, (Optional<string>) abilityKey);
      medikitConflict.SetTarget(target);
      return medikitConflict;
    }
  }
}
