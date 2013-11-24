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

        // Reset skill cooldown
        public void resetCooldown(int entityId, SkillType skillType)
        {
            Console.WriteLine("resetting skill type: {0}", skillType);
            SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            Skill skill = skillsComponent.getSkill(skillType);

            switch (skill.type)
            {
                case SkillType.MeleeAttack:
                case SkillType.RangedAttack:
                    skill.setCooldown(statsComponent.attackDelay);
                    break;

                default:
                    skill.setCooldown(skill.baseCooldown);
                    break;
            }
        }

        // Decrement skill cooldowns
        private void decrementSkillCooldowns(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    if (skill.cooldown > 0)
                    {
                        skill.decrementCooldown();
                    }
                }
            }
        }

        // Update
        public void update()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);

            // Decrement skill cooldowns
            decrementSkillCooldowns(skillsEntities);
        }
    }
}
