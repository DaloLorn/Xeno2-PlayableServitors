using Common.Util;
using Xenonauts.Common.Components;

namespace X2UnificationWar.Components
{
    public class ActorRoleGroupOverride : DataLoadedEnum<RoleGroup>
    {
        public static explicit operator ActorRoleGroupOverride(string name)
        {
            return new ActorRoleGroupOverride
            {
                value = name
            };
        }
    }
}