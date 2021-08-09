using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public enum layerType { text ,frame };
    public layerType type { get; set; }
    public string path { get; set; }
    public List<Placement> placement { get; set; }
    public List<Operation> operations { get; set; }

}
