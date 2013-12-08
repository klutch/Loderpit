using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Loderpit.Formations;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Systems;
using Loderpit.Screens;

namespace Loderpit
{
    using Key = Keyboard.Key;

    /*   A GameState should be any state of the game where the game's update and draw methods are called. For example,
     * if the game transitions from the main menu to a loaded level without a loading screen, then there's no need for
     * a LoadingLevel state. However, if there is a loading screen that needs to be updated/drawn, there should be a
     * LoadingLevel state.
     */
    public enum GameState
    {
        CreateTeam,
        Level,
        InterLevel
    }

    public class Game : IDisposable
    {
        public const float DT = 1f / 60f;
        private static RenderWindow _window;
        public static bool inFocus = true;
        public static KeyboardState newKeyState;
        public static KeyboardState oldKeyState;
        public static MouseState newMouseState;
        public static MouseState oldMouseState;
        public static Vector2f sfmlWorldMouse;
        public static Vector2f screenMouse;
        public static Vector2 worldMouse;
        public static bool paused;
        public static bool singleStep;
        private static GameState _state;
        private Stopwatch _stopwatch;
        private int _fps;
        private Text _fpsText;
        private FixedMouseJoint _mouseJoint;

        public static RenderWindow window { get { return _window; } }

        public Game()
        {
            _window = new RenderWindow(new VideoMode(1280, 720), "Loderpit");
            _stopwatch = new Stopwatch();
            _window.Closed += new EventHandler(_window_Closed);
            _window.GainedFocus += new EventHandler(_window_GainedFocus);
            _window.LostFocus += new EventHandler(_window_LostFocus);
            _window.SetVerticalSyncEnabled(true);

            // Load content
            loadContent();

            // Create systems
            SystemManager.physicsSystem = new PhysicsSystem();
            SystemManager.statSystem = new StatSystem();
            SystemManager.levelSystem = new LevelSystem();
            SystemManager.teamSystem = new TeamSystem();
            SystemManager.cameraSystem = new CameraSystem();
            SystemManager.characterSystem = new CharacterSystem();
            SystemManager.renderSystem = new RenderSystem();
            SystemManager.obstacleSystem = new ObstacleSystem();
            SystemManager.groupSystem = new GroupSystem();
            SystemManager.skillSystem = new SkillSystem();
            SystemManager.combatSystem = new CombatSystem();
            SystemManager.enemyAISystem = new EnemyAISystem();
            SystemManager.interLevelSystem = new InterLevelSystem();
            SystemManager.spellSystem = new SpellSystem();
            SystemManager.explosionSystem = new ExplosionSystem();

            // Open create team screen
            startCreateTeamState();
        }

        // Window event handlers
        private void _window_LostFocus(object sender, EventArgs e)
        {
            inFocus = false;
        }
        private void _window_GainedFocus(object sender, EventArgs e)
        {
            inFocus = true;
        }
        private void _window_Closed(object sender, EventArgs e)
        {
            _window.Close();
        }

        // Dispose
        public void Dispose()
        {
        }

        // Load assets
        public void loadContent()
        {
            // Permanent/global
            ResourceManager.addResource("font", new Font("resources/courbd.ttf"));
            ResourceManager.addResource("sword_icon", new Texture("resources/ui/action_icons/sword.png"));
            ResourceManager.addResource("bow_icon", new Texture("resources/ui/action_icons/bow.png"));
            ResourceManager.addResource("wand_icon", new Texture("resources/ui/action_icons/wand.png"));
            ResourceManager.addResource("rope_icon", new Texture("resources/ui/action_icons/rope.png"));
            ResourceManager.addResource("bridge_icon", new Texture("resources/ui/action_icons/bridge.png"));
            ResourceManager.addResource("power_shot_icon", new Texture("resources/ui/action_icons/power_shot.png"));
            ResourceManager.addResource("power_swing_icon", new Texture("resources/ui/action_icons/power_swing.png"));
            ResourceManager.addResource("fireball_icon", new Texture("resources/ui/action_icons/fireball.png"));
            ResourceManager.addResource("shield_bash_icon", new Texture("resources/ui/action_icons/bash.png"));
            ResourceManager.addResource("healing_blast_icon", new Texture("resources/ui/action_icons/healing_blast.png"));
            ResourceManager.addResource("proximity_mine_icon", new Texture("resources/ui/action_icons/proximity_mine.png"));
            ResourceManager.addResource("fatality_icon", new Texture("resources/ui/action_icons/fatality.png"));
            ResourceManager.addResource("infusion_icon", new Texture("resources/ui/action_icons/infusion.png"));
            ResourceManager.addResource("rain_of_fire_icon", new Texture("resources/ui/action_icons/rain_of_fire.png"));
            ResourceManager.addResource("dispel_icon", new Texture("resources/ui/action_icons/dispel.png"));
            ResourceManager.addResource("reticle", new Texture("resources/ui/reticle.png"));

            // Debug
            _fpsText = new Text("FPS:", ResourceManager.getResource<Font>("font"), 14);
            _fpsText.Color = Color.Red;
            _fpsText.Position = new Vector2f(16, 16);
        }

        // Start create team state
        public static void startCreateTeamState()
        {
            ScreenManager.addScreen(new CreateTeamScreen());
            _state = GameState.CreateTeam;
        }

        // End create team state
        public static void endCreateTeamState()
        {
            ScreenManager.removeScreen(ScreenType.CreateTeam);
        }

        // Start level state -- TODO: Load data instead of having it supplied
        public static void startLevelState(List<CharacterClass> characterClasses)
        {
            int playerGroupId;

            SystemManager.levelSystem.generateLevel(12);
            playerGroupId = EntityFactory.createPlayerGroup(characterClasses);
            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(playerGroupId);
            SystemManager.skillSystem.initializeSkills();
            ScreenManager.addScreen(new LevelScreen());
            _state = GameState.Level;
        }

        // End level state
        public static void endLevelState()
        {
            SystemManager.levelSystem.unload();
            SystemManager.teamSystem.playerGroup = null;
            ScreenManager.removeScreen(ScreenType.Level);
        }

        // Start inter-level state -- TODO: Load data instead of having it supplied
        public static void startInterLevelState(List<CharacterClass> characterClasses)
        {
            int playerGroupId;

            SystemManager.interLevelSystem.load();
            playerGroupId = EntityFactory.createPlayerGroup(characterClasses);
            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(playerGroupId);
            ScreenManager.addScreen(new InterLevelScreen());
            _state = GameState.InterLevel;
        }

        // End inter-level state
        public static void endInterLevelState()
        {
            SystemManager.interLevelSystem.unload();
            SystemManager.teamSystem.playerGroup = null;
            ScreenManager.removeScreen(ScreenType.InterLevel);
        }

        // Game loop
        public void run()
        {
            float frameTime = 0f;
            int frameCount = 0;

            _stopwatch.Start();
            while (_window.IsOpen())
            {
                float currentTime = (float)_stopwatch.Elapsed.TotalSeconds;

                frameTime += currentTime;
                _stopwatch.Restart();

                update();
                draw();
                frameCount++;

                if (frameTime >= 1)
                {
                    _fps = frameCount;
                    frameCount = 0;
                    frameTime = 0;
                }
            }
        }

        // Read input
        private void readInput()
        {
            oldKeyState = newKeyState;
            newKeyState = KeyboardState.get();
            oldMouseState = newMouseState;
            newMouseState = MouseState.get();
        }

        // Update FPS label (calculated in run())
        private void updateFPS()
        {
            _fpsText.DisplayedString = _fps.ToString();
        }

        // Update when in create team state
        private void updateCreateTeamState()
        {
            readInput();
            ScreenManager.update();
        }

        // Draw when in create team state
        private void drawCreateTeamState()
        {
            ScreenManager.draw();
        }

        // Update when in level state
        private void updateLevelState()
        {
            Vector2i screenMousePosition = _window.InternalGetMousePosition();
            List<int> entities = SystemManager.teamSystem.getTeamEntities();

            readInput();
            screenMouse = new Vector2f(screenMousePosition.X, screenMousePosition.Y);
            sfmlWorldMouse = _window.MapPixelToCoords(screenMousePosition, SystemManager.cameraSystem.worldView);
            worldMouse = new Vector2(sfmlWorldMouse.X, sfmlWorldMouse.Y);

            // Debug input
            if (newKeyState.isPressed(Key.F3) && oldKeyState.isReleased(Key.F3))
            {
                Game.paused = !Game.paused;
            }
            if (Game.paused && newKeyState.isPressed(Key.F1) && oldKeyState.isReleased(Key.F1))
            {
                Game.singleStep = true;
            }

            // Normal update logic
            if (!Game.paused || Game.singleStep)
            {
                if (inFocus)
                {
                    if (newMouseState.isRightButtonPressed && !oldMouseState.isRightButtonPressed)
                    {
                        Fixture fixture = SystemManager.physicsSystem.world.TestPoint(worldMouse);

                        if (fixture != null)
                        {
                            _mouseJoint = JointFactory.CreateFixedMouseJoint(SystemManager.physicsSystem.world, fixture.Body, worldMouse);
                        }
                    }
                    else if (newMouseState.isRightButtonPressed && oldMouseState.isRightButtonPressed)
                    {
                        if (_mouseJoint != null)
                        {
                            _mouseJoint.WorldAnchorB = worldMouse;
                        }
                    }
                    else if (!newMouseState.isRightButtonPressed && oldMouseState.isRightButtonPressed)
                    {
                        if (_mouseJoint != null)
                        {
                            SystemManager.physicsSystem.world.RemoveJoint(_mouseJoint);
                            _mouseJoint = null;
                        }
                    }
                }

                ScreenManager.update();
                SystemManager.physicsSystem.update();
                SystemManager.levelSystem.update();
                SystemManager.teamSystem.update();
                SystemManager.groupSystem.update();
                SystemManager.obstacleSystem.update();
                SystemManager.cameraSystem.update();
                SystemManager.enemyAISystem.update();
                SystemManager.characterSystem.update();
                SystemManager.statSystem.update();
                SystemManager.renderSystem.update();
                SystemManager.skillSystem.update();
                SystemManager.spellSystem.update();
                SystemManager.combatSystem.update();
                SystemManager.explosionSystem.update();
            }

            Game.singleStep = false;
            updateFPS();
        }

        // Draw when in level state
        private void drawLevelState()
        {
            // Switch to world view
            _window.SetView(SystemManager.cameraSystem.worldView);

            // Draw render system
            SystemManager.renderSystem.draw();

            // Restore screen view
            _window.SetView(_window.DefaultView);

            // Draw render system again, now that the default view has been restored
            SystemManager.renderSystem.drawUsingScreenCoords();

            // Screens
            ScreenManager.draw();

            // FPS
            _window.Draw(_fpsText);
        }

        // Update inter-level state
        private void updateInterLevelState()
        {
            readInput();
            SystemManager.physicsSystem.update();
            SystemManager.characterSystem.update();
            SystemManager.interLevelSystem.update();
            SystemManager.cameraSystem.update();
            SystemManager.renderSystem.update();
            ScreenManager.update();
            updateFPS();
        }

        // Draw inter-level state
        public void drawInterLevelState()
        {
            // Switch to world view
            _window.SetView(SystemManager.cameraSystem.worldView);

            // Draw render system
            SystemManager.renderSystem.draw();

            // Restore screen view
            _window.SetView(_window.DefaultView);

            // Draw render system again, now that the default view has been restored
            SystemManager.renderSystem.drawUsingScreenCoords();

            // Screens
            ScreenManager.draw();

            // FPS
            _window.Draw(_fpsText);
        }

        // Main update method
        public void update()
        {
            _window.DispatchEvents();

            if (_state == GameState.CreateTeam)
            {
                updateCreateTeamState();
            }
            else if (_state == GameState.Level)
            {
                updateLevelState();
            }
            else if (_state == GameState.InterLevel)
            {
                updateInterLevelState();
            }
        }

        // Main draw method
        public void draw()
        {
            _window.Clear();

            if (_state == GameState.CreateTeam)
            {
                drawCreateTeamState();
            }
            else if (_state == GameState.Level)
            {
                drawLevelState();
            }
            else if (_state == GameState.InterLevel)
            {
                drawInterLevelState();
            }

            _window.Display();
        }
    }
}
