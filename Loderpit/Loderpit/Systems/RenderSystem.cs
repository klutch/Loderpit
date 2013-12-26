using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Loderpit.Backgrounds;
using Loderpit.Managers;
using Loderpit.Components;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class RenderSystem : ISystem
    {
        private const int MAX_HP_BARS = 1000;
        private const int MAX_ROPE_KNOTS = 10000;
        private const int MAX_BRIDGE_SEGMENTS = 2000;
        private const float HP_BAR_WIDTH = 30f;
        private const float HP_BAR_HEIGHT = 4f;
        private DebugView _debugView;
        private RectangleShape _buildBridgeShape;
        private RectangleShape _reticleShape;
        private Font _font;
        private Text _actionLabel;
        private RectangleShape[] _hpBarBackgrounds;
        private RectangleShape[] _hpBarForegrounds;
        private int _usedHpBarCount;
        private Dictionary<AnimationCategory, Dictionary<AnimationType, List<Texture>>> _animations;
        private RectangleShape[] _ropeKnotShapes;
        private RectangleShape[] _bridgeSegmentShapes;
        private List<int> _activeRopeKnotShapes;
        private List<int> _activeBridgeSegmentShapes;
        private Texture _ropeKnotTexture;
        private Texture _bridgeSegmentTexture;
        private Dictionary<BackgroundType, Background> _backgrounds;
        private BackgroundType _currentBackground;

        public SystemType systemType { get { return SystemType.Render; } }

        public RenderSystem()
        {
            _debugView = new DebugView();

            _font = ResourceManager.getResource<Font>("gooddog_font");

            _actionLabel = new Text("", _font, 14);
            _actionLabel.Color = Color.White;

            _buildBridgeShape = new RectangleShape(new Vector2f(0.2f, 0f));
            _buildBridgeShape.FillColor = new Color(255, 255, 0, 128);
            _buildBridgeShape.Origin = new Vector2f(0.1f, 0f);

            _hpBarBackgrounds = new RectangleShape[MAX_HP_BARS];
            _hpBarForegrounds = new RectangleShape[MAX_HP_BARS];

            for (int i = 0; i < MAX_HP_BARS; i++)
            {
                RectangleShape background = new RectangleShape();
                RectangleShape foreground = new RectangleShape();

                background.Size = new Vector2f(HP_BAR_WIDTH + 2f, HP_BAR_HEIGHT + 2f);
                background.Origin = background.Size * 0.5f;
                background.FillColor = Color.Black;

                foreground.Size = new Vector2f(HP_BAR_WIDTH, HP_BAR_HEIGHT);
                foreground.Origin = foreground.Size * 0.5f;
                foreground.FillColor = Color.Green;

                _hpBarBackgrounds[i] = background;
                _hpBarForegrounds[i] = foreground;
            }

            _reticleShape = new RectangleShape();
            _reticleShape.Texture = ResourceManager.getResource<Texture>("reticle");
            _reticleShape.Size = new Vector2f(_reticleShape.Texture.Size.X, _reticleShape.Texture.Size.Y) / 35f;
            _reticleShape.Origin = _reticleShape.Size * 0.5f;

            _activeRopeKnotShapes = new List<int>();
            _ropeKnotTexture = ResourceManager.getResource<Texture>("rope_particle");
            _ropeKnotShapes = new RectangleShape[MAX_ROPE_KNOTS];
            for (int i = 0; i < MAX_ROPE_KNOTS; i++)
            {
                _ropeKnotShapes[i] = new RectangleShape();
                _ropeKnotShapes[i].Texture = _ropeKnotTexture;
                _ropeKnotShapes[i].Size = new Vector2f(0.25f, 0.3f);
                _ropeKnotShapes[i].Origin = _ropeKnotShapes[i].Size * 0.5f;
            }

            _activeBridgeSegmentShapes = new List<int>();
            _bridgeSegmentTexture = ResourceManager.getResource<Texture>("bridge_normal_0");
            _bridgeSegmentShapes = new RectangleShape[MAX_BRIDGE_SEGMENTS];
            for (int i = 0; i < MAX_BRIDGE_SEGMENTS; i++)
            {
                _bridgeSegmentShapes[i] = new RectangleShape();
                _bridgeSegmentShapes[i].Texture = _bridgeSegmentTexture;
                _bridgeSegmentShapes[i].Size = new Vector2f(2f, 0.5f);
                _bridgeSegmentShapes[i].Origin = _bridgeSegmentShapes[i].Size * 0.5f;
            }

            // Initialize animation textures
            initializeAnimations();

            // Initialize backgrounds
            initializeBackgrounds();
        }

        // Initialize animation textures
        private void initializeAnimations()
        {
            _animations = new Dictionary<AnimationCategory, Dictionary<AnimationType, List<Texture>>>();
            _animations.Add(AnimationCategory.Character, new Dictionary<AnimationType, List<Texture>>());
            _animations.Add(AnimationCategory.Drone, new Dictionary<AnimationType, List<Texture>>());
            _animations.Add(AnimationCategory.Enemy, new Dictionary<AnimationType, List<Texture>>());
            _animations.Add(AnimationCategory.ServoBot, new Dictionary<AnimationType, List<Texture>>());
            _animations[AnimationCategory.Character].Add(AnimationType.Idle, new List<Texture>(new[] { ResourceManager.getResource<Texture>("character_idle_0") }));
            _animations[AnimationCategory.Drone].Add(AnimationType.Idle, new List<Texture>( new [] { ResourceManager.getResource<Texture>("drone_idle_0") }));
            _animations[AnimationCategory.Enemy].Add(AnimationType.Idle, new List<Texture>(new [] { ResourceManager.getResource<Texture>("enemy_idle_0") }));
            _animations[AnimationCategory.ServoBot].Add(AnimationType.Idle, new List<Texture>(new [] { ResourceManager.getResource<Texture>("servo_bot_idle_0") }));

            // Finish initializing character animations
            foreach (string direction in new[] { "left", "right" })
            {
                List<Texture> characterResults = new List<Texture>();
                List<Texture> enemyResults = new List<Texture>();

                for (int i = 0; i < 6; i++)
                {
                    characterResults.Add(ResourceManager.getResource<Texture>(String.Format("character_walk_{0}_{1}", direction, i)));
                }

                for (int i = 0; i < 7; i++)
                {
                    enemyResults.Add(ResourceManager.getResource<Texture>(String.Format("enemy_walk_{0}_{1}", direction, i)));
                }

                if (direction == "left")
                {
                    _animations[AnimationCategory.Character].Add(AnimationType.WalkLeft, characterResults);
                    _animations[AnimationCategory.Enemy].Add(AnimationType.WalkLeft, enemyResults);
                }
                else if (direction == "right")
                {
                    _animations[AnimationCategory.Enemy].Add(AnimationType.WalkRight, enemyResults);
                    _animations[AnimationCategory.Character].Add(AnimationType.WalkRight, characterResults);
                }
            }
        }

        // Initialize backgrounds
        private void initializeBackgrounds()
        {
            Background caveBackground = new Background(BackgroundType.Cave);

            _backgrounds = new Dictionary<BackgroundType, Background>();

            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_0"), new Vector2f(0.1f, 0.1f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_1"), new Vector2f(0.09f, 0.09f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_2"), new Vector2f(0.07f, 0.07f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_3"), new Vector2f(0.05f, 0.05f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_4"), new Vector2f(0.03f, 0.03f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_5"), new Vector2f(0.02f, 0.02f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_6"), new Vector2f(0.01f, 0.01f));
            caveBackground.addLayer(ResourceManager.getResource<Texture>("background_cave_7"), new Vector2f(0f, 0f));
            caveBackground.basePosition = new Vector2f(100f, 10f);
            _currentBackground = caveBackground.type;

            _backgrounds.Add(caveBackground.type, caveBackground);
        }

        // Prepare hp bars
        private void prepareHpBars(List<int> entities)
        {
            _usedHpBarCount = 0;

            foreach (int entityId in entities)
            {
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
                float percentHp = (float)statsComponent.currentHp / (float)SystemManager.statSystem.getMaxHp(entityId);
                Vector2f worldPosition = new Vector2f(positionComponent.position.X, positionComponent.position.Y);
                Vector2i screenPosition = Game.window.MapCoordsToPixel(worldPosition, SystemManager.cameraSystem.worldView);
                Vector2f screenPositionF = new Vector2f(screenPosition.X, screenPosition.Y) + new Vector2f(0, 28f);

                _hpBarBackgrounds[_usedHpBarCount].Position = screenPositionF;
                _hpBarForegrounds[_usedHpBarCount].Position = screenPositionF;
                _hpBarForegrounds[_usedHpBarCount++].Size = new Vector2f(HP_BAR_WIDTH * percentHp, HP_BAR_HEIGHT);
            }
        }

        // Prepare color primitive render components
        private void prepareColorPrimitiveRender(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                ColorPrimitiveRenderComponent colorPrimitiveRenderComponent = EntityManager.getColorPrimitiveRenderComponent(entityId);

                for (int i = 0; i < colorPrimitiveRenderComponent.renderData.Count; i++)
                {
                    BodyRenderData renderData = colorPrimitiveRenderComponent.renderData[i];
                    Transform transform = Transform.Identity;
                    Vector2 bodyPosition = renderData.body.Position;
                    float bodyAngleInRadians = renderData.body.Rotation;

                    transform.Translate(new Vector2f(bodyPosition.X, bodyPosition.Y));
                    transform.Rotate(Helpers.radToDeg(bodyAngleInRadians) + 90);

                    renderData.transform = transform;
                    colorPrimitiveRenderComponent.renderData[i] = renderData;
                }
            }
        }

        // Prepare animation components
        private void prepareAnimation(List<int> entities)
        {
            if (SystemManager.physicsSystem.isSlowMotion && !SystemManager.physicsSystem.isReadyForSlowMotionTick)
            {
                return;
            }

            foreach (int entityId in entities)
            {
                AnimationComponent animationComponent = EntityManager.getAnimationComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);

                if (animationComponent.ticksSinceFrameChange >= animationComponent.ticksPerFrame)
                {
                    Texture texture;

                    animationComponent.frameIndex = (animationComponent.frameIndex + 1) % _animations[animationComponent.animationCategory][animationComponent.animationType].Count;
                    animationComponent.ticksSinceFrameChange = 0;

                    texture = _animations[animationComponent.animationCategory][animationComponent.animationType][animationComponent.frameIndex];

                    animationComponent.shape.Texture = texture;
                    animationComponent.shape.Size = (new Vector2f((float)texture.Size.X, (float)texture.Size.Y) / CameraSystem.ORIGINAL_SCALE);
                    animationComponent.shape.Origin = animationComponent.shape.Size * 0.5f;
                }
                else
                {
                    animationComponent.ticksSinceFrameChange++;
                }

                animationComponent.shape.Position = new Vector2f(positionComponent.position.X, positionComponent.position.Y);
            }
        }

        // Prepare rope rendering
        private void prepareRopeRender(List<int> entities)
        {
            _activeRopeKnotShapes.Clear();

            foreach (int entityId in entities)
            {
                RopeComponent ropeComponent = EntityManager.getRopeComponent(entityId);

                for (int i = 0; i < ropeComponent.bodies.Count; i++)
                {
                    int shapeIndex = _activeRopeKnotShapes.Count;
                    Body body = ropeComponent.bodies[i];
                    Vector2 a, b, c, d;

                    // Calculate point a
                    if (i == 0)
                    {
                        Body nextBody = ropeComponent.bodies[i + 1];
                        Vector2 relative = nextBody.Position - body.Position;

                        a = body.Position - relative;
                    }
                    else
                    {
                        Body previousBody = ropeComponent.bodies[i - 1];

                        a = previousBody.Position;
                    }

                    // Calculate point b
                    b = (a + body.Position) * 0.5f;

                    // Calculate point d
                    if (i == ropeComponent.bodies.Count - 1)
                    {
                        Body previousBody = ropeComponent.bodies[i - 1];
                        Vector2 relative = previousBody.Position - body.Position;

                        d = body.Position - relative;
                    }
                    else
                    {
                        Body nextBody = ropeComponent.bodies[i + 1];

                        d = nextBody.Position;
                    }

                    // Calculate point c
                    c = (d + body.Position) * 0.5f;

                    for (float mu = 0; mu <= 1f; mu += 0.1f)
                    {
                        RectangleShape shape = _ropeKnotShapes[shapeIndex];
                        Vector2 point;

                        Helpers.interpolate(ref a, ref b, ref c, ref d, mu, out point);
                        shape.Position = new Vector2f(point.X, point.Y);

                        _activeRopeKnotShapes.Add(shapeIndex);
                        shapeIndex++;
                    }
                }
            }
        }

        // Prepare bridge render
        private void prepareBridgeRender(List<int> entities)
        {
            _activeBridgeSegmentShapes.Clear();

            foreach (int entityId in entities)
            {
                BridgeComponent bridgeComponent = EntityManager.getBridgeComponent(entityId);

                for (int i = 0; i < bridgeComponent.bodies.Count; i++)
                {
                    Body body = bridgeComponent.bodies[i];
                    int shapeIndex = _activeBridgeSegmentShapes.Count;
                    RectangleShape shape = _bridgeSegmentShapes[shapeIndex];

                    shape.Position = new Vector2f(body.Position.X, body.Position.Y);
                    shape.Rotation = Helpers.radToDeg(body.Rotation) + 90;

                    _activeBridgeSegmentShapes.Add(shapeIndex);
                }
            }
        }

        // Update background
        private void updateBackground()
        {
            _backgrounds[_currentBackground].update();
        }

        // Draw reticle (helper method)
        private void drawReticle(Vector2f worldPosition, Color color)
        {
            _reticleShape.Position = worldPosition;
            _reticleShape.FillColor = color;
            Game.window.Draw(_reticleShape);
        }

        // Draw actions currently being performed
        private void drawCurrentActions()
        {
            TeamSystem teamSystem = SystemManager.teamSystem;

            // Nothing to draw
            if (teamSystem.initializingSkill == null)
            {
                return;
            }

            _actionLabel.Position = Game.screenMouse + new Vector2f(16f, 0);

            if (teamSystem.initializingSkill.type == SkillType.ThrowRope)
            {
                _actionLabel.DisplayedString = "Throw Rope";
                drawReticle(Game.sfmlWorldMouse, Color.Yellow);
            }
            else if (teamSystem.initializingSkill.type == SkillType.BuildBridge)
            {
                Vector2 pointA = teamSystem.createBridgeAnchorA;
                Vector2 pointB = teamSystem.createBridgeAnchorB;
                Vector2 relative = pointB - pointA;
                float length = relative.Length();
                float angle = Helpers.radToDeg((float)Math.Atan2(relative.Y, relative.X));

                _buildBridgeShape.Position = new Vector2f(pointA.X, pointA.Y);
                _buildBridgeShape.Rotation = angle;
                _buildBridgeShape.Size = new Vector2f(0.2f, length);
                _actionLabel.DisplayedString = "Build Bridge";
                Game.window.Draw(_buildBridgeShape);
                drawReticle(new Vector2f(pointA.X, pointA.Y), Color.Yellow);
                drawReticle(new Vector2f(pointB.X, pointB.Y), Color.Yellow);
            }
            else if (teamSystem.initializingSkill.type == SkillType.MeleeAttack || teamSystem.initializingSkill.type == SkillType.RangedAttack)
            {
                _actionLabel.DisplayedString = "Attack";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.PowerShot)
            {
                _actionLabel.DisplayedString = "Power Shot";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.PowerSwing)
            {
                _actionLabel.DisplayedString = "Power Swing";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Fireball)
            {
                _actionLabel.DisplayedString = "Cast Fireball";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.HealingBlast)
            {
                _actionLabel.DisplayedString = "Healing Blast";
                drawReticle(Game.sfmlWorldMouse, Color.Green);
            }
            else if (teamSystem.initializingSkill.type == SkillType.ProximityMine)
            {
                _actionLabel.DisplayedString = "Proximity Mine";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Fatality)
            {
                _actionLabel.DisplayedString = "Fatality";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Infusion)
            {
                _actionLabel.DisplayedString = "Cast Infusion";
                drawReticle(Game.sfmlWorldMouse, Color.Cyan);
            }
            else if (teamSystem.initializingSkill.type == SkillType.RainOfFire)
            {
                _actionLabel.DisplayedString = "Cast Rain of Fire";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Dispel)
            {
                _actionLabel.DisplayedString = "Cast Dispel";
                drawReticle(Game.sfmlWorldMouse, Color.Green);
            }
            else if (teamSystem.initializingSkill.type == SkillType.ArrowTime)
            {
                _actionLabel.DisplayedString = "Arrow Time";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Volley)
            {
                _actionLabel.DisplayedString = "Volley";
                drawReticle(Game.sfmlWorldMouse, Color.Red);
            }
            else if (teamSystem.initializingSkill.type == SkillType.GolemStance)
            {
                _actionLabel.DisplayedString = "Golem Stance";
                drawReticle(Game.sfmlWorldMouse, Color.Yellow);
            }
            else if (teamSystem.initializingSkill.type == SkillType.Fortification)
            {
                _actionLabel.DisplayedString = "Fortification";
                drawReticle(Game.sfmlWorldMouse, Color.Cyan);
            }
        }

        // Draw color primitives
        private void drawColorPrimitives(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                ColorPrimitiveRenderComponent colorPrimitiveRenderComponent = EntityManager.getColorPrimitiveRenderComponent(entityId);

                Game.window.Draw(colorPrimitiveRenderComponent);
            }
        }

        // Draw animations
        private void drawAnimations(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                AnimationComponent animationComponent = EntityManager.getAnimationComponent(entityId);

                Game.window.Draw(animationComponent.shape);
            }
        }

        // Draw ropes
        private void drawRopes()
        {
            foreach (int ropeKnotId in _activeRopeKnotShapes)
            {
                Game.window.Draw(_ropeKnotShapes[ropeKnotId]);
            }
        }

        // Draw bridge segments
        private void drawBridgeSegments()
        {
            foreach (int bridgeSegmentId in _activeBridgeSegmentShapes)
            {
                Game.window.Draw(_bridgeSegmentShapes[bridgeSegmentId]);
            }
        }

        // Draw background
        private void drawBackground()
        {
            Background background = _backgrounds[_currentBackground];

            for (int i = background.numLayers - 1 ; i >= 0; i--)
            {
                RectangleShape shape = background.shapes[i];

                Game.window.Draw(shape);
            }
        }

        // Update
        public void update()
        {
            List<int> renderHealthEntities = EntityManager.getEntitiesPossessing(ComponentType.RenderHealth);

            // Update background
            updateBackground();

            // Prepare to draw hp bars
            prepareHpBars(renderHealthEntities);

            // Prepare color primitive render components
            prepareColorPrimitiveRender(EntityManager.getEntitiesPossessing(ComponentType.ColorPrimitiveRender));

            // Prepare rope rendering
            prepareRopeRender(EntityManager.getEntitiesPossessing(ComponentType.Rope));

            // Prepare bridge rendering
            prepareBridgeRender(EntityManager.getEntitiesPossessing(ComponentType.Bridge));

            // Prepare character animation components
            prepareAnimation(EntityManager.getEntitiesPossessing(ComponentType.Animation));
        }

        // Draw
        public void draw()
        {
            // Draw physical world (debug view)
            //_debugView.draw();

            // Draw background
            drawBackground();

            // Draw color primitives
            drawColorPrimitives(EntityManager.getEntitiesPossessing(ComponentType.ColorPrimitiveRender));

            // Draw particle effects
            SystemManager.particleRenderSystem.draw();

            // Draw animations
            drawAnimations(EntityManager.getEntitiesPossessing(ComponentType.Animation));

            // Draw rope
            drawRopes();

            // Draw bridge segments
            drawBridgeSegments();

            // Draw actions currently being performed
            drawCurrentActions();
        }

        // A separate draw method for after the window has been switched back to the default screen view (not world coordinates)
        public void drawUsingScreenCoords()
        {
            // Draw hp bars
            for (int i = 0; i < _usedHpBarCount; i++)
            {
                Game.window.Draw(_hpBarBackgrounds[i]);
                Game.window.Draw(_hpBarForegrounds[i]);
            }

            // Draw action label
            if (SystemManager.teamSystem.initializingSkill != null)
            {
                Game.window.Draw(_actionLabel);
            }
        }
    }
}
