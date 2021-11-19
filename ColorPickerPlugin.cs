using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using LordAshes;
using UnityEngine.Events;

namespace LonelyZombie
{

    public class ColorPickerWindowHandle : MonoBehaviour
    {
        // Events
        public ColorPickerPlugin.ColorSelectEvent event_color_select;
        public UnityEvent event_color_close;
        public ColorPickerPlugin.ColorSelectEvent event_color_select_dynamic;
        // Selected color
        public Color selected_color;
        // Static variables
        static GameObject plugin_canvas;
        static AssetBundle color_picker_asset_bundle;
        // Private variables
        GameObject color_picker;
        GameObject go_color_strip;

        public ColorPickerWindowHandle()
        {
            Debug.Log("Color Picker - Plugin Initialized");
            // Create unity Event Instances
            if (event_color_select == null)
                event_color_select = new ColorPickerPlugin.ColorSelectEvent();
            if (event_color_close == null)
                event_color_close = new UnityEvent();
            if (event_color_select_dynamic == null)
                event_color_select_dynamic = new ColorPickerPlugin.ColorSelectEvent();
            SetupPluginWindow();
            setSizeNormalized(0.2f, 0.2f);
            setPositionNormalized(0.75f, 0.75f);
        }
        void SetupPluginWindow()
        {
            if (color_picker_asset_bundle == null)
            {
                color_picker_asset_bundle = FileAccessPlugin.AssetBundle.Load("colorpickerplugin");
            }
            // Create plugin canvas to place color picker on
            if (plugin_canvas == null)
            {
                plugin_canvas = new GameObject("ColorPluginCanvas");
                plugin_canvas.AddComponent<Canvas>();
            }
            Canvas myCanvas = plugin_canvas.GetComponent<Canvas>();
            myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            plugin_canvas.AddComponent<CanvasScaler>();
            plugin_canvas.AddComponent<GraphicRaycaster>();
            plugin_canvas = Instantiate(plugin_canvas);

            // Ensure that canvas survives loading of other scenes
            DontDestroyOnLoad(plugin_canvas);

            //Load Color Picker Assets and attach Scripts to them
            GameObject colorPickerAsset = (GameObject)color_picker_asset_bundle.LoadAsset("ColorPicker_noScripts");
            color_picker = Instantiate(colorPickerAsset);
            GameObject go_header = color_picker.transform.GetChild(0).gameObject;
            go_color_strip = color_picker.transform.GetChild(1).gameObject;
            GameObject go_color_gradient = color_picker.transform.GetChild(2).gameObject;
            GameObject go_color_preview = color_picker.transform.GetChild(3).gameObject;
            GameObject go_accept = color_picker.transform.GetChild(4).gameObject;
            GameObject go_cancel = color_picker.transform.GetChild(5).gameObject;
            ColorGradient color_gradient_script = go_color_gradient.AddComponent<ColorGradient>();
            color_gradient_script.ColorPreview = go_color_preview;
            color_gradient_script.myHandle = this;
            color_gradient_script.Initialize();
            ColorPicker color_picker_script = go_color_strip.AddComponent<ColorPicker>();
            color_picker_script.header = go_header;
            color_picker_script.ColorPreview = go_color_preview;
            color_picker_script.ColorGradient = go_color_gradient;
            color_picker_script.accept = go_accept;
            color_picker_script.cancel = go_cancel;
            color_picker_script.myHandle = this;
            color_picker_script.Initialize();

            // Set Color Picker off by default an create its instance
            color_picker.SetActive(false);

            // Attach Color Picker to plugin canvas
            color_picker.transform.parent = plugin_canvas.transform;
        }

        public void changeColorPickerVisibility(bool visible)
        {
            color_picker.SetActive(visible);
        }

        public void resetSelectedColor()
        {
            color_picker.GetComponent<ColorPicker>().resetSelectorPosition();
        }

        public void setPositionNormalized(float screen_x_percent, float screen_y_percent)
        {
            screen_x_percent = Mathf.Clamp(screen_x_percent, 0.0f, 1.0f);
            screen_y_percent = Mathf.Clamp(screen_y_percent, 0.0f, 1.0f);
            RectTransform canvas_transform = plugin_canvas.GetComponent<RectTransform>();
            color_picker.GetComponent<RectTransform>().position = new Vector3(canvas_transform.sizeDelta.x * screen_x_percent, canvas_transform.sizeDelta.y * screen_y_percent);
        }

        public void setPosition(int x, int y)
        {
            RectTransform canvas_transform = plugin_canvas.GetComponent<RectTransform>();
            x = Mathf.Clamp(x, 0, (int)canvas_transform.sizeDelta.x);
            y = Mathf.Clamp(y, 0, (int)canvas_transform.sizeDelta.y);
            color_picker.GetComponent<RectTransform>().position = new Vector3(x, y);
        }

        public void setSizeNormalized(float screen_width_percent, float screen_height_percent)
        {
            screen_width_percent = Mathf.Clamp(screen_width_percent, 0.1f, 0.5f);
            screen_height_percent = Mathf.Clamp(screen_height_percent, 0.1f, 0.5f);
            RectTransform canvas_transform = plugin_canvas.GetComponent<RectTransform>();
            color_picker.GetComponent<RectTransform>().sizeDelta = new Vector2(canvas_transform.sizeDelta.x * screen_width_percent, canvas_transform.sizeDelta.y * screen_height_percent);
            go_color_strip.GetComponent<ColorPicker>().resetSelectorPosition();
        }

        public void setSize(int width, int height)
        {
            RectTransform canvas_transform = plugin_canvas.GetComponent<RectTransform>();
            width = Mathf.Clamp(width, 0, (int)(canvas_transform.sizeDelta.x * 0.5f));
            height = Mathf.Clamp(height, 0, (int)(canvas_transform.sizeDelta.y * 0.1f));
            color_picker.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            go_color_strip.GetComponent<ColorPicker>().resetSelectorPosition();
        }
    }

    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public class ColorPickerPlugin : BaseUnityPlugin
    {
        public const string Name = "ColorPicker";
        public const string Guid = "org.lonleyzombie.plugins.colorpicker";
        public const string Version = "1.0.0.0";

        public class ColorSelectEvent : UnityEvent<Color>
        {
        }

    }
}
