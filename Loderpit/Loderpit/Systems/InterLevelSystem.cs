using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Loderpit.Loaders;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class InterLevelSystem : ISystem
    {
        private InterLevelLoader _loader;

        public SystemType systemType { get { return SystemType.InterLevel; } }

        public InterLevelSystem()
        {
            _loader = new InterLevelLoader();
        }

        //
        public void load()
        {
            _loader.load(SystemManager.physicsSystem.world, "resources/modules/interlevel_0.json", true);
        }

        // Called by Game.endInterLevelState() -- Unload inter-level by removing all physical bodies and destroying all entities
        public void unload()
        {
            // Destroy all entities
            EntityManager.destroyAllEntities();
            SystemManager.physicsSystem.world.Step(1f / 60f);

            // Remove physical bodies
            foreach (Body body in SystemManager.physicsSystem.world.BodyList)
            {
                SystemManager.physicsSystem.world.RemoveBody(body);
            }
            SystemManager.physicsSystem.world.Step(1f / 60f);
        }

        // Update
        public void update()
        {
        }
    }
}
