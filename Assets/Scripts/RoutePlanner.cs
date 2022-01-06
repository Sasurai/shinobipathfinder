using System;
using System.Collections.Generic;
using System.Text;
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
        private Text _totalTimeText;
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

            // TODO It may be useful to have some editor only "data validator" here (v.g. check for duplicates in RouteType within a list, etc)

            PlanRoute(_origin, _target, new TravelPreferences());
        }

        private void PlanRoute(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            var pathfinder = new DijkstraPathfinder(dataNodes);
            var route = pathfinder.FindPath(origin, destination, preferences);
            NodeDataScriptable current = null;
            NodeDataScriptable first = null;
            var totalTime = 0;
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
                    routeView.GetComponent<RouteEntryView>().SetData(prev, current, preferences, out int timeMin);
                    totalTime += timeMin;
                }
            }
            _titleText.text = $"Ruta: {first.nodeName} a {current.nodeName}";
            _totalTimeText.text = $"Tiempo total: {GetTotalTimeString(totalTime)}";
        }

        // This is pretty much copy-pasted from RouteEntryView.GetDurationText
        private string GetTotalTimeString(int time)
        {
            var builder = new StringBuilder();

            var timeSpan = TimeSpan.FromMinutes(time);
            if (timeSpan.Days > 0) builder.Append("Días: ").Append(timeSpan.Days).Append(", ");
            if (timeSpan.Hours > 0) builder.Append("Horas: ").Append(timeSpan.Hours).Append(", ");
            if (timeSpan.Minutes > 0) builder.Append("Minutos: ").Append(timeSpan.Minutes).Append(", ");

            // Remove trailing comma and whitespace
            builder.Remove(builder.Length - 2, 2);
            return builder.ToString();
        }
    }
}