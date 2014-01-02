using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;
using Loderpit.Systems;

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
        private Color _characterLabelDeselectedColor = new Color(120, 120, 120, 255);
        private List<Texture> _unfilledLevelOrbs;
        private List<Texture> _filledLevelOrbs;
        private Texture _skillIcon;
        private List<SkillPaneComponent> _skillPaneComponents;
        private Dictionary<int, Dictionary<SkillType, int>> _skillsBought;
        private int _numSkillsBought;
        private List<RectangleShape> _playerSkillOrbs;
        private List<RectangleShape> _playerSpentSkillOrbs;
        private int _numSkillOrbs;
        private Text _skillOrbLabel;

        public SkillsScreen(InterLevelScreen interLevelScreen)
            : base(ScreenType.Skills)
        {
            _interLevelScreen = interLevelScreen;
        }

        public override void loadContent()
        {
            float screenWidth = Game.window.GetView().Size.X;
            float screenHeight = Game.window.GetView().Size.Y;
            CharacterClass[] characterClasses = (CharacterClass[])Enum.GetValues(typeof(CharacterClass));
            Random rng = new Random();

            _numSkillOrbs = SystemManager.teamSystem.skillOrbs;
            _skillPaneComponents = new List<SkillPaneComponent>();
            _skillsBought = new Dictionary<int, Dictionary<SkillType, int>>();
            _playerSkillOrbs = new List<RectangleShape>(_numSkillOrbs);
            _playerSpentSkillOrbs = new List<RectangleShape>(_numSkillOrbs);

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
            _pane.Position = new Vector2f(screenWidth - (32f + _pane.Texture.Size.X), 90f);

            _skillIcon = new Texture("resources/ui/skills_screen/skill_icon.png");

            _unfilledLevelOrbs = new List<Texture>();
            _filledLevelOrbs = new List<Texture>();
            for (int i = 0; i < 6; i++)
            {
                _filledLevelOrbs.Add(new Texture(String.Format("resources/ui/skills_screen/filled_level_orb_{0}.png", i + 1)));
                _unfilledLevelOrbs.Add(new Texture(String.Format("resources/ui/skills_screen/level_orb_{0}.png", i + 1)));
            }

            _okayButton = new RectangleShape();
            _okayButton.Texture = new Texture("resources/ui/skills_screen/okay_button.png");
            _okayButton.Size = new Vector2f(_okayButton.Texture.Size.X, _okayButton.Texture.Size.Y);
            _okayButton.Position = new Vector2f(screenWidth - (32f + _okayButton.Size.X), _pane.Position.Y + _pane.Size.Y + 32f);
            _okayButton.FillColor = Color.Green;

            _cancelButton = new RectangleShape();
            _cancelButton.Texture = new Texture("resources/ui/skills_screen/cancel_button.png");
            _cancelButton.Size = new Vector2f(_cancelButton.Texture.Size.X, _cancelButton.Texture.Size.Y);
            _cancelButton.Position = new Vector2f(_okayButton.Position.X - (_cancelButton.Size.X + 32f), _okayButton.Position.Y);
            _cancelButton.FillColor = Color.Red;

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

            for (int i = 0; i < _numSkillOrbs; i++)
            {
                RectangleShape orb = new RectangleShape();
                RectangleShape spentOrb;

                orb.Texture = _filledLevelOrbs[rng.Next(_filledLevelOrbs.Count)];
                orb.Size = new Vector2f(orb.Texture.Size.X, orb.Texture.Size.Y);
                orb.Position = new Vector2f(32f, Game.window.GetView().Size.Y - 48f) + new Vector2f(32f * i, 0f);

                spentOrb = new RectangleShape(orb);
                spentOrb.Texture = _unfilledLevelOrbs[rng.Next(_unfilledLevelOrbs.Count)];
                spentOrb.FillColor = new Color(120, 120, 120, 255);

                _playerSkillOrbs.Add(orb);
                _playerSpentSkillOrbs.Add(spentOrb);
            }

            _skillOrbLabel = new Text("Skill Orbs", _font, 32);
            _skillOrbLabel.Position = new Vector2f(32f, Game.window.GetView().Size.Y - 104f);
            _skillOrbLabel.Color = Color.White;
        }

        public override void initialize()
        {
        }

        private bool characterPaneTestPoint(Vector2f point, int slot)
        {
            FloatRect pointRect = new FloatRect(point.X, point.Y, 1f, 1f);
            FloatRect highlightRect = _characterButtonHighlights[slot].GetGlobalBounds();
            FloatRect slotRect = new FloatRect(highlightRect.Left + 8f, highlightRect.Top + 8f, highlightRect.Width - 16f, highlightRect.Height - 16f);

            return pointRect.Intersects(slotRect);
        }

        private List<Skill> getAllUpgradableSkills(int entityId)
        {
            CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
            SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
            List<Skill> allSkills = EntityFactory.getAllSkills(entityId, characterComponent.characterClass);
            List<Skill> results = new List<Skill>(skillsComponent.upgradableSkills);

            foreach (Skill skill in allSkills)
            {
                bool alreadyHasSkill = false;

                if (!skill.isUpgradable)
                {
                    continue;
                }

                foreach (Skill existingSkill in results)
                {
                    if (existingSkill.type == skill.type)
                    {
                        alreadyHasSkill = true;
                        break;
                    }
                }

                if (!alreadyHasSkill)
                {
                    results.Add(skill);
                }
            }

            return results;
        }

        private void onSelectedTeammateChange()
        {
            int entityId = SystemManager.teamSystem.playerGroup.entities[_selectedTeammate];
            CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
            //SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
            List<Skill> allUpgradableSkills = getAllUpgradableSkills(entityId);
            Vector2f panePosition = _pane.Position;

            _title.DisplayedString = characterComponent.characterClass.ToString();
            _title.Origin = new Vector2f(_title.GetLocalBounds().Width, 0f);
            _characterButtons[_selectedTeammate].FillColor = _characterClassSelectedColor;
            _characterButtonLabels[_selectedTeammate].Color = _characterLabelSelectedColor;

            // Remove previous skill components
            foreach (SkillPaneComponent skillPaneComponent in _skillPaneComponents)
            {
                removeScreenComponent(skillPaneComponent);
            }
            _skillPaneComponents.Clear();

            // Add current skills
            for (int i = 0; i < allUpgradableSkills.Count; i++)
            {
                Skill skill = allUpgradableSkills[i];
                int gridX = (i % 3);
                int gridY = i / 3;
                Vector2f innerPosition = new Vector2f(gridX * (_skillIcon.Size.X + 32), gridY * 190) + new Vector2f(32, 32);
                SkillPaneComponent skillPaneComponent;
                int additionalLevelValue = 0;

                if (_skillsBought.ContainsKey(entityId))
                {
                    if (_skillsBought[entityId].ContainsKey(skill.type))
                    {
                        additionalLevelValue += _skillsBought[entityId][skill.type];
                    }
                }

                skillPaneComponent = new SkillPaneComponent(this, _skillIcon, _unfilledLevelOrbs, _filledLevelOrbs, entityId, skill, panePosition + innerPosition, additionalLevelValue);

                addScreenComponent(skillPaneComponent);
                _skillPaneComponents.Add(skillPaneComponent);
            }
        }

        public bool trySpendSkillOrb(Skill skill)
        {
            int entityId = skill.entityId;
            int newNumSkillsBought = _numSkillsBought + 1;

            // Stop if no orbs left
            if (newNumSkillsBought > _numSkillOrbs)
            {
                return false;
            }

            // Stop if at max skill level
            if (newNumSkillsBought > SkillSystem.MAX_SKILL_LEVEL)
            {
                return false;
            }

            // Remember skill to buy
            if (!_skillsBought.ContainsKey(entityId))
            {
                _skillsBought.Add(entityId, new Dictionary<SkillType, int>());
            }
            if (!_skillsBought[entityId].ContainsKey(skill.type))
            {
                _skillsBought[entityId].Add(skill.type, 0);
            }
            _skillsBought[entityId][skill.type]++;
            _numSkillsBought++;
            return true;
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

            // Handle mouse input for character buttons
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

            // Handle mouse input for okay/cancel buttons
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                if (_cancelButton.GetGlobalBounds().Intersects(Game.newMouseState.rectangle))
                {
                    _interLevelScreen.closeSkillsMenu();
                }
            }

            // Determine whether selected character has changed
            if (_selectedTeammate != _previousSelectedTeammate)
            {
                onSelectedTeammateChange();
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

            // Player skill orbs
            Game.window.Draw(_skillOrbLabel);
            for (int i = 0; i < _numSkillOrbs; i++)
            {
                int spentCutoff = _numSkillOrbs - _numSkillsBought;

                if (i < spentCutoff)
                {
                    Game.window.Draw(_playerSkillOrbs[i]);
                }
                else
                {
                    Game.window.Draw(_playerSpentSkillOrbs[i]);
                }
            }

            base.draw();
        }
    }
}
