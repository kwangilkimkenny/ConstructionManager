using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/

public class CircleDrawing : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = new Vector3[3] { new Vector3(50, 50, 50), new Vector3(100, 50, 250), new Vector3(200, 50, 50) };
        DrawTriangle(positions, 0.02f, 0.02f);
    }

    void DrawTriangle(Vector3[] vertexPositions, float startWidth, float endWidth)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 3;
        lineRenderer.SetPositions(vertexPositions);
    }
}
