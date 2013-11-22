using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using Loderpit.Managers;
using Loderpit.Components;

namespace Loderpit.Systems
{
    public class RenderSystem : ISystem
    {
        private DebugView _debugView;
        private CircleShape _createRopeShape;
        private RectangleShape _createBridgeShape;

        public SystemType systemType { get { return SystemType.Render; } }

        public RenderSystem()
        {
            _debugView = new DebugView();
            
            _createRopeShape = new CircleShape(1f);
            _createRopeShape.Origin = new Vector2f(1f, 1f);
            _createRopeShape.FillColor = new Color(0, 255, 0, 128);

            _createBridgeShape = new RectangleShape(new Vector2f(0.2f, 0f));
            _createBridgeShape.FillColor = new Color(255, 255, 0, 128);
            _createBridgeShape.Origin = new Vector2f(0.1f, 0f);
        }

        // Draw actions currently being performed
        private void drawCurrentActions()
        {
            /*
            TeamSystem teamSystem = SystemManager.teamSystem;

            if (teamSystem.performingAction == CharacterAction.CreateRope)
            {
                _createRopeShape.Position = new Vector2f(teamSystem.createRopeAnchor.X, teamSystem.createRopeAnchor.Y);
                Game.window.Draw(_createRopeShape);
            }
            else if (teamSystem.performingAction == CharacterAction.CreateBridge)
            {
                Vector2 pointA = teamSystem.createBridgeAnchorA;
                Vector2 pointB = teamSystem.createBridgeAnchorB;
                Vector2 relative = pointB - pointA;
                float length = relative.Length();
                float angle = Helpers.radToDeg((float)Math.Atan2(relative.Y, relative.X));

                _createBridgeShape.Position = new Vector2f(pointA.X, pointA.Y);
                _createBridgeShape.Rotation = angle;
                _createBridgeShape.Size = new Vector2f(0.2f, length);
                Game.window.Draw(_createBridgeShape);
            }*/
        }

        public void update()
        {
        }

        public void draw()
        {
            // Draw physical world (debug view)
            _debugView.draw();

            // Draw actions currently being performed
            drawCurrentActions();

            // Draw already created actions that are waiting to be executed
        }
    }
}
