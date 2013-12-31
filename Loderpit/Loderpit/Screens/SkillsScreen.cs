using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public class SkillsScreen : Screen
    {
        private InterLevelScreen _interLevelScreen;
        private RectangleShape _background;
        private RectangleShape _pane;
        private Font _titleFont;
        private Text _title;
        private int _selectedTeammate = 0;
        private int _previousSelectedTeammate = -1;
        //private RectangleShape _leftArrow;
        //private RectangleShape _rightArrow;
        private RectangleShape _okayButton;
        private RectangleShape _cancelButton;
        private List<RectangleShape> _characterButtons;
        private List<Text> _characterButtonLabels;
        private List<RectangleShape> _characterButtonHighlights;
        private List<Texture> _classTextures;
        private Font _font;
        private Color _characterClassSelectedColor = Color.White;
        private Color _characterClassDeselectedColor = new Color(255, 255, 255, 128);
        private Color _characterLabelSelectedColor = Color.White;
        private Color _characterLabelDeselectedColor = new Color(180, 180, 180, 255);

        public SkillsScreen(InterLevelScreen interLevelScreen)
            : base(ScreenType.Skills)
        {
            _interLevelScreen = interLevelScreen;
        }

        public override void initialize()
        {
        }

        public override void loadContent()
        {
            float screenWidth = Game.window.GetView().Size.X;
            float screenHeight = Game.window.GetView().Size.Y;
            CharacterClass[] characterClasses = (CharacterClass[])Enum.GetValues(typeof(CharacterClass));

            _font = ResourceManager.getResource<Font>("immortal_font");

            _background = new RectangleShape();
            _background.FillColor = new Color(0, 0, 0, 235);
            _background.Size = new Vector2f(screenWidth, screenHeight);

            _titleFont = ResourceManager.getResource<Font>("immortal_font");
            _title = new Text("Title", _titleFont, 72);
            _title.Position = new Vector2f(Game.window.GetView().Size.X - 32f, 0f);
            _title.Color = Color.White;

            _pane = new RectangleShape();
            _pane.Texture = new Texture("resources/ui/skills_screen/pane.png");
            _pane.Size = new Vector2f(_pane.Texture.Size.X, _pane.Texture.Size.Y);
            _pane.Origin = new Vector2f(_pane.Size.X, 0);
            _pane.Position = new Vector2f(screenWidth - 32f, 90f);

            /*
            _leftArrow = new RectangleShape();
            _leftArrow.Texture = new Texture("resources/ui/skills_screen/left_arrow.png");
            _leftArrow.Size = new Vector2f(_leftArrow.Texture.Size.X, _leftArrow.Texture.Size.Y);
            _leftArrow.Position = new Vector2f(screenWidth - (32f + _pane.Size.X), _pane.Position.Y + _pane.Size.Y + 32f);

            _rightArrow = new RectangleShape();
            _rightArrow.Texture = new Texture("resources/ui/skills_screen/right_arrow.png");
            _rightArrow.Size = new Vector2f(_rightArrow.Texture.Size.X, _rightArrow.Texture.Size.Y);
            _rightArrow.Position = _leftArrow.Position + new Vector2f(_leftArrow.Size.X + 32f, 0f);
            */

            _cancelButton = new RectangleShape();
            _cancelButton.Texture = new Texture("resources/ui/skills_screen/cancel_button.png");
            _cancelButton.Size = new Vector2f(_cancelButton.Texture.Size.X, _cancelButton.Texture.Size.Y);
            _cancelButton.Position = new Vector2f(screenWidth - (32f + _cancelButton.Size.X), _pane.Position.Y + _pane.Size.Y + 32f);

            _okayButton = new RectangleShape();
            _okayButton.Texture = new Texture("resources/ui/skills_screen/okay_button.png");
            _okayButton.Size = new Vector2f(_okayButton.Texture.Size.X, _okayButton.Texture.Size.Y);
            _okayButton.Position = new Vector2f(_cancelButton.Position.X - (_okayButton.Size.X + 32f), _cancelButton.Position.Y);

            _classTextures = new List<Texture>();
            for (int i = 0; i < characterClasses.Length; i++)
            {
                _classTextures.Add(new Texture("resources/ui/class_icons/" + (characterClasses[i].ToString().ToLower() + ".png")));
            }

            // Character buttons
            _characterButtons = new List<RectangleShape>();
            _characterButtonLabels = new List<Text>();
            _characterButtonHighlights = new List<RectangleShape>();
            foreach (int entityId in SystemManager.teamSystem.playerGroup.entities)
            {
                RectangleShape shape = new RectangleShape();
                RectangleShape highlightShape = new RectangleShape();
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                Text label = new Text(characterComponent.characterClass.ToString(), _font, 32);

                shape.Texture = _classTextures[(int)characterComponent.characterClass];
                shape.Size = new Vector2f(shape.Texture.Size.X, shape.Texture.Size.Y);
                shape.Position = new Vector2f(32f, _pane.Position.Y) + new Vector2f(0f, 64f * _characterButtons.Count);

                highlightShape.OutlineColor = Color.Yellow;
                highlightShape.OutlineThickness = 3;
                highlightShape.FillColor = Color.Transparent;
                highlightShape.Size = shape.Size + new Vector2f(256f + 8f, 8f);
                highlightShape.Position = shape.Position;
                highlightShape.Origin = new Vector2f(4f, 4f);

                label.Color = Color.White;
                label.Position = shape.Position + new Vector2f(shape.Size.X, 10f);

                _characterButtons.Add(shape);
                _characterButtonLabels.Add(label);
                _characterButtonHighlights.Add(highlightShape);
            }
        }

        private bool characterPaneTestPoint(Vector2f point, int slot)
        {
            FloatRect pointRect = new FloatRect(point.X, point.Y, 1f, 1f);
            FloatRect highlightRect = _characterButtonHighlights[slot].GetGlobalBounds();
            FloatRect slotRect = new FloatRect(highlightRect.Left + 8f, highlightRect.Top + 8f, highlightRect.Width - 16f, highlightRect.Height - 16f);

            return pointRect.Intersects(slotRect);
        }

        public override void update()
        {
            Vector2f mouse = new Vector2f(Game.newMouseState.position.X, Game.newMouseState.position.Y);

            base.update();

            if (Game.newKeyState.isPressed(Keyboard.Key.Escape) && Game.oldKeyState.isReleased(Keyboard.Key.Escape))
            {
                _interLevelScreen.closeSkillsMenu();
            }

            // Restore character colors
            for (int i = 0; i < _characterButtons.Count; i++)
            {
                if (i != _selectedTeammate)
                {
                    _characterButtons[i].FillColor = _characterClassDeselectedColor;
                    _characterButtonLabels[i].Color = _characterLabelDeselectedColor;
                }
            }

            // Handle mouse input
            for (int i = 0; i < _characterButtons.Count; i++)
            {
                if (characterPaneTestPoint(mouse, i))
                {
                    _characterButtons[i].FillColor = _characterClassSelectedColor;
                    _characterButtonLabels[i].Color = _characterLabelSelectedColor;

                    if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
                    {
                        _selectedTeammate = i;
                    }
                }
            }

            // Determine whether selected character has changed
            if (_selectedTeammate != _previousSelectedTeammate)
            {
                _title.DisplayedString = EntityManager.getCharacterComponent(SystemManager.teamSystem.playerGroup.entities[_selectedTeammate]).characterClass.ToString();
                _title.Origin = new Vector2f(_title.GetLocalBounds().Width, 0f);
                _characterButtons[_selectedTeammate].FillColor = _characterClassSelectedColor;
                _characterButtonLabels[_selectedTeammate].Color = _characterLabelSelectedColor;
            }

            _previousSelectedTeammate = _selectedTeammate;
        }

        public override void draw()
        {
            Game.window.Draw(_background);
            Game.window.Draw(_title);
            Game.window.Draw(_pane);
            Game.window.Draw(_cancelButton);
            Game.window.Draw(_okayButton);

            // Character buttons
            for (int i = 0; i < _characterButtons.Count; i++)
            {
                if (i == _selectedTeammate)
                {
                    Game.window.Draw(_characterButtonHighlights[i]);
                }

                Game.window.Draw(_characterButtons[i]);
            }
            foreach (Text label in _characterButtonLabels)
            {
                Game.window.Draw(label);
            }

            base.draw();
        }
    }
}
