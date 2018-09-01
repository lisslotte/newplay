using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
public interface IOnClickHandler
{
    void OnClick(Player myPlayer);
}
public class ClickManager : MonoBehaviour
{
    public GameObject clickEff;
    Ray ray  ;
    RaycastHit hitInfo;
    Highlighter lastHighlighter = null;
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo)) 
            {
                GameObject go = hitInfo.collider.gameObject;
                if (go.tag == "Player")
                {
                    try { go.GetComponent<IOnClickHandler>().OnClick(GameManager.instance.myPlayer); } catch { }
                }
                else 
                {
                    try
                    {
                        GameManager.instance.myPlayer.NavToPoint(hitInfo.point);
                        Instantiate(clickEff, hitInfo.point, Quaternion.identity);
                    }
                    catch { }
                }
            }
        }
        else
        {
            SetHightlight();
        }
    }
    void SetHightlight()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject go = hitInfo.collider.gameObject;
            if (go.tag == "Player")
            {
                if (lastHighlighter == null)
                {
                    lastHighlighter = go.GetComponent<Highlighter>();
                    lastHighlighter.ConstantOn(Color.green);
                }
            }
            else
            {
                if (lastHighlighter!=null)
                {
                    lastHighlighter.ConstantOff();
                    lastHighlighter = null;
                }
            }
        }
    }

}
