using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditor;
using System;
namespace LonelyZombie
{
    

    public class ColorPicker : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerExitHandler, IBeginDragHandler, IPointerEnterHandler
    {
        // Start is called before the first frame update
        GameObject ColorSelector;
        public GameObject ColorGradient;
        public GameObject ColorPreview;
        public Color _searchColor;
        private Image myImage;
        private RectTransform myRect;
        private bool dragging = false;
        private ColorGradient gradientBox;
        public GameObject header;
        public GameObject accept;
        public GameObject cancel;
        public ColorPickerWindowHandle myHandle;

        public void Initialize()
        {
            TMPro.TextMeshProUGUI headerText = header.AddComponent<TMPro.TextMeshProUGUI>();
            headerText.enableAutoSizing = true;
            headerText.fontSizeMin = 6;
            headerText.fontSizeMax = 26;
            headerText.alignment = TMPro.TextAlignmentOptions.Center;
            headerText.text = "Color Picker";
            TMPro.TextMeshProUGUI acceptText = accept.transform.GetChild(0).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
            acceptText.enableAutoSizing = true;
            acceptText.fontSizeMin = 6;
            acceptText.fontSizeMax = 26;
            acceptText.alignment = TMPro.TextAlignmentOptions.Center;
            acceptText.text = "Select";
            TMPro.TextMeshProUGUI closeText = cancel.transform.GetChild(0).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
            closeText.enableAutoSizing = true;
            closeText.fontSizeMin = 6;
            closeText.fontSizeMax = 26;
            closeText.alignment = TMPro.TextAlignmentOptions.Center;
            closeText.text = "Close";
            ColorSelector = transform.GetChild(0).gameObject;
            Debug.Log(ColorSelector);
            myImage = GetComponent<Image>();
            myRect = GetComponent<RectTransform>();
            generateVerticalStrip();
            gradientBox = ColorGradient.GetComponent<ColorGradient>();
            Vector2 point = CalculateTexturePosition(ColorSelector.transform.position);
            ColorPreview.GetComponent<Image>().color = myImage.sprite.texture.GetPixel((int)point.x, (int)point.y);
            gradientBox.generateGradient(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));
            accept.GetComponent<Button>().onClick.AddListener(onAccept);
            cancel.GetComponent<Button>().onClick.AddListener(onCancel);
        }

        public void resetSelectorPosition()
        {
            Debug.Log("beginning reset position");
            Debug.Log(ColorSelector);
            Debug.Log(myRect);
            ColorSelector.transform.position = new Vector3(myRect.transform.position.x + myRect.rect.width - 1, myRect.transform.position.y + 1);
            Debug.Log("Changed my selector posityion");
            Vector2 point = CalculateTexturePosition(ColorSelector.transform.position);
            Debug.Log("point calculated");
            gradientBox.generateGradient(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));
            Debug.Log("gradient made");
            gradientBox.resetSelectorPosition();
            Debug.Log("gradient reset");
        }
        void onAccept()
        {
            myHandle.event_color_select.Invoke(ColorPreview.GetComponent<Image>().color);
        }

        void onCancel()
        {
            myHandle.changeColorPickerVisibility(false);
            myHandle.event_color_close.Invoke();
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
            ColorSelector.transform.position = new Vector2(eventData.position.x, eventData.position.y);
            Vector2 point = CalculateTexturePosition(eventData.position);
            ColorPreview.GetComponent<Image>().color = myImage.sprite.texture.GetPixel((int)point.x, (int)point.y);
            gradientBox.generateGradient(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));

        }
        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                ColorSelector.transform.position = new Vector2(eventData.position.x, eventData.position.y);
                Vector2 point = CalculateTexturePosition(eventData.position);
                gradientBox.generateGradient(myImage.sprite.texture.GetPixel((int)point.x, (int)point.y));
            }
        }

        Vector2 CalculateTexturePosition(Vector2 pointerPosition)
        {
            return new Vector2(((myRect.position.x - pointerPosition.x) / myRect.rect.size.x) * myImage.sprite.texture.width, ((pointerPosition.y - myRect.position.y) / myRect.rect.size.y) * myImage.sprite.texture.height);
        }

        void generateVerticalStrip()
        {
            Texture2D texture = new Texture2D(20, 1530);
            texture.filterMode = FilterMode.Point;
            Color[] pixels = texture.GetPixels();
            int i = 0;
            int k = 0;
            Color[] copy = (Color[])pixels.Clone();
            bool addition = false;
            foreach (Color pixel in pixels)
            {
                copy[k].a = 1;
                if (k % 20 == 0)
                {
                    if (k % (255 * 20) == 0)
                    {
                        addition = !addition;
                    }
                    if (addition)
                        i += 1;
                    else
                        i -= 1;
                }
                if (k < 255 * 20)
                {
                    copy[k].r = 1;
                    copy[k].g = i / 255.0f;
                    copy[k].b = 0;
                }
                else if (k < 510 * 20)
                {
                    copy[k].r = i / 255.0f;
                    copy[k].g = 1;
                    copy[k].b = 0;
                }
                else if (k < 765 * 20)
                {
                    copy[k].r = 0;
                    copy[k].g = 1;
                    copy[k].b = i / 255.0f;
                }
                else if (k < 1020 * 20)
                {
                    copy[k].r = 0;
                    copy[k].g = i / 255.0f;
                    copy[k].b = 1;
                }
                else if (k < 1275 * 20)
                {
                    copy[k].r = i / 255.0f;
                    copy[k].g = 0;
                    copy[k].b = 1;
                }
                else if (k < 1530 * 20)
                {
                    copy[k].r = 1;
                    copy[k].g = 0;
                    copy[k].b = i / 255.0f;
                }
                k++;
            }
            texture.SetPixels(copy);
            texture.Apply();
            myImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

    }
}