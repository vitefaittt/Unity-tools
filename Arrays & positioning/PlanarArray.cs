using UnityEngine;

namespace Arrays
{
    public class PlanarArray : Array
    {
        [SerializeField]
        float spacing = 1;

        public override void UpdateItemsTransform()
        {
            for (int i = 0, x = 0; x < amount; x++)
                for (int y = 0; y < amount; y++, i++)
                    transform.GetChild(i).transform.localPosition = new Vector3(x * spacing - (amount - 1) * .5f * spacing, 0, y * spacing - (amount - 1) * .5f * spacing);
        }

        protected override int TotalAmount()
        {
            return amount * amount;
        }
    }
}