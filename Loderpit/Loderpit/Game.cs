﻿using System;
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
        private static RenderWindow _window;
        private static bool _skipDraw;
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
            SystemManager.animationSystem = new AnimationSystem();
            SystemManager.particleRenderSystem = new ParticleRenderSystem();
            SystemManager.renderSystem = new RenderSystem();
            SystemManager.obstacleSystem = new ObstacleSystem();
            SystemManager.groupSystem = new GroupSystem();
            SystemManager.skillSystem = new SkillSystem();
            SystemManager.combatSystem = new CombatSystem();
            SystemManager.aiSystem = new AISystem();
            SystemManager.interLevelSystem = new InterLevelSystem();
            SystemManager.spellSystem = new SpellSystem();
            SystemManager.explosionSystem = new ExplosionSystem();
            SystemManager.proxySystem = new ProxySystem();
            SystemManager.battleDroneSystem = new BattleDroneSystem();

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
            ResourceManager.addResource("arrow_time_icon", new Texture("resources/ui/action_icons/arrow_time.png"));
            ResourceManager.addResource("volley_icon", new Texture("resources/ui/action_icons/volley.png"));
            ResourceManager.addResource("riposte_icon", new Texture("resources/ui/action_icons/riposte.png"));
            ResourceManager.addResource("golem_stance_icon", new Texture("resources/ui/action_icons/golem_stance.png"));
            ResourceManager.addResource("frenzy_icon", new Texture("resources/ui/action_icons/frenzy.png"));
            ResourceManager.addResource("gale_force_icon", new Texture("resources/ui/action_icons/gale_force.png"));
            ResourceManager.addResource("fortification_icon", new Texture("resources/ui/action_icons/fortification.png"));
            ResourceManager.addResource("fluid_particle", new Texture("resources/particles/fluid_1.png"));
            ResourceManager.addResource("shot_trail", new Texture("resources/particles/shot_trail.png"));
            ResourceManager.addResource("character_idle_0", new Texture("resources/characters/prototype/idle_0.png"));
            ResourceManager.addResource("character_walk_left_0", new Texture("resources/characters/prototype/walk_left_0.png"));
            ResourceManager.addResource("character_walk_left_1", new Texture("resources/characters/prototype/walk_left_1.png"));
            ResourceManager.addResource("character_walk_left_2", new Texture("resources/characters/prototype/walk_left_2.png"));
            ResourceManager.addResource("character_walk_left_3", new Texture("resources/characters/prototype/walk_left_3.png"));
            ResourceManager.addResource("character_walk_left_4", new Texture("resources/characters/prototype/walk_left_4.png"));
            ResourceManager.addResource("character_walk_left_5", new Texture("resources/characters/prototype/walk_left_5.png"));
            ResourceManager.addResource("character_walk_left_6", new Texture("resources/characters/prototype/walk_left_6.png"));
            ResourceManager.addResource("character_walk_right_0", new Texture("resources/characters/prototype/walk_right_0.png"));
            ResourceManager.addResource("character_walk_right_1", new Texture("resources/characters/prototype/walk_right_1.png"));
            ResourceManager.addResource("character_walk_right_2", new Texture("resources/characters/prototype/walk_right_2.png"));
            ResourceManager.addResource("character_walk_right_3", new Texture("resources/characters/prototype/walk_right_3.png"));
            ResourceManager.addResource("character_walk_right_4", new Texture("resources/characters/prototype/walk_right_4.png"));
            ResourceManager.addResource("character_walk_right_5", new Texture("resources/characters/prototype/walk_right_5.png"));
            ResourceManager.addResource("character_walk_right_6", new Texture("resources/characters/prototype/walk_right_6.png"));
            ResourceManager.addResource("enemy_idle_0", new Texture("resources/characters/enemy/idle_0.png"));
            ResourceManager.addResource("enemy_walk_left_0", new Texture("resources/characters/enemy/walk_left_0.png"));
            ResourceManager.addResource("enemy_walk_left_1", new Texture("resources/characters/enemy/walk_left_1.png"));
            ResourceManager.addResource("enemy_walk_left_2", new Texture("resources/characters/enemy/walk_left_2.png"));
            ResourceManager.addResource("enemy_walk_left_3", new Texture("resources/characters/enemy/walk_left_3.png"));
            ResourceManager.addResource("enemy_walk_left_4", new Texture("resources/characters/enemy/walk_left_4.png"));
            ResourceManager.addResource("enemy_walk_left_5", new Texture("resources/characters/enemy/walk_left_5.png"));
            ResourceManager.addResource("enemy_walk_left_6", new Texture("resources/characters/enemy/walk_left_6.png"));
            ResourceManager.addResource("enemy_walk_left_7", new Texture("resources/characters/enemy/walk_left_7.png"));
            ResourceManager.addResource("enemy_walk_right_0", new Texture("resources/characters/enemy/walk_right_0.png"));
            ResourceManager.addResource("enemy_walk_right_1", new Texture("resources/characters/enemy/walk_right_1.png"));
            ResourceManager.addResource("enemy_walk_right_2", new Texture("resources/characters/enemy/walk_right_2.png"));
            ResourceManager.addResource("enemy_walk_right_3", new Texture("resources/characters/enemy/walk_right_3.png"));
            ResourceManager.addResource("enemy_walk_right_4", new Texture("resources/characters/enemy/walk_right_4.png"));
            ResourceManager.addResource("enemy_walk_right_5", new Texture("resources/characters/enemy/walk_right_5.png"));
            ResourceManager.addResource("enemy_walk_right_6", new Texture("resources/characters/enemy/walk_right_6.png"));
            ResourceManager.addResource("enemy_walk_right_7", new Texture("resources/characters/enemy/walk_right_7.png"));
            ResourceManager.addResource("drone_idle_0", new Texture("resources/characters/drone/idle_0.png"));
            ResourceManager.addResource("servo_bot_idle_0", new Texture("resources/characters/servo_bot/idle_0.png"));
            ResourceManager.addResource("rope_particle", new Texture("resources/particles/rope_1.png"));
            ResourceManager.addResource("bridge_normal_0", new Texture("resources/bridges/normal/bridge_0.png"));
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
            _skipDraw = true;
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

            SystemManager.levelSystem.generateLevel(2);
            playerGroupId = EntityFactory.createPlayerGroup(characterClasses);
            SystemManager.teamSystem.playerGroup = EntityManager.getGroupComponent(playerGroupId);
            SystemManager.skillSystem.initializeSkills();
            ScreenManager.addScreen(new LevelScreen());
            _state = GameState.Level;
            _skipDraw = true;
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
            _skipDraw = true;
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
                SystemManager.aiSystem.update();
                SystemManager.characterSystem.update();
                SystemManager.statSystem.update();
                SystemManager.animationSystem.update();
                SystemManager.renderSystem.update();
                SystemManager.particleRenderSystem.update();
                SystemManager.skillSystem.update();
                SystemManager.battleDroneSystem.update();
                SystemManager.spellSystem.update();
                SystemManager.combatSystem.update();
                SystemManager.explosionSystem.update();
                SystemManager.proxySystem.update();
            }

            Game.singleStep = false;
            updateFPS();
        }

        // Draw when in level state
        private void drawLevelState()
        {
            // Switch to world view
            _window.SetView(SystemManager.cameraSystem.worldView);

            // Draw particle effects
            SystemManager.particleRenderSystem.draw();

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
            if (_skipDraw)
            {
                _skipDraw = false;
                return;
            }

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
