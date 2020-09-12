using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public static class XRUtilities
{
    public static void DropObjectNow(this XRDirectInteractor interactor)
    {
        if (interactor.selectTarget == null || !interactor.allowSelect)
            return;
        interactor.allowSelect = false;
        interactor.StartCoroutine(WaitForTwoFrames(() => interactor.allowSelect = true));
    }

    static IEnumerator WaitForTwoFrames(System.Action EndAction)
    {
        yield return null;
        yield return null;
        EndAction?.Invoke();
    }
}
