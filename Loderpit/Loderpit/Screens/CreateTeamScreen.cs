using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    using Key = Keyboard.Key;

    public class CreateTeamScreen : Screen
    {
        private Font _font;
        private Texture _upArrowTexture;
        private Texture _downArrowTexture;
        private Texture _classSelectorTexture;
        private List<Texture> _classTextures;
        private List<ClassSelectorComponent> _classSelectorComponents;
        private Text _instructions;
        private int _selectedSlot;

        public CreateTeamScreen()
            : base(ScreenType.CreateTeam)
        {
        }

        // Load assets
        public override void loadContent()
        {
            _font = ResourceManager.getResource<Font>("gooddog_font");

            _upArrowTexture = new Texture("resources/ui/create_team_screen/up_arrow.png");
            _downArrowTexture = new Texture("resources/ui/create_team_screen/down_arrow.png");
            _classSelectorTexture = new Texture("resources/ui/create_team_screen/class_selector.png");

            _classTextures = new List<Texture>();
            for (int i = 0; i < Enum.GetValues(typeof(CharacterClass)).Length; i++)
            {
                _classTextures.Add(new Texture("resources/ui/class_icons/" + ((CharacterClass)i).ToString().ToLower() + ".png"));
            }

            _instructions = new Text("Use WASD and Enter to create your team.", _font, 14);
            _instructions.Position = new Vector2f(16, 16);
        }

        // Initialize assets and components
        public override void initialize()
        {
            int count = 4;
            float offsetWidth = 72f;
            float totalWidth = (count - 1) * offsetWidth;
            Vector2f position = new Vector2f(Game.window.Size.X, Game.window.Size.Y) * 0.5f - new Vector2f(totalWidth * 0.5f, 0f);

            _classSelectorComponents = new List<ClassSelectorComponent>();
            for (int i = 0; i < count; i++)
            {
                Vector2f offset = new Vector2f(72f, 0f) * i;
                ClassSelectorComponent classSelectorComponent = new ClassSelectorComponent(this, _upArrowTexture, _downArrowTexture, _classSelectorTexture, _classTextures, position + offset);

                addScreenComponent(classSelectorComponent);
                _classSelectorComponents.Add(classSelectorComponent);
            }
        }

        // Create team
        private void createTeam()
        {
            int playerGroupId;
            List<CharacterClass> chosenClasses = new List<CharacterClass>();
            int playerUid = PlayerDataManager.getUnusedPlayerUid();

            // Construct temporary game data
            foreach (ClassSelectorComponent component in _classSelectorComponents)
            {
                chosenClasses.Add(component.selectedClass);
            }

            playerGroupId = EntityFactory.createPlayerGroup(chosenClasses);
            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(playerGroupId);

            // Save temporary player structures
            PlayerDataManager.savePlayerData(playerUid);

            // Clear temporary game data
            EntityManager.destroyAllEntities();

            Game.endCreateTeamState();
            Game.startInterLevelState(playerUid);
        }

        // Select previous slot
        private void selectPreviousSlot()
        {
            _selectedSlot = _selectedSlot - 1 < 0 ? _classSelectorComponents.Count - 1 : _selectedSlot - 1;
        }

        // Select next slot
        private void selectNextSlot()
        {
            _selectedSlot = _selectedSlot + 1 > _classSelectorComponents.Count - 1 ? 0 : _selectedSlot + 1;
        }

        // Select next class
        private void selectNextClass()
        {
            int numClass = Enum.GetValues(typeof(CharacterClass)).Length;
            ClassSelectorComponent component = _classSelectorComponents[_selectedSlot];
            int nextClassInt = (int)component.selectedClass + 1;

            component.selectedClass = nextClassInt > numClass - 1 ? (CharacterClass)0 : (CharacterClass)nextClassInt;
        }

        // Select previous class
        private void selectPreviousClass()
        {
            int numClass = Enum.GetValues(typeof(CharacterClass)).Length;
            ClassSelectorComponent component = _classSelectorComponents[_selectedSlot];
            int previousClassInt = (int)component.selectedClass - 1;

            component.selectedClass = previousClassInt < 0 ? (CharacterClass)(numClass - 1) : (CharacterClass)previousClassInt;
        }

        // Update
        public override void update()
        {
            // Handle input
            if (Game.newKeyState.isPressed(Key.A) && Game.oldKeyState.isReleased(Key.A))
            {
                selectPreviousSlot();
            }
            if (Game.newKeyState.isPressed(Key.D) && Game.oldKeyState.isReleased(Key.D))
            {
                selectNextSlot();
            }
            if (Game.newKeyState.isPressed(Key.W) && Game.oldKeyState.isReleased(Key.W))
            {
                selectNextClass();
            }
            if (Game.newKeyState.isPressed(Key.S) && Game.oldKeyState.isReleased(Key.S))
            {
                selectPreviousClass();
            }
            if (Game.newKeyState.isPressed(Key.Return) && Game.oldKeyState.isReleased(Key.Return))
            {
                createTeam();
            }

            // Update selected component
            for (int i = 0; i < _classSelectorComponents.Count; i++)
            {
                ClassSelectorComponent component = _classSelectorComponents[i];

                component.selected = i == _selectedSlot;
            }

            base.update();
        }

        // Draw
        public override void draw()
        {
            Game.window.Draw(_instructions);

            base.draw();
        }
    }
}
