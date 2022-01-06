using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShinobiPathfinder
{
    public class RoutePlan : IComparable
    {
        public TimeSpan duration;
        // TODO decide, origin will probably be implicit to avoid repetition
        public List<NodeDataScriptable> route;

        RoutePlan Clone()
        {
            var clone = new RoutePlan { duration = duration, route = new List<NodeDataScriptable>(route) };
            return clone;
        }

        public int CompareTo(object obj)
        {
            return duration.CompareTo(obj);
        }
    }

    public class RoutePlanner : MonoBehaviour
    {
        private NodeDataScriptable[] dataNodes;
        // Start is called before the first frame update
        void Start()
        {
            // Not sure this is the best option but it should work and we avoid having to add each node manually somewhere
            dataNodes = Resources.LoadAll<NodeDataScriptable>("Data");
            Debug.Log($"Loaded {dataNodes.Length} data nodes");

            PlanRoute(dataNodes[0], null, new TravelPreferences());
        }

        private void PlanRoute(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            var pathfinder = new DijkstraPathfinder(dataNodes);
            pathfinder.FindPath(origin, destination, preferences);
        }
    }
}