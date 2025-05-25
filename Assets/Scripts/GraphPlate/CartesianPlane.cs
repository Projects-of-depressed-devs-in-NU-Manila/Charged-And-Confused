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

    [Header("Point Settings")]
    [SerializeField] private int numAxisUnits;
    [SerializeField] private float axisTickWidth;
    [SerializeField] private float unitFontSize;
    private GameObject canvasObj;


    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector3 center = new Vector3(screenWidth / 2, screenHeight / 2, 0f);

        Vector2 startPoint = new Vector2(0f, center.y);
        Vector2 endPoint = new Vector2(screenWidth, center.y);
        LineRenderer lr;

        canvasObj = new GameObject("Canvas");
        canvasObj.AddComponent<Canvas>();

        // lr = DrawLine("xAxisObj", startPoint, endPoint, colorX, axisWidth);
        // lr.transform.parent = transform;

        // startPoint = new Vector2(center.x, 0f);
        // endPoint = new Vector2(center.x, screenHeight);
        // lr = DrawLine("yAxisObj", startPoint, endPoint, colorY, axisWidth);
        // lr.transform.parent = transform;

        lr = DrawAxisUnits("xUnits", numAxisUnits, colorX, center, screenWidth, screenHeight, axisTickWidth, 20.0f, "x");
        lr.transform.parent = transform;
        lr = DrawAxisUnits("xUnits", numAxisUnits, colorY, center, screenWidth, screenHeight, axisTickWidth, 20.0f, "y");
        lr.transform.parent = transform;
    }

    private LineRenderer DrawAxisUnits(string name, int numUnits, Color color, Vector3 center, float screenWidth, float screenHeight, float lineWidth, float tickerHieght, string axis)
    {
        GameObject gameObj = new GameObject(name);
        LineRenderer line = gameObj.AddComponent<LineRenderer>();

        line.positionCount = (numUnits + (3 * numUnits) + 4) * 2;
        line.startWidth = line.endWidth = lineWidth;

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = color;

        float distanceBetweenTicks = screenWidth / 2.0f / numUnits;
        float tickerHalfHeight = tickerHieght / 2.0f;

        int unitCount = numUnits * -1;
        if (axis == "x")
        {
            float curX = 0f;
            float curY;

            for (int i = 0; i < line.positionCount; i++)
            {
                SetLinePos(curX, center.y, line, i);
                i++;

                curY = center.y + tickerHalfHeight;
                SetLinePos(curX, curY, line, i);
                i++;

                curY = center.y - tickerHalfHeight;
                SetLinePos(curX, curY, line, i);
                if (unitCount != 0)
                {
                    AddText($"{unitCount.ToString()}cm", new Vector2(curX, curY), canvasObj);
                }
                unitCount++;   
                i++;

                curY = center.y;
                SetLinePos(curX, curY, line, i);
                curX += distanceBetweenTicks;
                // Debug.Log(i);
            }
        }
        else
        {
            float curX;
            float curY;
            curY = center.y - (screenWidth / 2.0f);

            for (int i = 0; i < line.positionCount; i++)
            {
                SetLinePos(center.x, curY, line, i);
                i++;


                curX = center.x - tickerHalfHeight;
                SetLinePos(curX, curY, line, i);
                i++;

                curX = center.x + tickerHalfHeight;
                SetLinePos(curX, curY, line, i);
                if (unitCount != 0)
                {
                    AddText($"{unitCount.ToString()}cm", new Vector2(curX+20, curY), canvasObj);
                }
                unitCount++;
                i++;

                curX = center.x;
                SetLinePos(curX, curY, line, i);
                curY += distanceBetweenTicks;
                // Debug.Log(i);
            }
        }

        return line;
    }

    private void AddText(string text, Vector2 position, GameObject canvasObj)
    {
        GameObject tmProObject = new GameObject(text);
        TextMeshPro tm = tmProObject.AddComponent<TextMeshPro>();
        tmProObject.transform.parent = canvasObj.transform;
        canvasObj.transform.parent = this.transform;
        tm.text = text;
        tm.fontSize = unitFontSize;
        tm.alignment = TextAlignmentOptions.Center;

        RectTransform rt = tm.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2f, 0f);

        Vector3 pointScreen = new Vector3(position.x, position.y - 10.0f);
        Vector3 pointWorld = Camera.main.ScreenToWorldPoint(pointScreen);
        pointWorld.z = 0f;

        tm.transform.position = pointWorld;
    }

    private LineRenderer DrawLine(string name, Vector2 startPoint, Vector2 endPoint, Color color, float lineWidth)
    {
        GameObject gameobj = new GameObject(name);
        LineRenderer line = gameobj.AddComponent<LineRenderer>();

        Vector3 startWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(startPoint.x, startPoint.y));
        startWorldPoint.z = 0f;
        Vector3 endWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(endPoint.x, endPoint.y));
        endWorldPoint.z = 0f;

        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = color;

        line.SetPosition(0, startWorldPoint);
        line.SetPosition(1, endWorldPoint);

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
