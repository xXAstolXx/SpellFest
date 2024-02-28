using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Path Display")]
    [SerializeField] private Color validColor;
    private Gradient colorGradient;

    private Material material;

    [SerializeField] private float lineWidth;
    [SerializeField] private float segmentDensity;
    // range : [0,1]

    [SerializeField] private float speed;

    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float opacity;
    [SerializeField] private float targetRadius;
    [SerializeField] private float thickness;


    private void Awake()
    {
        colorGradient = new Gradient();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.widthMultiplier = lineWidth;

        lineRenderer.positionCount = 2;
    }


    public void RenderPath(Vector2 direction, float length) 
    {
        lineRenderer.enabled = true;
        lineRenderer.colorGradient = colorGradient;

        lineRenderer.material.SetFloat("_lineWidth", lineWidth);
        lineRenderer.material.SetFloat("_speed", speed);
        lineRenderer.material.SetFloat("_width", width);
        lineRenderer.material.SetFloat("_height", height);
        lineRenderer.material.SetFloat("_opacity", opacity);
        lineRenderer.material.SetFloat("_targetRadius", targetRadius);
        lineRenderer.material.SetFloat("_thicknessCircle", thickness);
        lineRenderer.material.SetColor("_color", validColor);
        lineRenderer.material.SetFloat("_segmentDensity", segmentDensity * length * -1f);
        lineRenderer.material.SetFloat("_endSegment", lineWidth / (length + 0.5f * lineWidth));

        lineRenderer.SetPosition(0, (Vector2)transform.position + direction.normalized * 0.4f);
        lineRenderer.SetPosition(1, (Vector2)(transform.position) + direction * length);
    }

    public void ClearPath() 
    {
        lineRenderer.enabled = false;
    }
}
