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

                // TODO : If node == destination we can stop here and return the route already
                ExploreNodeNeighbours(node, preferences);
            }

            // Cleanup
            unexplored.Clear();

            Debug.Log("Path finished");
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

        /*
          function Dijkstra(Graph, source):
 2
 3      create vertex set Q
 4
 5      for each vertex v in Graph:            
 6          dist[v] <- INFINITY                 
 7          prev[v] <- UNDEFINED                
 8          add v to Q                     
 9      dist[source] <- 0                       
10     
11      while Q is not empty:
12          u <- vertex in Q with min dist[u]   
13                                             
14          remove u from Q
15         
16          for each neighbor v of u still in Q:
17              alt <- dist[u] + length(u, v)
18              if alt < dist[v]:              
19                  dist[v] <- alt
20                  prev[v] <- u
21
22      return dist[], prev[]


        If we are only interested in a shortest path between vertices source and target,
        we can terminate the search after line 15 if u = target. Now we can read the shortest path from source to target by reverse iteration: 
    */

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
