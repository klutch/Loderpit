using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class MainMenuScreen : Screen
    {
        private Texture _logoTexture;
        private RectangleShape _logoShape;
        private Font _font;
        private List<Text> _options;
        private int _selectedOption = -1;
        private Color _selectedColor;
        private Color _unselectedColor;

        public MainMenuScreen()
            : base(ScreenType.MainMenu)
        {
        }

        override public void initialize()
        {
            _selectedColor = Color.Yellow;
            _unselectedColor = new Color(150, 150, 150, 255);
        }

        override public void loadContent()
        {
            float logoScale;

            _logoTexture = ResourceManager.getResource<Texture>("logo_1");
            _logoShape = new RectangleShape();
            _logoShape.Texture = _logoTexture;
            _logoShape.Size = new Vector2f(_logoTexture.Size.X, _logoTexture.Size.Y);
            logoScale = Game.window.GetView().Size.X / (float)_logoTexture.Size.X;
            _logoShape.Scale = new Vector2f(logoScale, logoScale);

            _font = ResourceManager.getResource<Font>("gooddog_font");
            _options = new List<Text>();
            _options.Add(new Text("New Game", _font, 48));
            _options.Add(new Text("Continue", _font, 48));
            _options.Add(new Text("Options", _font, 48));
            _options.Add(new Text("Exit", _font, 48));

            for (int i = 0; i < _options.Count; i++)
            {
                Text text = _options[i];

                text.Position = new Vector2f(128, i * 48 + _logoShape.Size.Y * _logoShape.Scale.Y + 64);
            }
        }

        public override void update()
        {
            bool overAnyOptions = false;

            base.update();

            // Restore text color
            for (int i = 0; i < _options.Count; i++)
            {
                Text text = _options[i];

                if (i != _selectedOption)
                {
                    text.Color = _unselectedColor;
                }
            }

            // Handle mouse input
            for (int i = 0; i < _options.Count; i++)
            {
                Text text = _options[i];
                FloatRect box = text.GetGlobalBounds();
                FloatRect mouse = Game.newMouseState.rectangle;

                if (box.Intersects(mouse))
                {
                    _selectedOption = i;
                    overAnyOptions = true;
                }
            }

            if (overAnyOptions)
            {
                _options[_selectedOption].Color = _selectedColor;

                // Check for clicks
                if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
                {
                    switch (_selectedOption)
                    {
                        // New game
                        case 0:
                            Game.endMainMenuState();
                            Game.startCreateTeamState();
                            break;

                        // Continue
                        case 1:
                            break;

                        // Options
                        case 2:
                            break;

                        // Quit
                        case 3:
                            Game.quit();
                            break;
                    }
                }
            }
            else
            {
                _selectedOption = -1;
            }
        }

        public override void draw()
        {
            Game.window.Draw(_logoShape);

            foreach (Text text in _options)
            {
                Game.window.Draw(text);
            }

            base.draw();
        }
    }
}
