using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class BigLabeledButtonComponent : ScreenComponent
    {
        private Texture _buttonTexture;
        private RectangleShape _buttonShape;
        private Vector2f _position;
        private Text _firstLetter;
        private Text _word;
        private Color _buttonColor;
        private Color _selectedColor;
        private Action _onClick;
        private Font _font;
        private bool _selected;

        public bool selected { get { return _selected; } set { _selected = value; } }
        public Action onClick { get { return _onClick; } }

        public BigLabeledButtonComponent(Screen screen, Texture buttonTexture, Vector2f position, string text, Color buttonColor, Action onClick)
            : base(screen)
        {
            _buttonTexture = buttonTexture;
            _position = position;
            _buttonColor = buttonColor;
            _selectedColor = new Color(
                (byte)Math.Min(255, (int)_buttonColor.R + 50),
                (byte)Math.Min(255, (int)_buttonColor.G + 50),
                (byte)Math.Min(255, (int)_buttonColor.B + 50),
                255);
            _onClick = onClick;
            _font = ResourceManager.getResource<Font>("immortal_font");

            // Initialize button shape
            _buttonShape = new RectangleShape();
            _buttonShape.Texture = _buttonTexture;
            _buttonShape.Position = position;
            _buttonShape.Size = new Vector2f(_buttonShape.Texture.Size.X, _buttonShape.Texture.Size.Y);
            _buttonShape.FillColor = _buttonColor;

            // Initialize text
            _firstLetter = new Text(text.Substring(0, 1), _font, 72);
            _firstLetter.Position = position + new Vector2f(30, 0);
            _firstLetter.Color = Color.White;
            _word = new Text(text.Substring(1, text.Length - 1), _font, 48);
            _word.Position = _firstLetter.Position + new Vector2f(_firstLetter.GetLocalBounds().Width + 4, 13);
            _word.Color = Color.White;
        }

        public bool testPoint(Vector2f point)
        {
            FloatRect pointRect = new FloatRect(point.X, point.Y, 1, 1);

            if (pointRect.Intersects(_buttonShape.GetGlobalBounds()))
            {
                return true;
            }

            if (pointRect.Intersects(_firstLetter.GetGlobalBounds()))
            {
                return true;
            }

            if (pointRect.Intersects(_word.GetGlobalBounds()))
            {
                return true;
            }

            return false;
        }

        public override void update()
        {
            _buttonShape.FillColor = _selected ? _selectedColor : _buttonColor;
        }

        public override void draw()
        {
            Game.window.Draw(_buttonShape);
            Game.window.Draw(_firstLetter);
            Game.window.Draw(_word);
        }
    }
}
