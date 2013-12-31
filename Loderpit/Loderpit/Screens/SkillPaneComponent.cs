using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Screens
{
    public class SkillPaneComponent : ScreenComponent
    {
        private SkillsScreen _skillsScreen;
        private Skill _skill;
        private Vector2f _position;
        private Texture _skillIcon;
        private RectangleShape _shape;
        private Text _text;
        private Font _font;
        private int _numOrbs;
        private List<RectangleShape> _unfilledOrbs;
        private List<RectangleShape> _filledOrbs;

        public SkillPaneComponent(SkillsScreen screen, Texture skillIcon, List<Texture> unfilledOrbTextures, List<Texture> filledOrbTextures, int entityId, Skill skill, Vector2f position)
            : base(screen)
        {
            _skillsScreen = screen;
            _position = position;
            _skillIcon = skillIcon;
            _skill = skill;

            _unfilledOrbs = new List<RectangleShape>();
            _filledOrbs = new List<RectangleShape>();
            _numOrbs = unfilledOrbTextures.Count;
            for (int i = 0; i < _numOrbs; i++)
            {
                Texture unfilledOrbTexture = unfilledOrbTextures[i];
                Texture filledOrbTexture = filledOrbTextures[i];
                RectangleShape unfilledShape = new RectangleShape();
                RectangleShape filledShape;

                unfilledShape.Texture = unfilledOrbTexture;
                unfilledShape.Size = new Vector2f(unfilledShape.Texture.Size.X, unfilledShape.Texture.Size.Y);
                unfilledShape.Position = position + new Vector2f(i * 39f, 138);
                filledShape = new RectangleShape(unfilledShape);
                filledShape.Texture = filledOrbTexture;

                _unfilledOrbs.Add(unfilledShape);
                _filledOrbs.Add(filledShape);
            }

            _font = ResourceManager.getResource<Font>("immortal_font");
            _text = new Text(SystemManager.skillSystem.getSkillName(skill.type), _font, 24);
            _text.Position = position;
            _text.Color = Color.White;

            _shape = new RectangleShape();
            _shape.Texture = skillIcon;
            _shape.Size = new Vector2f(skillIcon.Size.X, skillIcon.Size.Y);
            _shape.Position = position + new Vector2f(0, 32f);
        }

        public override void update()
        {
        }

        public override void draw()
        {
            Game.window.Draw(_shape);
            Game.window.Draw(_text);

            // Orbs
            for (int i = 0; i < _numOrbs; i++)
            {
                if (_skill.level >= i + 1)
                {
                    Game.window.Draw(_filledOrbs[i]);
                }
                else
                {
                    Game.window.Draw(_unfilledOrbs[i]);
                }
            }
        }
    }
}
