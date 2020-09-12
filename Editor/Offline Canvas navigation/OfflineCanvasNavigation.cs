using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OfflineCanvasNavigation : MonoBehaviour
{
    [SerializeField]
    GraphicRaycaster raycaster;

    readonly float targetScreenRatio = 1.7778f;


    void Reset()
    {
        this.RenameFromType();
    }

#if UNITY_EDITOR
    public void Navigate()
    {
        // Il y a un problème : quand la fenêtre est trop grande (scale du canvas > 1), la position du curseur ne matche pas le point auquel le raycaster fait son raycast.
        // Faire aller la position du curseur entre 0 et 1, et s'arranger pour se baser sur ça dans ce qu'on donne au raycaster ?


        // Get the mouse position within the gameWindow viewport.
        Vector2 mousePosOnScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        EditorWindow gameWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView, UnityEditor"));
        EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneView, UnityEditor"));
        float windowWidth = gameWindow.position.width;
        float windowHeight = gameWindow.position.height - 20;
        Vector2 mousePosInGameWindow = mousePosOnScreen - gameWindow.position.position;
        Vector2 mousePosInViewport = mousePosInGameWindow.SetY(mousePosInGameWindow.y - 40);
        mousePosInViewport = mousePosInViewport.SetY(windowHeight - mousePosInViewport.y);

        // Get the mouse position within the gameWindow render.
        Vector2 mousePosInRender = mousePosInViewport;
        float screenRatio = windowWidth / windowHeight;
        if (screenRatio < targetScreenRatio)
        {
            float correctHeight = windowWidth / 1.7778f;
            float heightOffset = (windowHeight - correctHeight) * .5f;
            mousePosInRender = mousePosInRender.SetY(mousePosInRender.y - heightOffset);
        }
        else if (screenRatio > targetScreenRatio)
        {
            float correctWidth = windowHeight * 1.7778f;
            float widthOffset = (windowWidth - correctWidth) * .5f;
            mousePosInRender = mousePosInRender.SetX(mousePosInRender.x - widthOffset);
        }

        // Get UI elements under the cursor.
        PointerEventData eventData = new PointerEventData(null);
        eventData.position = mousePosInRender;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        if (results.Count < 1)
            return;

        // If the UI element is a button or a child of a button, click it.
        if (results[0].gameObject.GetComponent<Button>())
            UIUtilities.ExecuteOnClickManually(results[0].gameObject.GetComponent<Button>());
        else if (results[0].gameObject.transform.parent.GetComponent<Button>())
            UIUtilities.ExecuteOnClickManually(results[0].gameObject.transform.parent.GetComponent<Button>());
    }
#endif
}
