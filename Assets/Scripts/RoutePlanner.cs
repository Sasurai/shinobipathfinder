using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShinobiPathfinder
{
    public class RoutePlan : IComparable
    {
        public DateTime duration;
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
        }

        private void PlanRoute(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            List<RoutePlan> plans = new List<RoutePlan>(10);

            // Start from origin

            // 1 possible route per exit from origin (picking option according to preferences)

            // Iterate each route (probably easier to do this recursively)

            // Discard routes if we hit a dead end (no other exits, only non-usable modes)

            // Merge routes if they hit a common node and keep only shortest? This is good for pruning but may be expensive
            // Alternative: keep list of visited nodes, discard if a route hits an already visited node (some other route already uses it, may lead to suboptimal routes)

            // Sort plans and print shortest (in the future we may want to also display other N options?)

            // Print error if no plan could be found with the given preferences
        }
    }
}