using System;
using System.Collections.Generic;
using System.Linq;
using Loderpit.Skills;

namespace Loderpit.Components
{
    public class SkillsComponent : IComponent
    {
        private int _entityId;
        private List<Skill> _skills;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Skills; } }
        public List<Skill> skills { get { return _skills; } }
        public List<Skill> activatableSkills { get { return new List<Skill>(from s in _skills where s.activatable select s); } }
        public List<Skill> attackSkills
        {
            get
            {
                return new List<Skill>(from s in _skills where (s.type == SkillType.MeleeAttack || s.type == SkillType.RangedAttack || s.type == SkillType.Piercing || s.type == SkillType.Kick) select s);
            }
        }

        public SkillsComponent(int entityId, List<Skill> skills = null)
        {
            _entityId = entityId;
            _skills = skills ?? new List<Skill>();
        }

        public Skill getSkill(SkillType type)
        {
            return (from s in _skills where s.type == type select s).FirstOrDefault();
        }
    }
}
