/*
 * Rich FX
 * Copyright (c) 2020-Present, Inan Evin, Inc. All rights reserved.
 * Author: Inan Evin
 * contact: inanevin@gmail.com
 *
 * Feel free to ask about the package and talk about recommendations!
 *
 *
 *
 */

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Rendering.HighDefinition;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace IE
{
    public class RichFXAP : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string[] entries = Array.FindAll(importedAssets, name => name.Contains("RichFXStartupEditor") && !name.EndsWith(".meta"));

            for (int i = 0; i < entries.Length; i++)
                if (RichFXStartupEditor.Init(false))
                    break;
        }
    }
    public sealed class RichFXStartupEditor : EditorWindow
    {
        public static string versionID = "ogs_v";
        static string imagePath = "Assets/InanEvin/RichFX/Editor/Images/RichFX_cover_img.png";

        private static Texture2D coverImage;
        Vector2 changelogScroll = Vector2.zero;
        GUIStyle labelStyle;
        GUIStyle buttonStyle;
        GUIStyle iconButtonStyle;
        RenderPipelineGlobalSettings globalSettings;
        private bool warningRPAsset;

        public enum CategoryFindResult { NotFound, FoundFull, FoundEmpty };

        [MenuItem("Help/RichFX/Launcher", false, 0)]
        public static void MenuInit()
        {
            Init(true);
        }

        [MenuItem("Help/RichFX/Online Docs", false, 0)]
        public static void MenuManual()
        {
            Application.OpenURL("https://inan.gitbook.io/rich-fx-documentation/");
        }

        public static bool Init(bool force)
        {
            imagePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("RichFX_cover_img", null)[0]);
            if (force || EditorPrefs.GetString(versionID) != changelogText.Split('\n')[0])
            {
                RichFXStartupEditor window;
                window = GetWindow<RichFXStartupEditor>(true, "About Rich FX", true);
                Vector2 size = new Vector2(620, 800);
                window.minSize = size;
                window.maxSize = size;
                window.ShowUtility();
                Refresh();
                return true;
            }

            return false;
        }

        void OnEnable()
        {
            EditorPrefs.SetString(versionID, changelogText.Split('\n')[0]);

            Refresh();
        }

        public static void Refresh()
        {
            imagePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("RichFX_cover_img", null)[0]);

            string versionColor = EditorGUIUtility.isProSkin ? "#ffffffee" : "#000000ee";
            int maxLength = 10000;
            bool tooLong = changelogText.Length > maxLength;
            if (tooLong)
            {
                changelogText = changelogText.Substring(0, maxLength);
                changelogText += "...\n\n<color=" + versionColor + ">[Check online documentation for more.]</color>";
            }
            changelogText = Regex.Replace(changelogText, @"^[0-9].*", "<color=" + versionColor + "><size=13><b>Version $0</b></size></color>", RegexOptions.Multiline);
            changelogText = Regex.Replace(changelogText, @"^- (\w+:)", "  <color=" + versionColor + ">$0</color>", RegexOptions.Multiline);
            coverImage = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
        }

        private void SetupLabelStyles()
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.richText = true;
            labelStyle.wordWrap = true;
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.richText = true;
        }

        void OnGUI()
        {
            SetupLabelStyles();

            Rect headerRect = new Rect(0, 0, 620, 245);
            GUI.DrawTexture(headerRect, coverImage, ScaleMode.StretchToFill, false);

            GUILayout.Space(250);

            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // Doc
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("<b><size=13>Documentation</size></b>\n<size=11>Online manual.</size>", buttonStyle, GUILayout.MaxWidth(310), GUILayout.Height(56)))
                        Application.OpenURL("https://inan.gitbook.io/rich-fx-documentation/");

                    if (GUILayout.Button("<b><size=13>Rate & Review</size></b>\n<size=11>Rate Rich FX on Asset Store!</size>", buttonStyle, GUILayout.Height(56)))
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/167489");

                    //if (GUILayout.Button("<b>Unity Forum Post</b>\n<size=9>Unity Community</size>", buttonStyle, GUILayout.Height(56)))
                        //Application.OpenURL("https://assetstore.unity.com/packages/slug/167489");
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("<b>E-mail</b>\n<size=9>inanevin@gmail.com</size>", buttonStyle, GUILayout.MaxWidth(310), GUILayout.Height(56)))
                        Application.OpenURL("mailto:inanevin@gmail.com");

                    if (GUILayout.Button("<b>Twitter</b>\n<size=9>@lineupthesky</size>", buttonStyle, GUILayout.Height(56)))
                        Application.OpenURL("http://twitter.com/lineupthesky");

                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                EditorGUILayout.BeginVertical("GroupBox");
                EditorGUILayout.LabelField("You can use the button below to automatically add all the effects to your render pipeline asset.");
                EditorGUILayout.LabelField("Or if you prefer, you can add them manually, check documentation for more details!");
                EditorGUILayout.EndVertical();

                EditorGUILayout.HelpBox("In Unity 2021.2.6 and above, the system uses Render Pipeline Global Settings asset file instead of HDRenderPipelineAsset.asset file. Every installation step " +
                    "mentioned in the documentation is same, except the file type.", MessageType.Warning);

                EditorGUILayout.BeginHorizontal();
                globalSettings = EditorGUILayout.ObjectField(" Global Settings .asset: ", globalSettings, typeof(RenderPipelineGlobalSettings), true) as RenderPipelineGlobalSettings;

                if (GUILayout.Button("Add Effects to Settings Asset", buttonStyle))
                {
                    if (globalSettings != null)
                    {
                        if (EditorUtility.DisplayDialog("Warning!", "Please make sure you have backed up your Global Settings asset. Any error might corrupt the asset file, rendering project unworkable. " +
                            "Please do back up your asset before proceeding!", "Proceed", "Cancel"))
                        {
                            string path = AssetDatabase.GetAssetPath(globalSettings);
                            List<string> bpTemp = new List<string>();
                            List<string> apTemp = new List<string>();
                            bpTemp.AddRange(bpFullEffectList);
                            apTemp.AddRange(apFullEffectList);

                            AddEffectsToList(path, beforePPFullID, beforePPEmptyID, afterPPFullID, afterPPEmptyID, bpTemp, bpEmptyEffectListAddition);
                            AddEffectsToList(path, afterPPFullID, afterPPEmptyID, "inanevinrichfxend", "inanevinrichfxend", apTemp, apEmptyEffectListAddition);
                        }
                       
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Asset is null!", "Please assign a RenderPipelineGlobalSettings asset!", "Done");
                    }
                }

                EditorGUILayout.EndHorizontal();


                if (globalSettings == null)
                {
                    EditorGUILayout.HelpBox("Please select your Global Settings file to add effects to it automatically!", MessageType.Warning);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                EditorGUILayout.BeginVertical("GroupBox");
                EditorGUILayout.LabelField("You can use the buttons below to copy effect strings if you want to manually edit the HD Render Pipeline asset file.");
                EditorGUILayout.LabelField("Head over to the documentation for more information!");

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Copy Before Post Process"))
                    EditorGUIUtility.systemCopyBuffer = beforePPCopy;
                if (GUILayout.Button("Copy After Post Process"))
                    EditorGUIUtility.systemCopyBuffer = afterPPCopy;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Please make sure you back up your pipeline asset before making any changes!", MessageType.Info);

                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                using (var scope = new GUILayout.ScrollViewScope(changelogScroll))
                {
                    GUILayout.Label(changelogText, labelStyle);
                    changelogScroll = scope.scrollPosition;
                }
            }
        }


        void AddEffectsToList(string path, string categoryFullID, string categoryEmptyID, string nextCategoryEmptyID, string nextCategoryFullID, List<string> targetListToCheck, string emptyEffectListAddition)
        {
            // FIRST SHOW DIALOGUE
            string line = "";
            int targetLine = 0;
            CategoryFindResult findResult = CategoryFindResult.NotFound;

            // Read all the lines.
            string[] all = File.ReadAllLines(path);

            try
            {
                StreamReader sr = new StreamReader(path, true);

                // Read the lines one by one.
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace(" ", string.Empty);
                    if (line == categoryEmptyID)
                    {
                        findResult = CategoryFindResult.FoundEmpty;
                        break;
                    }
                    // Increment which line the category is.
                    targetLine++;
                }

                // Dispose of stream.
                sr.Close();
                sr.Dispose();

                List<string> categoryContents = new List<string>();

                // If the category is not found yet, try finding the full list.
                if (findResult == CategoryFindResult.NotFound)
                {
                    StreamReader sr2 = new StreamReader(path, true);
                    targetLine = 0;
                    line = "";
                    bool isFound = false;

                    // Read the lines.
                    while ((line = sr2.ReadLine()) != null)
                    {

                        // If the category is found full, add its contents to the list.
                        if (isFound)
                        {
                            string replaced = line.Replace(" ", string.Empty);

                            // If we've hit the next category, just break, else keep adding the lines to the list.
                            if (replaced == nextCategoryEmptyID || replaced == nextCategoryFullID)
                                break;
                            else
                                categoryContents.Add(line);

                        }

                        line = line.Replace(" ", string.Empty);
                        if (line == categoryFullID)
                        {

                            findResult = CategoryFindResult.FoundFull;
                            isFound = true;
                        }

                        // Increment which line the category is.
                        if (!isFound)
                            targetLine++;

                    }

                    // Dispose of stream.
                    sr2.Close();
                    sr2.Dispose();
                }

                // If category could not be found output error.
                if (findResult == CategoryFindResult.NotFound)
                {
                    Debug.LogError("Categories for post processing can not be found! Try adding them manually by checking the documentation.");
                }
                else
                {
                    // Refresh database.
                    AssetDatabase.Refresh();

                    // If an empty list is found just add the ingredients!
                    if (findResult == CategoryFindResult.FoundEmpty)
                    {
                        all[targetLine] = emptyEffectListAddition;
                        File.WriteAllLines(path, all);
                    }
                    else
                    {
                        // If the list is found but not empty, check the contents.
                        for (int i = 0; i < categoryContents.Count; i++)
                        {

                            // Iterate contents to be added.
                            for(int j = 0; j < targetListToCheck.Count; j++)
                            {
                                // Remove whitespaces and make a comparison.
                                string currentContentToAdd = targetListToCheck[j].Replace(" ", string.Empty);
                                currentContentToAdd = currentContentToAdd.Replace("\n", string.Empty);

                                // If content is already added, remove it from the list.
                                if (currentContentToAdd == categoryContents[i].Replace(" ", string.Empty))
                                {
                                    targetListToCheck.RemoveAt(j);
                                    break;
                                }
                            }
                        }

                        // Remove the new lines from the last element of list to be added.
                        targetListToCheck[targetListToCheck.Count - 1] = targetListToCheck[targetListToCheck.Count - 1].Replace("\n", string.Empty);

                        // Assign first line.
                        all[targetLine] = targetListToCheck[0];

                        // Go through the trimmed list and add the contents.
                        for(int i = 1; i < targetListToCheck.Count; i++)
                            all[targetLine] += targetListToCheck[i];

                        // Write all.
                        File.WriteAllLines(path, all);

                    }

                    // Refresh database.
                    AssetDatabase.Refresh();
                }



            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }

        }

        static string beforePPFullID = "beforePostProcessCustomPostProcesses:";
        static string beforePPEmptyID = "beforePostProcessCustomPostProcesses:[]";
        static string afterPPFullID = "afterPostProcessCustomPostProcesses:";
        static string afterPPEmptyID = "afterPostProcessCustomPostProcesses:[]";
        static string changelogText = "1.8 \n " +
            "- Added automatic installation support for Unity 2021.2 and above.\n" +
            "1.7 \n" +
            "- Minor fixes. \n" +
            "- Example assets are embedded with the package for compatibility with latest Unity versions. \n"+
            "1.6 \n" +
            "- Fixed missing reference on example scene controller script. \n" + 
            "1.5 \n" +
            "- Fixed screen offsets in Block glitch effect. \n" +
            "- Fixed letter-box not fitting to the current aspect when the game view is not in free aspect. \n" +
            "1.4 \n" +
            "- Bloom Streak now has a Hue Shift option per bloom color. \n"+
            "- Bug that causes the Sharpen effect to decrease overall brightness is fixed. \n"+
            "1.3 \n" +
            "- Added an option to colorize the Sketch Motion shader. \n" +
            "- About & Docs tab has been moved under Help>Rich FX in the toolbar. \n" +
            "1.1 - 1.2 \n" +
            "- Price changes. \n" +
            "1.0 \n" +
            "- Initial version.\n";

        static string bpEmptyEffectListAddition = "  beforePostProcessCustomPostProcesses:\n" +
            "  - IE.RichFX.BloomStreak, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";


        static List<string> bpFullEffectList = new List<string>() {
            "  beforePostProcessCustomPostProcesses:",
            "\n  - IE.RichFX.BloomStreak, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
        };

        static string beforePPCopy = "  - IE.RichFX.BloomStreak, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        static string afterPPCopy =
     "  - IE.RichFX.CGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SketchMotion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.PencilSketch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ColorEdges, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleOutline, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EdgeDetection, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Nightvision, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LowLightCam, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EightColor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBSplit, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Posterize, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Oil, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ChromaLines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RainbowFlow, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleFrostedGlass, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenGlitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Underwater, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Distort, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.TextureDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Zoom, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DisplaceView, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Wobble, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Pixelate, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Scanlines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.HueSaturationInvert, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Recolor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Overlay, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GrayScale, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Glitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Slice, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Sharpen, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DirectionalBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleGaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RadialBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenFuzz, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LetterBox, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        static string apEmptyEffectListAddition =
            "  afterPostProcessCustomPostProcesses:\n" +
     "  - IE.RichFX.CGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SketchMotion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.PencilSketch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ColorEdges, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleOutline, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EdgeDetection, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Nightvision, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LowLightCam, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EightColor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBSplit, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Posterize, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Oil, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ChromaLines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RainbowFlow, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleFrostedGlass, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenGlitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Underwater, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Distort, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.TextureDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Zoom, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DisplaceView, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Wobble, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Pixelate, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Scanlines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.HueSaturationInvert, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Recolor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Overlay, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GrayScale, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Glitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Slice, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Sharpen, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DirectionalBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleGaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RadialBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenFuzz, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LetterBox, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";


        static List<string> apFullEffectList = new List<string>() {
     "  afterPostProcessCustomPostProcesses:",
     "\n  - IE.RichFX.CGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EGAFilter, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SketchMotion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.PencilSketch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ColorEdges, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleOutline, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EdgeDetection, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Nightvision, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LowLightCam, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.EightColor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBSplit, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Posterize, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Oil, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RGBDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ChromaLines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RainbowFlow, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleFrostedGlass, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenGlitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Underwater, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Distort, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.TextureDistortion, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Zoom, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DisplaceView, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Wobble, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Pixelate, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Scanlines, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.HueSaturationInvert, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Recolor, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Overlay, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GrayScale, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Glitch, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Slice, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.Sharpen, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.DirectionalBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.SimpleGaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.RadialBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.ScreenFuzz, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.GaussianBlur, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
     "  - IE.RichFX.LetterBox, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
    };

    }
}
