using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class CharacterAnimationSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.CharacterAnimation; } }

        public CharacterAnimationSystem()
        {
        }

        // Determine correct character animation type
        private void determineCharacterAnimation(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                CharacterAnimationComponent characterAnimationComponent = EntityManager.getCharacterAnimationComponent(entityId);

                if (characterComponent.movementSpeed < 2)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkLeft;
                    characterAnimationComponent.ticksPerFrame = 1;
                }
                else if (characterComponent.movementSpeed < 1)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkLeft;
                    characterAnimationComponent.ticksPerFrame = 2;
                }
                else if (characterComponent.movementSpeed < 0)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkLeft;
                    characterAnimationComponent.ticksPerFrame = 3;
                }

                if (characterComponent.movementSpeed > 2)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkRight;
                    characterAnimationComponent.ticksPerFrame = 1;
                }
                else if (characterComponent.movementSpeed > 1)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkRight;
                    characterAnimationComponent.ticksPerFrame = 2;
                }
                else if (characterComponent.movementSpeed > 0)
                {
                    characterAnimationComponent.type = CharacterAnimationType.WalkRight;
                    characterAnimationComponent.ticksPerFrame = 3;
                }
                
                if (characterComponent.movementSpeed == 0)
                {
                    characterAnimationComponent.type = CharacterAnimationType.Idle;
                    characterAnimationComponent.ticksPerFrame = 1;
                }
            }
        }

        // Update
        public void update()
        {
            // Determine correct character animation type
            determineCharacterAnimation(EntityManager.getEntitiesPossessing(ComponentType.CharacterAnimation));
        }
    }
}
