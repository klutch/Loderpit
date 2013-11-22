using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;
using Loderpit.Systems;
using Loderpit.Components;

namespace Loderpit.Screens
{
    public class SelectedCharacterComponent : ScreenComponent
    {
        private Texture _texture;
        private RectangleShape _shape;

        public SelectedCharacterComponent(Screen screen, Texture texture)
            : base(screen)
        {
            _texture = texture;
            _shape = new RectangleShape(new Vector2f(_texture.Size.X, _texture.Size.Y));
            _shape.Texture = _texture;
            _shape.Origin = new Vector2f(_texture.Size.X, _texture.Size.Y) / 2f;
        }

        public override void update()
        {
            List<int> teammateEntities = SystemManager.teamSystem.getTeamEntities();
            int selectedEntityId = SystemManager.teamSystem.getTeammateEntityId(SystemManager.teamSystem.selectedTeammate);
            PositionComponent positionComponent = EntityManager.getPositionComponent(selectedEntityId);
            Vector2i screenCoords = Game.window.MapCoordsToPixel(new Vector2f(positionComponent.position.X, positionComponent.position.Y), SystemManager.cameraSystem.worldView);

            _shape.Position = new Vector2f(screenCoords.X, screenCoords.Y);
        }

        public override void draw()
        {
            Game.window.Draw(_shape);
        }
    }
}
