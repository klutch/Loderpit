using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Loderpit.Formations;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class CharacterSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Character; } }

        public CharacterSystem()
        {
        }

        // Grab a rope
        public void grabRope(int entityId, RopeComponent ropeComponent, Body ropeBody)
        {
            RopeGrabExclusionComponent ropeGrabExclusionComponent = EntityManager.getRopeGrabExclusionComponent(entityId);

            if (ropeGrabExclusionComponent == null || !ropeGrabExclusionComponent.excludedRopeComponents.Contains(ropeComponent))
            {
                GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                int index = ropeComponent.bodies.IndexOf(ropeBody);
                RevoluteJoint joint = JointFactory.CreateRevoluteJoint(SystemManager.physicsSystem.world, ropeBody, characterComponent.body, Vector2.Zero, Vector2.Zero);
                RopeGrabComponent ropeGrabComponent = new RopeGrabComponent(entityId, ropeComponent, joint, (float)index);

                EntityManager.addComponent(entityId, ropeGrabComponent);

                if (groupComponent != null)
                {
                    Formation activeFormation = groupComponent.activeFormation;
                    float maxPosition = ropeComponent.anchorBody.Position.X + 2f * groupComponent.entities.Count;
                    LimitedRangeFormation newFormation = new LimitedRangeFormation(groupComponent.entities, activeFormation.position, activeFormation.speed, float.MinValue, maxPosition);

                    ropeGrabComponent.formationToRemove = newFormation;
                    activeFormation.position = (ropeComponent.anchorBody.Position.X + maxPosition) * 0.5f;
                    groupComponent.addFormation(newFormation);
                }
            }
        }

        // Release a rope
        public void releaseRope(int entityId, RopeGrabComponent ropeGrabComponent)
        {
            RopeGrabExclusionComponent ropeGrabExclusionComponent = EntityManager.getRopeGrabExclusionComponent(entityId);
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);

            if (ropeGrabExclusionComponent == null)
            {
                ropeGrabExclusionComponent = new RopeGrabExclusionComponent(entityId);
                EntityManager.addComponent(entityId, ropeGrabExclusionComponent);
            }

            ropeGrabExclusionComponent.excludedRopeComponents.Add(ropeGrabComponent.ropeComponent);
            EntityManager.removeComponent(entityId, ComponentType.RopeGrab);

            if (groupComponent != null)
            {
                groupComponent.removeFormation(ropeGrabComponent.formationToRemove);
            }
        }

        // Move along a grabbed rope
        private void updateGrab(int entityId, RopeGrabComponent ropeGrabComponent, CharacterComponent characterBodyComponent)
        {
            Vector2 localAnchor;
            Body body;
            int oldIndex = (int)Math.Floor(ropeGrabComponent.progress);
            int newIndex;
            float fraction;
            float segmentLength = 1.2f;

            SystemManager.physicsSystem.world.RemoveJoint(ropeGrabComponent.joint);
            ropeGrabComponent.progress += 0.05f;
            newIndex = (int)Math.Floor(ropeGrabComponent.progress);

            if (newIndex < 0 || newIndex > ropeGrabComponent.ropeComponent.bodies.Count - 1)
            {
                releaseRope(entityId, ropeGrabComponent);
            }
            else
            {
                fraction = ropeGrabComponent.progress - newIndex;
                body = ropeGrabComponent.ropeComponent.bodies[newIndex];
                localAnchor = new Vector2((fraction - 0.5f) * segmentLength, 0f);
                ropeGrabComponent.joint = JointFactory.CreateRevoluteJoint(SystemManager.physicsSystem.world, body, characterBodyComponent.body, localAnchor, Vector2.Zero);
            }
        }

        // Handle movement on a rope
        private void handleRopeMovement(List<int> characterEntities)
        {
            foreach (int entityId in characterEntities)
            {
                RopeGrabComponent ropeGrabComponent;

                if ((ropeGrabComponent = EntityManager.getRopeGrabComponent(entityId)) != null)
                {
                    CharacterComponent characterBodyComponent = EntityManager.getCharacterComponent(entityId);

                    if (ropeGrabComponent.ropeComponent.climbDirection == ClimbDirection.Down)
                    {
                        updateGrab(entityId, ropeGrabComponent, characterBodyComponent);
                    }
                }
            }
        }

        // Handle create rope actions
        private void handleCreateRope(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                CreateRopeComponent createRopeComponent = EntityManager.getCreateRopeComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                PositionTargetComponent targetComponent = EntityManager.getPositionTargetComponent(entityId);
                float difference = Math.Abs(targetComponent.position - positionComponent.position.X) - 0.1f;

                if (difference <= targetComponent.tolerance)
                {
                    if (createRopeComponent.delay <= 0)
                    {
                        // Perform action
                        GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);

                        EntityFactory.createRope(createRopeComponent.position);
                        EntityManager.removeComponent(entityId, ComponentType.CreateRope);
                        EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
                        SystemManager.skillSystem.resetCooldown(entityId, SkillType.ThrowRope);

                        if (groupComponent != null)
                        {
                            float currentPosition = groupComponent.getFormation(FormationType.LimitedRange).position;

                            groupComponent.getFormation(FormationType.Default).position = currentPosition;
                            groupComponent.removeFormation(createRopeComponent.formationToRemove);
                        }
                    }
                    else
                    {
                        // Decrement delay
                        createRopeComponent.delay--;
                    }
                }
            }
        }

        // Handle create bridge actions
        private void handleCreateBridge(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                CreateBridgeComponent createBridgeComponent = EntityManager.getCreateBridgeComponent(entityId);
                PositionTargetComponent targetComponent = EntityManager.getPositionTargetComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                float difference = Math.Abs(targetComponent.position - positionComponent.position.X) - 0.1f;

                if (difference <= targetComponent.tolerance)
                {
                    if (createBridgeComponent.delay <= 0)
                    {
                        // Perform action
                        GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);

                        EntityFactory.createBridge(createBridgeComponent.positionA, createBridgeComponent.positionB);
                        EntityManager.removeComponent(entityId, ComponentType.CreateBridge);
                        EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
                        SystemManager.skillSystem.resetCooldown(entityId, SkillType.BuildBridge);

                        if (groupComponent != null)
                        {
                            // Update the default formation position to the limited range formation's position, then remove the limited range formation.
                            float currentPosition = groupComponent.getFormation(FormationType.LimitedRange).position;

                            groupComponent.removeFormation(createBridgeComponent.formationToRemove);
                            groupComponent.getFormation(FormationType.Default).position = currentPosition;
                        }
                    }
                    else
                    {
                        // Decrement delay timer
                        createBridgeComponent.delay--;
                    }
                }
            }
        }

        // Handle normal walking movement
        private void handleWalkingMovement(List<int> characterEntities)
        {
            foreach (int entityId in characterEntities)
            {
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                Formation formation = SystemManager.groupSystem.getActiveFormationByEntityId(entityId);
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                PositionTargetComponent targetComponent = EntityManager.getPositionTargetComponent(entityId);

                if (targetComponent == null && formation == null)   // skip movement if there's nothing telling the character where to move
                {
                    continue;
                }
                else
                {
                    Vector2 entityPosition = positionComponent.position;
                    float idealPosition = targetComponent == null ? formation.getSlotPositionByEntityId(entityId) : targetComponent.position;
                    float difference = idealPosition - entityPosition.X;
                    float absDifference = Math.Abs(difference);
                    int normalSpeed = formation == null ? characterComponent.movementSpeed : formation.speed;
                    float tolerance = targetComponent == null ? 0 : targetComponent.tolerance;
                    bool backwards = difference < 0.0001;

                    if (absDifference < 0.025f + tolerance)  // Close enough to stop
                    {
                        characterComponent.movementSpeed = 0;
                    }
                    else if (absDifference < 0.05f + tolerance)  // Should slow down a little bit
                    {
                        characterComponent.movementSpeed = backwards ? -1 : 1;
                    }
                    else if (absDifference < 0.1f + tolerance) // Continue at normal speed
                    {
                        characterComponent.movementSpeed = normalSpeed;
                    }
                    else if (absDifference < 0.2f + tolerance) // Should speed up a little
                    {
                        characterComponent.movementSpeed = backwards ? -2 : 2;
                    }
                    else if (absDifference < 0.3f + tolerance) // Should speed up a little more
                    {
                        characterComponent.movementSpeed = backwards ? -3 : 3;
                    }
                    else // Should speed up a lot
                    {
                        characterComponent.movementSpeed = backwards ? -4 : 4;
                    }
                }
            }
        }

        public void update()
        {
            List<int> characterEntities = EntityManager.getEntitiesPossessing(ComponentType.Character);
            List<int> createRopeEntities = EntityManager.getEntitiesPossessing(ComponentType.CreateRope);
            List<int> createBridgeEntities = EntityManager.getEntitiesPossessing(ComponentType.CreateBridge);

            // Handle rope movement
            handleRopeMovement(characterEntities);

            // Handle create rope action
            handleCreateRope(createRopeEntities);

            // Handle create bridge action
            handleCreateBridge(createBridgeEntities);

            // Handle walking movement
            handleWalkingMovement(characterEntities);
        }
    }
}
