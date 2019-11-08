// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A way to control a list of radial type buttons or tabs
    /// </summary>
    public class InteractableToggleCollection : MonoBehaviour
    {
        [Tooltip("Array of Interactables that will be managed by this controller")]
        [SerializeField, FormerlySerializedAs("ToggleList")]
        private Interactable[] toggleList;

        /// <summary>
        /// Array of Interactables that will be managed by this controller
        /// </summary>
        public Interactable[] ToggleList
        {
            get => toggleList;
            set => toggleList = value;
        }

        [Tooltip("Currently selected index in the ToggleList, default is 0")]
        [SerializeField]
        private int currentIndex;

        /// <summary>
        /// The current index in the array of interactable toggles
        /// </summary>
        public int CurrentIndex
        {
            get => currentIndex;
            set => SetSelection(value, false, true);
        }

        [Tooltip("This event is triggered when any of the toggles in the ToggleList are selected")]
        /// <summary>
        /// This event is triggered when any of the toggles in the ToggleList are selected
        /// </summary>
        public UnityEvent OnSelectionEvents = new UnityEvent();

        private List<UnityAction> toggleActions = new List<UnityAction>();

        private void Start()
        {
            if (ToggleList != null)
            {
                // Add listeners to each toggle in ToggleList
                for (int i = 0; i < ToggleList.Length; ++i)
                {
                    int itemIndex = i;

                    UnityAction setSelectionAction = () =>
                    {
                        SetSelection(itemIndex, true, false);
                    };

                    toggleActions.Add(setSelectionAction);

                    ToggleList[i].OnClick.AddListener(setSelectionAction);
                    ToggleList[i].CanDeselect = false;
                }

                SetSelection(CurrentIndex, true, true);
            }
        }

        /// <summary>
        /// Set the selection of a an element in the toggle collection based on index.
        /// <param name="index">Index of an element in ToggleList</param>
        /// <param name="force">Force selection set</param>
        /// <param name="fireOnClick">The manual trigger of the OnClick event. OnClick event is manually triggered 
        /// when the CurrentIndex is updated via script or inspector</param>
        /// </summary>
        public void SetSelection(int index, bool force = false, bool fireOnClick = false)
        {
            if (index < 0 || ToggleList.Length <= index || ToggleList == null || !isActiveAndEnabled)
            {
                Debug.LogWarning("Index out of range of ToggleList: " + index);
                return;
            }

            if (CurrentIndex != index || force)
            {
                currentIndex = index;

                OnSelection(index);

                if (fireOnClick)
                {
                    ToggleList[index].TriggerOnClick();
                }
            }
        }

        // Update the visual appearance and set the states of the selected and unselected toggles within 
        // Interactable
        protected virtual void OnSelection(int index, bool force = false)
        {
            for (int i = 0; i < ToggleList.Length; ++i)
            {
                ToggleList[i].IsToggled = (i == index);
            }

            OnSelectionEvents?.Invoke();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < toggleActions.Count; ++i)
            {
                ToggleList[i]?.OnClick.RemoveListener(toggleActions[i]);
            }

            toggleActions.Clear();
        }
    }
}
