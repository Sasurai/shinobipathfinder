using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShinobiPathfinder
{
    public class RoutePlanner : MonoBehaviour
    {
        // UI Stuff
        [SerializeField]
        private Text _titleText;
        [SerializeField]
        private RectTransform _routeContainer;
        [SerializeField]
        private GameObject _routeViewPrefab;

        // Hacky for testing, set origin and target from editor
        [SerializeField]
        private NodeDataScriptable _origin;
        [SerializeField]
        private NodeDataScriptable _target;

        private NodeDataScriptable[] dataNodes;
        // Start is called before the first frame update
        void Start()
        {
            // Not sure this is the best option but it should work and we avoid having to add each node manually somewhere
            dataNodes = Resources.LoadAll<NodeDataScriptable>("Data");
            Debug.Log($"Loaded {dataNodes.Length} data nodes");

            PlanRoute(_origin, _target, new TravelPreferences());
        }

        private void PlanRoute(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            var pathfinder = new DijkstraPathfinder(dataNodes);
            var route = pathfinder.FindPath(origin, destination, preferences);
            NodeDataScriptable current = null;
            NodeDataScriptable first = null;
            foreach(var node in route)
            {
                // Process & print result
                var prev = current;
                current = node;
                if(prev == null)
                {
                    first = node;
                }
                else
                {
                    var routeView = Instantiate(_routeViewPrefab, _routeContainer);
                    routeView.GetComponent<RouteEntryView>().SetData(prev, current, preferences);
                }
            }
            _titleText.text = $"Ruta: {first.nodeName} a {current.nodeName}";
        }
    }
}