using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class LoadGameScreen : Screen
    {
        private Texture _logoTexture;
        private RectangleShape _logoShape;
        private List<XElement> _allPlayerData;
        private List<RectangleShape> _shapes;
        private Font _font;
        private List<Text> _labels;
        private Dictionary<CharacterClass, Texture> _classTextures;
        private List<List<RectangleShape>> _characterIconRows;
        private Color _unselectedButtonColor = new Color(50, 50, 50, 255);
        private Color _selectedButtonColor = new Color(128, 128, 128, 255);

        public LoadGameScreen()
            : base(ScreenType.LoadGame)
        {
        }

        public override void initialize()
        {
        }

        public override void loadContent()
        {
            float logoScale;
            CharacterClass[] characterClasses = (CharacterClass[])Enum.GetValues(typeof(CharacterClass));

            _logoTexture = ResourceManager.getResource<Texture>("logo_1");
            _logoShape = new RectangleShape();
            _logoShape.Texture = _logoTexture;
            _logoShape.Size = new Vector2f(_logoTexture.Size.X, _logoTexture.Size.Y);
            logoScale = Game.window.GetView().Size.X / (float)_logoTexture.Size.X;
            _logoShape.Scale = new Vector2f(logoScale, logoScale);

            _classTextures = new Dictionary<CharacterClass, Texture>();
            for (int i = 0; i < characterClasses.Length; i++)
            {
                CharacterClass characterClass = characterClasses[i];

                _classTextures.Add(characterClass, new Texture("resources/ui/class_icons/" + characterClass.ToString().ToLower() + ".png"));
            }

            _allPlayerData = PlayerDataManager.getAllPlayerData();
            _shapes = new List<RectangleShape>();
            _labels = new List<Text>();
            _font = ResourceManager.getResource<Font>("gooddog_font");
            _characterIconRows = new List<List<RectangleShape>>();
            foreach (XElement playerData in _allPlayerData)
            {
                RectangleShape shape = new RectangleShape();
                string text = "player_" + playerData.Attribute("uid").Value;
                Text label = new Text(text, _font, 24);
                List<RectangleShape> characterIcons = new List<RectangleShape>();

                shape.Position = new Vector2f(128, (float)_logoShape.Texture.Size.Y * _logoShape.Scale.Y + 64 + _shapes.Count * 64);
                shape.Size = new Vector2f(340, 48);
                shape.FillColor = Color.Blue;

                label.Position = shape.Position + new Vector2f(24f, 8f);
                label.Color = Color.White;

                _shapes.Add(shape);
                _labels.Add(label);

                foreach (XElement characterData in playerData.Elements("Character"))
                {
                    CharacterClass characterClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), characterData.Attribute("class").Value);
                    RectangleShape rectangleShape = new RectangleShape();

                    rectangleShape.Texture = _classTextures[characterClass];
                    rectangleShape.Size = new Vector2f(rectangleShape.Texture.Size.X, rectangleShape.Texture.Size.Y);
                    rectangleShape.Scale = new Vector2f(0.5f, 0.5f);
                    rectangleShape.Position = label.Position + new Vector2f(128 + 48 * characterIcons.Count, 0);

                    characterIcons.Add(rectangleShape);
                }

                _characterIconRows.Add(characterIcons);
            }
        }

        public override void update()
        {
            base.update();

            // Restore all button colors
            foreach (RectangleShape shape in _shapes)
            {
                shape.FillColor = _unselectedButtonColor;
            }

            // Check mouse input
            for (int i = 0; i < _shapes.Count; i++)
            {
                int uid = int.Parse(_allPlayerData[i].Attribute("uid").Value);
                RectangleShape shape = _shapes[i];
                FloatRect shapeBounds = shape.GetGlobalBounds();
                FloatRect mouseBounds = Game.newMouseState.rectangle;

                if (shapeBounds.Intersects(mouseBounds))
                {
                    shape.FillColor = _selectedButtonColor;

                    if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
                    {
                        Game.endContinueGameState();
                        Game.startInterLevelState(uid);
                    }
                    break;
                }
            }
        }

        public override void draw()
        {
            base.draw();

            Game.window.Draw(_logoShape);

            foreach (RectangleShape shape in _shapes)
            {
                Game.window.Draw(shape);
            }

            foreach (Text text in _labels)
            {
                Game.window.Draw(text);
            }

            foreach (List<RectangleShape> characterIconRow in _characterIconRows)
            {
                foreach (RectangleShape shape in characterIconRow)
                {
                    Game.window.Draw(shape);
                }
            }
        }
    }
}
