using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public static class PUNUtilities
{
    public static PhotonView SetupPhotonView(this Component component)
    {
        PhotonView view = component.GetOrAddComponent<PhotonView>();
        if (view.ObservedComponents == null)
            view.ObservedComponents = new List<Component>();
        if (!view.ObservedComponents.Contains(component))
            view.ObservedComponents.Add(component);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(component);
#endif
        return view;
    }
}
