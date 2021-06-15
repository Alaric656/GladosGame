using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
 
    private float fExplosionRadius = 1.5f;

    private int iHighlightFidelity = 30; //Number of points in radius circle
    public Color highlightColour;
    public Material highlightMaterial;

    private MeshRenderer radiusMeshRenderer;
    private MeshFilter radiusMeshFilter;
    private Mesh radiusMesh;

    private GameObject radiusObject;
    void Start()
    {
        
    }


    public void SetExplosionRadius(float r)
    {
        fExplosionRadius = r;
        CreateRadius();
    }

    public void Explode()
    {

    }

    public void ShowRadius()
    {
        if (radiusObject == null)
        {
            CreateRadius();
        }
        radiusObject.SetActive(true);
    }

    public void HideRadius()
    {
        radiusObject.SetActive(false);
    }

    public void CreateRadius()
    {

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Wall"); //Only allows our raycast to hit walls
        if (radiusObject != null)
        {
            radiusObject.GetComponent<MeshFilter>().mesh.Clear();
        }
        else
        {
            radiusObject = new GameObject();
            radiusObject.transform.parent = gameObject.transform;
            radiusObject.transform.position = Vector3.zero;
            radiusMeshFilter = radiusObject.AddComponent<MeshFilter>();
            radiusMeshRenderer = radiusObject.AddComponent<MeshRenderer>();
            radiusMeshRenderer.material = highlightMaterial;
            radiusMeshRenderer.sortingOrder = 1;
        }
        radiusMesh = radiusMeshFilter.mesh;

        float fAngle = 0f;
        RaycastHit2D rayHit;

        //Find the points to create the shape.
        Vector3[] Vertices = new Vector3[iHighlightFidelity+1];
        Vertices[0] = gameObject.transform.position; //The centre point that all other points connect to to form the triangles that build the circle
        for(int x=1; x < Vertices.Length; x++)
        {
            Vector3 nextPoint = new Vector3(0, 0, 0);
            nextPoint.x = gameObject.transform.position.x + (fExplosionRadius * Mathf.Cos(fAngle));
            nextPoint.y = gameObject.transform.position.y + (fExplosionRadius * Mathf.Sin(fAngle));
            Vector3 direction = nextPoint - gameObject.transform.position;

            rayHit = Physics2D.Raycast(gameObject.transform.position,direction,direction.magnitude,layerMask);
            if (rayHit.collider != null)
            {
                nextPoint = rayHit.point;
            }
            Vertices[x] = nextPoint;
            fAngle += Mathf.PI * 2 / iHighlightFidelity;
        }

        //Creates the triangles list, which is a list of integers in groups of 3 that point at the index of each vertex in the vetice list
        int[] Triangles = new int[((Vertices.Length-2) *3) +3]; //Add an extra 3 for the final segment

        int v1 = 0;
        int v2 = Vertices.Length - 1;
        int v3 = Vertices.Length - 2; 
        for(int x=0; x<Triangles.Length-3; x += 3)
        {
            Triangles[x] = v1;
            Triangles[x + 1] = v2;
            Triangles[x + 2] = v3;
            v2--;
            v3--;
        }
        Triangles[Triangles.Length - 3] = 0;
        Triangles[Triangles.Length - 2] = 1;
        Triangles[Triangles.Length - 1] = Vertices.Length - 1; //these three points are the final segment


        radiusMesh.vertices = Vertices;
        radiusMesh.triangles = Triangles;

        radiusMesh.RecalculateNormals();
        radiusMesh.RecalculateBounds();
        radiusMeshFilter.mesh = radiusMesh;
        radiusMeshRenderer.sortingOrder = 1;
    }
}

