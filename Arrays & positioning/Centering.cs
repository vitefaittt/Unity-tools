void CreateButtons()
{
    for (int i = 0; i < shownButtons.Count; i++)
        shownButtons[i].transform.localPosition = new Vector3(spacing.x * (i % amountPerRow), -spacing.y * (i / amountPerRow), shownButtons[i].transform.localPosition.z);
    CenterOnX(shownButtons);
}

void CenterOnX(List<ActionButton> buttons)
{
    int rowAmount = Mathf.Max(1, buttons.Count / amountPerRow);
    for (int i = 0; i < rowAmount; i++)
    {
        float totalSpacing = 0;
        int rowItemsAmount = 1;
        for (int j = 0; j < amountPerRow; j++)
        {
            if (i * j + j >= buttons.Count - 1)
                j = amountPerRow;
            else
            {
                totalSpacing += spacing.x;
                rowItemsAmount++;
            }
        }
        for (int j = 0; j < rowItemsAmount; j++)
            buttons[j].transform.localPosition = buttons[j].transform.localPosition + Vector3.left * (totalSpacing * .5f);
    }
}