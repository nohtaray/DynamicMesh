using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private DynamicCurve dynamicCurvePrefab;

    private Camera _camera;
    private readonly List<Vector3> _centerPoints = new List<Vector3>();


    private DynamicCurve _dynamicCurve;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dynamicCurve = Instantiate(dynamicCurvePrefab);
        }

        if (Input.GetMouseButton(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            foreach (var hit in Physics.RaycastAll(ray))
            {
                if (hit.collider.gameObject != gameObject) continue;

                const float threshold = 0.2f;
                var isFar = _centerPoints.Count == 0 ||
                            (_centerPoints[_centerPoints.Count - 1] - hit.point).magnitude >= threshold;
                if (isFar)
                {
                    _centerPoints.Add(hit.point);
                    _dynamicCurve.UpdateCenterPoints(_centerPoints);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dynamicCurve.EnablePhysics();
            _centerPoints.Clear();
        }
    }
}