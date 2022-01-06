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

        public IEnumerable<NodeDataScriptable> FindPath(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            Debug.Log($"FindPath: {origin.nodeName}, {destination.nodeName}");

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
                Debug.Log($"FindPath Iteration: {node.nodeName}");

                // If node == destination we can stop here and return the route already
                if (node == destination)
                {
                    break;
                }
                ExploreNodeNeighbours(node, preferences);
            }

            var routePlan = RetrieveRoutePlan(origin, destination);

            // Cleanup
            unexplored.Clear();
            _distances = null;
            _previous = null;

            return routePlan;
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
                var option = travelPreferences.PickBestTravelOption(route);
                if (option == null)
                {
                    continue;
                }
                var targetIdx = route.target.index;
                var alternative = dist + option.FullTimeInMinutes;
                if(alternative < _distances[targetIdx])
                {
                    _distances[targetIdx] = alternative;
                    _previous[targetIdx] = node;
                }
            }
        }

        private Stack<NodeDataScriptable> RetrieveRoutePlan(NodeDataScriptable origin, NodeDataScriptable destination)
        {
            var plan = new Stack<NodeDataScriptable>();
            var current = destination;
            while(current != origin)
            {
                plan.Push(current);
                current = _previous[current.index];
            }

            plan.Push(origin);
            return plan;
        }
    }
}
