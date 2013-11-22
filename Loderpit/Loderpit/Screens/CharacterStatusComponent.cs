using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class CharacterStatusComponent : ScreenComponent
    {
        private int _entityId;
        private PositionComponent _positionComponent;
        private Text _text;
        private Font _font;
        private bool _enabled;
        private Vector2f _offset;

        public CharacterStatusComponent(Screen screen, Font font, int entityId, Vector2f offset)
            : base(screen)
        {
            _font = font;
            _entityId = entityId;
            _text = new Text("", font, 14);
            _offset = offset;
        }

        public override void update()
        {
            _positionComponent = EntityManager.getPositionComponent(_entityId);

            if (_positionComponent == null)
            {
                _enabled = false;
            }
            else
            {
                Vector2i screenPosition = Game.window.MapCoordsToPixel(new Vector2f(_positionComponent.position.X, _positionComponent.position.Y), SystemManager.cameraSystem.worldView);

                if (EntityManager.getCreateRopeComponent(_entityId) != null)
                {
                    _enabled = true;
                    _text.DisplayedString = "Creating Rope";
                }
                else if (EntityManager.getCreateBridgeComponent(_entityId) != null)
                {
                    _enabled = true;
                    _text.DisplayedString = "Creating Bridge";
                }
                /*else if (EntityManager.getAttackObstacleComponent(_entityId) != null)
                {
                    _enabled = true;
                    _text.DisplayedString = "Attacking Obstacle";
                }*/
                else
                {
                    _enabled = false;
                }

                _text.Position = new Vector2f(screenPosition.X, screenPosition.Y) - new Vector2f(_text.GetLocalBounds().Width / 2f, 0) + _offset;
            }
        }

        public override void draw()
        {
            if (_enabled)
            {
                Game.window.Draw(_text);
            }
        }
    }
}
