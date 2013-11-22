using System;
using System.Collections.Generic;
using System.Linq;
using Loderpit.Formations;

namespace Loderpit.Components
{
    public class GroupComponent : IComponent
    {
        private int _entityId;
        private List<int> _entities;
        private List<Formation> _formations;
        private bool _anyOnRopeLastFrame;
        private bool _anyOnRopeThisFrame;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Group; } }
        public List<int> entities { get { return _entities; } }
        public Formation activeFormation { get { return _formations.OrderByDescending(f => f.priority).First(); } }
        public bool anyOnRopeLastFrame { get { return _anyOnRopeLastFrame; } set { _anyOnRopeLastFrame = value; } }
        public bool anyOnRopeThisFrame { get { return _anyOnRopeThisFrame; } set { _anyOnRopeThisFrame = value; } }

        public GroupComponent(int entityId, List<int> entities = null, List<Formation> formations = null)
        {
            _entityId = entityId;
            _entities = entities ?? new List<int>();
            _formations = formations ?? new List<Formation>();
        }

        public Formation getFormation(FormationType formationType)
        {
            foreach (Formation formation in _formations)
            {
                if (formation.type == formationType)
                {
                    return formation;
                }
            }

            return null;
        }

        public void addFormation(Formation formation)
        {
            _formations.Add(formation);
        }

        public void removeFormation(Formation formation)
        {
            _formations.Remove(formation);
        }
    }
}
