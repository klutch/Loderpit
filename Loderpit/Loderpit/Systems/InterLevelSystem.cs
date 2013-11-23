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

        public void load()
        {
            _loader.load(SystemManager.physicsSystem.world, "resources/modules/interlevel_0.json", true);
        }

        public void update()
        {
        }
    }
}
