using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using FarseerRubeLoader;
using Loderpit.Managers;
using Loderpit.Components;

namespace Loderpit.Loaders
{
    public class TerrainModuleLoader : RubeLoader
    {
        private Map _map;
        private Vector2 _anchorBuffer;

        public TerrainModuleLoader(Map map)
            : base()
        {
            _map = map;
        }

        protected override void beforeLoadBodies(World world, CustomProperties customWorldProperties)
        {
            _offset = _map.moduleEndPointsCount == 0 ? _map.initialPosition : _map.getLastModuleEndPoint();

            foreach (XElement bodyData in _worldData.Elements("body"))
            {
                string name = bodyData.Element("name").Value;

                if (name == "moduleAnchorA")
                {
                    // Do nothing with this for now
                }
                else if (name == "moduleAnchorB")
                {
                    _anchorBuffer = loadVector2(bodyData.Element("position"));
                }
            }

            base.beforeLoadBodies(world, customWorldProperties);
        }

        protected override void afterLoadBodies()
        {
            _map.addModuleEndPoint(_anchorBuffer + _offset);

            base.afterLoadBodies();
        }

        protected override bool beforeLoadBody(string name, XElement bodyData)
        {
            if (name == "moduleAnchorA" || name == "moduleAnchorB")
            {
                _bodies.Add(null);
                return false;
            }
            
            return base.beforeLoadBody(name, bodyData);
        }

        protected override void afterLoadBody(string name, Body body, CustomProperties customProperties, XElement bodyData)
        {
            EntityFactory.afterLoadBody(name, body, customProperties, bodyData);
            base.afterLoadBody(name, body, customProperties, bodyData);
        }
    }
}
