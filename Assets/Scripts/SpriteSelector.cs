using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSelector : MonoBehaviour
{
    public Sprite[] groundSprites;

    void Start()
    {
        ChangeGroundSprite();
    }

    void ChangeGroundSprite()
    {
        if (groundSprites.Length > 0)
        {
            int randomGroundIndex = Random.Range(0, groundSprites.Length);
            Sprite selectedSprite = groundSprites[randomGroundIndex];

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 1001;
            spriteRenderer.sprite = selectedSprite;

            float scaleX = 8.328769f;
            float scaleY = 6.269063f;

            spriteRenderer.size = new Vector2(spriteRenderer.size.x * scaleX, spriteRenderer.size.y * scaleY);
        }
        else
        {
            Debug.LogError("No ground sprites assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
