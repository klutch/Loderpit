using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FarseerPhysics.Dynamics;
using FarseerRubeLoader;

namespace Loderpit.Loaders
{
    public class InterLevelLoader : RubeLoader
    {
        public InterLevelLoader()
        {
        }

        protected override void afterLoadBody(string name, Body body, CustomProperties customProperties, XElement bodyData)
        {
            EntityFactory.afterLoadBody(name, body, customProperties, bodyData);
            base.afterLoadBody(name, body, customProperties, bodyData);
        }
    }
}
