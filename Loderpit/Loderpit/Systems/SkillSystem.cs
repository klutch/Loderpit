using System;
using System.Collections.Generic;

namespace Loderpit.Systems
{
    public class SkillSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Skill; } }

        public SkillSystem()
        {
        }

        public void update()
        {
        }
    }
}
