using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class EnemyAISystem : ISystem
    {
        private const float ENGAGE_PLAYER_DISTANCE = 10f;
        public SystemType systemType { get { return SystemType.EnemyAI; } }

        public EnemyAISystem()
        {
        }

        // Handle setting of targets for entities that aren't in a group
        private void handleUngroupedTargets(List<int> entities)
        {
            List<int> playerEntities = SystemManager.teamSystem.getTeamEntities();

            foreach (int entityId in entities)
            {
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                PositionComponent enemyPositionComponent = EntityManager.getPositionComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
                MeleeAttackSkill meleeAttackSkill;

                // Skip if not enemy
                if (factionComponent.faction != Faction.Enemy)
                {
                    continue;
                }

                // Skip if has group
                if (SystemManager.groupSystem.getGroupComponentContaining(entityId) != null)
                {
                    continue;
                }

                // Skip if has target
                if (EntityManager.getCombatTargetComponent(entityId) != null)
                {
                    continue;
                }

                // Skip if doesn't have melee attack skill
                if ((meleeAttackSkill = skillsComponent.getSkill(SkillType.MeleeAttack) as MeleeAttackSkill) == null)
                {
                    continue;
                }

                foreach (int playerId in playerEntities)
                {
                    PositionComponent playerPositionComponent = EntityManager.getPositionComponent(playerId);
                    IncapacitatedComponent playerIncapacitatedComponent = EntityManager.getIncapacitatedComponent(playerId);
                    Vector2 relative = playerPositionComponent.position - enemyPositionComponent.position;

                    // Skip if player's incapacitated
                    if (playerIncapacitatedComponent != null)
                    {
                        continue;
                    }

                    if (relative.Length() <= ENGAGE_PLAYER_DISTANCE)
                    {
                        SystemManager.combatSystem.startActiveAttack(entityId, playerId, meleeAttackSkill.range);
                        break;
                    }
                }
            }
        }

        public void update()
        {
            List<int> factionEntities = EntityManager.getEntitiesPossessing(ComponentType.Faction);

            // Handle ungrouped targets
            handleUngroupedTargets(factionEntities);
        }
    }
}
