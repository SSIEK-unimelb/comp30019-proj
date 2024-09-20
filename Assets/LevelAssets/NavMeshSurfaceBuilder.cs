using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshSurfaceBuilder : MonoBehaviour
{
    public void BuildNavMeshSurface() {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
