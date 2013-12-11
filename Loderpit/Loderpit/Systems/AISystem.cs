using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class AISystem : ISystem
    {
        private const float ENGAGEMENT_DISTANCE = 10f;
        public SystemType systemType { get { return SystemType.AI; } }

        public AISystem()
        {
        }

        // Handle basic combat AI
        private void handleBasicCombatAi(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
                List<int> attackableEntities;

                // Skip if has target
                if (EntityManager.getCombatTargetComponent(entityId) != null)
                {
                    return;
                }

                // Skip if doesn't have any attack skills
                if (skillsComponent.attackSkills.Count == 0)
                {
                    return;
                }

                // Find hostile entities within the engagement distance
                attackableEntities = Helpers.findEntitiesWithinRange(positionComponent.position, ENGAGEMENT_DISTANCE, factionComponent.hostileFaction);

                foreach (int targetId in attackableEntities)
                {
                    PositionComponent targetPositionComponent = EntityManager.getPositionComponent(targetId);
                    IncapacitatedComponent targetIncapacitatedComponent = EntityManager.getIncapacitatedComponent(targetId);
                    Vector2 relative = targetPositionComponent.position - positionComponent.position;

                    // Skip if player's incapacitated
                    if (targetIncapacitatedComponent != null)
                    {
                        continue;
                    }

                    SystemManager.combatSystem.startActiveAttack(entityId, targetId, skillsComponent.attackSkills[0].range);
                    break;
                }
            }
        }

        public void update()
        {
            List<int> basicCombatAiEntities = EntityManager.getEntitiesPossessing(ComponentType.BasicCombatAI);
            List<int> frenzyAiEntities = EntityManager.getEntitiesPossessing(ComponentType.FrenzyAI);

            // Handle basic combat ai
            handleBasicCombatAi(basicCombatAiEntities);

            // Handle frenzy ai
            handleBasicCombatAi(frenzyAiEntities);
        }
    }
}
