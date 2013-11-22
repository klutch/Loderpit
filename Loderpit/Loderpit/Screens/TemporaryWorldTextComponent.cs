using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class TemporaryWorldTextComponent : ScreenComponent
    {
        private Font _font;
        private Text _text;
        private string _value;
        private Vector2 _worldPosition;
        private Vector2f _screenPosition;
        private int _delay = 120;
        private Vector2f _acceleration = new Vector2f(0f, 0.025f);
        private Vector2f _velocity;
        private Vector2f _offset;

        public int delay { get { return _delay; } }

        public TemporaryWorldTextComponent(Screen screen, Font font, string value, Vector2 position)
            : base(screen)
        {
            _font = font;
            _value = value;
            _worldPosition = position;
            _text = new Text(value, font, 18);
        }

        public override void update()
        {
            Vector2i intScreenPosition = Game.window.MapCoordsToPixel(new Vector2f(_worldPosition.X, _worldPosition.Y), SystemManager.cameraSystem.worldView);

            _screenPosition.X = intScreenPosition.X;
            _screenPosition.Y = intScreenPosition.Y;
            _velocity += _acceleration;
            _offset += _velocity;
            _text.DisplayedString = _value;
            _text.Position = _screenPosition + _offset;
            _delay--;
        }

        public override void draw()
        {
            Game.window.Draw(_text);
        }
    }
}
