using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SketchManager : MonoBehaviour
{
    // Managers
    Transform _AppManager;
    Transform _ActionsManager;
    Transform _GrabManager;
    Transform _HistoryManager;

    // Headset & Controllers
    Transform headset;
    Transform leftController;
    Transform rightController;

    // Elements on controllers
    Transform leftSketchStarter;
    Transform rightSketchStarter;
    Transform leftBrush;
    Transform rightBrush;
    Transform leftEraser;
    Transform rightEraser;
    Transform leftGroupSelector;
    Transform rightGroupSelector;
    Transform leftAudioSource;
    Transform rightAudioSource;

    // Particles effects
    ParticleSystem leftBrushEffect;
    ParticleSystem rightBrushEffect;
    ParticleSystem leftEraserEffect;
    ParticleSystem rightEraserEffect;

    // Work & trash folders
    Transform workFolder;

    // Sketching
    [Header("Sketch & Erase")]
    public GameObject sketchPrefab;
    public GameObject eraserPrefab;
    GameObject mySketch;
    LineRenderer myLineRenderer;

    bool isSketching = false;
    bool isErasing = false;

    int pointsOnMySketch = 0;
    Color sketchColor = Color.white;
    Color lastSketchColor = Color.white;
    float sketchSize = 0.01f;
    float eraseSize = 0.01f;
    int sketchId = 0;
    float sketchTimer = 0;

    // Symmetric sketch
    [Header("Symmetric sketch")]
    public GameObject newSymmetryPlane;
    GameObject mySymmetryPlane;
    GameObject symmetryLeftBrush;
    GameObject symmetryRightBrush;
    GameObject mySymmetricSketch;
    LineRenderer mySymmetricLineRenderer;
    bool symmetryMode = false;

    // Sketch folder
    [Header("Sketch folder")]
    public GameObject sketchFolderPrefab;
    GameObject mySketchFolder;
    int folderNumber = 0;
    bool isSelectingGroup = false;

    // Sketching menu
    [Header("Menu")]
    public GameObject newSketchingMenu;
    GameObject sketchingMenu;
    bool sketchingMenuIsOpen = false;
    string currentMode = "SketchManager";
    string previousSketchingTool = "";

    // Pointer on menu
    [Header("Pointer")]
    public GameObject newPointer;
    GameObject leftPointer;
    GameObject rightPointer;
    bool pointerIsOpen = false;

    // Player
    bool rightHander = true;
    //bool wasRightHander = true;

    // Menu cleaner
    Transform menuCleaner;

    // Helpers on controllers
    bool isHelping = false;
    Transform mainHelperOnLeftController;
    Transform mainHelperOnRightController;
    private List<Transform> leftHelpers = new List<Transform>();
    private List<Transform> rightHelpers = new List<Transform>();

    // Other
    bool coroutineIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get other managers
        _AppManager = GameObject.Find("_AppManager").transform;
        _ActionsManager = GameObject.Find("_ActionsManager").transform;
        _GrabManager = GameObject.Find("_GrabManager").transform;
        _HistoryManager = GameObject.Find("_HistoryManager").transform;

        // Get the working folder
        if (workFolder == null && GameObject.Find("WorkFolder"))
        {
            workFolder = GameObject.Find("WorkFolder").transform;
        }

        // Get the menu cleaner
        menuCleaner = GameObject.Find("UX_MenuCleaner").transform;

        // Init controllers
        if (leftBrush == null || rightBrush == null)
        {
            InitControllers();
        }

        // Init Sketching menu
        InitSketchingMenu();

        // Init the symmetry mode
        InitSymmetryMode();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG ONLY
        /*if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            DisplaySketchFoldersByColor(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            DisplaySketchFoldersByColor(false);
        }*/

        // Read the sketching function if the user pressed the trigger
        if (isSketching == true)
        {
            IsPressingTrigger();
        }
    }



    #region INIT the SKETCH MANAGER (Controllers, Menu & Controllers elements)
    public void InitManager()
    {
        // Init controllers
        if (leftBrush == null || rightBrush == null)
        {
            InitControllers();
        }

        // Init Sketching menu
        InitSketchingMenu();

        // Init the symmetry mode
        InitSymmetryMode();
    }

    /// <summary>
    /// Init controllers, pointers & elements
    /// </summary>
    void InitControllers()
    {
        if (GameObject.FindGameObjectWithTag("LeftController"))
        {
            // Get the left controller
            leftController = GameObject.FindGameObjectWithTag("LeftController").transform;

            // Init the pointer on left controller
            leftPointer = Instantiate(newPointer, new Vector3(0, 0, 0), Quaternion.identity);
            leftPointer.name = "Sketcher_LeftPointer";
            leftPointer.transform.parent = leftController;
            leftPointer.transform.localPosition = new Vector3(0, 0, 0); // Only for HTC Vive
            leftPointer.SetActive(false);

            // Get the sketcher tool on left controller
            for (int x = 0; x < leftController.childCount; x++)
            {
                if (leftController.GetChild(x).name == "Controller_Sketcher")
                {
                    leftSketchStarter = leftController.GetChild(x);
                }
            }

            // Get list of helpers on left controller
            for (int x = 0; x < leftController.childCount; x++)
            {
                if (leftController.GetChild(x).name == "Controller_Helper")
                {
                    mainHelperOnLeftController = leftController.GetChild(x);

                    for (int y = 0; y < mainHelperOnLeftController.GetChild(0).childCount; y++)
                    {
                        leftHelpers.Add(mainHelperOnLeftController.GetChild(0).GetChild(y));
                    }
                }
            }

            // Get the audio source on left controller
            for (int y = 0; y < leftSketchStarter.childCount; y++)
            {
                if (leftSketchStarter.GetChild(y).name.StartsWith("AudioSource"))
                {
                    leftAudioSource = leftSketchStarter.GetChild(y);
                }
            }

            // Get the brush & the eraser on left controller
            for (int y = 0; y < leftSketchStarter.childCount; y++)
            {
                if (leftSketchStarter.GetChild(y).name == "Brush")
                {
                    leftBrush = leftSketchStarter.GetChild(y);
                    leftBrush.localScale = new Vector3(sketchSize * 200, sketchSize * 200, sketchSize * 200);
                    leftBrush.gameObject.SetActive(true);

                    leftBrushEffect = leftBrush.GetComponentInChildren<ParticleSystem>();
                    leftBrushEffect.Stop();

                }
                else if (leftSketchStarter.GetChild(y).name == "Eraser")
                {
                    leftEraser = leftSketchStarter.GetChild(y);
                    leftEraser.localScale = new Vector3(sketchSize * 200, sketchSize * 200, sketchSize * 200);
                    leftEraser.gameObject.SetActive(false);

                    leftEraserEffect = leftEraser.GetComponentInChildren<ParticleSystem>();
                    leftEraserEffect.Stop();
                }
                else if (leftSketchStarter.GetChild(y).name == "GroupSelector")
                {
                    leftGroupSelector = leftSketchStarter.GetChild(y);
                    leftGroupSelector.gameObject.SetActive(false);
                }
            }
        }

        if (GameObject.FindGameObjectWithTag("RightController"))
        {
            // Get the right controller
            rightController = GameObject.FindGameObjectWithTag("RightController").transform;

            // Init the pointer on the right controller
            rightPointer = Instantiate(newPointer, new Vector3(0, 0, 0), Quaternion.identity);
            rightPointer.name = "Sketcher_RightPointer";
            rightPointer.transform.parent = rightController;
            rightPointer.transform.localPosition = new Vector3(0, 0, 0); // Only for HTC Vive
            rightPointer.SetActive(false);

            // Get the sketcher tool on right controller
            for (int x = 0; x < rightController.childCount; x++)
            {
                if (rightController.GetChild(x).name == "Controller_Sketcher")
                {
                    rightSketchStarter = rightController.GetChild(x);
                }
            }

            // Get list of helpers on right controller
            for (int x = 0; x < rightController.childCount; x++)
            {
                if (rightController.GetChild(x).name == "Controller_Helper")
                {
                    mainHelperOnRightController = rightController.GetChild(x);

                    for (int y = 0; y < mainHelperOnRightController.GetChild(0).childCount; y++)
                    {
                        rightHelpers.Add(mainHelperOnRightController.GetChild(0).GetChild(y));
                    }
                }
            }

            // Get the audio source on right controller
            for (int y = 0; y < rightSketchStarter.childCount; y++)
            {
                if (rightSketchStarter.GetChild(y).name.StartsWith("AudioSource"))
                {
                    rightAudioSource = rightSketchStarter.GetChild(y);
                }
            }

            // Get the brush & the eraser on right controller
            for (int y = 0; y < rightSketchStarter.childCount; y++)
            {
                if (rightSketchStarter.GetChild(y).name == "Brush")
                {
                    rightBrush = rightSketchStarter.GetChild(y);
                    rightBrush.localScale = new Vector3(sketchSize * 200, sketchSize * 200, sketchSize * 200);
                    rightBrush.gameObject.SetActive(true);

                    rightBrushEffect = rightBrush.GetComponentInChildren<ParticleSystem>();
                    rightBrushEffect.Stop();
                }
                else if (rightSketchStarter.GetChild(y).name == "Eraser")
                {
                    rightEraser = rightSketchStarter.GetChild(y);
                    rightEraser.localScale = new Vector3(sketchSize * 200, sketchSize * 200, sketchSize * 200);
                    rightEraser.gameObject.SetActive(false);

                    rightEraserEffect = rightEraser.GetComponentInChildren<ParticleSystem>();
                    rightEraserEffect.Stop();
                }
                else if (rightSketchStarter.GetChild(y).name == "GroupSelector")
                {
                    rightGroupSelector = rightSketchStarter.GetChild(y);
                    rightGroupSelector.gameObject.SetActive(false);
                }
            }
        }

        if (GameObject.FindGameObjectWithTag("MenuLookAt"))
        {
            headset = GameObject.FindGameObjectWithTag("MenuLookAt").transform;
        }

        //Debug.Log("<b>[SketchManager]</b> Initialized");

        DisableHelp();
    }

    /// <summary>
    /// Init the sketching menu
    /// </summary>
    void InitSketchingMenu()
    {
        sketchingMenu = Instantiate(newSketchingMenu, new Vector3(0, -5, 0), Quaternion.identity);
        sketchingMenu.name = "Sketcher_Menu";
        sketchingMenu.transform.position = new Vector3(0, -5, 0);
        sketchingMenu.transform.localScale = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Init the symmetry mode
    /// </summary>
    void InitSymmetryMode()
    {
        if (mySymmetryPlane == null)
        {
            symmetryMode = false;

            // Init the symmetry plane
            mySymmetryPlane = Instantiate(newSymmetryPlane, new Vector3(0, 0, 0), Quaternion.identity);
            mySymmetryPlane.name = "SymmetryPlane";
            mySymmetryPlane.transform.parent = workFolder;
            mySymmetryPlane.transform.position = new Vector3(0, 1, 0);
            mySymmetryPlane.transform.eulerAngles = new Vector3(90, 0, 0);
            mySymmetryPlane.transform.localScale = new Vector3(1, 1, 1);
            mySymmetryPlane.SetActive(false);

            // Init the symmetry left brush
            symmetryLeftBrush = new GameObject();
            symmetryLeftBrush.name = "SymmetryLeftBrush";
            symmetryLeftBrush.transform.position = new Vector3(0, 0, 0);
            symmetryLeftBrush.transform.eulerAngles = new Vector3(0, 0, 0);
            symmetryLeftBrush.transform.localScale = new Vector3(1, 1, 1);

            // Init the symmetry right brush
            symmetryRightBrush = new GameObject();
            symmetryRightBrush.name = "SymmetryRightBrush";
            symmetryRightBrush.transform.position = new Vector3(0, 0, 0);
            symmetryRightBrush.transform.eulerAngles = new Vector3(0, 0, 0);
            symmetryRightBrush.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    #endregion


    #region ENABLE or DISABLE the SKETCH MANAGER
    /// <summary>
    /// Enable the Sketching mode
    /// </summary>
    public void EnableManager()
    {
        // Enable menu
        /*sketchingMenu.transform.localScale = new Vector3(0, 0, 0);
        sketchingMenu.transform.position = new Vector3(0, -5, 0);*/

        leftBrush.gameObject.SetActive(true);
        leftBrushEffect.Stop();
        rightBrush.gameObject.SetActive(true);
        rightBrushEffect.Stop();

        leftEraser.gameObject.SetActive(false);
        leftEraserEffect.Stop();
        rightEraser.gameObject.SetActive(false);
        rightEraserEffect.Stop();

        leftGroupSelector.gameObject.SetActive(false);
        rightGroupSelector.gameObject.SetActive(false);

        // Open menu
        OpenSketchingMenu(_ActionsManager.GetComponent<ActionsManager>().WasRightHander());
    }

    /// <summary>
    /// Disable the Sketching mode
    /// </summary>
    public void DisableManager()
    {
        // Stop the sketching action
        UnpressTriggger();

        // Disable menu
        sketchingMenu.transform.localScale = new Vector3(0, 0, 0);
        sketchingMenu.transform.position = new Vector3(0, -5, 0);
        sketchingMenuIsOpen = false;

        // Disable pointer
        leftPointer.gameObject.SetActive(false);
        rightPointer.gameObject.SetActive(false);

        // Disable elements
        leftBrush.gameObject.SetActive(false);
        rightBrush.gameObject.SetActive(false);
        leftEraser.gameObject.SetActive(false);
        rightEraser.gameObject.SetActive(false);
        leftGroupSelector.gameObject.SetActive(false);
        rightGroupSelector.gameObject.SetActive(false);

        /*if (sketchingMenuIsOpen == true)
        {
            CloseSketchingMenu();
        }*/
    }
    #endregion


    #region INTERACTIONS with CONTROLLERS (Trigger) (Sketching, Erasing, Select group...)
    /// <summary>
    /// Start a new sketch
    /// </summary>
    public void PressTrigger(bool myPlayerHander)
    {
        // Si le menu de dessin n'est pas ouvert
        if (sketchingMenuIsOpen == false)
        {
            // Si le mode "SKETCH" est activé
            if (currentMode.StartsWith("Sketch"))
            {
                // Si le joueur n'est pas déjà en train de dessiner
                if (isSketching == false)
                {
                    // Get the player hander
                    rightHander = myPlayerHander;

                    // Init controllers
                    if (leftBrush == null || rightBrush == null)
                    {
                        InitControllers();
                    }

                    // Instantiate the sketch
                    mySketch = Instantiate(sketchPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    mySketch.name = "Sketch_" + sketchId;
                    sketchId++;
                    myLineRenderer = mySketch.GetComponent<LineRenderer>();

                    // Get the color & the size from the brush
                    if (rightHander == true)
                    {
                        myLineRenderer.material.color = rightBrush.GetComponent<MeshRenderer>().material.color;
                        sketchSize = rightBrush.transform.localScale.x / 200;
                        rightBrushEffect.Play();
                    }
                    else
                    {
                        myLineRenderer.material.color = leftBrush.GetComponent<MeshRenderer>().material.color;
                        sketchSize = leftBrush.transform.localScale.x / 200;
                        leftBrushEffect.Play();
                    }

                    // Set the color of the sketch
                    myLineRenderer.startColor = Color.white;
                    myLineRenderer.endColor = Color.white;

                    // Set the size of the sketch
                    myLineRenderer.startWidth = sketchSize;
                    myLineRenderer.endWidth = sketchSize;

                    // Display a smooth line (start & end width)
                    AnimationCurve curve = new AnimationCurve();
                    curve.AddKey(0.0f, 0.0f);
                    curve.AddKey(0.1f, sketchSize);
                    curve.AddKey(0.9f, sketchSize);
                    curve.AddKey(1.0f, 0.0f);
                    myLineRenderer.widthCurve = curve;

                    // Si le joueur travaille en symétrie
                    if (symmetryMode == true)
                    {
                        // Instantiate the sketch
                        mySymmetricSketch = Instantiate(sketchPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        mySymmetricSketch.name = "Sketch_" + sketchId;
                        sketchId++;
                        mySymmetricLineRenderer = mySymmetricSketch.GetComponent<LineRenderer>();

                        // Get the color & the size from the brush
                        if (rightHander == true)
                        {
                            mySymmetricLineRenderer.material.color = rightBrush.GetComponent<MeshRenderer>().material.color;
                            sketchSize = rightBrush.transform.localScale.x / 200;
                        }
                        else
                        {
                            mySymmetricLineRenderer.material.color = leftBrush.GetComponent<MeshRenderer>().material.color;
                            sketchSize = leftBrush.transform.localScale.x / 200;
                        }

                        // Set the color of the sketch
                        mySymmetricLineRenderer.startColor = Color.white;
                        mySymmetricLineRenderer.endColor = Color.white;

                        // Set the size of the sketch
                        mySymmetricLineRenderer.startWidth = sketchSize;
                        mySymmetricLineRenderer.endWidth = sketchSize;

                        // Display a smooth line (start & end width)
                        mySymmetricLineRenderer.widthCurve = curve;
                    }

                    // Init the number of points in the sketch
                    pointsOnMySketch = 0;

                    // Début de lecture du fichier audio
                    if (rightHander == true)
                    {
                        rightAudioSource.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        leftAudioSource.GetComponent<AudioSource>().Play();
                    }

                    isSketching = true;
                }
            }

            // Si le mode "ERASER" est activé
            else if (currentMode.StartsWith("Eraser"))
            {
                // Si le joueur n'est pas déjà en train de dessiner
                if (isSketching == false)
                {
                    // Get the player hander
                    rightHander = myPlayerHander;

                    // Init controllers
                    if (leftBrush == null || rightBrush == null)
                    {
                        InitControllers();
                    }

                    // Send info to the erasing trigger
                    if (rightHander == true)
                    {
                        leftEraser.GetComponent<ErasingTrigger>().IsErasing(false);
                        rightEraser.GetComponent<ErasingTrigger>().IsErasing(true);
                        rightEraserEffect.Play();
                    }
                    else
                    {
                        rightEraser.GetComponent<ErasingTrigger>().IsErasing(false);
                        leftEraser.GetComponent<ErasingTrigger>().IsErasing(true);
                        leftEraserEffect.Play();
                    }

                    // Instantiate the sketch
                    mySketch = Instantiate(eraserPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    mySketch.name = "Eraser";
                    myLineRenderer = mySketch.GetComponent<LineRenderer>();

                    // Set the size of the sketch
                    myLineRenderer.startWidth = eraseSize;
                    myLineRenderer.endWidth = eraseSize;

                    // Display a smooth line (start & end width)
                    AnimationCurve curve = new AnimationCurve();
                    curve.AddKey(0.0f, 0.0f);
                    curve.AddKey(0.1f, eraseSize);
                    curve.AddKey(0.9f, eraseSize);
                    curve.AddKey(1.0f, 0.0f);
                    myLineRenderer.widthCurve = curve;

                    // Init the number of points in the sketch
                    pointsOnMySketch = 0;

                    // Début de lecture du fichier audio
                    if (rightHander == true)
                    {
                        rightAudioSource.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        leftAudioSource.GetComponent<AudioSource>().Play();
                    }

                    isErasing = true;
                    isSketching = true;
                }
            }

            // Si le mode "SELECT GROUP" est activé
            else if (currentMode.StartsWith("SelectGroup"))
            {
                // Si le joueur n'est pas déjà en train de dessiner
                if (isSelectingGroup == true)
                {
                    // Get the player hander
                    rightHander = myPlayerHander;

                    //Debug.Log("Check >>> " + rightGroupSelector.name + " " + leftGroupSelector);

                    // Get the selected group
                    if (rightHander == true && rightGroupSelector.GetComponent<SelectGroupTrigger>().GetSelectedGroup() != null)
                    {
                        mySketchFolder = rightGroupSelector.GetComponent<SelectGroupTrigger>().GetSelectedGroup().gameObject;
                    }
                    else if (rightHander == false && leftGroupSelector.GetComponent<SelectGroupTrigger>().GetSelectedGroup() != null)
                    {
                        mySketchFolder = leftGroupSelector.GetComponent<SelectGroupTrigger>().GetSelectedGroup().gameObject;
                    }
                    else
                    {
                        OpenSketchingMenu(rightHander);
                    }

                    //Debug.Log("Select : " + mySketchFolder);

                    // Init all groups colors
                    DisplaySketchFoldersByColor(false);

                    // Stop the group selection tool
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);
                    isSelectingGroup = false;

                    Debug.Log("<b>[SketchManager]</b> " + mySketchFolder.name + " selected");

                    UpdateMode("Sketch");
                }
            }
        }

        // Si le menu de dessin est ouvert
        else if (sketchingMenuIsOpen == true)
        {
            // Update the sketching mode
            //UpdateSketchingTool();

            if (myPlayerHander == rightHander)
            {
                UpdateMode("");
                rightHander = myPlayerHander;
            }
            else
            {
                //CloseSketchingMenu();
            }
        }

        //rightHander = myPlayerHander;
    }

    /// <summary>
    /// User is sketching
    /// </summary>
    public void IsPressingTrigger()
    {
        if (isSketching == true)
        {
            // Ajout des points à la ligne
            myLineRenderer.positionCount = pointsOnMySketch + 1;

            if (rightHander == true)
            {
                myLineRenderer.SetPosition(pointsOnMySketch, rightBrush.transform.position);
            }
            else
            {
                myLineRenderer.SetPosition(pointsOnMySketch, leftBrush.transform.position);
            }

            // Si le joueur travaille en symétrie
            if (symmetryMode == true && isErasing == false)
            {
                // Ajout des points à la ligne
                mySymmetricLineRenderer.positionCount = pointsOnMySketch + 1;

                if (rightHander == true)
                {
                    // Calcul de la symétrie de la brosse
                    Vector3 planeNormal_RightBrush = Vector3.Dot((mySymmetryPlane.transform.position - rightBrush.transform.position), mySymmetryPlane.transform.up) * mySymmetryPlane.transform.up;
                    Vector3 rightProjection = rightBrush.transform.position + planeNormal_RightBrush * 2;
                    symmetryRightBrush.transform.position = rightProjection;

                    // Mise à jour de la position du point symétrique
                    mySymmetricLineRenderer.SetPosition(pointsOnMySketch, symmetryRightBrush.transform.position);
                }
                else
                {
                    // Calcul de la symétrie de la brosse
                    Vector3 planeNormal_LeftBrush = Vector3.Dot((mySymmetryPlane.transform.position - leftBrush.transform.position), mySymmetryPlane.transform.up) * mySymmetryPlane.transform.up;
                    Vector3 leftProjection = leftBrush.transform.position + planeNormal_LeftBrush * 2;
                    symmetryLeftBrush.transform.position = leftProjection;

                    // Mise à jour de la position du point symétrique
                    mySymmetricLineRenderer.SetPosition(pointsOnMySketch, symmetryLeftBrush.transform.position);
                }
            }

            pointsOnMySketch++;

            // Stop the sketching process if the user is drawing the same sketch during 10 seconds
            if (sketchTimer < 10)
            {
                sketchTimer += Time.deltaTime;
            }
            else
            {
                sketchTimer = 0;
                UnpressTriggger();
            }
        }
    }

    /// <summary>
    /// Stop the current sketch
    /// </summary>
    public void UnpressTriggger()
    {
        // Si l'utilisateur est en train de dessiner
        if (isSketching == true)
        {
            // Fin de lecture du fichier audio
            leftAudioSource.GetComponent<AudioSource>().Stop();
            rightAudioSource.GetComponent<AudioSource>().Stop();

            // Si le mode "SKETCH" est activé
            if (currentMode.StartsWith("Sketch"))
            {
                // Arrêt de l'effet
                leftBrushEffect.Stop();
                rightBrushEffect.Stop();

                // Optimisation de la courbe
                int pointsWithoutOpti = myLineRenderer.positionCount;
                if (pointsWithoutOpti > 0)
                {
                    // 1 -> Réduction du nombre de points sur la ligne
                    List<Vector3> points = new List<Vector3>();
                    for (int x = 0; x < myLineRenderer.positionCount; x++)
                    {
                        points.Add(myLineRenderer.GetPosition(x));
                    }
                    float tolerance = 0.01f; //Valeur initiale = 0.0008f;
                    List<Vector3> simplifiedPoints = new List<Vector3>();
                    LineUtility.Simplify(points.ToList(), tolerance, simplifiedPoints);
                    myLineRenderer.positionCount = simplifiedPoints.Count;
                    myLineRenderer.SetPositions(simplifiedPoints.ToArray());
                    points.Clear();

                    // 2 -> Lissage de la courbe
                    Vector3[] linePositions = simplifiedPoints.ToArray();
                    for (int i = 0; i < simplifiedPoints.Count; i++)
                    {
                        linePositions[i] = simplifiedPoints[i];
                    }
                    simplifiedPoints.Clear();
                    Vector3[] smoothedPoints = Sketch_LineSmoother.SmoothLine(linePositions, 0.01f);//0.005f //0.010f ou 0.012f
                    linePositions = null;

                    // 3 -> Mise à jour de la courbe
                    myLineRenderer.positionCount = smoothedPoints.Length;
                    myLineRenderer.SetPositions(smoothedPoints);

                    // TEST TubeRenderer
                    /*GameObject myTube = Instantiate(tubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    myTube.GetComponent<TubeRenderer>().SetPoints(smoothedPoints, 0.01f, Color.cyan);*/

                    smoothedPoints = null;

                    // Open a color menu if user creates a sketch point
                    //if (myLineRenderer.positionCount <= 2)
                    if (myLineRenderer.positionCount <= 3 || (myLineRenderer.positionCount <= 5 && sketchTimer < 0.5f))
                    {
                        // Destroy the current sketch
                        Destroy(mySketch);

                        // Open the sketching menu
                        if (sketchingMenuIsOpen == false)
                        {
                            OpenSketchingMenu(rightHander);
                        }

                        //Debug.Log("<b>[SketchManager]</b> ----- Open sketching menu -----");
                    }
                    else
                    {
                        // Create the collider
                        if (mySketch.GetComponent<MeshCollider>() == null)
                        {
                            mySketch.GetComponent<Sketch_LineCollider>().CreateColliderFromLine(myLineRenderer);
                        }

                        // Assignation du parent
                        if (mySketchFolder == null)
                        {
                            CreateNewSketchFolder();
                        }

                        // Envoi de l'échelle lors de la création

                        mySketch.GetComponent<Sketch>().SetSketchSettings(workFolder.localScale.x, sketchSize, myLineRenderer.material.color);
                        //mySketch.GetComponent<Sketch>().SetSketchSettings_V2(workFolder.localScale.x, sketchSize, myLineRenderer.material.color, mySketch.transform.localScale.x);

                        mySketch.transform.parent = mySketchFolder.transform;

                        mySketch.GetComponent<Sketch>().SetSketchSettings_V2(mySketch.transform.localScale.x);

                        // Send action to history
                        _HistoryManager.GetComponent<HistoryManager>().AddAction_Z("Create a sketch", mySketch.transform);

                        // Try to create a tube
                        /*mySketch.transform.GetChild(0).gameObject.AddComponent<TubeRenderer>();
                        mySketch.transform.GetComponentInChildren<TubeRenderer>().Init();
                        mySketch.GetComponent<LineRenderer>().enabled = false;*/

                        // Try to create a pipe
                        /*PipeMeshGenerator pmg = mySketch.GetComponentInChildren<PipeMeshGenerator>();

                        for (int i = 0; i < mySketch.GetComponent<LineRenderer>().positionCount; i++)
                        {
                            Debug.Log(mySketch.GetComponent<LineRenderer>().GetPosition(i));
                            pmg.points.Add(mySketch.GetComponent<LineRenderer>().GetPosition(i));
                        }

                        pmg.RenderPipe();
                        mySketch.GetComponent<LineRenderer>().enabled = false;*/

                        Debug.Log("<b>[SketchManager]</b> New SKETCH created (" + mySketch.name + ") (" + myLineRenderer.positionCount + " points)");
                    }
                }

                // Si le joueur travaille en symétrie
                if (symmetryMode == true)
                {
                    if (pointsWithoutOpti > 0)
                    {
                        // 1 -> Réduction du nombre de points sur la ligne
                        List<Vector3> points = new List<Vector3>();
                        for (int x = 0; x < mySymmetricLineRenderer.positionCount; x++)
                        {
                            points.Add(mySymmetricLineRenderer.GetPosition(x));
                        }
                        float tolerance = 0.01f; //Valeur initiale = 0.0008f;
                        List<Vector3> simplifiedPoints = new List<Vector3>();
                        LineUtility.Simplify(points.ToList(), tolerance, simplifiedPoints);
                        mySymmetricLineRenderer.positionCount = simplifiedPoints.Count;
                        mySymmetricLineRenderer.SetPositions(simplifiedPoints.ToArray());
                        points.Clear();

                        // 2 -> Lissage de la courbe
                        Vector3[] linePositions = simplifiedPoints.ToArray();
                        for (int i = 0; i < simplifiedPoints.Count; i++)
                        {
                            linePositions[i] = simplifiedPoints[i];
                        }
                        simplifiedPoints.Clear();
                        Vector3[] smoothedPoints = Sketch_LineSmoother.SmoothLine(linePositions, 0.005f);//0.010f ou 0.012f
                        linePositions = null;

                        // 3 -> Mise à jour de la courbe
                        mySymmetricLineRenderer.positionCount = smoothedPoints.Length;
                        mySymmetricLineRenderer.SetPositions(smoothedPoints);

                        // TEST TubeRenderer
                        /*GameObject myTube = Instantiate(tubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        myTube.GetComponent<TubeRenderer>().SetPoints(smoothedPoints, 0.01f, Color.cyan);*/

                        smoothedPoints = null;

                        // Open a color menu if user creates a sketch point
                        //if (mySymmetricLineRenderer.positionCount <= 2)
                        if (myLineRenderer.positionCount <= 3 || (myLineRenderer.positionCount <= 5 && sketchTimer < 0.5f))
                        {
                            // Destroy the current sketch
                            Destroy(mySymmetricSketch);

                            // Open the sketching menu
                            /*if (sketchingMenuIsOpen == false)
                            {
                                OpenSketchingMenu(rightHander);
                            }*/

                            //Debug.Log("<b>[SketchManager]</b> Open sketching menu");
                        }
                        else
                        {
                            // Create the collider
                            if (mySymmetricSketch.GetComponent<MeshCollider>() == null)
                            {
                                mySymmetricSketch.GetComponent<Sketch_LineCollider>().CreateColliderFromLine(mySymmetricLineRenderer);
                            }

                            // Assignation du parent
                            if (mySketchFolder == null)
                            {
                                CreateNewSketchFolder();
                            }

                            // Envoi de l'échelle lors de la création
                            /*mySketch.GetComponent<Sketch>().SetInitialScale(workFolder.localScale.x);
                            mySketch.GetComponent<Sketch>().SetInitialColor(myLineRenderer.material.color);*/
                            mySymmetricSketch.GetComponent<Sketch>().SetSketchSettings(workFolder.localScale.x, sketchSize, mySymmetricLineRenderer.material.color);

                            mySymmetricSketch.transform.parent = mySketchFolder.transform;

                            // Send action to history
                            _HistoryManager.GetComponent<HistoryManager>().AddAction_Z("Create a sketch (symmetric)", mySymmetricSketch.transform);

                            Debug.Log("<b>[SketchManager]</b> New SYMMETRIC SKETCH created (" + mySymmetricSketch.name + ") (" + mySymmetricLineRenderer.positionCount + " points)");
                        }
                    }
                }

                pointsOnMySketch = 0;
                isSketching = false;
            }

            // Si le mode "ERASER" est activé
            else if (currentMode.StartsWith("Eraser"))
            {
                // Arrêt de l'effet
                leftEraserEffect.Stop();
                rightEraserEffect.Stop();

                // Send info to the erasing trigger
                rightEraser.GetComponent<ErasingTrigger>().IsErasing(false);
                leftEraser.GetComponent<ErasingTrigger>().IsErasing(false);

                // Optimisation de la courbe
                int pointsWithoutOpti = myLineRenderer.positionCount;
                if (pointsWithoutOpti > 0)
                {
                    // 1 -> Réduction du nombre de points sur la ligne
                    List<Vector3> points = new List<Vector3>();
                    for (int x = 0; x < myLineRenderer.positionCount; x++)
                    {
                        points.Add(myLineRenderer.GetPosition(x));
                    }
                    float tolerance = 0.01f; //Valeur initiale = 0.0008f;
                    List<Vector3> simplifiedPoints = new List<Vector3>();
                    LineUtility.Simplify(points.ToList(), tolerance, simplifiedPoints);
                    myLineRenderer.positionCount = simplifiedPoints.Count;
                    myLineRenderer.SetPositions(simplifiedPoints.ToArray());
                    points.Clear();

                    // 2 -> Lissage de la courbe
                    Vector3[] linePositions = simplifiedPoints.ToArray();
                    for (int i = 0; i < simplifiedPoints.Count; i++)
                    {
                        linePositions[i] = simplifiedPoints[i];
                    }
                    simplifiedPoints.Clear();
                    Vector3[] smoothedPoints = Sketch_LineSmoother.SmoothLine(linePositions, 0.008f);//0.010f ou 0.012f
                    linePositions = null;

                    // 3 -> Mise à jour de la courbe
                    myLineRenderer.positionCount = smoothedPoints.Length;
                    myLineRenderer.SetPositions(smoothedPoints);

                    smoothedPoints = null;

                    // Open a color menu if user creates a sketch point
                    //if (myLineRenderer.positionCount <= 2)
                    if (myLineRenderer.positionCount <= 3 || (myLineRenderer.positionCount <= 5 && sketchTimer < 0.5f))
                    {
                        // Destroy the current sketch
                        Destroy(mySketch);

                        // Open the sketching menu
                        if (sketchingMenuIsOpen == false)
                        {
                            OpenSketchingMenu(rightHander);
                        }

                        //Debug.Log("<b>[SketchManager]</b> Open sketching menu");
                    }
                    else
                    {
                        // Destroy the current sketch
                        mySketch.GetComponent<Sketch>().EraseSketch();

                        Debug.Log("<b>[SketchManager]</b> New ERASING SKETCH created (" + mySketch.name + ") (" + myLineRenderer.positionCount + " points)");
                    }
                }
                pointsOnMySketch = 0;
                isErasing = false;
                isSketching = false;
            }

            // Reset the sketching timer
            sketchTimer = 0;
        }
    }
    #endregion

    #region INTERACTIONS with SKETCHING MENU
    /// <summary>
    /// Open the sketching menu
    /// </summary>
    public void OpenSketchingMenu(bool myPlayerHander)
    {
        if (coroutineIsRunning == false)
        {
            // Disable the GrabManager
            _GrabManager.GetComponent<GrabManager>().DisableGrabbingAction(true);

            // Get the player hander (left or right)
            rightHander = myPlayerHander;

            // Get the current controller used to open the menu
            Transform currentController;
            if (rightHander == true)
            {
                currentController = rightController;
            }
            else
            {
                currentController = leftController;
            }

            // Set the menu in front of the controller
            sketchingMenu.transform.parent = currentController;
            sketchingMenu.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            sketchingMenu.transform.parent = null;

            // Menu is looking at the headset
            sketchingMenu.transform.rotation = Quaternion.LookRotation(sketchingMenu.transform.position - headset.position);

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Open the menu
            //sketchingMenu.SetActive(true);
            StartCoroutine(OnSketchingMenu());

            // Enable a pointer to interact with the menu (pointer is only available on the controller used to open the menu)
            if (rightHander == true)
            {
                rightPointer.SetActive(true);
            }
            else
            {
                leftPointer.SetActive(true);
            }

            // Set the current state of the menu
            sketchingMenuIsOpen = true;
        }
    }

    /// <summary>
    /// Update the current mode of the sketching menu
    /// </summary>
    public void UpdateMode(string toolToOpen)
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UncleanMenuArea();

        // Enable the GrabManager
        _GrabManager.GetComponent<GrabManager>().DisableGrabbingAction(false);

        // Set the menu button hit by the user
        Transform hitButton = null;

        // If the current mode is updated by the user with the menu pointer...
        if (toolToOpen == "")
        {
            // Get the hit button
            if (rightHander == true)
            {
                hitButton = rightPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
            }
            else
            {
                hitButton = leftPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
            }

            // If user clicked inside the menu
            if (hitButton != null)
            {
                currentMode = hitButton.name;
            }
            else
            {
                currentMode = "Sketch";
            }
        }

        // Else if the current tool is updated by script...
        else if (toolToOpen != "")
        {
            currentMode = toolToOpen;
        }

        Debug.Log("<b>[SketchManager]</b> ----- Select : " + currentMode + " -----");

        // Update the current mode :
        switch (currentMode)
        {
            // 1. Back to the Main Manager
            case "Back":
                //currentMode = "Sketch";
                DisableHelp();
                _AppManager.GetComponent<AppManager>().UpdateMode("MainManager");
                break;

            // 2. Load the Project Manager
            case "ProjectManager":
                //currentMode = "Sketch";
                DisableHelp();
                _AppManager.GetComponent<AppManager>().UpdateMode("ProjectManager");
                break;

            // 3. Load the Capture Manager
            case "CaptureManager":
                //currentMode = "Sketch";
                DisableHelp();
                _AppManager.GetComponent<AppManager>().UpdateMode("CaptureManager");
                break;

            // 4. Close the current group & create a new group
            case "Group":

                // Update the help on controller
                UpdateHelp();

                // Create new folder
                CreateNewSketchFolder();

                // Reset the sketching tool by default
                currentMode = "Sketch";
                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);

                // Close the sketching menu
                CloseSketchingMenu();
                break;

            // 5. Erase a sketch
            case "Eraser":

                // Update the help on controller
                UpdateHelp();

                // Disable the brush & Enable the eraser
                eraseSize = 0.01f;

                rightBrush.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(true);
                rightEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                rightEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                rightEraserEffect.Stop();

                var rightPs = rightEraserEffect.main;
                rightPs.startColor = new Color(1, 0.5f, 0);

                leftBrush.gameObject.SetActive(false);
                leftEraser.gameObject.SetActive(true);
                leftEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                leftEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                leftEraserEffect.Stop();

                var leftPs = leftEraserEffect.main;
                leftPs.startColor = new Color(1, 0.5f, 0);

                // Close the sketching menu
                CloseSketchingMenu();
                break;

            // 6. Enable or disable the symmetry mode
            case "Symmetry":

                // Update the help on controller
                UpdateHelp();

                // Switch the symmetry mode
                symmetryMode = !symmetryMode;

                // Enable or disable the symmetry plane
                if (symmetryMode == false)
                {
                    mySymmetryPlane.SetActive(false);
                }
                else
                {
                    mySymmetryPlane.SetActive(true);
                }

                // Reset the sketching tool by default
                currentMode = "Sketch";

                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);

                // Close the sketching menu
                CloseSketchingMenu();
                break;

            // 7. Select a group of sketches
            case "SelectGroup":

                // Update the help on controller
                UpdateHelp();

                // Display each sketch folder
                DisplaySketchFoldersByColor(true);

                // Enable the group selector on the controller
                if (rightHander == true)
                {
                    leftGroupSelector.gameObject.SetActive(false);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    rightGroupSelector.gameObject.SetActive(true);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }
                else
                {
                    rightGroupSelector.gameObject.SetActive(false);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    leftGroupSelector.gameObject.SetActive(true);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }

                isSelectingGroup = true;

                // Close the sketching menu
                CloseSketchingMenu();
                break;

            // 8. Close the sketching menu
            case "CloseMenu":
                CloseSketchingMenu();
                break;

            // 9. Close the sketching menu
            case "":
                CloseSketchingMenu();
                break;

            // 10. Erase parts of the mesh
            case "EraseMesh":

                CloseSketchingMenu();
                break;
        }

        // Update the sketching tool :
        if (currentMode.StartsWith("Sketch"))
        {
            leftEraser.gameObject.SetActive(false);
            rightEraser.gameObject.SetActive(false);
            leftBrush.gameObject.SetActive(true);
            rightBrush.gameObject.SetActive(true);

            leftBrushEffect.Stop();
            rightBrushEffect.Stop();

            // Update the help on controller
            UpdateHelp();

            // Update sketch color
            if (currentMode.StartsWith("Sketch_Color_") && hitButton != null)
            {
                if (rightHander == true)
                {
                    rightBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                    var rightPs = rightBrushEffect.main;
                    rightPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                }
                else
                {
                    leftBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                    var leftPs = leftBrushEffect.main;
                    leftPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                }
                sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnSizeButttons(hitButton.GetChild(0).GetComponent<Image>().color);

                // Update the gradient
                if (!currentMode.Contains("Gradient"))
                {
                    sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnGradientButtons(hitButton.GetChild(0).GetComponent<Image>().color);
                }
            }

            // Update sketch size
            else if (currentMode.StartsWith("Sketch_Size_"))
            {
                // Get the selected size
                string myStringSize = currentMode.Replace("Sketch_Size_", "");
                Debug.Log(myStringSize);
                float mySize = float.Parse(myStringSize);
                sketchSize = mySize;
                mySize = mySize * 200;

                // Set the brush size
                if (rightHander == true)
                {
                    rightEraser.gameObject.SetActive(false);
                    rightBrush.gameObject.SetActive(true);
                    rightBrush.localScale = new Vector3(mySize, mySize, mySize);

                    ParticleSystem.ShapeModule _editableShape = rightBrushEffect.shape;
                    _editableShape.scale = new Vector3(mySize, mySize, mySize);
                }
                else
                {
                    leftEraser.gameObject.SetActive(false);
                    leftBrush.gameObject.SetActive(true);
                    leftBrush.localScale = new Vector3(mySize, mySize, mySize);

                    ParticleSystem.ShapeModule _editableShape = leftBrushEffect.shape;
                    _editableShape.scale = new Vector3(mySize, mySize, mySize);
                }
            }

            // Close the sketching menu
            CloseSketchingMenu();
        }
    }

    /// <summary>
    /// Close the sketching menu
    /// </summary>
    public void CloseSketchingMenu()
    {
        if (coroutineIsRunning == false)
        {
            // Fermeture du menu
            if (sketchingMenuIsOpen == true)
            {
                StartCoroutine(OffSketchingMenu());
                //sketchingMenu.gameObject.SetActive(false);
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                sketchingMenuIsOpen = false;
            }
        }

        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UncleanMenuArea();
    }

    /// <summary>
    /// Animation to show the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OnSketchingMenu()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = captureMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning = true;

        // Enable buttons
        for (int x = 0; x < sketchingMenu.transform.GetChild(1).childCount; x++)
        {
            if (sketchingMenu.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                sketchingMenu.transform.GetChild(1).GetChild(x).GetComponent<SketchingMenu_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            sketchingMenu.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OffSketchingMenu()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = sketchingMenu.transform.localScale;
        Vector3 finalScale = sketchingMenu.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning = true;

        // Disable buttons
        for (int x = 0; x < sketchingMenu.transform.GetChild(1).childCount; x++)
        {
            if (sketchingMenu.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                sketchingMenu.transform.GetChild(1).GetChild(x).GetComponent<SketchingMenu_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            sketchingMenu.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        sketchingMenu.transform.localScale = new Vector3(0, 0, 0);
        sketchingMenu.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning = false;

        yield return null;
    }
    #endregion


    #region GROUPS of SKETCHES
    /// <summary>
    /// Create a new folder (= a group of sketches)
    /// </summary>
    public void CreateNewSketchFolder()
    {
        // Suppression du dernier dossier créé si il est vide
        if (workFolder.childCount > 0)
        {
            if (workFolder.GetChild(workFolder.childCount - 1).name != "TrashFolder" && workFolder.GetChild(workFolder.childCount - 1).childCount == 0)
            {
                Destroy(workFolder.GetChild(workFolder.childCount - 1).gameObject);
            }
        }

        // Création d'un nouveau dossier
        mySketchFolder = Instantiate(sketchFolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mySketchFolder.transform.parent = workFolder;
        mySketchFolder.name = "SketchFolder_" + folderNumber;
        mySketchFolder.transform.localScale = new Vector3(1, 1, 1);
        folderNumber++;

        Debug.Log("<b>[SketchManager]</b> New FOLDER created (" + mySketchFolder.name + ")");
    }

    /// <summary>
    /// Duplicate a folder of sketches
    /// </summary>
    public void DuplicateSketchFolder(bool useRightController)
    {
        // Manipulation avec le controller droit
        if (_GrabManager.GetComponent<GrabManager>().GetGrabbedObject(useRightController) != null)
        {
            // Récupération de l'objet à dupliquer
            Transform folderToDuplicate = _GrabManager.GetComponent<GrabManager>().GetGrabbedObject(useRightController);

            // Suppression du dernier dossier créé si il est vide
            if (workFolder.GetChild(workFolder.childCount - 1).name != "TrashFolder" && workFolder.GetChild(workFolder.childCount - 1).childCount == 0)
            {
                Destroy(workFolder.GetChild(workFolder.childCount - 1).gameObject);
            }

            // Création d'un nouveau dossier
            GameObject duplicatedFolder = Instantiate(sketchFolderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            duplicatedFolder.transform.parent = workFolder;
            duplicatedFolder.name = "SketchFolder_" + folderNumber;
            folderNumber++;

            // Positionnement du dossier dans l'espace
            duplicatedFolder.transform.localPosition = folderToDuplicate.transform.localPosition;
            duplicatedFolder.transform.localRotation = folderToDuplicate.transform.localRotation;
            duplicatedFolder.transform.localScale = folderToDuplicate.transform.localScale;

            // Création de chaque dessin présent dans le dossier
            for (int x = 0; x < folderToDuplicate.childCount; x++)
            {
                GameObject duplicatedSketch = Instantiate(sketchPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                duplicatedSketch.name = "Sketch_" + sketchId;
                sketchId++;

                // Version précédente
                /*duplicatedSketch.transform.parent = duplicatedFolder.transform;
                duplicatedSketch.transform.localPosition = new Vector3(0, 0, 0);
                duplicatedSketch.transform.localRotation = new Quaternion(0, 0, 0, 0);
                duplicatedSketch.transform.localScale = new Vector3(1, 1, 1);*/

                // Nouvelle version
                duplicatedSketch.transform.parent = duplicatedFolder.transform;
                duplicatedSketch.transform.localPosition = folderToDuplicate.GetChild(x).transform.localPosition;
                duplicatedSketch.transform.localRotation = folderToDuplicate.GetChild(x).transform.localRotation;
                duplicatedSketch.transform.localScale = folderToDuplicate.GetChild(x).transform.localScale;

                /*duplicatedSketch.GetComponent<Sketch>().SetInitialScale(folderToDuplicate.GetChild(x).GetComponent<Sketch>().GetInitialScale());
                duplicatedSketch.GetComponent<Sketch>().SetInitialColor(duplicatedSketch.GetComponent<LineRenderer>().material.color);*/
                duplicatedSketch.GetComponent<Sketch>().SetSketchSettings(folderToDuplicate.GetChild(x).GetComponent<Sketch>().GetInitialScale(), folderToDuplicate.GetChild(x).GetComponent<Sketch>().GetInitialSize(), folderToDuplicate.GetChild(x).GetComponent<Sketch>().GetInitialColor());

                LineRenderer duplicatedLr = duplicatedSketch.GetComponent<LineRenderer>();
                duplicatedLr.startColor = Color.white;
                duplicatedLr.endColor = Color.white;
                duplicatedLr.material.color = duplicatedSketch.GetComponent<Sketch>().GetInitialColor();

                // Test
                duplicatedLr.widthMultiplier = folderToDuplicate.GetChild(x).GetComponent<LineRenderer>().widthMultiplier;

                // Display a smooth line (start & end width)
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0.0f, 0.0f);
                curve.AddKey(0.1f, duplicatedSketch.GetComponent<Sketch>().GetInitialSize());
                curve.AddKey(0.9f, duplicatedSketch.GetComponent<Sketch>().GetInitialSize());
                curve.AddKey(1.0f, 0.0f);
                duplicatedLr.widthCurve = curve;

                /*duplicatedLr.startWidth = folderToDuplicate.GetChild(x).GetComponent<LineRenderer>().startWidth;
                duplicatedLr.endWidth = folderToDuplicate.GetChild(x).GetComponent<LineRenderer>().endWidth;*/

                // Copie des points de passage du dessin
                for (int y = 0; y < folderToDuplicate.GetChild(x).GetComponent<LineRenderer>().positionCount; y++)
                {
                    duplicatedLr.positionCount = y + 1;
                    duplicatedLr.SetPosition(y, folderToDuplicate.GetChild(x).GetComponent<LineRenderer>().GetPosition(y));
                }

                // Create the collider
                if (duplicatedSketch.GetComponent<MeshCollider>() == null)
                {
                    duplicatedSketch.GetComponent<Sketch_LineCollider>().CreateColliderFromLine(duplicatedLr);
                }
            }

            Debug.Log("<b>[SketchManager]</b> Duplicate FOLDER (" + folderToDuplicate.name + ")");

            // Création d'un nouveau dossier de dessins (on part du principe que l'utilisateur ne va pas modifier le dessin dupliqué)
            CreateNewSketchFolder();
        }


        // Manipulation avec le controller gauche
        // TO DO...

    }

    /// <summary>
    /// Display each sketch folder with a unique color
    /// </summary>
    public void DisplaySketchFoldersByColor(bool displayColor)
    {
        /*int folderNumber = 0;
        int currentFolder = 0;
        foreach (Transform sketchFolder in workFolder)
        {
            if (sketchFolder.name.StartsWith("SketchFolder_"))
            {
                folderNumber++;
            }   
        }*/

        foreach (Transform sketchFolder in workFolder)
        {
            if (sketchFolder.name.StartsWith("SketchFolder_"))
            {
                // Random color
                Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

                // Gradient color
                /*currentFolder++;
                float myGradient = 0.8f * currentFolder / folderNumber;
                Color shadedColor = new Color(myGradient, myGradient, myGradient);
                Debug.Log(myGradient);*/

                foreach (Transform sketch in sketchFolder)
                {
                    if (displayColor == true)
                    {
                        sketch.GetComponent<LineRenderer>().material.color = randomColor;
                    }
                    else if (displayColor == false)
                    {
                        sketch.GetComponent<Sketch>().ResetColor();
                    }
                }
            }
        }
    }
    #endregion



    #region HELPERS on Controllers
    public void EnableHelp()
    {
        // Switch helping state
        isHelping = !isHelping;

        // Show help on conrollers
        if (isHelping == true)
        {
            UpdateHelp();
        }

        // Hide help on conrollers
        else if (isHelping == false)
        {
            DisableHelp();
        }

        Debug.Log("<b>[SketchManager]</b> Display help (" + isHelping + ")");
    }

    public void UpdateHelp()
    {
        // Show help on conrollers
        if (isHelping == true)
        {
            string myCurrentMode = currentMode;

            // Rename with a shortest mode (to avoid each variant of "Sketch_Color...")
            if (myCurrentMode.StartsWith("Sketch_"))
            {
                myCurrentMode = "Sketch";
            }

            // Update helper
            switch (myCurrentMode)
            {
                default:
                    leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip : Press the grip to grab a sketch
                    leftHelpers[1].GetComponent<ControllerHelper>().EnableHelper(""); // Interactor
                    leftHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    leftHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    leftHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Press once the trigger to open the menu"); // Trigger

                    rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip : Press the grip to grab a sketch
                    rightHelpers[1].GetComponent<ControllerHelper>().EnableHelper(""); // Interactor
                    rightHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    rightHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    rightHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Press once the trigger to open the menu"); // Trigger
                    break;

                case "Sketch":

                    if (symmetryMode == true)
                    {
                        leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab the symmetry plane"); // Grip
                        rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab the symmetry plane"); // Grip
                    }
                    else
                    {
                        leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip
                        rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip
                    }

                    leftHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Paintbrush"); // Interactor
                    leftHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    leftHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    leftHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Keep pressed the trigger to sketch"); // Trigger

                    rightHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Paintbrush"); // Interactor
                    rightHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    rightHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    rightHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Keep pressed the trigger to sketch"); // Trigger
                    break;

                case "Eraser":
                    leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip
                    leftHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Eraser"); // Interactor
                    leftHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    leftHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    leftHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Keep pressed the trigger to erase a sketch"); // Trigger

                    rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper("Press the grip to grab a sketch"); // Grip
                    rightHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Eraser"); // Interactor
                    rightHelpers[2].GetComponent<ControllerHelper>().EnableHelper("Undo action"); // Touchpad_Left
                    rightHelpers[3].GetComponent<ControllerHelper>().EnableHelper("Redo action"); // Touchpad_Right
                    rightHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Keep pressed the trigger to erase a sketch"); // Trigger
                    break;

                case "SelectGroup":
                    leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper(""); // Grip
                    leftHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Group picker"); // Interactor
                    leftHelpers[2].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Left
                    leftHelpers[3].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Right
                    leftHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Press the trigger to pick a group"); // Trigger

                    rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper(""); // Grip
                    rightHelpers[1].GetComponent<ControllerHelper>().EnableHelper("Group picker"); // Interactor
                    rightHelpers[2].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Left
                    rightHelpers[3].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Right
                    rightHelpers[4].GetComponent<ControllerHelper>().EnableHelper("Press the trigger to pick a group"); // Trigger
                    break;
            }
        }
    }

    public void DisableHelp()
    {
        leftHelpers[0].GetComponent<ControllerHelper>().EnableHelper(""); // Grip
        leftHelpers[1].GetComponent<ControllerHelper>().EnableHelper(""); // Interactor
        leftHelpers[2].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Left
        leftHelpers[3].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Right
        leftHelpers[4].GetComponent<ControllerHelper>().EnableHelper(""); // Trigger

        rightHelpers[0].GetComponent<ControllerHelper>().EnableHelper(""); // Grip
        rightHelpers[1].GetComponent<ControllerHelper>().EnableHelper(""); // Interactor
        rightHelpers[2].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Left
        rightHelpers[3].GetComponent<ControllerHelper>().EnableHelper(""); // Touchpad_Right
        rightHelpers[4].GetComponent<ControllerHelper>().EnableHelper(""); // Trigger

        isHelping = false;
    }
    #endregion


    // ---------------------------------------------------------------------------------------------
    // --- UNUSED ----------------------------------------------------------------------------------

    #region [UNUSED] [V0] Update sketching tool
    /// <summary>
    /// Update the current mode of the sketching menu
    /// </summary>
    /*public void UpdateSketchingTool_0()
    {
        // Save the last sketching mode
        previousSketchingTool = currentMode;

        // Get the new sketching mode from the hit button
        Transform hitButton = null;
        if (rightHander == true)
        {
            hitButton = rightPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
        }
        else
        {
            hitButton = leftPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
        }

        // If user clicked outside the menu
        if (hitButton == null)
        {
            currentMode = "";
        }
        // Else if user clicked inside the menu
        else
        {
            currentMode = hitButton.name;
        }

        // A- LEAVE SKETCHING MODE
        // A.1- BACK TO MAIN MANAGER
        if (currentMode == "Back")
        {
            currentMode = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("Main");
        }
        // A.2- OPEN PROJECT MODULE
        else if (currentMode == "ProjectManager")
        {
            currentMode = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("ProjectManager");
        }
        // A.3- OPEN CAPTURE MODULE
        else if (currentMode == "CaptureManager")
        {
            currentMode = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("Capture");
        }

        // B- STAY IN SKETCHING MODE
        else
        {
            // B.1- CLICK OUTSIDE THE MENU
            if (currentMode == "")
            {
                currentMode = previousSketchingTool;
            }

            // B.2- CLOSE THE CURRENT GROUP OF SKETCHES
            else if (currentMode.StartsWith("Group"))
            {
                // Create new folder
                CreateNewSketchFolder();

                // Reset the sketching tool by default
                currentMode = "Sketch";

                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);
            }
            // B.3- ERASE SKETCHES
            else if (currentMode.StartsWith("Eraser"))
            {
                // Disable the brush & Enable the eraser
                eraseSize = 0.01f;

                rightBrush.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(true);
                rightEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                rightEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                rightEraserEffect.Stop();

                var rightPs = rightEraserEffect.main;
                rightPs.startColor = new Color(1, 0.5f, 0);

                leftBrush.gameObject.SetActive(false);
                leftEraser.gameObject.SetActive(true);
                leftEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                leftEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                leftEraserEffect.Stop();

                var leftPs = leftEraserEffect.main;
                leftPs.startColor = new Color(1, 0.5f, 0);
            }
            // B.3- CLOSE THE CURRENT GROUP OF SKETCHES
            else if (currentMode.StartsWith("Symmetry"))
            {
                symmetryMode = !symmetryMode;

                if (symmetryMode == false)
                {
                    mySymmetryPlane.SetActive(false);
                }
                else
                {
                    mySymmetryPlane.SetActive(true);
                }

                // Reset the sketching tool by default
                currentMode = "Sketch";

                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);
            }
            // B.4- SELECT A PREVIOUS GROUP OF SKETCHES
            else if (currentMode.StartsWith("SelectGroup"))
            {
                // Display each sketch folder
                DisplaySketchFoldersByColor(true);

                // Enable the group selector on the controller
                if (rightHander == true)
                {
                    leftGroupSelector.gameObject.SetActive(false);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    rightGroupSelector.gameObject.SetActive(true);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }
                else
                {
                    rightGroupSelector.gameObject.SetActive(false);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    leftGroupSelector.gameObject.SetActive(true);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }

                previousSketchingTool = "Sketch";

                isSelectingGroup = true;
            }

            // C- CREATE SKETCHES
            if (currentMode.StartsWith("Sketch"))
            {
                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);

                leftBrushEffect.Stop();
                rightBrushEffect.Stop();

                // B.3.1- UPDATE SKETCH COLOR
                if (currentMode.StartsWith("Sketch_Color_") && hitButton != null)
                {
                    if (rightHander == true)
                    {
                        rightEraser.gameObject.SetActive(false);
                        rightBrush.gameObject.SetActive(true);
                        rightBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                        var rightPs = rightBrushEffect.main;
                        rightPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                    }
                    else
                    {
                        leftEraser.gameObject.SetActive(false);
                        leftBrush.gameObject.SetActive(true);
                        leftBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                        var leftPs = leftBrushEffect.main;
                        leftPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                    }
                    sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnSizeButttons(hitButton.GetChild(0).GetComponent<Image>().color);

                    // Update the gradient
                    if (!currentMode.Contains("Gradient"))
                    {
                        sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnGradientButtons(hitButton.GetChild(0).GetComponent<Image>().color);
                    }
                }

                // B.3.2- UPDATE SKETCH SIZE
                else if (currentMode.StartsWith("Sketch_Size_"))
                {
                    // Get the selected size
                    string myStringSize = currentMode.Replace("Sketch_Size_", "");
                    Debug.Log(myStringSize);
                    float mySize = float.Parse(myStringSize);
                    sketchSize = mySize;
                    mySize = mySize * 200;

                    // Set the brush size
                    if (rightHander == true)
                    {
                        rightEraser.gameObject.SetActive(false);
                        rightBrush.gameObject.SetActive(true);
                        rightBrush.localScale = new Vector3(mySize, mySize, mySize);

                        ParticleSystem.ShapeModule _editableShape = rightBrushEffect.shape;
                        _editableShape.scale = new Vector3(mySize, mySize, mySize);
                    }
                    else
                    {
                        leftEraser.gameObject.SetActive(false);
                        leftBrush.gameObject.SetActive(true);
                        leftBrush.localScale = new Vector3(mySize, mySize, mySize);

                        ParticleSystem.ShapeModule _editableShape = leftBrushEffect.shape;
                        _editableShape.scale = new Vector3(mySize, mySize, mySize);
                    }
                }
            }

            // Close the sketching menu
            CloseSketchingMenu();
        }
    }*/
    #endregion

    #region [UNUSED] [V1] Update sketching tool
    /// <summary>
    /// Update the current mode of the sketching menu
    /// </summary>
    /*public void UpdateMode(string toolToOpen)
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UncleanMenuArea();

        // Enable the GrabManager
        _GrabManager.GetComponent<GrabManager>().DisableGrabbingAction(false);

        // Save the last sketching mode
        previousSketchingTool = currentSketchingTool;

        // Set the menu button hit by the user
        Transform hitButton = null;

        // If the current mode is updated by the user with the menu pointer...
        if (toolToOpen == "")
        {
            // Get the hit button
            if (rightHander == true)
            {
                hitButton = rightPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
            }
            else
            {
                hitButton = leftPointer.GetComponent<SketchingMenu_Pointer>().GetHitButton();
            }

            // If user clicked inside the menu
            if (hitButton != null)
            {
                currentSketchingTool = hitButton.name;
            }
            // Else if user clicked outside the menu
            else
            {
                //currentSketchingTool = "NoTool";
            }
        }

        // Else if the current tool is updated by script...
        else if (toolToOpen != "")
        {
            currentSketchingTool = toolToOpen;
        }


        // UPDATE THE CURRENT MODE :
        // A- Leave the sketching mode

        // A.1- Back to the main manager
        if (currentSketchingTool == "Back")
        {
            currentSketchingTool = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("Main");
        }
        // A.2- Open the project manager
        else if (currentSketchingTool == "ProjectManager")
        {
            currentSketchingTool = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("ProjectManager");
        }
        // A.3- Open the capture manager
        else if (currentSketchingTool == "CaptureManager")
        {
            currentSketchingTool = "Sketch";
            _AppManager.GetComponent<AppManager>().UpdateMode("Capture");
        }

        // B- Stay in sketching mode
        else
        {
            // B.1- CLOSE THE CURRENT GROUP OF SKETCHES
            if (currentSketchingTool.StartsWith("Group"))
            {
                // Create new folder
                CreateNewSketchFolder();

                // Reset the sketching tool by default
                currentSketchingTool = "Sketch";

                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);
            }
            // B.2- ERASE SKETCHES
            else if (currentSketchingTool.StartsWith("Eraser"))
            {
                // Disable the brush & Enable the eraser
                eraseSize = 0.01f;

                rightBrush.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(true);
                rightEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                rightEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                rightEraserEffect.Stop();

                var rightPs = rightEraserEffect.main;
                rightPs.startColor = new Color(1, 0.5f, 0);

                leftBrush.gameObject.SetActive(false);
                leftEraser.gameObject.SetActive(true);
                leftEraser.GetComponent<MeshRenderer>().material.color = Color.red;
                leftEraser.localScale = new Vector3(eraseSize * 200, eraseSize * 200, eraseSize * 200);
                leftEraserEffect.Stop();

                var leftPs = leftEraserEffect.main;
                leftPs.startColor = new Color(1, 0.5f, 0);
            }
            // B.3- Start/Stop the symmetry mode
            else if (currentSketchingTool.StartsWith("Symmetry"))
            {
                symmetryMode = !symmetryMode;

                if (symmetryMode == false)
                {
                    mySymmetryPlane.SetActive(false);
                }
                else
                {
                    mySymmetryPlane.SetActive(true);
                }

                // Reset the sketching tool by default
                currentSketchingTool = "Sketch";

                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);
            }
            // B.4- SELECT A PREVIOUS GROUP OF SKETCHES
            else if (currentSketchingTool.StartsWith("SelectGroup"))
            {
                // Display each sketch folder
                DisplaySketchFoldersByColor(true);

                // Enable the group selector on the controller
                if (rightHander == true)
                {
                    leftGroupSelector.gameObject.SetActive(false);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    rightGroupSelector.gameObject.SetActive(true);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }
                else
                {
                    rightGroupSelector.gameObject.SetActive(false);
                    rightGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(false);

                    leftGroupSelector.gameObject.SetActive(true);
                    leftGroupSelector.GetComponent<SelectGroupTrigger>().IsSelectingGroup(true);
                }

                //previousSketchingTool = "Sketch";

                isSelectingGroup = true;
            }

            // B.5- CREATE SKETCHES
            else if (currentSketchingTool.StartsWith("Sketch"))
            {
                leftEraser.gameObject.SetActive(false);
                rightEraser.gameObject.SetActive(false);
                leftBrush.gameObject.SetActive(true);
                rightBrush.gameObject.SetActive(true);

                leftBrushEffect.Stop();
                rightBrushEffect.Stop();

                // B.5.1- UPDATE SKETCH COLOR
                if (currentSketchingTool.StartsWith("Sketch_Color_") && hitButton != null)
                {
                    if (rightHander == true)
                    {
                        rightBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                        var rightPs = rightBrushEffect.main;
                        rightPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                    }
                    else
                    {
                        leftBrush.GetComponent<MeshRenderer>().material.color = hitButton.GetChild(0).GetComponent<Image>().color;

                        var leftPs = leftBrushEffect.main;
                        leftPs.startColor = hitButton.GetChild(0).GetComponent<Image>().color;
                    }
                    sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnSizeButttons(hitButton.GetChild(0).GetComponent<Image>().color);

                    // Update the gradient
                    if (!currentSketchingTool.Contains("Gradient"))
                    {
                        sketchingMenu.GetComponent<SketchingMenu>().UpdateColorOnGradientButttons(hitButton.GetChild(0).GetComponent<Image>().color);
                    }
                }

                // B.5.2 - UPDATE SKETCH SIZE
                else if (currentSketchingTool.StartsWith("Sketch_Size_"))
                {
                    // Get the selected size
                    string myStringSize = currentSketchingTool.Replace("Sketch_Size_", "");
                    Debug.Log(myStringSize);
                    float mySize = float.Parse(myStringSize);
                    sketchSize = mySize;
                    mySize = mySize * 200;

                    // Set the brush size
                    if (rightHander == true)
                    {
                        rightEraser.gameObject.SetActive(false);
                        rightBrush.gameObject.SetActive(true);
                        rightBrush.localScale = new Vector3(mySize, mySize, mySize);

                        ParticleSystem.ShapeModule _editableShape = rightBrushEffect.shape;
                        _editableShape.scale = new Vector3(mySize, mySize, mySize);
                    }
                    else
                    {
                        leftEraser.gameObject.SetActive(false);
                        leftBrush.gameObject.SetActive(true);
                        leftBrush.localScale = new Vector3(mySize, mySize, mySize);

                        ParticleSystem.ShapeModule _editableShape = leftBrushEffect.shape;
                        _editableShape.scale = new Vector3(mySize, mySize, mySize);
                    }
                }
            }

            // Close the sketching menu
            CloseSketchingMenu();
        }
    }*/
    #endregion

}
