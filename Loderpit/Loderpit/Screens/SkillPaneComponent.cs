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
        private Vector2f _position;
        private Texture _skillIcon;
        private RectangleShape _shape;
        private Text _text;
        private Font _font;

        public SkillPaneComponent(SkillsScreen screen, Texture skillIcon, int entityId, Skill skill, Vector2f position)
            : base(screen)
        {
            _skillsScreen = screen;
            _position = position;
            _skillIcon = skillIcon;

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
        }
    }
}
