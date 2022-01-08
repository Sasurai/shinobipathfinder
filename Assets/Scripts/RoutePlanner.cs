using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ShinobiPathfinder
{
    public class RoutePlanner : MonoBehaviour
    {
        // Canvases references (for showing & hiding)
        [SerializeField]
        private GameObject _inputCanvas;
        [SerializeField]
        private GameObject _resultsCanvas;

        // Input UI
        [SerializeField]
        private Dropdown _originDropdown;
        [SerializeField]
        private Dropdown _targetDropdown;

        // Results UI
        [SerializeField]
        private Text _titleText;
        [SerializeField]
        private Text _totalTimeText;
        [SerializeField]
        private RectTransform _routeContainer;
        [SerializeField]
        private GameObject _routeViewPrefab;

        // Internal stuff
        private NodeDataScriptable[] _dataNodes;
        private Dictionary<string, NodeDataScriptable> _nameDataMap;
        private TravelPreferences _travelPreferences = new TravelPreferences();


        void Start()
        {
            // Not sure this is the best option but it should work and we avoid having to add each node manually somewhere
            _dataNodes = Resources.LoadAll<NodeDataScriptable>("Data");
            Debug.Log($"Loaded {_dataNodes.Length} data nodes");

            // TODO It may be useful to have some editor only "data validator" here (v.g. check for duplicates in RouteType within a list, etc)

            PopulateNameDataMap();

            _resultsCanvas.SetActive(false);
            UpdateDropdowns();
        }

        // OnClick handler
        public void OnSearchRoute()
        {
            _inputCanvas.SetActive(false);
            _resultsCanvas.SetActive(true);

            var originTxt = _originDropdown.options[_originDropdown.value].text;
            var targetTxt = _targetDropdown.options[_targetDropdown.value].text;

            // Hacky, this relies on the internal values of the enum
            _travelPreferences.PrefRouteType = (RouteType)_typeSelectedInt;

            PlanRoute(_nameDataMap[originTxt], _nameDataMap[targetTxt], _travelPreferences);
        }

        // Option toggle handlers (Ship, Train, Dirigible)
        public void OnShipOptionToggle(bool opt)
        {
            _travelPreferences.UseShips = opt;
        }

        public void OnTrainOptionToggle(bool opt)
        {
            _travelPreferences.UseTrains = opt;
        }

        public void OnDirigibleOptionToggle(bool opt)
        {
            _travelPreferences.UseDirigibles = opt;
        }

        private int _typeSelectedInt = 0;
        // Type selected dropdown handler
        public void OnTypeChanged(int selection)
        {
            _typeSelectedInt = selection;
        }

        private void UpdateDropdowns()
        {
            UpdateDropdown(_originDropdown);
            UpdateDropdown(_targetDropdown);
        }

        // TODO for search option: Add filtering parameter
        private void UpdateDropdown(Dropdown dropdown)
        {
            var options = new List<string>();
            foreach(var node in _dataNodes)
            {
                options.Add(node.nodeName);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        }

        private void PlanRoute(NodeDataScriptable origin, NodeDataScriptable destination, TravelPreferences preferences)
        {
            // Same node edge case
            if(origin == destination)
            {
                _titleText.text = $"Te quedas en {origin.nodeName}.";
                _totalTimeText.text = String.Empty;
                return;
            }

            var pathfinder = new DijkstraPathfinder(_dataNodes);
            var route = pathfinder.FindPath(origin, destination, preferences);

            if(route == null)
            {
                _titleText.text = $"No se ha podido encontrar ruta de {origin.nodeName} a {destination.nodeName}.";
                _totalTimeText.text = "Esto puede ser causado por modos de viaje desactivados.";
                return;
            }

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

        private void PopulateNameDataMap()
        {
            _nameDataMap = new Dictionary<string, NodeDataScriptable>(_dataNodes.Length);
            foreach(var node in _dataNodes)
            {
                _nameDataMap.Add(node.nodeName, node);
            }
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