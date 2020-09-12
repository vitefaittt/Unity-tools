using UnityEngine;

namespace Arrays
{
    public class InlineArray : Array
    {
        [SerializeField]
        float spacing = 1;
        [SerializeField]
        BoolVector3 direction = BoolVector3.right;

        public override void UpdateItemsTransform()
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).transform.localPosition = (Vector3)direction * spacing * i;
        }
    }
}
