﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Providers.OculusAndroid
{
    /// <summary>
    /// Responsible for synchronizing the user's current input with Oculus Quest controller models.
    /// </summary>
    /// <seealso cref="OculusGoControllerVisualizer"/>
    public class OculusGoControllerVisualizer : OculusAndroidControllerVisualizer
    {
        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;
                GetComponent<OVRControllerHelper>().m_controller = (value.ControllerHandedness & Utilities.Handedness.Left) != 0 ? OVRInput.Controller.LTrackedRemote : OVRInput.Controller.RTrackedRemote;
            }
        }
    }
}