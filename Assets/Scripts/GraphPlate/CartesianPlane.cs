using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartesianPlane : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private float axisWidth = 0.05f;
    [SerializeField] private Color colorX = Color.red;
    [SerializeField] private Color colorY = Color.green;
    [SerializeField] private float graphHeight = 10f;
    [SerializeField] private float graphWidth = 10f;
    [SerializeField] private Material lineMaterial;

    [Header("Point Settings")]
    [SerializeField] private int numAxisUnits = 10;
    [SerializeField] private float axisTickWidth = 0.025f;
    [SerializeField] private float unitFontSize = 3f;
    [SerializeField] private TMP_FontAsset unitFont;

    private GameObject canvasObj;
    [SerializeField] private GameObject mainAxesParent;
    [SerializeField] private GameObject mainGridParent;

    private float lastZoomLevel = -1f;

    private List<GameObject> axesChildren = new List<GameObject>();
    private List<GameObject> gridChildren = new List<GameObject>();
    private List<GameObject> canvasParents = new List<GameObject>();

    private readonly float[] scales = { 0.3f, 1.5f, 2.5f, 4.5f, 5.5f };
    private readonly float[] zoomThresholds = { 1f, 5f, 10f, 15f, float.MaxValue };
    private readonly float[] unitSteps = { 0.1f, 0.5f, 1f, 5f, 10f };

    void Start()
    {
        canvasObj = new GameObject("WorldSpaceCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;
        canvasObj.AddComponent<GraphicRaycaster>();
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100, 100);

        for (int i = 0; i < scales.Length; i++)
        {
            float scale = scales[i];
            float approxZoom = zoomThresholds[i];
            float step = GetBestUnitStep(approxZoom);
            string unitLabel = step < 1f ? "mm" : "cm";

            GameObject axesChild = new GameObject($"Axes_{i}");
            axesChild.transform.SetParent(mainAxesParent.transform);
            axesChildren.Add(axesChild);

            GameObject gridChild = new GameObject($"Grid_{i}");
            gridChild.transform.SetParent(mainGridParent.transform);
            gridChildren.Add(gridChild);

            GameObject canvasParent = new GameObject($"Canvas_{i}");
            canvasParent.transform.SetParent(canvasObj.transform);
            canvasParents.Add(canvasParent);

            DrawAxesAndTicks(step, unitLabel, scale, axesChild.transform, canvasParent.transform);
            DrawGridLines(step, scale, gridChild.transform);

            axesChild.SetActive(false);
            gridChild.SetActive(false);
            canvasParent.SetActive(false);
        }

        UpdateGridActive(Camera.main.orthographicSize);
    }

    void Update()
    {
        float zoom = Camera.main.orthographicSize;
        if (Mathf.Abs(zoom - lastZoomLevel) > 0.1f)
        {
            lastZoomLevel = zoom;
            UpdateGridActive(zoom);
        }
    }

    private void UpdateGridActive(float zoom)
    {
        int activeIndex = 0;
        for (int i = 0; i < zoomThresholds.Length; i++)
        {
            if (zoom < zoomThresholds[i])
            {
                activeIndex = i;
                break;
            }
        }

        for (int i = 0; i < axesChildren.Count; i++)
        {
            bool active = (i == activeIndex);
            axesChildren[i].SetActive(active);
            gridChildren[i].SetActive(active);
            canvasParents[i].SetActive(active);
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
        return unitSteps[^1];
    }

    private void DrawAxesAndTicks(float step, string label, float scale, Transform axesParent, Transform canvasParent)
    {
        Vector3 center = Vector3.zero;

        DrawLinePreGenerated("xAxis", new Vector2(center.x - graphWidth / 2f, center.y), new Vector2(center.x + graphWidth / 2f, center.y), colorX, axisWidth, axesParent);
        DrawLinePreGenerated("yAxis", new Vector2(center.x, center.y - graphHeight / 2f), new Vector2(center.x, center.y + graphHeight / 2f), colorY, axisWidth, axesParent);

        DrawAxisTicksPreGenerated("xTicks", center, graphWidth, true, step, label, scale, axesParent, canvasParent);
        DrawAxisTicksPreGenerated("yTicks", center, graphHeight, false, step, label, scale, axesParent, canvasParent);
    }

    private void DrawGridLines(float step, float scale, Transform gridParent)
    {
        Vector3 center = Vector3.zero;
        float halfWidth = graphWidth / 2f;
        float halfHeight = graphHeight / 2f;

        float gridLineWidth = axisWidth / 4f;
        Color gridColor = new Color(0.85f, 0.85f, 0.85f, 0.1f);

        int numXTicks = Mathf.CeilToInt(halfWidth / step);
        int numYTicks = Mathf.CeilToInt(halfHeight / step);

        for (int i = -numXTicks; i <= numXTicks; i++)
        {
            if (i == 0) continue;
            float x = center.x + i * step;
            DrawLinePreGenerated($"GridLine_Vertical_{i}", new Vector3(x, center.y - halfHeight, 0), new Vector3(x, center.y + halfHeight, 0), gridColor, gridLineWidth, gridParent);
        }

        for (int i = -numYTicks; i <= numYTicks; i++)
        {
            if (i == 0) continue;
            float y = center.y + i * step;
            DrawLinePreGenerated($"GridLine_Horizontal_{i}", new Vector3(center.x - halfWidth, y, 0), new Vector3(center.x + halfWidth, y, 0), gridColor, gridLineWidth, gridParent);
        }
    }

    private void DrawAxisTicksPreGenerated(string name, Vector3 center, float length, bool isXAxis, float unitStep, string unitLabel, float scale, Transform axesParent, Transform canvasParent)
    {
        float halfLength = length / 2f;
        float tickSize = axisTickWidth;
        float labelOffset = 0.1f;

        int numTicks = Mathf.CeilToInt(halfLength / unitStep);
        for (int i = -numTicks; i <= numTicks; i++)
        {
            float value = i * unitStep;
            if (Mathf.Abs(value) < 0.001f) continue;

            Vector3 tickStart, tickEnd, textPos;
            string label = unitLabel == "mm" ? (value * 10f).ToString("0") + unitLabel : value.ToString("0.#") + unitLabel;

            if (isXAxis)
            {
                float x = center.x + value;
                tickStart = new Vector3(x, center.y - tickSize / 2f, 0f);
                tickEnd = new Vector3(x, center.y + tickSize / 2f, 0f);
                textPos = new Vector3(x, center.y - tickSize - labelOffset, 0f);
                DrawLinePreGenerated($"{name}_tick_{i}", tickStart, tickEnd, colorX, axisWidth / 2f, axesParent);
            }
            else
            {
                float y = center.y + value;
                tickStart = new Vector3(center.x - tickSize / 2f, y, 0f);
                tickEnd = new Vector3(center.x + tickSize / 2f, y, 0f);
                textPos = new Vector3(center.x + tickSize + labelOffset, y, 0f);
                DrawLinePreGenerated($"{name}_tick_{i}", tickStart, tickEnd, colorY, axisWidth / 2f, axesParent);
            }

            AddTextPreGenerated(label, textPos, scale, canvasParent);
        }
    }

    private void AddTextPreGenerated(string text, Vector3 position, float scale, Transform parent)
    {
        GameObject labelObj = new GameObject(text);
        labelObj.transform.SetParent(parent);
        TextMeshPro tm = labelObj.AddComponent<TextMeshPro>();

        tm.text = text;
        tm.font = unitFont;
        tm.fontSize = unitFontSize;
        tm.alignment = TextAlignmentOptions.Center;
        tm.transform.position = position;
        tm.transform.localScale = Vector3.one * scale;
    }

    private void DrawLinePreGenerated(string name, Vector2 start, Vector2 end, Color color, float width, Transform parent)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(parent);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPositions(new[] { (Vector3)start, (Vector3)end });
        lr.startWidth = lr.endWidth = width;
        lr.material = lineMaterial;
        lr.material.color = color;
        lr.useWorldSpace = true;
    }
}
