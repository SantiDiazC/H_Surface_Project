using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceModifier : MonoBehaviour
{
    public int resolution = 100000; // Number of vertices per side of the plane
    public float scale = 1f; // Scale factor for the function values
    public float heightMultiplier = 1f; // Multiplier for the height of the vertices
    public float size = 10f; // Size of the plane in units
    public Gradient colorGradient; // Gradient for the vertex colors

    private MeshFilter meshFilter;
    private Mesh mesh;
    public SurfaceFunctionName function;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        GeneratePlane();
        ModifySurface();
        ModifyVertexColors();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
    }

    private void ModifySurface()
    {
        Vector3[] vertices = mesh.vertices;
        int numVertices = vertices.Length;
        SurfaceFunction f= functions[(int)function];
        float t = Time.time;
        //float t = 0.5f;
        for (int i = 0; i < numVertices; i++)
        {
            Vector3 vertex = vertices[i];
            float x = vertex.x / scale;
            float y = vertex.z / scale;

            // Calculate the function value at (x, y)
            float z = f(x, y, t) * heightMultiplier;

            // Modify the vertex height
            vertex.y = z;

            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
    }

    private void GeneratePlane()
    {
        int numVertices = (resolution + 1) * (resolution + 1);
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];

        float stepSize = size / resolution;

        for (int i = 0, y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++, i++)
            {
                float xPos = x * stepSize - size * 0.5f;
                float yPos = y * stepSize - size * 0.5f;

                vertices[i] = new Vector3(xPos, 0f, yPos);
                uv[i] = new Vector2(x * stepSize / size, y * stepSize / size);
            }
        }

        int[] triangles = new int[resolution * resolution * 6];

        for (int ti = 0, vi = 0, y = 0; y < resolution; y++, vi++)
        {
            for (int x = 0; x < resolution; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + resolution + 1;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + resolution + 1;
                triangles[ti + 5] = vi + resolution + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void ModifyVertexColors()
    {
    Vector3[] vertices = mesh.vertices;
    int numVertices = vertices.Length;

    Color[] colors = new Color[numVertices];

    for (int i = 0; i < numVertices; i++)
    {
        Vector3 vertex = vertices[i];

        // Get the normalized height value
        float normalizedHeight = Mathf.InverseLerp(0f, heightMultiplier, vertex.y);

        // Get the color from the gradient based on the normalized height
        Color color = colorGradient.Evaluate(normalizedHeight);

        colors[i] = color;
    }

    mesh.colors = colors;
    }


    private void Update()
    {
        GeneratePlane();
        ModifySurface();
        ModifyVertexColors();
    }

    // The function f(x, y) that you need to provide
    //private float f(float x, float y)
    //{
        // Replace this with your own function logic
    //    return Mathf.Sin(x) + Mathf.Cos(y);
    //}

    static SurfaceFunction[] functions = { 
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple 
        };

    
    const float pi = Mathf.PI;

    static float Sine2DFunction(float x, float y, float t) {
        float z = Mathf.Sin(pi* (x + t));
        z += Mathf.Sin(pi* (y + t));
        z *= 0.2f;
        return z;
    }
    
    static float SineFunction(float x, float y, float t) {
        return 0.2f*Mathf.Sin(pi* (x + t));
    }
    
    static float MultiSineFunction(float x, float y, float t) {
        float z = Mathf.Sin(pi * (x + t));
        z += Mathf.Sin(2f * pi * (x + 2f * t)) / 2f;
        z *= 2f / 6f;
        return y;
    }

    static float MultiSine2DFunction(float x, float y, float t) {
        float z = Mathf.Sin(pi * (x + y + 0.5f * t));
        z += Mathf.Sin(pi * (x + t));
        z += Mathf.Sin(2f * pi * (y + 2f * t)) * 0.5f;
        z *= 1f/10.5f;
        return z;
    }

    static float Ripple(float x, float y, float t) {
        float d = Mathf.Sqrt(x*x + y*y);
        //float y = d;
        float z = Mathf.Sin(pi * (4f * d - t));
        z /= 1f + 10f * d;
        return z*0.1f;
    }

}