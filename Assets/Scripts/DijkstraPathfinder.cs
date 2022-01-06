using System.Collections.Generic;
using UnityEngine;

namespace ShinobiPathfinder
{
    public class DijkstraPathfinder
    {
        private NodeDataScriptable[] AllNodes;
        public DijkstraPathfinder(NodeDataScriptable[] allNodes)
        {
            AllNodes = allNodes;
        }

        // Putting this as members instead of local vars to I have to pass less parameters around ^^"
        int[] _distances;
        NodeDataScriptable[] _previous;

        public void FindPath(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            // Initialisation

            // We'll use distances as minutes for simplicity here
            _distances = new int[AllNodes.Length];
            _previous = new NodeDataScriptable[AllNodes.Length];
            var unexplored = new Dictionary<int, NodeDataScriptable>(AllNodes.Length);
            for(int i = 0; i < AllNodes.Length; ++i)
            {
                var node = AllNodes[i];
                node.index = i;

                _distances[i] = origin == node ? 0 : int.MaxValue; 
                _previous[i] = null;
                unexplored.Add(i, node);
            }

            // Actual algorithm
            while(unexplored.Count > 0)
            {
                var nodeIdx = SelectNextNode(unexplored);
                if(nodeIdx == -1)
                {
                    Debug.LogError("Couldn't find any more connected nodes");
                    break;
                }
                var node = unexplored[nodeIdx];
                unexplored.Remove(nodeIdx);

                // If node == destination we can stop here and return the route already
                if (node == destination)
                {
                    break;
                }
                ExploreNodeNeighbours(node, preferences);
            }

            var routePlan = RetrieveRoutePlan(origin, destination, preferences);

            // TODO rebuild / print complete route with the plan + PickBestTravelOption calls

            // Cleanup
            unexplored.Clear();
            _distances = null;
            _previous = null;
        }

        private int SelectNextNode(Dictionary<int, NodeDataScriptable> unexplored)
        {
            var idx = -1;
            var distance = int.MaxValue;
            foreach(var pair in unexplored)
            {
                var cIdx = pair.Key;
                var cDist = _distances[cIdx];
                if(cDist < distance)
                {
                    distance = cDist;
                    idx = cIdx;
                }
            }
            return idx;
        }

        private void ExploreNodeNeighbours(NodeDataScriptable node, TravelPreferences travelPreferences)
        {
            var dist = _distances[node.index];
            foreach(var route in node.routes)
            {
                var option = PickBestTravelOption(route, travelPreferences);
                var targetIdx = route.target.index;
                var alternative = dist + option.FullTimeInMinutes;
                if(alternative < _distances[targetIdx])
                {
                    _distances[targetIdx] = alternative;
                    _previous[targetIdx] = node;
                }
            }
        }

        private Stack<NodeDataScriptable> RetrieveRoutePlan(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            var plan = new Stack<NodeDataScriptable>();
            var current = destination;
            while(current != origin)
            {
                plan.Push(current);
                current = _previous[current.index];
            }

            return plan;
        }

        private RouteData PickBestTravelOption(Route route, TravelPreferences preferences)
        {
            foreach(var opt in route.options)
            {
                var type = opt.type;
                if(type == preferences.PrefRouteType)
                {
                    return opt;
                }
                // TODO if multiple options of ship/train/dirigible are available, it'll pick the first one and ignore the others, even if shorter
                if(type == RouteType.Ship && preferences.UseShips)
                {
                    return opt;
                }
                if (type == RouteType.Train && preferences.UseTrains)
                {
                    return opt;
                }
                if (type == RouteType.Dirigible && preferences.UseDirigibles)
                {
                    return opt;
                }
            }
            return null;
        }
        
    }
}
