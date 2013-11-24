using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

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
