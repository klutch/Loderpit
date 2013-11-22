using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Loderpit.Systems;
using Loderpit.Loaders;
using FarseerRubeLoader;

namespace Loderpit
{
    public enum TerrainModuleType
    {
        Normal,
        Gap,
        Bridge,
        Cliff,
        Start,
        End
    }

    public enum ObstacleModuleType
    {
        Stalactite
    }

    public class Map
    {
        private World _world;
        private Random _rng;
        private TerrainModuleLoader _terrainLoader;
        private ObstacleModuleLoader _obstacleLoader;
        private List<Vector2> _moduleEndPoints;

        public int moduleEndPointsCount { get { return _moduleEndPoints.Count; } }
        public Vector2 initialPosition { get { return new Vector2(0f, 0f); } }
        public List<Vector2> moduleEndPoints { get { return _moduleEndPoints; } }

        public Map(World world)
        {
            _world = world;
            _rng = new Random();
            _moduleEndPoints = new List<Vector2>();
            _terrainLoader = new TerrainModuleLoader(this);
            _obstacleLoader = new ObstacleModuleLoader(this);
        }

        private int getNumVariations(TerrainModuleType moduleType)
        {
            switch (moduleType)
            {
                case TerrainModuleType.Normal: return 1;
                case TerrainModuleType.Gap: return 1;
                case TerrainModuleType.Bridge: return 1;
                case TerrainModuleType.Cliff: return 1;
                case TerrainModuleType.Start: return 1;
                case TerrainModuleType.End: return 1;
            }

            return 0;
        }

        private int getNumVariations(ObstacleModuleType obstacleModuleType)
        {
            switch (obstacleModuleType)
            {
                case ObstacleModuleType.Stalactite: return 1;
            }

            return 0;
        }

        public void addModuleEndPoint(Vector2 point)
        {
            _moduleEndPoints.Add(point);
        }

        public Vector2 getLastModuleEndPoint()
        {
            return _moduleEndPoints[_moduleEndPoints.Count - 1];
        }

        public float getLastModuleWidth()
        {
            return _moduleEndPoints[_moduleEndPoints.Count - 1].X - _moduleEndPoints[_moduleEndPoints.Count - 2].X;
        }

        public void generate(int iterations = 16)
        {
            List<TerrainModuleType> validTerrainModuleTypes = new List<TerrainModuleType>();
            List<ObstacleModuleType> validObstacleModuleTypes = new List<ObstacleModuleType>();

            foreach (TerrainModuleType type in Enum.GetValues(typeof(TerrainModuleType)))
            {
                if (type != TerrainModuleType.Start && type != TerrainModuleType.End)
                {
                    validTerrainModuleTypes.Add(type);
                }
            }

            validObstacleModuleTypes.Add(ObstacleModuleType.Stalactite);

            _terrainLoader.load(_world, "resources/modules/start_0.json", true);

            for (int i = 0; i < iterations; i++)
            {
                TerrainModuleType terrainModuleType = validTerrainModuleTypes[_rng.Next(validTerrainModuleTypes.Count)];
                ObstacleModuleType obstacleModuleType = validObstacleModuleTypes[_rng.Next(validObstacleModuleTypes.Count)];
                int terrainVariation = _rng.Next(getNumVariations(terrainModuleType));
                int obstacleVariation = _rng.Next(getNumVariations(obstacleModuleType));
                string terrainModuleFile = string.Format("resources/modules/{0}_{1}.json", terrainModuleType.ToString().ToLower(), terrainVariation);
                string obstacleModuleFile = string.Format("resources/obstacles/{0}_{1}.json", obstacleModuleType.ToString().ToLower(), obstacleVariation);

                Console.WriteLine(terrainModuleFile);
                _terrainLoader.load(_world, terrainModuleFile, true);
                _obstacleLoader.load(_world, obstacleModuleFile, true);
            }

            _terrainLoader.load(_world, "resources/modules/end_0.json", true);
        }
    }
}
