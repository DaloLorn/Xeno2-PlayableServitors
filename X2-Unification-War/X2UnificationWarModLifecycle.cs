using System;
using System.Collections.Generic;
using System.Reflection;
using Artitas;
using Artitas.Utils;
using Common.Content;
using Common.Modding;
using HarmonyLib;
using log4net;
using X2UnificationWar.Components;

namespace X2UnificationWar {
    
    public class X2UnificationWarModLifecycle : IModLifecycle {
        
        #region Logging

        private static readonly ILog Log = ArtitasLogger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly bool IsWarnEnabled = Log.IsWarnEnabled;
        private static readonly bool IsInfoEnabled = Log.IsInfoEnabled;

        #endregion
        
        public void Create(Mod mod, Harmony patcher)
        {
            Log.Warn("[X2-Unification-War] Loaded Unification War!");

            var LowercaseKeyToType = (Dictionary<string, Type>) typeof(TypeKeyIndexRuntime).GetField("LowercaseKeyToType", BindingFlags.Static | BindingFlags.NonPublic).GetValue(TypeKeyIndexRuntime.Instance);
            var TypeToKey = (Dictionary<Type, string>)
                typeof(TypeKeyIndexRuntime).GetField("TypeToKey", BindingFlags.Static | BindingFlags.NonPublic).GetValue(TypeKeyIndexRuntime.Instance);

            RegisterType(LowercaseKeyToType, TypeToKey, "DL_X2UW_MedisprayAbility", typeof(MedisprayAbilityDefinition));
        }

        void RegisterType(Dictionary<string, Type> LowercaseKeyToType, Dictionary<Type, string> TypeToKey, string Key, Type Type)
        {
            LowercaseKeyToType.Add(Key, Type);
            TypeToKey.Add(Type, Key);
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