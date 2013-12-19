using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class AnimationSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Animation; } }

        public AnimationSystem()
        {
        }

        // Determine correct animation type
        private void determineAnimation(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                AnimationComponent animationComponent = EntityManager.getAnimationComponent(entityId);
                PhysicsComponent physicsComponent = EntityManager.getPhysicsComponent(entityId);

                switch (animationComponent.animationCategory)
                {
                    case AnimationCategory.Enemy:
                    case AnimationCategory.Character:
                        Vector2 velocity = physicsComponent.bodies[0].LinearVelocity;
                        float speed = velocity.Length();

                        animationComponent.ticksPerFrame = 10;

                        if (speed > 1f)
                        {
                            animationComponent.ticksPerFrame = 1;
                            animationComponent.animationType = velocity.X < 0 ? AnimationType.WalkLeft : AnimationType.WalkRight;
                        }
                        else if (speed > 0.5f)
                        {
                            animationComponent.ticksPerFrame = 5;
                            animationComponent.animationType = velocity.X < 0 ? AnimationType.WalkLeft : AnimationType.WalkRight;
                        }
                        else
                        {
                            animationComponent.ticksPerFrame = 60;
                            animationComponent.animationType = AnimationType.Idle;
                        }

                        break;

                    case AnimationCategory.Drone:
                        animationComponent.ticksPerFrame = 60;
                        animationComponent.animationType = AnimationType.Idle;
                        break;
                }
            }
        }

        // Update
        public void update()
        {
            // Determine correct animation type
            determineAnimation(EntityManager.getEntitiesPossessing(ComponentType.Animation));
        }
    }
}
