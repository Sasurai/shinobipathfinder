using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShinobiPathfinder
{
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
    }
}