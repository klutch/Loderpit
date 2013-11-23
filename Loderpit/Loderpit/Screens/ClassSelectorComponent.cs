using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Loderpit.Screens
{
    public class ClassSelectorComponent : ScreenComponent
    {
        private Vector2f _position;
        private bool _selected;
        private RectangleShape _upArrow;
        private RectangleShape _downArrow;
        private RectangleShape _classSelector;
        private RectangleShape _classIcon;
        private List<Texture> _classTextures;
        private CharacterClass _selectedClass;

        public bool selected { get { return _selected; } set { _selected = value; } }
        public CharacterClass selectedClass { get { return _selectedClass; } set { _selectedClass = value; } }

        public ClassSelectorComponent(Screen screen, Texture upArrowTexture, Texture downArrowTexture, Texture classSelectorTexture, List<Texture> classTextures, Vector2f position)
            : base(screen)
        {
            _position = position;
            _classTextures = classTextures;

            _upArrow = new RectangleShape();
            _upArrow.Position = position + new Vector2f(0f, -42f);
            _upArrow.Origin = new Vector2f(8f, 8f);
            _upArrow.Texture = upArrowTexture;
            _upArrow.Size = new Vector2f(16f, 16f);

            _classSelector = new RectangleShape();
            _classSelector.Position = position;
            _classSelector.Origin = new Vector2f(32f, 32f);
            _classSelector.Texture = classSelectorTexture;
            _classSelector.Size = new Vector2f(64f, 64f);

            _classIcon = new RectangleShape();
            _classIcon.Position = position;
            _classIcon.Origin = new Vector2f(32f, 32f);
            _classIcon.Texture = _classTextures[(int)_selectedClass];
            _classIcon.Size = new Vector2f(64f, 64f);

            _downArrow = new RectangleShape();
            _downArrow.Position = position + new Vector2f(0f, 42f);
            _downArrow.Origin = new Vector2f(8f, 8f);
            _downArrow.Texture = downArrowTexture;
            _downArrow.Size = new Vector2f(16f, 16f);
        }

        public override void update()
        {
            _classIcon.Texture = _classTextures[(int)_selectedClass];
        }

        public override void draw()
        {
            Game.window.Draw(_classIcon);

            if (_selected)
            {
                Game.window.Draw(_upArrow);
                Game.window.Draw(_classSelector);
                Game.window.Draw(_downArrow);
            }
        }
    }
}
