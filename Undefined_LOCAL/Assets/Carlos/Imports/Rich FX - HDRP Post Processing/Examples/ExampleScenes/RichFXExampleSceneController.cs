/* 
 * Author: Inan Evin
 * www.inanevin.com
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace IE.RichFX
{
    public class RichFXExampleSceneController : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField]
        private Dropdown effectCategoryDropdown = null;

        [SerializeField]
        private Volume[] postProcessVolumes = null;

        [SerializeField]
        private Volume[] combinationVolumes = null;

        [SerializeField]
        private Text effectNameText = null;

        [SerializeField]
        private bool incrementCategory = true;

        private bool releaseMode = false;

        #endregion

        #region Other Properties

        private List<int> volumeSelectedIndices = new List<int>();
        private int currentSelectedVolume = 0;
        private bool categoryVolumesActive = true;
        private int activeCombinationVolume = 0;
        private Coroutine activateCombVolume;

        #endregion

        #region Unity Operation(s)

        private void Awake()
        {
            // Disable all volumes except first.
            for (int i = 1; i < postProcessVolumes.Length; i++)
                postProcessVolumes[i].weight = 0.0f;

            // Fill Indices list.
            for (int i = 0; i < postProcessVolumes.Length; i++)
                volumeSelectedIndices.Add(0);
        }

        void Start()
        {
            // Initialize effects.
            DropdownValueChanged();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                NextButton();
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                PreviousButton();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                ActivateCombinationVolume(0, "Combination: Security Cam");
            if (Input.GetKeyDown(KeyCode.Alpha5))
                ActivateCombinationVolume(1, "Combination: Underwater");
            if (Input.GetKeyDown(KeyCode.Alpha6))
                ActivateCombinationVolume(2, "Combination: Weapon Cam");
            if (Input.GetKeyDown(KeyCode.Alpha7))
                DeactivateCombinationVolumes();

        }
        #endregion

        #region Exposed Operation(s)

        public void NextButton()
        {
            if (!categoryVolumesActive)
            {
                if (activeCombinationVolume == 0)
                    ActivateCombinationVolume(1, "Combination: Underwater");
                else if (activeCombinationVolume == 1)
                    ActivateCombinationVolume(2, "Combination: Weapon Cam");

                return;
            }

            // Disable current component.
            postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = false;

            // Increment component counter.
            volumeSelectedIndices[currentSelectedVolume]++;

            // Make sure counter doesn't exceed the limit.
            if (volumeSelectedIndices[currentSelectedVolume] == postProcessVolumes[currentSelectedVolume].profile.components.Count)
            {
                volumeSelectedIndices[currentSelectedVolume] = 0;

                if (incrementCategory)
                {
                    // Disable current component.
                    postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = false;
                    postProcessVolumes[currentSelectedVolume].weight = 0.0f;

                    currentSelectedVolume++;

                    if (currentSelectedVolume == volumeSelectedIndices.Count)
                    {
                        currentSelectedVolume = 0;
                        ActivateCombinationVolume(0, "Combination: Security Cam");
                    }
                    else
                    {
                        volumeSelectedIndices[currentSelectedVolume] = 0;

                        // Enable the current volume.
                        postProcessVolumes[currentSelectedVolume].weight = 1.0f;

                        // Enable current component.
                        postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = true;

                        // Update UI
                        UpdateUI();
                    }


                }
            }
            else
            {
                // Enable current component.
                postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = true;

                // Update UI
                UpdateUI();
            }


        }

        public void PreviousButton()
        {
            bool decrementVolumeSelected = true;

            if (!categoryVolumesActive)
            {
                if (activeCombinationVolume == 0)
                {
                    // Reset selected ones.
                    currentSelectedVolume = postProcessVolumes.Length - 1;
                    volumeSelectedIndices[currentSelectedVolume] = postProcessVolumes[currentSelectedVolume].profile.components.Count - 1;

                    // Enable current component.
                    postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = true;

                    // Disablecombinations.
                    DeactivateCombinationVolumes();

                    // Volume index already decremented.
                    decrementVolumeSelected = false;

                }
                else if (activeCombinationVolume == 1)
                {
                    ActivateCombinationVolume(0, "Combination: Security Cam");
                    return;
                }
                else if (activeCombinationVolume == 2)
                {
                    ActivateCombinationVolume(1, "Combination: Underwater");
                    return;
                }


            }

            // Disable current component.
            postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = false;

            // Decrement component counter.
            if (decrementVolumeSelected)
                volumeSelectedIndices[currentSelectedVolume]--;

            // Make sure counter doesn't exceed the limit.
            if (volumeSelectedIndices[currentSelectedVolume] == -1)
            {
                volumeSelectedIndices[currentSelectedVolume] = postProcessVolumes[currentSelectedVolume].profile.components.Count - 1;

                if (incrementCategory)
                {
                    // Disable current component.
                    postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = false;
                    postProcessVolumes[currentSelectedVolume].weight = 0.0f;

                    currentSelectedVolume--;

                    if (currentSelectedVolume == -1)
                    {
                        currentSelectedVolume = 0;
                        volumeSelectedIndices[currentSelectedVolume] = 0;
                    }
                    else
                        volumeSelectedIndices[currentSelectedVolume] = postProcessVolumes[currentSelectedVolume].profile.components.Count - 1;

                    // Enable the current volume.
                    postProcessVolumes[currentSelectedVolume].weight = 1.0f;
                }
            }

            // Enable current component.
            postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = true;

            // Update UI
            UpdateUI();
        }

        public void DropdownValueChanged()
        {
            // Disable current component.
            postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = false;
            postProcessVolumes[currentSelectedVolume].weight = 0.0f;

            // Update selected volume
            currentSelectedVolume = effectCategoryDropdown.value;

            // Enable the current volume.
            postProcessVolumes[currentSelectedVolume].weight = 1.0f;

            // Enable current component.
            postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].active = true;

            UpdateUI();
        }
        #endregion

        #region Class Operation(s)

        private void ActivateCombinationVolume(int index, string combVolumeName)
        {
            // Disable the current category volume if active.
            if (categoryVolumesActive)
                CurrentCategoryVolumeActivation(false);

            combinationVolumes[activeCombinationVolume].weight = 0.0f;

            // Stop activation coroutine if already is active.
            if (activateCombVolume != null)
                StopCoroutine(activateCombVolume);

            // Start activation coroutine & update index & UI.
            activateCombVolume = StartCoroutine(ActivateCombinationVolumeRoutine(index));
            activeCombinationVolume = index;
            UpdateUI(combVolumeName);
        }

        private void DeactivateCombinationVolumes()
        {
            if (activateCombVolume != null)
                StopCoroutine(activateCombVolume);

            // Deactivate combination volume & activate the category volumes.
            combinationVolumes[activeCombinationVolume].weight = 0.0f;
            CurrentCategoryVolumeActivation(true);

            // Update UI
            UpdateUI();
        }
        private void CurrentCategoryVolumeActivation(bool enabled)
        {
            postProcessVolumes[currentSelectedVolume].weight = enabled ? 1.0f : 0.0f;
            categoryVolumesActive = enabled;
        }

        private void UpdateUI(string overrideName = "")
        {
            string effectName = postProcessVolumes[currentSelectedVolume].profile.components[volumeSelectedIndices[currentSelectedVolume]].name;
            string effectNameTrimmed = effectName.Replace("(Clone)", string.Empty);
            string counter = "(" + (volumeSelectedIndices[currentSelectedVolume] + 1).ToString() + "/" + postProcessVolumes[currentSelectedVolume].profile.components.Count + ")";

            if (releaseMode)
            {
                counter = "";
                effectNameTrimmed = effectNameTrimmed.ToUpper();
            }

            if (overrideName == "")
                effectNameText.text = effectNameTrimmed + " " + counter;
            else
                effectNameText.text = overrideName;

            if (combinationVolumes[activeCombinationVolume].weight == 0.0f)
                effectCategoryDropdown.value = currentSelectedVolume;
        }
        #endregion

        #region Utility Operation(s)

        #endregion

        #region Coroutine(s)

        IEnumerator ActivateCombinationVolumeRoutine(int index)
        {
            float i = 0.0f;
            float duration = 0.8f;

            while (i < 1.0f)
            {
                i += Time.deltaTime * 1.0f / duration;
                combinationVolumes[index].weight = Mathf.Lerp(0.0f, 1.0f, i);
                yield return null;
            }
        }

        IEnumerator AutoIncrementEffects()
        {
            int i = 0;
            int target = 0;
            float duration = 4;

            for (int k = 0; k < postProcessVolumes.Length; k++)
                target += postProcessVolumes[k].profile.components.Count;

            while (i < target)
            {
                yield return new WaitForSeconds(duration);
                NextButton();
                i++;
            }

            CurrentCategoryVolumeActivation(false);

            ActivateCombinationVolume(0, "Combination: Security Cam");
            yield return new WaitForSeconds(duration);
            ActivateCombinationVolume(1, "Combination: Underwater");
            yield return new WaitForSeconds(duration);
            ActivateCombinationVolume(2, "Combination: Weapon Cam");

        }
        #endregion
    }
}
