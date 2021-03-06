﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    private LineRenderer line;
    private bool mousePress;
    private List<Vector3> pointsList;
    private Vector3 mousePos;
    private GameObject playerSword;
    private GameObject secondSword;
    private GameObject playerHalberd;

    public int sword, long_sword, halberd;

    public enum Pattern { none, sword, long_sword, halberd, dual_wield, hammer, scythe}
    private Pattern weapon;

    struct myLine
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
    };

    void Awake()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetVertexCount(0);
        line.SetWidth(0.1f, 0.1f);
        line.SetColors(Color.green, Color.green);
        line.useWorldSpace = true;
        mousePress = false;
        pointsList = new List<Vector3>();
        weapon = Pattern.long_sword;
        playerSword = GameObject.Find("First_Sword");
        secondSword = GameObject.Find("Second_Sword");
        playerHalberd = GameObject.Find("Halberd");
    }//end Awake
	
    void Update()
    {
        Camera cam = GetComponent<Camera>();
        //enable ortho drawing
        cam.orthographic = true;
        if (Input.GetMouseButtonDown(0))
        {
            mousePress = true;
            line.SetVertexCount(0);
            pointsList.RemoveRange(0, pointsList.Count);
            line.SetColors(Color.green, Color.green);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mousePress = false;
            
            setWeapon(pointsList[0].x, pointsList[pointsList.Count - 1].x);
        }

        if (mousePress)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (!pointsList.Contains(mousePos))
            {
                pointsList.Add(mousePos);
                line.SetVertexCount(pointsList.Count);
                line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                if (lineCollider())
                {
                    mousePress = false;
                    line.SetColors(Color.red, Color.red);
                }
            }
        }
        cam.orthographic = false;

        //reanable and disable according weapons
        playerHalberd.SetActive(weapon == Pattern.halberd);
        secondSword.SetActive(weapon == Pattern.dual_wield);
        GameObject sword = playerSword;
        sword.SetActive(weapon == Pattern.dual_wield || weapon == Pattern.sword);
        sword.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
        if (weapon == Pattern.long_sword)
        {
            
            sword.SetActive(true);
            sword.transform.localScale = new Vector3(0.0025f, 0.0035f, 0.0025f);
        }

        //set the sword transparency
        Renderer rende = playerSword.GetComponent<Renderer>();
        rende.material.color = new Color(rende.material.color.r, rende.material.color.g, rende.material.color.b
            , PlayerMovement.sand / 100);

        rende = secondSword.GetComponent<Renderer>();
        rende.material.color = new Color(rende.material.color.r, rende.material.color.g, rende.material.color.b
            , PlayerMovement.sand / 100);

        rende = playerHalberd.GetComponent<Renderer>();
        rende.material.color = new Color(rende.material.color.r, rende.material.color.g, rende.material.color.b
            , PlayerMovement.sand / 100);


    }//end update

    private bool lineCollider()
    {
        if (pointsList.Count < 2) { return false; }

        int TotalLines = pointsList.Count - 1;
        myLine[] lines = new myLine[TotalLines];

        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i].startPoint = (Vector3)pointsList[i];
                lines[i].endPoint = (Vector3)pointsList[i + 1];
            }
        }

        for (int i = 0; i < TotalLines - 1; i++)
        {
            myLine currentLine;
            currentLine.startPoint = (Vector3)pointsList[pointsList.Count - 2];
            currentLine.endPoint = (Vector3)pointsList[pointsList.Count - 1];

            if (intersection(lines[i], currentLine)) { return true; }
        }

        return false;
    }//end line collider

    private bool checkPoints(Vector3 pointA, Vector3 pointB) { return (pointA.x == pointB.x && pointA.y == pointB.y); }

    private bool intersection(myLine L1, myLine L2)
    {
        if (checkPoints(L1.startPoint, L2.startPoint) ||
            checkPoints(L1.startPoint, L2.endPoint) ||
            checkPoints(L1.endPoint, L2.startPoint) ||
            checkPoints(L1.endPoint, L2.endPoint))
            return false;

        return ((Mathf.Max(L1.startPoint.x, L1.endPoint.x) >= Mathf.Min(L2.startPoint.x, L2.endPoint.x)) &&
               (Mathf.Max(L2.startPoint.x, L2.endPoint.x) >= Mathf.Min(L1.startPoint.x, L1.endPoint.x)) &&
               (Mathf.Max(L1.startPoint.y, L1.endPoint.y) >= Mathf.Min(L2.startPoint.y, L2.endPoint.y)) &&
               (Mathf.Max(L2.startPoint.y, L2.endPoint.y) >= Mathf.Min(L1.startPoint.y, L1.endPoint.y))
               );
    }

    private void setWeapon(double firstX, double secondX)
    {
        if (lineCollider()) { weapon = Pattern.dual_wield; }
        else
        {
            double difference = secondX - firstX;

            if(difference < long_sword) { weapon = Pattern.sword; }
            else if (difference < halberd) { weapon = Pattern.long_sword; }
            else if (difference >= halberd) { weapon = Pattern.halberd; }
            else { weapon = Pattern.none; }

        }

        print(weapon.ToString());//***
    }

    public Pattern getWeapon(int firstX, int secondX)
    {


        return weapon;
    }

}