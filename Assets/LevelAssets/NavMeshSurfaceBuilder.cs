using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshSurfaceBuilder : MonoBehaviour
{
    private bool buildNavMesh = false;
    private float waitTime = 0.5f;

    private void Update() {
        if (buildNavMesh) {
            StartCoroutine(BuildNavMeshSurface());
        }
    }

    public void SetBuildToTrue() {
        buildNavMesh = true;
    }

    private IEnumerator BuildNavMeshSurface() {
        yield return new WaitForSeconds(waitTime);
        GetComponent<NavMeshSurface>().BuildNavMesh();
        buildNavMesh = false;
    }
}
