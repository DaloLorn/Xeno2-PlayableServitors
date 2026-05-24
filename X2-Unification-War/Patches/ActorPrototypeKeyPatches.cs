using Artitas;
using Common.Util;
using HarmonyLib;
using Xenonauts.Common;
using Xenonauts.Common.Assets.Identifiers;
using Xenonauts.Common.Components;
using static X2UnificationWar.ModConstants;

namespace X2UnificationWar.Patches {
    
    [HarmonyPatch(typeof(ActorPrototypeKey), "CreateFactory")]
    public static class ActorPrototypeKeyPatches {
        
        /// <summary>
        /// Updates the KeyFactory.KeyTransform associated with combatant::body_identifier.group,
        /// in order to remap "xenonaut" to "default". This allows me to reuse the default prefabs
        /// for alien units while still defining unique defector actors!
        /// </summary>
        [HarmonyPostfix]
        public static void CreateFactory(ref KeyFactory __result)
        {
            // Log message to show this patch is working
            Log.Info("[X2-Unification-War] Intercepted ActorPrototypeKey.CreateFactory");

            var part = __result.Find(
                part => part.Key == "combatant" && part.Pattern == "body_identifier.group");

            if (part != null)
            {
                part.Transform = obj =>
                {
                    var composable = (IComposable)obj;
                    var bodyId = composable.Get<BodyIdentifierComponent>();
                    var groupOverride = composable.Has<RoleGroup>() ? composable.Get<RoleGroup>() : null;
                    var group = bodyId.group;
                    return groupOverride != null ? groupOverride.value : group;
                };
                Log.Info($"[X2-Unification-War] Patched combatant::body_identifier.group - checking for override component ${nameof(RoleGroup)}");
            }
        }
    
    }
    
}