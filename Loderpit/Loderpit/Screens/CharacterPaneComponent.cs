using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;
using Loderpit.Components;
using Loderpit.Skills;

namespace Loderpit.Screens
{
    public class CharacterPaneComponent : ScreenComponent
    {
        public const int WIDTH = 180;
        public const int HEIGHT = 90;
        public const int MAX_SKILL_ICONS = 8;
        private int _teamPosition;
        private Font _font;
        private Text _classLabel;
        private RectangleShape _backgroundShape;
        private Vector2f _position;
        private Color _backgroundColor;
        private Color _selectedOutlineColor;
        private Color _deselectedOutlineColor;
        private int _entityId;
        private CharacterComponent _characterComponent;
        private StatsComponent _statsComponent;
        private RectangleShape[] _skillIconShapes;
        private RectangleShape _hpBarBackground;
        private RectangleShape _hpBarForeground;
        private int _skillCount;

        public CharacterPaneComponent(Screen screen, Font font, int teamPosition)
            : base(screen)
        {
            _font = font;
            _teamPosition = teamPosition;
            _skillIconShapes = new RectangleShape[MAX_SKILL_ICONS];

            for (int i = 0; i < MAX_SKILL_ICONS; i++)
            {
                _skillIconShapes[i] = new RectangleShape();
                _skillIconShapes[i].Size = new Vector2f(32, 32);
            }

            _position = new Vector2f(16 + (WIDTH + 16) * teamPosition, 610);
            _backgroundColor = new Color(50, 50, 50, 128);
            _selectedOutlineColor = new Color(255, 255, 50);
            _deselectedOutlineColor = new Color(0, 0, 0);

            _backgroundShape = new RectangleShape(new Vector2f(WIDTH, HEIGHT));
            _backgroundShape.FillColor = _backgroundColor;
            _backgroundShape.OutlineColor = _deselectedOutlineColor;
            _backgroundShape.OutlineThickness = 2f;
            _backgroundShape.Position = _position;

            _classLabel = new Text("", _font, 14);
            _classLabel.Position = _position + new Vector2f(8f, 8f);
            _classLabel.Color = Color.White;

            _hpBarBackground = new RectangleShape();
            _hpBarBackground.Position = _position + new Vector2f(8f, 32f) + new Vector2f(-1f, -1f);
            _hpBarBackground.Size = new Vector2f((WIDTH - 16f) + 2f, 6f);
            _hpBarBackground.FillColor = Color.Black;

            _hpBarForeground = new RectangleShape();
            _hpBarForeground.Position = _position + new Vector2f(8f, 32f);
            _hpBarForeground.Size = new Vector2f(WIDTH - 16f, 4f);
            _hpBarForeground.FillColor = Color.Green;
        }

        private void updateCharacterInformation()
        {
            _classLabel.DisplayedString = _characterComponent.characterClass.ToString();
            _backgroundShape.OutlineColor = SystemManager.teamSystem.selectedTeammate == _teamPosition ? _selectedOutlineColor : _deselectedOutlineColor;
        }

        private void updateHealthBar()
        {
            float fullWidth = WIDTH - 16f;
            float percentHp = (float)_statsComponent.currentHp / (float)SystemManager.statSystem.getMaxHp(_entityId);

            _hpBarForeground.Size = new Vector2f(fullWidth * percentHp, _hpBarForeground.Size.Y);
        }

        private void drawIcon(string resourceId)
        {
            RectangleShape shape = _skillIconShapes[_skillCount];

            shape.Position = _position + new Vector2f(4f, 54f) + new Vector2f(36f, 0f) * _skillCount;
            shape.Texture = ResourceManager.getResource<Texture>(resourceId);
            Game.window.Draw(shape);

            _skillCount++;
        }

        public override void update()
        {
            _entityId = SystemManager.teamSystem.getTeammateEntityId(_teamPosition);
            _skillCount = 0;

            if (_entityId != -1)
            {
                // Update entity/component references
                _characterComponent = EntityManager.getCharacterComponent(_entityId);
                _statsComponent = EntityManager.getStatsComponent(_entityId);

                // Update character information
                updateCharacterInformation();

                // Update health bar
                updateHealthBar();
            }
        }

        public override void draw()
        {
            Game.window.Draw(_backgroundShape);
            Game.window.Draw(_classLabel);

            // Draw hp bar
            Game.window.Draw(_hpBarBackground);
            Game.window.Draw(_hpBarForeground);

            // Draw skill icons
            if (_entityId != -1)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(_entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    switch (skill.type)
                    {
                        // Common
                        case SkillType.MeleeAttack: 
                            drawIcon("sword_icon");
                            break;

                        case SkillType.RangedAttack:
                            drawIcon((skill as RangedAttackSkill).textureResourceId);
                            break;

                        // Engineer
                        case SkillType.ThrowRope:
                            drawIcon("rope_icon");
                            break;

                        case SkillType.BuildBridge:
                            drawIcon("bridge_icon");
                            break;

                        // Archer
                        case SkillType.PowerShot:
                            drawIcon("power_shot_icon");
                            break;

                        // Fighter
                        case SkillType.PowerSwing:
                            drawIcon("power_swing_icon");
                            break;
                    }
                }
            }
        }
    }
}
