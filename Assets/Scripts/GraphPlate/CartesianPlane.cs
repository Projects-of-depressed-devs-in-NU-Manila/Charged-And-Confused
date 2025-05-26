using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CartesianPlane : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private float axisWidth;
    [SerializeField] private Color colorX;
    [SerializeField] private Color colorY;
    [SerializeField] private float graphHeight;
    [SerializeField] private float graphWidth;


    [Header("Point Settings")]
    [SerializeField] private int numAxisUnits;
    [SerializeField] private float axisTickWidth;
    [SerializeField] private float unitFontSize;
    private GameObject canvasObj;
    private float lastZoomLevel = -1f;
    private readonly float[] unitSteps = { 0.1f, 0.5f, 1f, 5f, 10f }; // 1mm < 5mm < 1cm < 5cm < 10cm


    void Start()
    {
        Vector3 center = new Vector3(0,0);
        center.z = 0f;

        Vector2 startPoint = new Vector2(center.x - graphWidth /2f, center.y);
        Vector2 endPoint = new Vector2(center.x + graphWidth/2f, center.y);
        LineRenderer lr;

        canvasObj = new GameObject("Canvas");
        canvasObj.AddComponent<Canvas>();

        lr = DrawLine("xAxisObj", startPoint, endPoint, colorX, axisWidth);
        lr.transform.parent = transform;

        startPoint = new Vector2(center.x, center.y - graphHeight / 2f);
        endPoint = new Vector2(center.x, center.y + graphHeight / 2f);
        lr = DrawLine("yAxisObj", startPoint, endPoint, colorY, axisWidth);
        lr.transform.parent = transform;

        DrawAxisTicks("xUnits", center, graphWidth, true, 1, "cm", 1f);
        DrawAxisTicks("yUnits", center, graphHeight, false, 1, "cm", 1f);
    }

    void Update()
    {
        float zoom = Camera.main.orthographicSize;
        if (Mathf.Abs(zoom - lastZoomLevel) > 0.1f)
        {
            lastZoomLevel = zoom;
            UpdateGraphScale(zoom);
        }
    }

    private float GetBestUnitStep(float zoom)
    {
        float desiredSpacing = zoom / 10f;  //spacing per unit

        foreach (float step in unitSteps)
        {
            float stepInWorld = step; 
            if (stepInWorld >= desiredSpacing)
                return step;
        }

        return unitSteps[unitSteps.Length - 1];
    }

    private void UpdateGraphScale(float zoom)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        foreach (Transform child in canvasObj.transform)
            Destroy(child.gameObject);

        float step = GetBestUnitStep(zoom);
        string unitLabel = step < 1f ? "mm" : "cm"; // 1f is equal to 1 real world cm (also equivalent to 1 traversal unit in unity)

        float scale;
        if (zoom < 2f)
            scale = 0.3f;
        else if (zoom < 6f)
            scale = 1f;
        else if (zoom < 10f)
            scale = 2f;
        else if (zoom < 15f)
            scale = 3f;
        else
            scale = 4f;

        DrawAxes(step, unitLabel, scale);
    }

    private void DrawAxes(float step, string label, float scale)
    {
        Vector3 center = new Vector3(0,0);
        center.z = 0f;

        //Redraw main axes
        DrawLine("xAxisObj", new Vector2(center.x - graphWidth / 2f, center.y), new Vector2(center.x + graphWidth / 2f, center.y), colorX, axisWidth);
        DrawLine("yAxisObj", new Vector2(center.x, center.y - graphHeight / 2f), new Vector2(center.x, center.y + graphHeight / 2f), colorY, axisWidth);

        //Redraw ticks
        DrawAxisTicks("xUnits", center, graphWidth, true, step, label, scale);
        DrawAxisTicks("yUnits", center, graphHeight, false, step, label, scale);
    }

    private void DrawAxisTicks(string name, Vector3 center, float length, bool isXAxis, float unitStep, string unitLabel, float scale)
    {
        float halfLength = length / 2f;
        float tickSize = axisTickWidth;
        float labelOffset = 0.1f;

        int numTicks = Mathf.CeilToInt(halfLength / unitStep);
        for (int i = -numTicks; i <= numTicks; i++)
        {
            float value = i * unitStep;
            if (Mathf.Abs(value) < 0.001f) continue; // skip origin label

            Vector3 tickStart, tickEnd, textPos;
            string label;
            if (unitLabel == "mm")
                label = (value * 10f).ToString("0") + unitLabel;
            else
                label = value.ToString("0.#") + unitLabel;

            if (isXAxis)
            {
                float x = center.x + value;
                tickStart = new Vector3(x, center.y - tickSize / 2f, 0f);
                tickEnd = new Vector3(x, center.y + tickSize / 2f, 0f);
                textPos = new Vector3(x, center.y - tickSize - labelOffset, 0f);
                DrawLine($"{name}_tick_{i}", tickStart, tickEnd, colorX, axisWidth / 2f);
            }
            else
            {
                float y = center.y + value;
                tickStart = new Vector3(center.x - tickSize / 2f, y, 0f);
                tickEnd = new Vector3(center.x + tickSize / 2f, y, 0f);
                textPos = new Vector3(center.x + tickSize + labelOffset, y, 0f);
                DrawLine($"{name}_tick_{i}", tickStart, tickEnd, colorY, axisWidth / 2f);
            }

            AddText(label, textPos, canvasObj, scale);
        }
    }

    private void AddText(string text, Vector3 position, GameObject canvasObj, float scale)
    {
        GameObject tmProObject = new GameObject(text);
        tmProObject.transform.parent = canvasObj.transform;
        TextMeshPro tm = tmProObject.AddComponent<TextMeshPro>();

        tm.text = text;
        tm.fontSize = unitFontSize;
        tm.alignment = TextAlignmentOptions.Center;
        tm.transform.position = position;
        tm.transform.rotation = Quaternion.identity;
        tm.transform.localScale = Vector3.one * scale;
    }

    private LineRenderer DrawLine(string name, Vector2 startPoint, Vector2 endPoint, Color color, float lineWidth)
    {
        GameObject gameobj = new GameObject(name);
        LineRenderer line = gameobj.AddComponent<LineRenderer>();

        Vector3 startWorldPoint = new Vector3(startPoint.x, startPoint.y, 0f);
        Vector3 endWorldPoint = new Vector3(endPoint.x, endPoint.y, 0f);

        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = color;

        line.SetPosition(0, startWorldPoint);
        line.SetPosition(1, endWorldPoint);

        gameobj.transform.parent = transform;
        return line;
    }

    private void SetLinePos(float x, float y, LineRenderer line, int index)
    {
        Vector3 pointScreen = new Vector3(x, y);
        Vector3 pointWorld = Camera.main.ScreenToWorldPoint(pointScreen);
        pointWorld.z = 0f;
        line.SetPosition(index, pointWorld);
    }
}
