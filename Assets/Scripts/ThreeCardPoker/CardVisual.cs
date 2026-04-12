using UnityEngine;

public class CardVisual : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite cardBack;

    public void SetCard(PokerCard card, bool faceUp)
    {
        if (card == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        if (faceUp)
        {
            string spriteName = card.GetSpriteName();
            Sprite loadedSprite = Resources.Load<Sprite>("Cards/" + spriteName); // Looks in Assets/Resources/Cards/

            if (loadedSprite != null) spriteRenderer.sprite = loadedSprite;
            else Debug.LogError($"Missing sprite: Resources/Cards/{spriteName}");
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}