using UnityEngine;

namespace Arrays
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Array))]
    public class ArrayTransformsRandomizer : MonoBehaviour
    {
        [SerializeField]
        Vector3 posOffset, rotOffset;
        Array array;
        int previousAmount;


        private void OnEnable()
        {
            if (array)
                return;
            array = GetComponent<Array>();
            RandomizeTransforms();
            array.TransformsUpdated += RandomizeNewTransforms;
        }

        private void OnValidate()
        {
            RandomizeTransforms();
        }

        private void OnDestroy()
        {
            array.TransformsUpdated -= RandomizeNewTransforms;
        }


        void RandomizeNewTransforms()
        {
            if (previousAmount == array.amount)
                return;

            RandomizeTransforms(previousAmount - 1);

            previousAmount = array.amount;
        }

        void RandomizeTransforms(int startIndex = 0)
        {
            print("randomizing");
            for (int i = startIndex; i < array.amount; i++)
            {
                print("previous pos: " + transform.GetChild(startIndex + i).position);
                transform.GetChild(startIndex + i).position += posOffset.Randomize();
                print("new pos: " + transform.GetChild(startIndex + i).position);
                //transform.GetChild(startIndex + i).eulerAngles += rotOffset.Randomize();
            }
        }
    }
}
