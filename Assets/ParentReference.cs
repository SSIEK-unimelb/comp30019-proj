using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentReference : MonoBehaviour
{
    [SerializeField] Transform m_Parent;
    // Start is called before the first frame update
    public Transform getTransform() { return m_Parent; }
}
