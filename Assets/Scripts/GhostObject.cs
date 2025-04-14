using UnityEditor;
using UnityEngine;

[ExecuteAlways] public class GhostObject : MonoBehaviour
{
    Material mat;
    Transform orbTransform;
    public Collider c;
    public float isInverted = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Application.isPlaying)
        {
            mat = GetComponent<Renderer>().material;
            c = GetComponent<Collider>();
            orbTransform = GameObject.Find("Player_ORB").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mat != null && orbTransform != null)
        {
            mat.SetVector("_OrbPosition", orbTransform.position);
            mat.SetFloat("_OrbRadius", Mathf.Clamp((orbTransform.localScale.x / 2f), 0.01f, 100f));
            mat.SetFloat("_IsInverted", isInverted);
        }
    }
}
