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
        private const float ENGAGE_PLAYER_DISTANCE = 10f;
        public SystemType systemType { get { return SystemType.AI; } }

        public AISystem()
        {
        }

        // Handle AI
        private void handleAi(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                AIComponent aiComponent = EntityManager.getAiComponent(entityId);

                switch (aiComponent.aiType)
                {
                    case AIType.BasicEnemy:
                        handleBasicEnemyAI(entityId);
                        break;
                }
            }
        }

        // Handle basic enemy AI
        private void handleBasicEnemyAI(int entityId)
        {
            List<int> playerEntities = SystemManager.teamSystem.getTeamEntities();
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PositionComponent enemyPositionComponent = EntityManager.getPositionComponent(entityId);
            SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
            MeleeAttackSkill meleeAttackSkill;

            // Skip if not enemy
            if (factionComponent.faction != Faction.Enemy)
            {
                return;
            }

            // Skip if has group
            if (SystemManager.groupSystem.getGroupComponentContaining(entityId) != null)
            {
                return;
            }

            // Skip if has target
            if (EntityManager.getCombatTargetComponent(entityId) != null)
            {
                return;
            }

            // Skip if doesn't have melee attack skill
            if ((meleeAttackSkill = skillsComponent.getSkill(SkillType.MeleeAttack) as MeleeAttackSkill) == null)
            {
                return;
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

        public void update()
        {
            List<int> aiEntities = EntityManager.getEntitiesPossessing(ComponentType.AI);

            // Handle ungrouped targets
            handleAi(aiEntities);
        }
    }
}
