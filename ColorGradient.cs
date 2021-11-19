using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace LonelyZombie
{
    public class ColorGradient : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerExitHandler, IBeginDragHandler, IPointerEnterHandler
    {
        GameObject selector;
        Image myImage;
        RectTransform myRect;
        public GameObject ColorPreview;
        public ColorPickerWindowHandle myHandle;
        private bool dragging = false;
        // Start is called before the first frame update
        public void Initialize()
        {
            selector = transform.GetChild(0).gameObject;
            myImage = GetComponent<Image>();
            myRect = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging)
                dragging = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            dragging = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            selector.transform.position = new Vector2(eventData.position.x, eventData.position.y);
            Vector2 point = CalculateTexturePosition(eventData.position);
            changeSelectedColor(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));

        }

        public void resetSelectorPosition()
        {
            selector.transform.position = new Vector3(myRect.transform.position.x + myRect.rect.width - 1, myRect.transform.position.y - 1);
            Vector2 point = CalculateTexturePosition(selector.transform.position);
            ColorPreview.GetComponent<Image>().color = myImage.sprite.texture.GetPixel((int)point.x, (int)point.y);
        }

        void changeSelectedColor(Color color)
        {
            myHandle.selected_color = color;
            myHandle.event_color_select_dynamic.Invoke(color);
            ColorPreview.GetComponent<Image>().color = color;
        }

        Vector2 CalculateTexturePosition(Vector2 pointerPosition)
        {
            return new Vector2(((pointerPosition.x - myRect.position.x) / myRect.rect.size.x) * myImage.sprite.texture.width, (1 - (myRect.position.y - pointerPosition.y) / myRect.rect.size.y) * myImage.sprite.texture.height);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                selector.transform.position = new Vector2(eventData.position.x, eventData.position.y);
                Vector2 point = CalculateTexturePosition(eventData.position);
                changeSelectedColor(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));
            }
        }


        float getColorVerticalSetp(float initialValue)
        {
            float retValue = 0;
            if (initialValue == 1)
                retValue = 1.0f;
            else if (initialValue > 0)
                retValue = initialValue;
            return retValue;
        }

        float getColorHorizontalstep(float initialValue, float targetValue)
        {
            float retValue = 0;
            if (initialValue == 0.0f)
                retValue = targetValue / 255.0f;
            else if (initialValue < 255.0f)
                retValue = (targetValue - initialValue) / 255.0f;
            return retValue;
        }
        public void generateGradient(Color startingColor)
        {
            Texture2D texture = new Texture2D(255, 255);
            texture.filterMode = FilterMode.Point;
            Color[] pixels = texture.GetPixels();
            float x = 0;
            float y = 0;
            int k = 0;
            Color[] copy = (Color[])pixels.Clone();
            float vert_r = getColorVerticalSetp(startingColor.r);
            float vert_g = getColorVerticalSetp(startingColor.g);
            float vert_b = getColorVerticalSetp(startingColor.b);
            float hori_r = 0;
            float hori_g = 0;
            float hori_b = 0;
            foreach (Color pixel in pixels)
            {
                copy[k].a = 1;
                if (x % 255 == 0)
                {
                    x = 0;
                    y++;
                    hori_r = getColorHorizontalstep(y * vert_r, y);
                    hori_g = getColorHorizontalstep(y * vert_g, y);
                    hori_b = getColorHorizontalstep(y * vert_b, y);
                }
                copy[k].r = y / 255.0f - (x * hori_r) / 255.0f;
                copy[k].g = y / 255.0f - (x * hori_g) / 255.0f;
                copy[k].b = y / 255.0f - (x * hori_b) / 255.0f;
                x++;
                k++;
            }
            texture.SetPixels(copy);
            texture.Apply();
            myImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Vector2 point = CalculateTexturePosition(selector.transform.position);
            changeSelectedColor(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));
        }
    }
}