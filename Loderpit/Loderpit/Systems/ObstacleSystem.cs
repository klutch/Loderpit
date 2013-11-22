using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class ObstacleSystem : ISystem
    {
        private List<SendActivateObstacleComponent> _sendComponentsToProcess;

        public SystemType systemType { get { return SystemType.Obstacle; } }

        public ObstacleSystem()
        {
            _sendComponentsToProcess = new List<SendActivateObstacleComponent>();
        }

        public void activateObstacle(SendActivateObstacleComponent sendActivateObstacleComponent)
        {
            _sendComponentsToProcess.Add(sendActivateObstacleComponent);
        }

        public void makeObstacleFall(ReceiveActivateObstacleFallComponent component)
        {
            component.body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
        }

        public void update()
        {
            foreach (SendActivateObstacleComponent sendActivateObstacleComponent in _sendComponentsToProcess)
            {
                foreach (int receivingEntityId in sendActivateObstacleComponent.receivingEntities)
                {
                    ReceiveActivateObstacleFallComponent receiveActivateObstacleFallComponent;

                    if (EntityManager.doesEntityExist(receivingEntityId))
                    {
                        if ((receiveActivateObstacleFallComponent = EntityManager.getReceiveActivateObstacleFallComponent(receivingEntityId)) != null)
                        {
                            makeObstacleFall(receiveActivateObstacleFallComponent);
                        }
                    }
                }
            }
            _sendComponentsToProcess.Clear();
        }
    }
}
