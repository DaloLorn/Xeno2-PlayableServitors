using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Artitas;
using Artitas.Utils;
using Common.Content;
using Common.Editor.Inspector;
using Common.Modding;
using HarmonyLib;
using log4net;
using Xenonauts.GroundCombat.Components;

namespace X2UnificationWar {
    
    public class X2UnificationWarModLifecycle : IModLifecycle {
        
        #region Logging

        private static readonly ILog Log = ArtitasLogger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly bool IsWarnEnabled = Log.IsWarnEnabled;
        private static readonly bool IsInfoEnabled = Log.IsInfoEnabled;

        #endregion

        public void Create(Mod mod, Harmony patcher) {
            Log.Warn("[X2-Unification-War] Loaded Unification War!");

            /* Don't mind me, was just messing around with the type registry
            var LowercaseKeyToType = (Dictionary<string, Type>) typeof(TypeKeyIndexRuntime).GetField("LowercaseKeyToType", BindingFlags.Static | BindingFlags.NonPublic).GetValue(TypeKeyIndexRuntime.Instance);
            var TypeToKey = (Dictionary<Type, string>)
                typeof(TypeKeyIndexRuntime).GetField("TypeToKey", BindingFlags.Static | BindingFlags.NonPublic).GetValue(TypeKeyIndexRuntime.Instance);

            LowercaseKeyToType.Add("MedikitAbility", typeof(MedikitAbilityDefinition));
            LowercaseKeyToType.Add("InventoryBoardSlot", typeof(InventorySlotWithBoardFactory));
            LowercaseKeyToType.Add("InventorySizeSlot", typeof(InventorySlotWithSizeFactory));
            */
        }
        
        public void Destroy() {
            Log.Warn("[X2-Unification-War] Destroyed Unification War! (For peace!)");
        }
        
        public void OnWorldCreate(IModLifecycle.Section section, WeakReference<World> world) {
            Log.Warn($"[X2-Unification-War] World Create: {section}");
        }

        public IEnumerable<Descriptor> GetRequiredAssets(IModLifecycle.Section section) {
            return [];
        }

        public void OnWorldDispose(IModLifecycle.Section section, WeakReference<World> world) {
            Log.Warn($"[X2-Unification-War] World Dispose: {section}");
        }
    }
    
}