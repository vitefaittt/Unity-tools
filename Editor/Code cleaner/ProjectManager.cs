using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ProjectManager : MonoBehaviour
{
    // Managers
   
    private Transform _ActionsManager;
    private Transform _HistoryManager;
    

    // Headset & Controllers
    private Transform headset;
    private Transform leftController;
    private Transform rightController;

    // Toolbox
    [Header("Toolbox")]
    public GameObject Toolbox_FilterCreator;

    // Project menu
    [Header("Menu")]
    public GameObject newProjectMenu;
    private GameObject projectMenu;
    private bool projectMenuIsOpen = false;
    private string currentMode = "";
    private GameObject projectNameOnMenu;

    // Pointer on menu
    [Header("Pointer")]
    public GameObject newPointer;
    private GameObject leftPointer;
    private GameObject rightPointer;
    private bool pointerIsOpen = false;

    // Project info
    private Project project;
    private string projectName = "";
    private string projectVersion = "";

    // Saving & loading
    private string folderPath = "";
    private string filePath = "";

    // Element prefabs
    public GameObject folderPrefab;
    public GameObject sketchPrefab;

    // Keyboard
    [Header("Keyboard")]
    public GameObject newKeyboard;
    private GameObject keyboard;
    private bool keyboardIsOpen = false;
    private string touchedKey = "";
    private string myText = "";
    private GameObject keyboardInput;

    // Warning info
    [Header("Warning info")]
    public GameObject newWarningInfo;
    private GameObject warningInfo;
    private bool warningInfoIsOpen = false;
    private string touchedWarningInfoButton = "";

    // Explorer
    [Header("Explorer")]
    public GameObject newExplorerMenu;
    private GameObject explorer;
    private bool explorerIsOpen = false;
    public GameObject newButtonToFolder;
    public GameObject newButtonToFile;
    private List<GameObject> explorerButtons = new List<GameObject>();
    private List<string> filesPath = new List<string>();
    private string subFolderPath = "";
    private string currentProjectPath = "";
    private string touchedFile = "";
    private int pageNumber = 0;
    private GameObject previousPageButton;
    private GameObject nextPageButton;
    private int maxPageNumber = 0;

    // Info window
    [Header("Info window")]
    public GameObject newInfoWindow;
    private GameObject infoWindow;
    private bool infoWindowIsOpen = false;
    private string touchedInfoWindowButton = "";

    // Player
    private bool rightHander = true;
    //private bool wasRightHander = true;

    // Other
    private bool coroutineIsRunning_ProjectMenu = false;
    private bool coroutineIsRunning_WarningInfo = false;
    private bool coroutineIsRunning_Keyboard = false;
    private bool coroutineIsRunning_Explorer = false;
    private bool coroutineIsRunning_InfoWindow = false;

    // Work folder
    private Transform workFolder;
    private Transform objectFolder;

    // Menu cleaner
    private Transform menuCleaner;

    // Helpers on controllers
    private bool isHelping = false;
    private Transform mainHelperOnLeftController;
    private Transform mainHelperOnRightController;
    private List<Transform> leftHelpers = new List<Transform>();
    private List<Transform> rightHelpers = new List<Transform>();




    // Start is called before the first frame update
    void Start()
    {
        // Get other managers
        
        _ActionsManager = GameObject.Find("_ActionsManager").transform;
        _HistoryManager = GameObject.Find("_HistoryManager").transform;

        // Get the menu cleaner
        menuCleaner = GameObject.Find("UX_MenuCleaner").transform;

        // Init controllers
        if (leftController == null || rightController == null)
        {
            InitControllers();
        }

        // Init project menu
        InitProjectMenu();

        // Init Keyboard
        InitKeyboard();

        // Init Explorer
        InitExplorer();

        // Init Warning info
        InitWarningInfo();

        // Init Info window
        InitInfoWindow();

        // Get the working folder
        workFolder = GameObject.Find("WorkFolder").transform;
        for (int x = 0; x < workFolder.childCount; x++)
        {
            if (workFolder.GetChild(x).name == "ObjectFolder")
            {
                objectFolder = workFolder.GetChild(x);
            }
        }

        // Create a first project
        CreateNewProject("");
    }


    #region INIT the PROJECT MANAGER (Controllers, Menu, Explorer, Keyboard, Warning & Info window)
    /// <summary>
    /// Init controllers & pointers
    /// </summary>
    private void InitControllers()
    {
        if (GameObject.FindGameObjectWithTag("LeftController"))
        {
            // Get the left controller
            leftController = GameObject.FindGameObjectWithTag("LeftController").transform;

            // Init the pointer on left controller
            leftPointer = Instantiate(newPointer, new Vector3(0, 0, 0), Quaternion.identity);
            leftPointer.name = "Project_LeftPointer";
            leftPointer.transform.parent = leftController;
            leftPointer.transform.localPosition = new Vector3(0, 0, 0); // Only for HTC Vive
            leftPointer.SetActive(false);

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
        }

        if (GameObject.FindGameObjectWithTag("RightController"))
        {
            // Get the right controller
            rightController = GameObject.FindGameObjectWithTag("RightController").transform;

            // Init the pointer on the right controller
            rightPointer = Instantiate(newPointer, new Vector3(0, 0, 0), Quaternion.identity);
            rightPointer.name = "Project_RightPointer";
            rightPointer.transform.parent = rightController;
            rightPointer.transform.localPosition = new Vector3(0, 0, 0); // Only for HTC Vive
            rightPointer.SetActive(false);

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
        }

        if (GameObject.FindGameObjectWithTag("MenuLookAt"))
        {
            headset = GameObject.FindGameObjectWithTag("MenuLookAt").transform;
        }

        DisableHelp();
    }

    /// <summary>
    /// Init the project menu
    /// </summary>
    private void InitProjectMenu()
    {
        // Project menu
        projectMenu = Instantiate(newProjectMenu, new Vector3(0, -5, 0), Quaternion.identity);
        projectMenu.name = "Project_Menu";
        projectMenu.transform.position = new Vector3(0, -5, 0);
        projectMenu.transform.localScale = new Vector3(0, 0, 0);

        // Project name
        projectNameOnMenu = projectMenu.transform.GetChild(1).GetChild(1).GetChild(1).gameObject;
        projectNameOnMenu.GetComponent<Text>().text = projectName;
    }

    /// <summary>
    /// Init the keyboard
    /// </summary>
    private void InitKeyboard()
    {
        // Keyboard
        keyboard = Instantiate(newKeyboard, new Vector3(0, -5, 0), Quaternion.identity);
        keyboard.name = "Keyboard";
        keyboard.transform.position = new Vector3(0, -5, 0);
        keyboard.transform.localScale = new Vector3(0, 0, 0);

        // Get the input field
        for (int k = 0; k < keyboard.transform.GetChild(1).childCount; k++)
        {
            if (keyboard.transform.GetChild(1).GetChild(k).name == "KeyboardInput")
            {
                keyboardInput = keyboard.transform.GetChild(1).GetChild(k).gameObject;
            }
        }
    }

    /// <summary>
    /// Init the warning info
    /// </summary>
    private void InitWarningInfo()
    {
        // Keyboard
        warningInfo = Instantiate(newWarningInfo, new Vector3(0, -5, 0), Quaternion.identity);
        warningInfo.name = "WarningInfo";
        warningInfo.transform.position = new Vector3(0, -5, 0);
        warningInfo.transform.localScale = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Init the explorer
    /// </summary>
    private void InitExplorer()
    {
        // Explorer
        explorer = Instantiate(newExplorerMenu, new Vector3(0, -5, 0), Quaternion.identity);
        explorer.name = "Explorer";
        explorer.transform.position = new Vector3(0, -5, 0);
        explorer.transform.localScale = new Vector3(0, 0, 0);

        // Get some butttons
        for (int x = 0; x < explorer.transform.GetChild(1).childCount; x++)
        {
            if (explorer.transform.GetChild(1).GetChild(x).name == "PreviousPage")
            {
                previousPageButton = explorer.transform.GetChild(1).GetChild(x).gameObject;
            }
            else if (explorer.transform.GetChild(1).GetChild(x).name == "NextPage")
            {
                nextPageButton = explorer.transform.GetChild(1).GetChild(x).gameObject;
            }
        }
    }

    /// <summary>
    /// Init the warning info
    /// </summary>
    private void InitInfoWindow()
    {
        // Info window
        infoWindow = Instantiate(newInfoWindow, new Vector3(0, -5, 0), Quaternion.identity);
        infoWindow.name = "InfoWindow";
        infoWindow.transform.position = new Vector3(0, -5, 0);
        infoWindow.transform.localScale = new Vector3(0, 0, 0);
    }
    #endregion


    #region ENABLE or DISABLE the PROJECT MANAGER
    /// <summary>
    /// Enable the Project Manager
    /// </summary>
    public void EnableManager()
    {
        //projectMenu.transform.localScale = new Vector3(0, 0, 0);
        //projectMenu.transform.position = new Vector3(0, -5, 0);

        OpenProjectMenu(_ActionsManager.GetComponent<ActionsManager>().WasRightHander());
    }

    /// <summary>
    /// Disable the Project Manager & all menus
    /// </summary>
    public void DisableManager()
    {
        // Disable pointer
        leftPointer.gameObject.SetActive(false);
        rightPointer.gameObject.SetActive(false);

        // Disable menu
        projectMenu.transform.localScale = new Vector3(0, 0, 0);
        projectMenu.transform.position = new Vector3(0, -5, 0);
        projectMenuIsOpen = false;

        // Disable keyboard
        keyboard.transform.localScale = new Vector3(0, 0, 0);
        keyboard.transform.position = new Vector3(0, -5, 0);

        // Disable explorer
        explorer.transform.localScale = new Vector3(0, 0, 0);
        explorer.transform.position = new Vector3(0, -5, 0);

        // Disable warning info
        warningInfo.transform.localScale = new Vector3(0, 0, 0);
        warningInfo.transform.position = new Vector3(0, -5, 0);

        // Disable info window
        infoWindow.transform.localScale = new Vector3(0, 0, 0);
        infoWindow.transform.position = new Vector3(0, -5, 0);
    }
    #endregion


    #region INTERACTIONS with CONTROLLERS (Trigger)
    /// <summary>
    /// If user is pressing the trigger...
    /// </summary>
    /// <param name="myPlayerHander"></param>
    public void PressTrigger(bool myPlayerHander)
    {
        // If none of the menus is open...
        if (projectMenuIsOpen == false && keyboardIsOpen == false && explorerIsOpen == false && warningInfoIsOpen == false && infoWindowIsOpen == false)
        {
            // Open the Project menu in front of the current controller
            OpenProjectMenu(myPlayerHander);
        }

        // Else if the Project menu is open...
        else if (projectMenuIsOpen == true && keyboardIsOpen == false && explorerIsOpen == false && warningInfoIsOpen == false && infoWindowIsOpen == false)
        {
            // If the player uses the same controller to open & interact with the menu...
            if (myPlayerHander == rightHander)
            {
                // Update the menu if user clicked on a menu button
                UpdateMode("");
                rightHander = myPlayerHander;
            }

            // Else if the player uses the other controller...
            else
            {
                // Close the menu if user pressed the trigger of the other controller
                //CloseProjectMenu();
            }
        }

        // Else if the Keyboard menu is open...
        else if (keyboardIsOpen == true)
        {
            // If the player uses the same controller to open & interact with the menu...
            if (myPlayerHander == rightHander)
            {
                // Update the menu if user clicked on a menu button
                UpdateKeyboard();
                rightHander = myPlayerHander;
            }

            // Else if the player uses the other controller...
            else
            {
                // Close the menu if user pressed the trigger of the other controller
                //CloseKeyboard();
            }
        }

        // Else if the Explorer menu is open...
        else if (explorerIsOpen == true)
        {
            // If the player uses the same controller to open & interact with the menu...
            if (myPlayerHander == rightHander)
            {
                // Update the menu if user clicked on a menu button
                UpdateExplorer();
                rightHander = myPlayerHander;
            }

            // Else if the player uses the other controller...
            else
            {
                // Close the menu if user pressed the trigger of the other controller
                //CloseExplorer();
            }
        }

        // Else if the Warning menu is open...
        else if (warningInfoIsOpen == true)
        {
            // If the player uses the same controller to open & interact with the menu...
            if (myPlayerHander == rightHander)
            {
                // Update the menu if user clicked on a menu button
                UpdateWarningInfo();
                rightHander = myPlayerHander;
            }

            // Else if the player uses the other controller...
            else
            {
                // Close the menu if user pressed the trigger of the other controller
                //CloseWarningInfo();
            }
        }

        // Else if the Info window is open...
        else if (infoWindowIsOpen == true)
        {
            // If the player uses the same controller to open & interact with the menu...
            if (myPlayerHander == rightHander)
            {
                // Update the menu if user clicked on a menu button
                UpdateInfoWindow();
                rightHander = myPlayerHander;
            }

            // Else if the player uses the other controller...
            else
            {
                // Close the menu if user pressed the trigger of the other controller
                //CloseInfoWindow();
            }
        }

        // Save the controller used to interact with the menu (left or right)
        //rightHander = myPlayerHander;
    }
    #endregion


    #region INTERACTIONS with PROJECT MENU
    /// <summary>
    /// Open the Project menu
    /// </summary>
    public void OpenProjectMenu(bool myPlayerHander)
    {
        if (coroutineIsRunning_ProjectMenu == false)
        {
            // Disable the GrabManager
            GrabManager.Instance.SetGrabbingActionEnabled(false);

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

            // Set the menu in front of the current controller
            projectMenu.transform.parent = currentController;
            projectMenu.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            projectMenu.transform.parent = null;

            // Menu is looking at the headset
            projectMenu.transform.rotation = Quaternion.LookRotation(projectMenu.transform.position - headset.position);

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Open the menu
            StartCoroutine(OnProjectMenu());

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
            projectMenuIsOpen = true;
        }
    }

    /// <summary>
    /// Update the current mode of the Project menu
    /// </summary>
    public void UpdateMode(string toolToOpen)
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();

        // Enable the GrabManager
        GrabManager.Instance.SetGrabbingActionEnabled(true);

        // Set the menu button hit by the user
        Transform hitButton = null;

        // If the current mode is updated by the user with the menu pointer...
        if (toolToOpen == "")
        {
            // Get the hit button
            if (rightHander == true)
            {
                hitButton = rightPointer.GetComponent<Project_Pointer>().GetHitButton();
            }
            else
            {
                hitButton = leftPointer.GetComponent<Project_Pointer>().GetHitButton();
            }

            // If user clicked inside the menu
            if (hitButton != null)
            {
                currentMode = hitButton.name;
            }
            else
            {
                currentMode = "";
            }
        }

        // Else if the current tool is updated by script...
        else if (toolToOpen != "")
        {
            currentMode = toolToOpen;
        }

        Debug.Log("<b>[ProjectManager]</b> ----- Select : " + currentMode + " -----");

        // Update the current mode :
        switch (currentMode)
        {
            // 1. Back to the Main Manager
            case "Back":
                DisableHelp();
                AppManager.Instance.UpdateMode("MainManager");
                break;

            // 2. Load the Sketcher Manager
            case "SketchManager":
                DisableHelp();
                AppManager.Instance.UpdateMode("SketchManager");
                break;

            // 3. Load the Capture Manager
            case "CaptureManager":
                DisableHelp();
                AppManager.Instance.UpdateMode("CaptureManager");
                break;

            // 4. Create a new project
            case "Project_New":
                CloseProjectMenu();

                // Open a warning info (= a new project will erase the current project)
                OpenWarningInfo(rightHander, "");
                break;

            // 5. Save the current project
            case "Project_Save":
                CloseProjectMenu();
                SaveProject();
                break;

            // 6. Load a project
            case "Project_Load":
                CloseProjectMenu();

                // Open a files explorer
                OpenExplorer(rightHander);
                break;

            // 7. Rename the project
            case "Project_Rename":
                CloseProjectMenu();

                // Open the keyboard
                OpenKeyboard(rightHander, "Enter the project name");
                break;

            // 8. Close the main menu
            case "CloseMenu":
                CloseProjectMenu();
                break;

            // 9. Close the main menu
            case "":
                CloseProjectMenu();
                break;
        }
    }

    /// <summary>
    /// Close the project menu
    /// </summary>
    public void CloseProjectMenu()
    {
        if (coroutineIsRunning_ProjectMenu == false)
        {
            if (projectMenuIsOpen == true)
            {
                StartCoroutine(OffProjectMenu());
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                projectMenuIsOpen = false;
            }
        }

        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();
    }

    /// <summary>
    /// Animation to open the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OnProjectMenu()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = mainMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning_ProjectMenu = true;

        // Enable buttons
        for (int x = 0; x < projectMenu.transform.GetChild(1).childCount; x++)
        {
            if (projectMenu.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                projectMenu.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            projectMenu.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning_ProjectMenu = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OffProjectMenu()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = projectMenu.transform.localScale;
        Vector3 finalScale = projectMenu.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning_ProjectMenu = true;

        // Disable buttons
        for (int x = 0; x < projectMenu.transform.GetChild(1).childCount; x++)
        {
            if (projectMenu.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                projectMenu.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            projectMenu.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        projectMenu.transform.localScale = new Vector3(0, 0, 0);
        projectMenu.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning_ProjectMenu = false;
        yield return null;
    }
    #endregion


    #region INTERACTIONS with WARNING INFO
    /// <summary>
    /// Open the warning info
    /// </summary>
    public void OpenWarningInfo(bool myPlayerHander, string description)
    {
        if (coroutineIsRunning_WarningInfo == false)
        {
            // Disable the GrabManager
            GrabManager.Instance.SetGrabbingActionEnabled(false);

            // Get the player hander (left or right)
            rightHander = myPlayerHander;

            // Get the current controller to display the menu
            Transform currentController;
            if (rightHander == true)
            {
                currentController = rightController;
            }
            else
            {
                currentController = leftController;
            }

            // Set the warning info in front of the controller
            warningInfo.transform.parent = currentController;
            warningInfo.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            warningInfo.transform.parent = null;

            // Menu is looking at the headset
            warningInfo.transform.rotation = Quaternion.LookRotation(warningInfo.transform.position - headset.position);

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Open the menu
            StartCoroutine(OnWarningInfo());

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
            warningInfoIsOpen = true;
        }
    }

    /// <summary>
    /// Update the warning info mode
    /// </summary>
    public void UpdateWarningInfo()
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();

        // Enable the GrabManager
        GrabManager.Instance.SetGrabbingActionEnabled(true);

        // Set the menu button hit by the user
        Transform hitButton = null;

        if (rightHander == true)
        {
            hitButton = rightPointer.GetComponent<Project_Pointer>().GetHitButton();
        }
        else
        {
            hitButton = leftPointer.GetComponent<Project_Pointer>().GetHitButton();
        }

        // If user clicked outside the menu
        if (hitButton == null)
        {

        }
        // Else if user close the menu
        else if (hitButton.name == "CloseWarningInfo")
        {
            CloseWarningInfo();
        }
        // Else if user clicked inside the menu
        else if (hitButton.name.StartsWith("Cancel"))
        {
            touchedWarningInfoButton = "Cancel";
            CloseWarningInfo();
        }
        else if (hitButton.name.StartsWith("Ok") || hitButton.name.StartsWith("OK"))
        {
            touchedWarningInfoButton = "Ok";
            CloseWarningInfo();

            // Show a warning message 
            OpenKeyboard(rightHander, "Enter the project name");

        }
    }

    /// <summary>
    /// Close the warning info
    /// </summary>
    public void CloseWarningInfo()
    {
        if (coroutineIsRunning_WarningInfo == false)
        {
            if (warningInfoIsOpen == true)
            {
                StartCoroutine(OffWarningInfo());
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                warningInfoIsOpen = false;
            }
        }

        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();
    }

    /// <summary>
    /// Animation to open the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OnWarningInfo()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = captureMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning_WarningInfo = true;

        // Enable buttons
        for (int x = 0; x < warningInfo.transform.GetChild(1).childCount; x++)
        {
            if (warningInfo.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                warningInfo.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            warningInfo.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning_WarningInfo = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OffWarningInfo()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = warningInfo.transform.localScale;
        Vector3 finalScale = warningInfo.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning_WarningInfo = true;

        // Disable buttons
        for (int x = 0; x < warningInfo.transform.GetChild(1).childCount; x++)
        {
            if (warningInfo.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                warningInfo.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            warningInfo.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        warningInfo.transform.localScale = new Vector3(0, 0, 0);
        warningInfo.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning_WarningInfo = false;

        yield return null;
    }
    #endregion


    #region INTERACTIONS with INFO WINDOW
    /// <summary>
    /// Open the warning info
    /// </summary>
    public void OpenInfoWindow(bool myPlayerHander, string infoText)
    {
        if (coroutineIsRunning_InfoWindow == false)
        {
            // Disable the GrabManager
            GrabManager.Instance.SetGrabbingActionEnabled(false);

            // Get the player hander (left or right)
            rightHander = myPlayerHander;

            // Get the current controller to display the menu
            Transform currentController;
            if (rightHander == true)
            {
                currentController = rightController;
            }
            else
            {
                currentController = leftController;
            }

            // Set the info window in front of the controller
            infoWindow.transform.parent = currentController;
            infoWindow.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            infoWindow.transform.parent = null;

            // Menu is looking at the headset
            infoWindow.transform.rotation = Quaternion.LookRotation(infoWindow.transform.position - headset.position);

            // Set the info on the window
            infoWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = infoText;

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Open the menu
            StartCoroutine(OnInfoWindow());

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
            infoWindowIsOpen = true;
        }
    }

    /// <summary>
    /// Update the info window mode
    /// </summary>
    public void UpdateInfoWindow()
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();

        // Enable the GrabManager
        GrabManager.Instance.SetGrabbingActionEnabled(true);

        // Set the menu button hit by the user
        Transform hitButton = null;

        if (rightHander == true)
        {
            hitButton = rightPointer.GetComponent<Project_Pointer>().GetHitButton();
        }
        else
        {
            hitButton = leftPointer.GetComponent<Project_Pointer>().GetHitButton();
        }

        // If user clicked outside the menu
        if (hitButton == null)
        {

        }
        // Else if user close the menu
        /*else if (hitButton.name == "CloseWarningInfo")
        {
            CloseWarningInfo();
        }*/
        // Else if user clicked inside the menu
        /*else if (hitButton.name.StartsWith("Cancel"))
        {
            touchedWarningInfoButton = "Cancel";
            CloseWarningInfo();
        }*/
        else if (hitButton.name.StartsWith("Ok") || hitButton.name.StartsWith("OK"))
        {
            touchedInfoWindowButton = "Ok";
            CloseInfoWindow();
        }
    }

    /// <summary>
    /// Close the info window
    /// </summary>
    public void CloseInfoWindow()
    {
        if (coroutineIsRunning_InfoWindow == false)
        {
            if (infoWindowIsOpen == true)
            {
                StartCoroutine(OffInfoWindow());
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                infoWindowIsOpen = false;
            }
        }

        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();
    }

    /// <summary>
    /// Animation to open the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OnInfoWindow()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = captureMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning_InfoWindow = true;

        // Enable buttons
        for (int x = 0; x < infoWindow.transform.GetChild(1).childCount; x++)
        {
            if (infoWindow.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                infoWindow.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            infoWindow.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning_InfoWindow = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OffInfoWindow()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = warningInfo.transform.localScale;
        Vector3 finalScale = warningInfo.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning_InfoWindow = true;

        // Disable buttons
        for (int x = 0; x < infoWindow.transform.GetChild(1).childCount; x++)
        {
            if (infoWindow.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                infoWindow.transform.GetChild(1).GetChild(x).GetComponent<Project_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            infoWindow.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        infoWindow.transform.localScale = new Vector3(0, 0, 0);
        infoWindow.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning_InfoWindow = false;

        yield return null;
    }
    #endregion


    #region INTERACTIONS with KEYBOARD
    /// <summary>
    /// Open the keyboard
    /// </summary>
    public void OpenKeyboard(bool myPlayerHander, string description)
    {
        if (coroutineIsRunning_Keyboard == false)
        {
            // Disable the GrabManager
            GrabManager.Instance.SetGrabbingActionEnabled(false);

            // Get the player hander
            rightHander = myPlayerHander;

            // Get the current controller to display the menu
            Transform currentController;
            if (rightHander == true)
            {
                currentController = rightController;
            }
            else
            {
                currentController = leftController;
            }

            // Init the description of the keyboard
            if (description != "")
            {
                keyboard.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = description;
            }
            else
            {
                keyboard.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = "Enter a text";
            }


            // Set the keyboard in front of the controller
            keyboard.transform.parent = currentController;
            keyboard.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            keyboard.transform.parent = null;

            // Menu is looking at the headset
            keyboard.transform.rotation = Quaternion.LookRotation(keyboard.transform.position - headset.position);

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Set the pointer
            if (rightHander == true)
            {
                rightPointer.SetActive(true);
            }
            else
            {
                leftPointer.SetActive(true);
            }

            StartCoroutine(OnKeyboard());

            keyboardIsOpen = true;
        }
    }

    /// <summary>
    /// Update the current mode of the sketching menu
    /// </summary>
    public void UpdateKeyboard()
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();

        // Enable the GrabManager
        GrabManager.Instance.SetGrabbingActionEnabled(true);

        // Set the menu button hit by the user
        Transform hitButton = null;

        if (rightHander == true)
        {
            hitButton = rightPointer.GetComponent<Project_Pointer>().GetHitButton();
        }
        else
        {
            hitButton = leftPointer.GetComponent<Project_Pointer>().GetHitButton();
        }

        // If user clicked outside the menu
        if (hitButton == null)
        {

        }
        // Else if user close the menu
        else if (hitButton.name == "CloseKeyboard")
        {
            CloseKeyboard();
        }
        // Else if user clicked inside the menu
        else if (hitButton.name.StartsWith("Key_"))
        {
            touchedKey = hitButton.name.Replace("Key_", "");
            myText = myText + touchedKey;
            keyboardInput.GetComponentInChildren<Text>().text = myText;
        }
        else if (hitButton.name.StartsWith("Clear"))
        {
            touchedKey = "Clear";
            myText = "";
            keyboardInput.GetComponentInChildren<Text>().text = myText;
        }
        else if (hitButton.name.StartsWith("BackLetter"))
        {
            touchedKey = "Back";
            if (myText.Length > 0)
            {
                myText = myText.Substring(0, myText.Length - 1);
                keyboardInput.GetComponentInChildren<Text>().text = myText;
            }
        }
        else if (hitButton.name.StartsWith("Ok") || hitButton.name.StartsWith("OK"))
        {
            //  Check if the project name is not empty
            if (myText.Length > 0)
            {
                touchedKey = "Ok";
                projectName = myText;
                CloseKeyboard();

                // Init the input text
                myText = "";
                keyboardInput.GetComponentInChildren<Text>().text = myText;

                // Display yhe project name over the menu title
                projectNameOnMenu.GetComponent<Text>().text = projectName;

                if (currentMode == "Project_New")
                {
                    // Create the new project
                    CreateNewProject(projectName);
                }
                else if (currentMode == "Project_Rename")
                {
                    project.projectDescription.name = projectName;

                    currentProjectPath = projectName;

                    // Show the info window if the current project is renamed
                    OpenInfoWindow(rightHander, "Project successfully renamed !");
                }
            }
        }
    }

    /// <summary>
    /// Close the keyboard
    /// </summary>
    public void CloseKeyboard()
    {
        if (coroutineIsRunning_Keyboard == false)
        {
            if (keyboardIsOpen == true)
            {
                StartCoroutine(OffKeyboard());
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                keyboardIsOpen = false;
            }
        }

        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();
    }

    /// <summary>
    /// Animation to open the keyboard
    /// </summary>
    /// <returns></returns>
    IEnumerator OnKeyboard()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = captureMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning_Keyboard = true;

        // Enable buttons
        for (int x = 0; x < keyboard.transform.GetChild(1).childCount; x++)
        {
            if (keyboard.transform.GetChild(1).GetChild(x).gameObject.activeSelf /*&& keyboard.transform.GetChild(1).GetChild(x).GetComponent<Keyboard_Button>()*/)
            {
                keyboard.transform.GetChild(1).GetChild(x).GetComponent<Keyboard_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            keyboard.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning_Keyboard = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the keyboard
    /// </summary>
    /// <returns></returns>
    IEnumerator OffKeyboard()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = keyboard.transform.localScale;
        Vector3 finalScale = keyboard.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning_Keyboard = true;

        // Disable buttons
        for (int x = 0; x < keyboard.transform.GetChild(1).childCount; x++)
        {
            if (keyboard.transform.GetChild(1).GetChild(x).gameObject.activeSelf /*&& keyboard.transform.GetChild(1).GetChild(x).GetComponent<Keyboard_Button>()*/)
            {
                keyboard.transform.GetChild(1).GetChild(x).GetComponent<Keyboard_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            keyboard.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        keyboard.transform.localScale = new Vector3(0, 0, 0);
        keyboard.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning_Keyboard = false;

        yield return null;
    }
    #endregion


    #region CREATE, SAVE & LOAD PROJECT
    /// <summary>
    /// Create a new project
    /// </summary>
    public void CreateNewProject(string myName)
    {
        // Clear the current project
        if (project != null)
        {
            // Clear folders & sketches in the scene
            for (int x = 0; x < workFolder.childCount; x++)
            {
                // Clear sketch folder
                if (workFolder.GetChild(x).name.StartsWith("SketchFolder_")) //if (workFolder.GetChild(x).name != "World" && workFolder.GetChild(x).name != "SymmetryPlane")
                {
                    Destroy(workFolder.GetChild(x).gameObject);
                }

                // Clear object folder
                if (workFolder.GetChild(x).name.StartsWith("ObjectFolder"))
                {
                    for (int y = 0; y < workFolder.GetChild(x).childCount; y++)
                    {
                        Destroy(workFolder.GetChild(x).GetChild(y).gameObject);
                    }
                }
            }

            // Clear the project & its elements (folder, sketches & objects)
            foreach (SketchFolderElement sfe in project.sketchFolderElement)
            {
                Debug.Log("Clear SKETCH");
                sfe.ClearSketch();
            }
            project.ClearSketchFolder();

            foreach (ObjectFolderElement ofe in project.objectFolderElement)
            {
                Debug.Log("Clear OBJECT");
                ofe.ClearObject_FlatFilter();
            }
            
            project.ClearObjectFolder();
        }

        // Clear actions in the history
        _HistoryManager.GetComponent<HistoryManager>().InitActionsLists_Y();
        _HistoryManager.GetComponent<HistoryManager>().InitActionsLists_Z();

        // Init the work folder & its children
        workFolder.transform.position = new Vector3(0, 0, 0);
        workFolder.transform.rotation = Quaternion.identity;
        workFolder.transform.localScale = new Vector3(1, 1, 1);
        objectFolder.transform.position = new Vector3(0, 0, 0);
        objectFolder.transform.rotation = Quaternion.identity;
        objectFolder.transform.localScale = new Vector3(1, 1, 1);

        // Init some value of the new project
        if (myName == "")
        {
            projectName = "My new project";
        }
        else
        {
            projectName = myName;
        }
        projectVersion = "001";

        project = new Project(projectName, projectVersion, workFolder.transform.position, workFolder.transform.rotation, workFolder.transform.localScale);

        currentProjectPath = projectName;

        Debug.Log("<b>[ProjectManager]</b> New project created (" + projectName + ") (v. " + projectVersion +")");
    }

    /// <summary>
    /// Save the current project
    /// </summary>
    public void SaveProject()
    {
        // Clear the previous saved project
        if (project != null)
        {
            foreach (SketchFolderElement fe in project.sketchFolderElement)
            {
                fe.ClearSketch();
            }
            project.ClearSketchFolder();

            foreach (ObjectFolderElement ofe in project.objectFolderElement)
            {
                ofe.ClearObject_FlatFilter();
            }
            project.ClearObjectFolder();
        }

        // Set the main saving folder
        string mainFolderPath = Application.dataPath + "/../_Projects/";
        if (!Directory.Exists(mainFolderPath))
        {
            Directory.CreateDirectory(mainFolderPath);
        }

        // Set the folder of the project
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            folderPath = mainFolderPath + currentProjectPath + "/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        // Set the file of the project
        string fileName = project.projectDescription.name + "_V" + project.projectDescription.version + "_" + System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.Day.ToString() + "_" + System.DateTime.Now.Hour.ToString() + "-" + System.DateTime.Now.Minute.ToString() + "-" + System.DateTime.Now.Second.ToString() + ".xml";
        string filePath = folderPath + fileName;

        // Set the world transform of the project
        project.projectDescription.worldPosition = workFolder.transform.position;
        project.projectDescription.worldRotation = workFolder.transform.rotation;
        project.projectDescription.worldScale = workFolder.transform.localScale;

        // Set folders, sketches & objects to save (don't save the 3D world of the scene)
        for (int a = 0; a < workFolder.childCount; a++)
        {
            // Save sketch folders & sketches
            if (workFolder.GetChild(a).name.StartsWith("SketchFolder_") ) 
            {
                // Add each sketching folder in the project
                Transform sketchFolderToSave = workFolder.GetChild(a);
                SketchFolderElement mySketchFolder = new SketchFolderElement(sketchFolderToSave.name, sketchFolderToSave.localPosition, sketchFolderToSave.localRotation, sketchFolderToSave.localScale);
                project.AddSketchFolder(mySketchFolder);

                // Add each sketch in the project
                for (int b = 0; b < workFolder.GetChild(a).childCount; b++)
                {
                    Sketch sketchToSave = workFolder.GetChild(a).GetChild(b).GetComponent<Sketch>();

                    // If the sketch is active
                    if (sketchToSave.gameObject.activeSelf)
                    {
                        List<Vector3> waypoints = new List<Vector3>();

                        for (int c = 0; c < sketchToSave.GetComponent<LineRenderer>().positionCount; c++)
                            waypoints.Add(sketchToSave.GetComponent<LineRenderer>().GetPosition(c));

                        SketchElement mySketch = new SketchElement(sketchToSave.name, 
                            sketchToSave.transform.localPosition, 
                            sketchToSave.transform.localRotation, 
                            sketchToSave.transform.localScale, 
                            sketchToSave.InitialColor, sketchToSave.InitialSize, waypoints, sketchToSave.InitialScale);
                        mySketchFolder.AddSketch(mySketch);

                        waypoints.Clear();
                    }
                }
            }

            if (workFolder.GetChild(a).name.StartsWith("ObjectFolder") ) 
            {
                // Add each object folder in the project
                Transform objectFolderToSave = workFolder.GetChild(a);
                ObjectFolderElement myObjectFolder = new ObjectFolderElement(objectFolderToSave.name, objectFolderToSave.localPosition, objectFolderToSave.localRotation, objectFolderToSave.localScale);
                project.AddObjectFolder(myObjectFolder);

                // Add each object in the project
                for (int d = 0; d < workFolder.GetChild(a).childCount; d++)
                {
                    Transform objectToSave = workFolder.GetChild(a).GetChild(d);

                    // If the sketch is active
                    if (objectToSave.name.StartsWith("MH_Filter_Flat_") && objectToSave.gameObject.activeSelf)
                    {
                        ObjectElement_FlatFilter myObject = new ObjectElement_FlatFilter(objectToSave.name, objectToSave.localPosition, objectToSave.localRotation, objectToSave.localScale,
                            objectToSave.GetComponent<MH_FlatFilter>().GetLength(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetWidth(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetHeight(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetThickness(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetMediaThickness(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetFolds(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetFrameColor(),
                            objectToSave.GetComponent<MH_FlatFilter>().GetMediaColor());
                        myObjectFolder.AddObject_FlatFilter(myObject);
                    }
                }
            }
        }

        // Save the current project
        project.SaveXml(filePath);

        // Show the info window if a project is saved
        OpenInfoWindow(rightHander, "Project successfully saved !");

        Debug.Log("<b>[ProjectManager]</b> Save project (" + currentProjectPath + "/" + fileName + ")");
    }

    /// <summary>
    /// Load a project
    /// </summary>
    public void LoadProject(string myPath)
    {
        // Create a new project
        CreateNewProject("");

        // Load the file
        project.LoadXml(Application.dataPath + "/../_Projects/" + myPath);

        // Set the world transform
        workFolder.transform.position = project.projectDescription.worldPosition;
        workFolder.transform.rotation = project.projectDescription.worldRotation;
        workFolder.transform.localScale = project.projectDescription.worldScale;

        // Set sketch folders & sketches
        foreach (SketchFolderElement sfe in project.sketchFolderElement)
        {
            CreateLoadedSketchFolder(sfe);

            foreach (SketchElement se in sfe.sketchElement)
                CreateLoadedSketch(se);
        }

        // Set object folders & objects
        foreach (ObjectFolderElement ofe in project.objectFolderElement)
        {
            SetObjectFolder(ofe);

            foreach (ObjectElement_FlatFilter oe_flatFilter in ofe.objectElement_FlatFilter)
                CreateLoadedObject_FlatFilter(oe_flatFilter);
        }

        currentProjectPath = subFolderPath;

        // Show the info window if a project is loaded
        OpenInfoWindow(rightHander, "Project successfully loaded !");

        Debug.Log("<b>[ProjectManager]</b> Load project (" + myPath + ")");
    }

    /// <summary>
    /// Create a loaded folder
    /// </summary>
    /// <param name="myFolder"></param>
    /// <returns></returns>
    public void CreateLoadedSketchFolder(SketchFolderElement myFolder)
    {
        //mySketch = PhotonNetwork.Instantiate("Prefabs/Flows/" + FLOW_Air_Clean.name, Vector3.zero, Quaternion.identity, 0);

        // Create the gameobject
        GameObject newFolder = Instantiate(folderPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // Set the gameobject
        if (newFolder != null)
        {
            newFolder.name = myFolder.id;
            newFolder.transform.parent = workFolder;
            newFolder.transform.localPosition = myFolder.position;
            newFolder.transform.localRotation = myFolder.rotation;
            newFolder.transform.localScale = myFolder.scale;
        }
    }

    /// <summary>
    /// Create a loaded sketch
    /// </summary>
    /// <param name="mySketchElement"></param>
    /// <returns></returns>
    public void CreateLoadedSketch(SketchElement mySketchElement)
    {
        // Create the gameobject
        GameObject newSketch = Instantiate(sketchPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        LineRenderer newLineRenderer = newSketch.GetComponent<LineRenderer>();
        
        // Set the gameobject
        if (newSketch != null)
        {
            // Set the name & the folder
            newSketch.name = mySketchElement.id;
            newSketch.transform.parent = workFolder.GetChild(workFolder.childCount - 1);

            // Set the transform
            newSketch.transform.localPosition = mySketchElement.position;
            newSketch.transform.localRotation = mySketchElement.rotation;
            newSketch.transform.localScale = mySketchElement.scale;

            // Set the color
            newLineRenderer.startColor = Color.white;
            newLineRenderer.endColor = Color.white;
            newSketch.GetComponent<Renderer>().material.color = mySketchElement.color;

            // Set the size of the sketch
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.0f);
            curve.AddKey(0.1f, mySketchElement.size);
            curve.AddKey(0.9f, mySketchElement.size);
            curve.AddKey(1.0f, 0.0f);
            newLineRenderer.widthCurve = curve;

            // Set the pathway
            newLineRenderer.positionCount = mySketchElement.pathway.Count;
            for (int p = 0; p < mySketchElement.pathway.Count; p++)
            {
                newLineRenderer.SetPosition(p, mySketchElement.pathway[p]);
            }

            // Create the collider
            if (newSketch.GetComponent<MeshCollider>() == null)
            {
                newSketch.GetComponent<Sketch_LineCollider>().CreateColliderFromLine(newLineRenderer);
            }

            // Set some values in the "Sketch" script
            newSketch.GetComponent<Sketch>().SetSettings(mySketchElement.scale.x, mySketchElement.size, mySketchElement.color);
            newSketch.GetComponent<Sketch>().SetSketchSettings_V2(mySketchElement.sketchScale);

            // Update the sketch size with the world scale
            newLineRenderer.widthMultiplier = workFolder.transform.localScale.x * newSketch.GetComponent<Sketch>().InitialScale;
        }
    }

    /// <summary>
    /// Create a loaded folder
    /// </summary>
    /// <param name="myObjectFolder"></param>
    /// <returns></returns>
    public void SetObjectFolder(ObjectFolderElement myObjectFolder)
    {
        objectFolder.transform.parent = workFolder;
        objectFolder.transform.localPosition = myObjectFolder.position;
        objectFolder.transform.localRotation = myObjectFolder.rotation;
        objectFolder.transform.localScale = myObjectFolder.scale;
    }

    /// <summary>
    /// Create a loaded sketch
    /// </summary>
    /// <param name="myObjectElement"></param>
    /// <returns></returns>
    public void CreateLoadedObject_FlatFilter(ObjectElement_FlatFilter myObjectElement)
    {
        // Create the gameobject
        GameObject newObject = Toolbox_FilterCreator.GetComponent<MH_FilterCreator>().CreateFlatFilter(myObjectElement.length, myObjectElement.width, myObjectElement.height, myObjectElement.thickness, myObjectElement.mediaThickness, myObjectElement.folds);

        // Set the gameobject
        if (newObject != null)
        {
            // Set the name & the folder
            newObject.name = myObjectElement.id;
            newObject.transform.parent = objectFolder;

            // Set the transform
            newObject.transform.localPosition = myObjectElement.position;
            newObject.transform.localRotation = myObjectElement.rotation;
            newObject.transform.localScale = myObjectElement.scale;

            // Set the color
            for (int x = 0; x < newObject.transform.childCount; x++)
                if (newObject.transform.GetChild(x).name == "FilterFrame")
                    newObject.transform.GetChild(x).GetComponent<Renderer>().material.color = myObjectElement.frameColor;
                else if (newObject.transform.GetChild(x).name == "FilterMedia")
                    newObject.transform.GetChild(x).GetComponent<Renderer>().material.color = myObjectElement.mediaColor;
        }
    }
    #endregion


    #region FOLDERS & FILES EXPLORER
    /// <summary>
    /// Open the sketching menu
    /// </summary>
    public void OpenExplorer(bool myPlayerHander)
    {
        if (coroutineIsRunning_Explorer == false)
        {
            // Disable the GrabManager
            GrabManager.Instance.SetGrabbingActionEnabled(false);

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

            // Set the explorer in front of the controller
            explorer.transform.parent = currentController;
            explorer.transform.localPosition = new Vector3(0, 0, 0.15f); // Only for HTC Vive
            explorer.transform.parent = null;

            // Menu is looking at the headset
            explorer.transform.rotation = Quaternion.LookRotation(explorer.transform.position - headset.position);
            //explorer.transform.localEulerAngles = new Vector3(currentController.transform.localEulerAngles.x + 30, currentController.transform.localEulerAngles.y, 0);

            // Clean the area between the controller & the menu
            menuCleaner.GetComponent<MenuCleaner>().CleanMenuArea(currentController, headset);

            // Open the menu
            StartCoroutine(OnExplorer());

            // Set the pointer
            if (rightHander == true)
            {
                rightPointer.SetActive(true);
            }
            else
            {
                leftPointer.SetActive(true);
            }

            // Set the current state of the menu
            explorerIsOpen = true;
        }
    }

    /// <summary>
    /// Update folders & files in the explorer
    /// </summary>
    public void UpdateExplorer()
    {
        // Unclean the area between the controller & the menu
        menuCleaner.GetComponent<MenuCleaner>().UndoCleaning();

        // Enable the GrabManager
        GrabManager.Instance.SetGrabbingActionEnabled(true);

        // Set the menu button hit by the user
        Transform hitButton = null;

        if (rightHander == true)
        {
            hitButton = rightPointer.GetComponent<Project_Pointer>().GetHitButton();
        }
        else
        {
            hitButton = leftPointer.GetComponent<Project_Pointer>().GetHitButton();
        }

        // If user clicked outside the menu
        if (hitButton == null)
        {
            CloseExplorer();
        }
        // Else if user close the menu
        else if (hitButton.name == "Close")
        {
            CloseExplorer();
        }
        // Else if user want to show folder parent
        else if (hitButton.name == "BackToParentFolder")
        {
            // If the current folder is a subfolder
            if (subFolderPath != "")
            {
                // Get all parents folders of the subfolder
                string[] folderParents = subFolderPath.Split('/');

                // Update the new folder path (without the last subfolder)
                subFolderPath = "";
                for (int w = 1; w < folderParents.Length - 1; w++)
                {
                    subFolderPath += "/" + folderParents[w];
                }

                // Update the explorer
                pageNumber = 0;
                DisplayFilesInExplorer(subFolderPath);
            }
        }
        // Else if user clicked inside the menu
        else if (hitButton.name.StartsWith("ExplorerButton_"))
        {
            touchedFile = hitButton.name.Replace("ExplorerButton_", "");

            // Open a file
            if (touchedFile.EndsWith(".xml"))
            {
                CloseExplorer();
                LoadProject(subFolderPath + "/" + touchedFile);
            }

            // Open a folder
            else
            {
                pageNumber = 0;
                subFolderPath += "/" + touchedFile;
                
                // Update the explorer
                pageNumber = 0;
                DisplayFilesInExplorer(subFolderPath);
            }

        }
        // Else if user clicked on the next page of files
        else if (hitButton.name.StartsWith("NextPage"))
        {
            // Update the explorer
            if (pageNumber < maxPageNumber)
            {
                pageNumber++;
                DisplayFilesInExplorer(subFolderPath);
            }
        }
        // Else if user clicked on the previous page of files
        else if (hitButton.name.StartsWith("PreviousPage"))
        {
            // Update the explorer
            if (pageNumber > 0)
            {
                pageNumber--;
                DisplayFilesInExplorer(subFolderPath);
            }
        }
    }


    /// <summary>
    /// Close the explorer menu
    /// </summary>
    public void CloseExplorer()
    {
        if (coroutineIsRunning_Explorer == false)
        {
            if (explorerIsOpen == true)
            {
                StartCoroutine(OffExplorer());
                leftPointer.gameObject.SetActive(false);
                rightPointer.gameObject.SetActive(false);
                explorerIsOpen = false;
            }
        }
    }

    /// <summary>
    /// Animation to open the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OnExplorer()
    {
        float elapsedTime = 0f;
        float finalTime = 0.025f; // 0.25f

        //Vector3 startScale = captureMenu.transform.localScale;
        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 finalScale = new Vector3(1, 1, 1);

        coroutineIsRunning_Explorer = true;

        // Init path to file
        filePath = "";
        subFolderPath = "";
        
        pageNumber = 0;
        previousPageButton.SetActive(false);
        nextPageButton.SetActive(false);

        DisplayFilesInExplorer("");

        // Enable buttons
        for (int x = 0; x < explorer.transform.GetChild(1).childCount; x++)
        {
            if (explorer.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                explorer.transform.GetChild(1).GetChild(x).GetComponent<ExplorerMenu_Button>().EnableButton();
            }
        }

        // Enable menu
        while (elapsedTime < finalTime)
        {
            explorer.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coroutineIsRunning_Explorer = false;

        yield return null;
    }

    /// <summary>
    /// Animation to close the menu
    /// </summary>
    /// <returns></returns>
    IEnumerator OffExplorer()
    {
        float elapsedTime = 0f;
        float finalTime = 0.25f;

        Vector3 startScale = explorer.transform.localScale;
        Vector3 finalScale = explorer.transform.localScale;
        //Vector3 finalScale = new Vector3(0, 0, 0);

        coroutineIsRunning_Explorer = true;

        // Disable buttons
        for (int x = 0; x < explorer.transform.GetChild(1).childCount; x++)
        {
            if (explorer.transform.GetChild(1).GetChild(x).gameObject.activeSelf)
            {
                explorer.transform.GetChild(1).GetChild(x).GetComponent<ExplorerMenu_Button>().DisableButton();
            }
        }

        // Disable menu
        while (elapsedTime < finalTime)
        {
            explorer.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / finalTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Set the menu in the waiting area
        explorer.transform.localScale = new Vector3(0, 0, 0);
        explorer.transform.position = new Vector3(0, -5, 0);

        coroutineIsRunning_Explorer = false;

        yield return null;
    }

    /// <summary>
    /// Display folders & files in the explorer
    /// </summary>
    public void DisplayFilesInExplorer(string mySubFolder)
    {
        // Destroy oldest buttons
        foreach (GameObject g in explorerButtons)
        {
            Destroy(g);
        }
        explorerButtons.Clear();

        // Chargement des dossiers contenant les revues
        string projectsMainFolder = Application.dataPath + "/../_Projects/";
        if (mySubFolder != "")
        {
            projectsMainFolder = Application.dataPath + "/../_Projects/" + mySubFolder + "/";
        }

        filesPath.Clear();

        int xPos = 0;
        int yPos = 0;

        // Get the path of each folder in the main folder
        foreach (string s in Directory.GetDirectories(projectsMainFolder))
        {
            filesPath.Add(s);
        }

        // Get the path of each file in the main folder
        foreach (string s in Directory.GetFiles(projectsMainFolder))
        {
            // Display .XML files only
            if (s.EndsWith(".xml"))
            {
                filesPath.Add(s);
            }
        }

        // Create each button in the explorer
        for (int z = 0; z < filesPath.Count; z++)
        {
            if (z < (pageNumber+1) * 15 && z >= pageNumber * 15)
            {
                // Get the button name
                string buttonName = filesPath[z].Replace(projectsMainFolder, "");

                // Add a button (to open a file or a folder)
                GameObject explorerButton;
                if (buttonName.EndsWith(".xml"))
                {
                    explorerButton = Instantiate(newButtonToFile);
                }
                else
                {
                    explorerButton = Instantiate(newButtonToFolder);
                }

                explorerButton.transform.parent = explorer.transform.GetChild(1);
                explorerButton.name = "ExplorerButton_" + buttonName;
                explorerButton.transform.localPosition = new Vector3(-135 + (135 * xPos), 67.5f - (45 * yPos), 0);
                explorerButton.transform.localRotation = new Quaternion(0, 0, 0, 1);
                explorerButton.transform.localScale = new Vector3(18, 6, 6);
                explorerButton.transform.GetComponentInChildren<Text>().text = buttonName;
                explorerButton.transform.GetComponent<BoxCollider>().enabled = true;
                explorerButtons.Add(explorerButton);

                // Set the initial transform values of the new button
                explorerButton.GetComponent<ExplorerMenu_Button>().InitInitialValues(explorerButton.transform.localPosition, explorerButton.transform.localScale);

                if (xPos < 2)
                {
                    xPos++;
                }
                else
                {
                    xPos = 0;
                    yPos++;
                }
            }
            
        }

        // Set the maximum page number
        maxPageNumber = filesPath.Count/15;

        previousPageButton.SetActive(true);
        nextPageButton.SetActive(true);
        
        // Display buttons to change the page of files
        /*if (filesPath.Count < (pageNumber + 1) * 15)
        {
            nextPageButton.SetActive(false);
        }
        else
        {
            nextPageButton.SetActive(true);
        }

        if (pageNumber == 0)
        {
            previousPageButton.SetActive(false);
        }
        else
        {
            previousPageButton.SetActive(true);
        }*/
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

        Debug.Log("<b>[ProjectManager]</b> Display help (" + isHelping + ")");
    }

    public void UpdateHelp()
    {
        // Show help on conrollers
        if (isHelping == true)
        {
            string myCurrentMode = currentMode;

            // Update helper
            switch (myCurrentMode)
            {
                default:
                    leftHelpers[0].GetComponent<ControllerHelper>().Display(""); // Grip : Press the grip to grab a sketch
                    leftHelpers[1].GetComponent<ControllerHelper>().Display(""); // Interactor
                    leftHelpers[2].GetComponent<ControllerHelper>().Display(""); // Touchpad_Left
                    leftHelpers[3].GetComponent<ControllerHelper>().Display(""); // Touchpad_Right
                    leftHelpers[4].GetComponent<ControllerHelper>().Display("Press once the trigger to open the menu"); // Trigger

                    rightHelpers[0].GetComponent<ControllerHelper>().Display(""); // Grip : Press the grip to grab a sketch
                    rightHelpers[1].GetComponent<ControllerHelper>().Display(""); // Interactor
                    rightHelpers[2].GetComponent<ControllerHelper>().Display(""); // Touchpad_Left
                    rightHelpers[3].GetComponent<ControllerHelper>().Display(""); // Touchpad_Right
                    rightHelpers[4].GetComponent<ControllerHelper>().Display("Press once the trigger to open the menu"); // Trigger
                    break;
            }
        }
    }

    public void DisableHelp()
    {
        leftHelpers[0].GetComponent<ControllerHelper>().Display(""); // Grip
        leftHelpers[1].GetComponent<ControllerHelper>().Display(""); // Interactor
        leftHelpers[2].GetComponent<ControllerHelper>().Display(""); // Touchpad_Left
        leftHelpers[3].GetComponent<ControllerHelper>().Display(""); // Touchpad_Right
        leftHelpers[4].GetComponent<ControllerHelper>().Display(""); // Trigger

        rightHelpers[0].GetComponent<ControllerHelper>().Display(""); // Grip
        rightHelpers[1].GetComponent<ControllerHelper>().Display(""); // Interactor
        rightHelpers[2].GetComponent<ControllerHelper>().Display(""); // Touchpad_Left
        rightHelpers[3].GetComponent<ControllerHelper>().Display(""); // Touchpad_Right
        rightHelpers[4].GetComponent<ControllerHelper>().Display(""); // Trigger

        isHelping = false;
    }
    #endregion
}
