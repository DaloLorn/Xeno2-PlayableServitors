using System.Reflection;
using Artitas.Utils;
using log4net;

namespace X2UnificationWar {
    
    public class ModConstants {
        
        #region Logging

        public static readonly ILog Log = ArtitasLogger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly bool IsWarnEnabled = Log.IsWarnEnabled;
        public static readonly bool IsInfoEnabled = Log.IsInfoEnabled;

        #endregion

        public static readonly string MedisprayType = "DL_X2UW_MedisprayAbility";
        public static readonly string ActorRoleGroupOverrideType = "DL_X2UW_ActorRoleGroupOverride";
    }
}