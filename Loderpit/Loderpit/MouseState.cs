using System;
using SFML.Window;
using SFML.Graphics;

namespace Loderpit
{
    public struct MouseState
    {
        private bool _leftButtonPressed;
        private bool _rightButtonPressed;
        private FloatRect _rectangle;
        private Vector2i _position;

        public bool isLeftButtonPressed { get { return _leftButtonPressed; } }
        public bool isRightButtonPressed { get { return _rightButtonPressed; } }
        public FloatRect rectangle { get { return _rectangle; } }
        public Vector2i position { get { return _position; } }

        public MouseState(bool leftButtonPressed, bool rightButtonPressed, Vector2i position)
        {
            _leftButtonPressed = leftButtonPressed;
            _rightButtonPressed = rightButtonPressed;
            _position = position;
            _rectangle.Left = position.X;
            _rectangle.Top = position.Y;
            _rectangle.Width = 1;
            _rectangle.Height = 1;
        }

        public static MouseState get()
        {

            return new MouseState(Mouse.IsButtonPressed(Mouse.Button.Left), Mouse.IsButtonPressed(Mouse.Button.Right), Mouse.GetPosition(Game.window));
        }
    }
}
