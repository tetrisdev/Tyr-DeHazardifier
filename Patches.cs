using System;
using SPT.Reflection.Patching;
using EFT.Interactive;
using HarmonyLib;
using System.Reflection;

namespace HazardPatches
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PatchTargetAttribute : Attribute
    {
        public Type TargetType { get; }
        public string MethodName { get; }

        public PatchTargetAttribute(Type targetType, string methodName)
        {
            TargetType = targetType;
            MethodName = methodName;
        }
    }

    public abstract class DisableDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var attr = GetType().GetCustomAttribute<PatchTargetAttribute>();
            return AccessTools.Method(attr.TargetType, attr.MethodName);
        }
    }

    [PatchTarget(typeof(Minefield), nameof(Minefield.IsInTriggerZone))]
    public class MinefieldTriggerPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(Minefield), nameof(Minefield.FireCoroutine))]
    public class MinefieldCoroutinePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(Minefield), nameof(Minefield.DealExplosionDamage))]
    public class MinefieldDamagePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(MinefieldView), nameof(MinefieldView.Explode))]
    public class MinefieldViewTriggerPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(MineDirectional), nameof(MineDirectional.Awake))]
    public class MineDirectionalAwakePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(MineDirectional), nameof(MineDirectional.OnTriggerEnter))]
    public class MineDirectionalTriggerPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(MineDirectional), nameof(MineDirectional.OnTriggerExit))]
    public class MineDirectionalTriggerColliderPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(MineDirectional), nameof(MineDirectional.GetDamageInfo))]
    public class MineDirectionalDamagePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(BarbedWire), nameof(BarbedWire.ProceedDamage))]
    public class BarbedWireDamagePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(BarbedWire), nameof(BarbedWire.AddPenalty))]
    public class BarbedWireSpeedPenaltyPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(BarbedWire), nameof(BarbedWire.RemovePenalty))]
    public class BarbedWireSpeedPenalty2Patch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperImitator), nameof(SniperImitator.Awake))]
    public class SniperImitatorAwakePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperImitator), nameof(SniperImitator.OnPlayerShotEvent))]
    public class SniperImitatorDamagePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperImitator), nameof(SniperImitator.method_0))]
    public class SniperImitatorShootPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperFiringZone), nameof(SniperFiringZone.Shoot))]
    public class SniperFiringZoneShootPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperFiringZone), nameof(SniperFiringZone.FireCoroutine))]
    public class SniperFiringZoneCoroutinePatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperFiringZone), nameof(SniperFiringZone.IsSniperImmuneBot))]
    public class SniperFiringZoneTargetPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(SniperFiringZone), nameof(SniperFiringZone.IsInBTR))]
    public class SniperFiringZoneTarget2Patch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }

    [PatchTarget(typeof(FlameDamageTrigger), nameof(FlameDamageTrigger.ProceedDamage))]
    public class FlameDamageTriggerPatch : DisableDamagePatch
    {
        [PatchPrefix]
        public static bool Prefix() => false;
    }
}
