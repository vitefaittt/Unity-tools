using System.Collections;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(GameobjectCreator))]
public class MeshCutter_Selection : MonoBehaviour {

    MeshCutterAsync cutter;
    Transform CutterGoTransform { get { return cutter.GameObjectToEdit.transform; } }
    Mesh meshToEdit;

    public Bounds LastBounds { get; private set; }

    Hand CurrentHand { get { return creator.currentHand; } }
    Vector3 HandPosition { get { return Player.instance.transform.TransformPoint(SteamVR_Input._default.inActions.Pose.GetLocalPosition(CurrentHand.handType)); } }

    [SerializeField]
    bool useControllerRotation = false;

    GameobjectCreator creator;

    [SerializeField]
    Color initHighlightColor = Color.white;
    Color invertedHighlightColor;
    [SerializeField]
    bool highlightSelection = true;

    private void Awake()
    {
        cutter = GetComponent<MeshCutterAsync>();

        // Validate selection when the GOCreator is done.
        creator = GetComponent<GameobjectCreator>();
        creator.Creating += OnCreationStarting;
        creator.Created += OnCreationComplete;

        invertedHighlightColor = new Color(1 - initHighlightColor.r, 1 - initHighlightColor.g, 1 - initHighlightColor.b);
    }

    void OnCreationStarting()
    {
        Destroy(creator.newGameObject);

        if (LastBounds != null && LastBounds.Contains(HandPosition))
        {
            // Cut if the player clicks inside of the previous created GO.
            if (highlightSelection && meshToEdit)
                meshToEdit.colors = new Color[0];
            cutter.TryCutMesh(LastBounds);
            creator.CancelCreation();
        }
        else
        {
            if (highlightSelection)
                StartCoroutine(UpdateDisplayedSelection());
        }
    }

    void OnCreationComplete()
    {
        if (creator.newGameObject.GetComponent<BoxCollider>())
        {
            LastBounds = creator.newGameObject.GetComponent<BoxCollider>().bounds;
            StartCoroutine(UpdateControllerHoverNotif());
        }
    }

    IEnumerator UpdateControllerHoverNotif()
    {
        bool leftIsIn = false;
        bool rightIsIn = false;
        while (!creator.isCreating)
        {
            // Trigger a haptic pulse when the hand gets in or out of the selection box.
            if (LastBounds.Contains(PlayerToolsManager.instance.LeftHand.transform.position) != leftIsIn)
            {
                leftIsIn = !leftIsIn;
                PlayerToolsManager.instance.LeftHand.TriggerHapticPulse(500);
            }
            if (LastBounds.Contains(PlayerToolsManager.instance.RightHand.transform.position) != rightIsIn)
            {
                rightIsIn = !rightIsIn;
                PlayerToolsManager.instance.RightHand.TriggerHapticPulse(500);
            }
            yield return null;
        }
    }


    #region old: Display the selection on the mesh to cut (too expensive) ----------------------------
    IEnumerator UpdateDisplayedSelection()
    {
        // Get the go to cut's mesh.
        cutter.InitInputs();
        meshToEdit = cutter.GameObjectToEdit.GetComponent<MeshFilter>().mesh;

        // Get the selection collider.
        Collider selectionCol = new Collider();
        do
        {
            if (creator.newGameObject)
                selectionCol = creator.newGameObject.GetComponent<Collider>();
            yield return null;
        } while (!selectionCol && creator.isCreating);

        // Change the colors of the go to cut's vertices contained in the selection.
        while (creator.isCreating)
        {
            Color[] colors = new Color[cutter.InputVertices.Length];
            for (int i = 0; i < cutter.InputVertices.Length; i++)
                colors[i] = selectionCol.bounds.Contains(cutter.InputVertices[i]) ? invertedHighlightColor : Color.white;
            meshToEdit.colors = colors;
            yield return null;
        }
    }
    #endregion  -----------------------------------------------------------------------------------
}
