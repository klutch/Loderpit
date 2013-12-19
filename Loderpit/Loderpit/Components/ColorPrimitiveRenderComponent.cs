using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public struct BodyRenderData
    {
        public Transform transform;
        public Body body;
        public List<ConvexShape> shapes;
    }

    public class ColorPrimitiveRenderComponent : IComponent, Drawable
    {
        private int _entityId;
        private Color _color;
        private List<BodyRenderData> _renderData;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.ColorPrimitiveRender; } }
        public Color color { get { return _color; } }
        public List<BodyRenderData> renderData { get { return _renderData; } set { _renderData = value; } }

        public ColorPrimitiveRenderComponent(int entityId, Color color)
        {
            _entityId = entityId;
            _color = color;
        }

        public void Draw(RenderTarget renderTarget, RenderStates renderStates)
        {
            Transform originalTransform = renderStates.Transform;

            foreach (BodyRenderData bodyRenderData in _renderData)
            {
                renderStates.Transform = originalTransform * bodyRenderData.transform;

                foreach (ConvexShape shape in bodyRenderData.shapes)
                {
                    renderTarget.Draw(shape, renderStates);
                }
            }
        }
    }
}
