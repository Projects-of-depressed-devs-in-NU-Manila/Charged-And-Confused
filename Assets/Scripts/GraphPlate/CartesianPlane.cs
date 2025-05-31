using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartesianPlane : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private float axisWidth;
    [SerializeField] private Color colorX;
    [SerializeField] private Color colorY;
    [SerializeField] private float graphHeight;
    [SerializeField] private float graphWidth;

    [Header("Point Settings")]
    [SerializeField] private float axisTickWidth = 3f;
    [SerializeField] private float tickLineWidthMultiplier = 0.01f;
    [SerializeField] private float gridLineWidthMultiplier = 0.5f;
    [SerializeField] private float unitFontSize;

    private GameObject canvasObj;
    private float lastZoomLevel = -1f;
    private readonly float[] unitSteps = { 0.1f, 0.5f, 1f, 5f, 10f };
    private Dictionary<float, GameObject> stepContainers = new();
    private List<TextMeshPro> allLabels = new();

    void Start()
    {
        canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = canvasObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(graphWidth, graphHeight);
        canvasObj.transform.localScale = Vector3.one * 0.01f;

        RenderFullGraph();
        DrawMainAxes();
    }

    void Update()
    {
        float zoom = Camera.main.orthographicSize;
        float baseScale = 0.5f;
        float scaleFactor = baseScale * zoom;

        if (Mathf.Abs(zoom - lastZoomLevel) > 0.1f)
        {
            lastZoomLevel = zoom;
            float bestStep = GetBestUnitStep(zoom);
            UpdateGraphVisibility(bestStep);
        }

        foreach (var label in allLabels)
        {
            if (label != null)
            {
                label.transform.localScale = Vector3.one * scaleFactor;
            }
        }
    }

    private float GetBestUnitStep(float zoom)
    {
        float desiredSpacing = zoom / 10f;
        foreach (float step in unitSteps)
        {
            if (step >= desiredSpacing)
                return step;
        }
        return unitSteps[unitSteps.Length - 1];
    }

    private void UpdateGraphVisibility(float visibleStep)
    {
        foreach (var kvp in stepContainers)
        {
            kvp.Value.SetActive(Mathf.Approximately(kvp.Key, visibleStep));
        }
    }

    private void RenderFullGraph()
    {
        foreach (float step in unitSteps)
        {
            string label = step < 1f ? "mm" : "cm";
            float scale = 1f;

            GameObject stepContainer = new GameObject($"Grid_{step}");
            stepContainer.transform.parent = canvasObj.transform;
            stepContainers[step] = stepContainer;

            DrawAxesLayer(step, label, scale, stepContainer);
            stepContainer.SetActive(false);
        }
    }

    private void DrawMainAxes()
    {
        GameObject mainAxes = new GameObject("MainAxes");
        mainAxes.transform.parent = canvasObj.transform;
        float mainAxisWidth = axisWidth * 2f;

        DrawLine("Main_X_Axis", new Vector2(-graphWidth / 2f, 0f), new Vector2(graphWidth / 2f, 0f), colorX, mainAxes, mainAxisWidth);
        DrawLine("Main_Y_Axis", new Vector2(0f, -graphHeight / 2f), new Vector2(0f, graphHeight / 2f), colorY, mainAxes, mainAxisWidth);
    }

    private void DrawAxesLayer(float step, string label, float scale, GameObject parent)
    {
        float halfWidth = graphWidth / 2f;
        float halfHeight = graphHeight / 2f;
        float tickHalfLength = axisTickWidth * 0.5f;
        float labelOffset = 0.1f;

        int numXTicks = Mathf.CeilToInt(halfWidth / step);
        for (int i = -numXTicks; i <= numXTicks; i++)
        {
            float x = i * step;
            if (Mathf.Abs(x) < 0.001f) continue;

            string unitText = label == "mm" ? (x * 10f).ToString("0") + label : x.ToString("0.#") + label;

            DrawLine($"x_tick_{step}_{i}", new Vector2(x, -tickHalfLength), new Vector2(x, tickHalfLength), colorX, parent, axisWidth * tickLineWidthMultiplier);
            DrawLine($"x_grid_{step}_{i}", new Vector2(x, -halfHeight), new Vector2(x, halfHeight), new Color(0.7f, 0.7f, 0.7f, 0.1f), parent, axisWidth * gridLineWidthMultiplier);
            AddText(unitText, new Vector3(x, -tickHalfLength * 2f - labelOffset, 0f), scale, $"x_label_{step}_{i}", parent);
        }

        int numYTicks = Mathf.CeilToInt(halfHeight / step);
        for (int i = -numYTicks; i <= numYTicks; i++)
        {
            float y = i * step;
            if (Mathf.Abs(y) < 0.001f) continue;

            string unitText = label == "mm" ? (y * 10f).ToString("0") + label : y.ToString("0.#") + label;

            DrawLine($"y_tick_{step}_{i}", new Vector2(-tickHalfLength, y), new Vector2(tickHalfLength, y), colorY, parent, axisWidth * tickLineWidthMultiplier);
            DrawLine($"y_grid_{step}_{i}", new Vector2(-halfWidth, y), new Vector2(halfWidth, y), new Color(0.7f, 0.7f, 0.7f, 0.1f), parent, axisWidth * gridLineWidthMultiplier);
            AddText(unitText, new Vector3(tickHalfLength * 2f + labelOffset, y, 0f), scale, $"y_label_{step}_{i}", parent);
        }
    }


    private void AddText(string text, Vector3 position, float scale, string name, GameObject parent)
    {
        GameObject tmProObject = new GameObject(name);
        tmProObject.transform.SetParent(parent.transform, false);
        TextMeshPro tm = tmProObject.AddComponent<TextMeshPro>();

        tm.text = text;
        tm.fontSize = unitFontSize;
        tm.alignment = TextAlignmentOptions.Center;
        tm.transform.localPosition = position;
        tm.transform.localScale = Vector3.one * scale;
        tm.sortingLayerID = SortingLayer.NameToID("Labels");
        tm.sortingOrder = 3;

        allLabels.Add(tm);
    }

    private GameObject DrawLine(string name, Vector2 startPoint, Vector2 endPoint, Color color, GameObject parent = null, float customWidth = -1f)
    {
        GameObject lineObj = new GameObject(name);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = color;
        line.widthMultiplier = customWidth > 0 ? customWidth : axisWidth / 2f;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        line.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0f));

        line.sortingLayerName = name.Contains("grid") ? "Grid" : "Ticks";
        line.sortingOrder = name.Contains("grid") ? 0 : 2;

        lineObj.transform.parent = parent ? parent.transform : transform;
        return lineObj;
    }
}
