
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelection : MonoBehaviour
{

    [SerializeField] LayerMask unitLayerMask;
    [SerializeField] RectTransform selectionBox;

    List<Unit> selectedList = new List<Unit>();
    Vector2 startMousePosition;

    //QuadTree quadTree;

    /* [SerializeField] */
    Camera camera;
    private void Start()
    {
        selectionBox.gameObject.SetActive(false);
        camera = Camera.main;
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mousePos = Input.mousePosition;
        //    quadTree.Insert(new Vector3(mousePos.x, 0, mousePos.y));
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    List<Rect> quads = new List<Rect>();
        //    quadTree.GetTree(ref quads);

        //    foreach (Rect quad in quads)
        //    {
        //        Debug.Log(quad.width);
        //        RectTransform tran = Instantiate(selectionBox, this.transform);
        //        tran.gameObject.SetActive(true);
        //        tran.sizeDelta = new Vector2(quad.width, quad.height);
        //        tran.position = new Vector3(quad.x, quad.y);

        //    }
        //}
        HandleLeftMouse();
        HandleRightMouse();
    }

    private void HandleRightMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            //Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log($"Sending ray from {ray.origin} in {transform.TransformDirection(Vector3.forward)}");
            if (Physics.Raycast(ray, out hit))
            {

                GameMaster.Instance.SetTargetPosition(selectedList, hit.point);
            }
        }
    }
    void HandleLeftMouse()
    {
        //start hold
        if (Input.GetMouseButtonDown(0))
        {
            selectedList = new List<Unit>();
            startMousePosition = Input.mousePosition;
        }
        //end hold
        if (Input.GetMouseButtonUp(0))
        {
            onLeftMouseRelease();
            selectionBox.gameObject.SetActive(false);
        }
        //during hold
        if (Input.GetMouseButton(0))
        {
            UpdateSelection(Input.mousePosition);
        }
    }
    private void onLeftMouseRelease()
    {
        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);
        var unitPositions = GameMaster.Instance.GetBoisPositionArray();
        selectedList = new List<Unit>();
        Boid[] boidArray = GameMaster.Instance.GetBoidArray();
        for (int i = 0; i < unitPositions.Length; i++)
        {
            Vector2 unitScreenPosition = camera.WorldToScreenPoint(unitPositions[i]);

            if (unitScreenPosition.x > min.x && unitScreenPosition.x < max.x && unitScreenPosition.y > min.y && unitScreenPosition.y < max.y)
            {
                selectedList.Add(boidArray[i].unit);
                boidArray[i].unit.Select();
            }
        }
        Debug.Log(selectedList.Count);
    }
    private void UpdateSelection(Vector2 mousePosition)
    {
        selectionBox.gameObject.SetActive(true);
        float width = mousePosition.x - startMousePosition.x;
        float heght = mousePosition.y - startMousePosition.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(heght));
        selectionBox.anchoredPosition = startMousePosition + new Vector2(width / 2, heght / 2);
    }


}
