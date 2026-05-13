using System.Reflection;
using Artitas.Utils;
using log4net;
using Xenonauts.Common.Components;

namespace X2UnificationWar {
    
    public class ModConstants {
        
        #region Logging

        public static readonly ILog Log = ArtitasLogger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly bool IsWarnEnabled = Log.IsWarnEnabled;
        public static readonly bool IsInfoEnabled = Log.IsInfoEnabled;

        #endregion

        public static readonly string[] PlayableSpecies = [Species.Human().value, Species.Mechanical().value];
        public static readonly string DefectorGroup = RoleGroup.Xenonaut().value;
        public static readonly string DefaultGroup = RoleGroup.Default().value;
    }
}