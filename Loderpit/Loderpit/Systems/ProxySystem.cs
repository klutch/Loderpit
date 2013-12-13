using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class ProxySystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Proxy; } }

        // Handle restore position targets
        private void handleRestorePositionTargets(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);

                if (positionTargetComponent == null)
                {
                    IsProxyComponent isProxyComponent = EntityManager.getIsProxyComponent(entityId);
                    PhysicsComponent physicsComponent = EntityManager.getPhysicsComponent(isProxyComponent.proxyForId);

                    EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, physicsComponent.bodies[0], 0.75f));
                }
            }
        }

        // Update
        public void update()
        {
            // Handle restore position targets
            handleRestorePositionTargets(EntityManager.getEntitiesPossessing(ComponentType.RestoreProxyPositionTarget));
        }
    }
}
