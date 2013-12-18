using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using Loderpit.Managers;
using Loderpit.Components;

namespace Loderpit.Systems
{
    public class CameraSystem : ISystem
    {
        public const float ORIGINAL_SCALE = 32f;
        private View _worldView;

        public View worldView { get { return _worldView; } }
        public SystemType systemType { get { return SystemType.Camera; } }

        public CameraSystem()
        {
            _worldView = new View(new Vector2f(0, 0), new Vector2f(Game.window.DefaultView.Size.X, Game.window.DefaultView.Size.Y));
            _worldView.Zoom(1f / ORIGINAL_SCALE);
        }

        public void update()
        {
            List<int> entities = SystemManager.teamSystem.getTeamEntities();
            Vector2 averagePosition = Vector2.Zero;
            Vector2 offset = new Vector2(0, -1f);

            foreach (int entityId in entities)
            {
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);

                averagePosition += positionComponent.position;
            }

            averagePosition /= entities.Count;
            averagePosition += offset;
            _worldView.Center = new Vector2f(averagePosition.X, averagePosition.Y);
        }
    }
}
