using System;
using UnityEngine;

namespace ShinobiPathfinder
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeDataScriptable", order = 1)]
    public class NodeDataScriptable : ScriptableObject
    {
        public string name;
        public Route[] routes;
    }
    [Serializable]
    public class Route
    {
        public NodeDataScriptable target;
        public RouteData[] options;
    }

    [Serializable]
    public class RouteData
    {
        public RouteType type;
        public int days;
        public int hours;
        public int minutes;
    }

    public enum RouteType
    {
        Walking,
        Caravan,
        Ninja,
        Ship,
        Train,
        Dirigible
    }
}