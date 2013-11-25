using System;
using System.Collections.Generic;
using Loderpit.Skills;

namespace Loderpit.Components
{
    public class PerformingSkillsComponent : IComponent
    {
        private int _entityId;
        private List<ExecuteSkill> _executingSkills;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.PerformingSkills; } }
        public List<ExecuteSkill> executingSkills { get { return _executingSkills; } }

        public PerformingSkillsComponent(int entityId)
        {
            _entityId = entityId;
            _executingSkills = new List<ExecuteSkill>();
        }

        public bool isPerformingSkill(SkillType skillType)
        {
            foreach (ExecuteSkill executeSkill in _executingSkills)
            {
                if (executeSkill.skill.type == skillType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
