using System.Collections.Generic;
using UnityEngine;

public class DynamicCurve : MonoBehaviour
{
    private const int VertCount = 7;
    private const float Radius = 0.3f;

    private MeshFilter _meshFilter;

    private void OnEnable()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        // var centerPoints = new List<Vector3>();
        // var vd = Vector3.forward;
        // var v = Vector3.zero;
        // for (var i = 0; i < 10; i++)
        // {
        //     centerPoints.Add(v);
        //     v += vd;
        //     vd = Quaternion.AngleAxis(20, Vector3.up) * vd;
        // }
        //
        // TurnedCylinder(centerPoints);
    }

    public void UpdateCenterPoints(IReadOnlyList<Vector3> centerPoints)
    {
        if (centerPoints.Count >= 3)
        {
            TurnedCylinder(centerPoints);
        }
        else
        {
            _meshFilter.mesh = new Mesh();
        }


        // 線でメッシュを表示する
        // _meshFilter.mesh.SetIndices(_meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);


        // Mesh Collider 使うならこうする
        // GetComponent<MeshCollider>().sharedMesh = _meshFilter.mesh;
    }

    public void EnablePhysics()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        var collider = GetComponent<MeshCollider>();
        collider.enabled = true;
        collider.sharedMesh = _meshFilter.mesh;
    }

    private void TurnedCylinder(IReadOnlyList<Vector3> centerPoints)
    {
        var mesh = new Mesh();

        var up = Vector3.up;
        var hCount = centerPoints.Count;

        var vertices = new List<Vector3>();
        for (var h = 0; h < hCount; h++)
        {
            var forward =
                ((centerPoints[h] - centerPoints[Mathf.Clamp(h - 1, 0, hCount - 1)]).normalized +
                 (centerPoints[Mathf.Clamp(h + 1, 0, hCount - 1)] - centerPoints[h]).normalized).normalized;
            var o = centerPoints[h];
            var d = up * Radius;
            for (var i = 0; i < VertCount; i++)
            {
                vertices.Add(o + d);
                d = Quaternion.AngleAxis(360.0f / VertCount, forward) * d;
            }
        }

        var triangles = new List<int>();
        for (var h = 0; h < hCount - 1; h++)
        {
            for (var i = 0; i < VertCount; i++)
            {
                // h == 0, VERT_COUNT == 6 のとき、
                // 0, 1, 6, 6, 1, 7
                var h1 = h * VertCount;
                var h2 = (h + 1) * VertCount;
                var w1 = i;
                var w2 = (i + 1) % VertCount;
                triangles.AddRange(new[]
                {
                    h1 + w1,
                    h1 + w2,
                    h2 + w1,
                    h2 + w1,
                    h1 + w2,
                    h2 + w2,
                });
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
    }
}