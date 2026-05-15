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
                    // The fact that this works despite my fetching the wrong component 
                    // (I was experimenting with a different approach before
                    // landing on ActorRoleGroupOverride, and forgot to edit it)
                    // speaks volumes about the game's inner workings.
                    //
                    // Unfortunately, it's all Wraith to me. My best guess:
                    // https://discord.com/channels/702822278148390983/1352327469180387388/1504914652763521094
                    //
                    // (Sidenote: I don't know if the Has check is needed,
                    // but didn't want to risk crashing the game on a Get 
                    // against a nonexistent component.)
                    var groupOverride = composable.Has<RoleGroup>() ? composable.Get<RoleGroup>() : null;
                    var group = bodyId.group;
                    return groupOverride != null ? groupOverride.value : group;
                };
                Log.Info($"[X2-Unification-War] Patched combatant::body_identifier.group - checking for override component ${ActorRoleGroupOverrideType}");
            }
        }
    
    }
    
}