using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class ExplosionSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Explosion; } }

        public ExplosionSystem()
        {
        }

        // Create explosion
        public List<int> createExplosion(int entityId, Vector2 position, float radius, int damage, float force)
        {
            AABB aabb = new AABB(position - new Vector2(radius, radius), position + new Vector2(radius, radius));
            List<int> affectedEntities = new List<int>();

            // Handle bodies with physical representations
            SystemManager.physicsSystem.world.QueryAABB((fixture) =>
            {
                int targetEntityId;
                Vector2 relative = fixture.Body.Position - position;
                Vector2 normal = Vector2.Normalize(relative);
                CharacterComponent characterComponent;

                // Skip fixtures whose bodies don't have a user data
                if (fixture.Body.UserData == null)
                {
                    return true;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip if outside of radius
                if (relative.Length() >= radius)
                {
                    return true;
                }

                // Store as an affected entity if we haven't already
                if (affectedEntities.Contains(targetEntityId))
                {
                    return true;
                }

                // Skip entities that aren't characters
                // TODO: handle other types of entities
                if ((characterComponent = EntityManager.getCharacterComponent(targetEntityId)) == null)
                {
                    return true;
                }

                characterComponent.body.ApplyLinearImpulse(new Vector2(0, -1f));
                characterComponent.body.ApplyForce(normal * force);
                SystemManager.combatSystem.applySpellDamage(targetEntityId, damage);
                affectedEntities.Add(targetEntityId);

                return true;
            },
                ref aabb);

            // TODO: Handle any other types of entities here...

            return affectedEntities;
        }

        // Handle explosion delays
        private void handleExplosionDelays(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                TimedExplosionComponent timedExplosionComponent = EntityManager.getTimedExplosionComponent(entityId);
                PositionComponent positionComponent;

                // Skip inactive timed explosions
                if (!timedExplosionComponent.active)
                {
                    continue;
                }

                if (timedExplosionComponent.delay == 0)
                {
                    positionComponent = EntityManager.getPositionComponent(entityId);
                    createExplosion(entityId, positionComponent.position, timedExplosionComponent.radius, Roller.roll(timedExplosionComponent.damageDie), timedExplosionComponent.force);
                    EntityManager.destroyEntity(entityId);
                }
                else
                {
                    timedExplosionComponent.delay--;
                }
            }
        }

        public void update()
        {
            List<int> timedExplosionEntities = EntityManager.getEntitiesPossessing(ComponentType.TimedExplosion);

            // Handle explosion delays
            handleExplosionDelays(timedExplosionEntities);
        }
    }
}
