using UnityEngine;
using UnityEngine.UI;

public class InterfaceNavigation : MonoBehaviour
{
    [SerializeField]
    NavItem[] navItems;


    private void Awake()
    {
        GetComponentInParent<DesktopInterface>().MenuOpened += UpdateNavigation;
    }

    private void Start()
    {
        UpdateNavigation(DesktopInterface.MenuType.Vocables);
    }


    void UpdateNavigation(DesktopInterface.MenuType menuType)
    {
        foreach (var navItem in navItems)
        {
            if (!navItem.Button)
                continue;
            navItem.Button.interactable = navItem.MenuType != menuType;
            if (UIVocableExtractor.Instance.IsEmpty && navItem.MenuType.EqualsAny(DesktopInterface.MenuType.VocableView, DesktopInterface.MenuType.Planete))
                navItem.Button.gameObject.SetActive(false);
            else
                navItem.Button.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
class NavItem
{
    [SerializeField]
    Button button;
    public Button Button { get { return button; } }
    [SerializeField]
    DesktopInterface.MenuType menuType;
    public DesktopInterface.MenuType MenuType { get { return menuType; } }
}