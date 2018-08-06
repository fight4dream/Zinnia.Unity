﻿namespace VRTK.Core.Prefabs.Locomotion.Teleporters
{
    using UnityEngine;
    using System.Collections.Generic;
    using VRTK.Core.Tracking;
    using VRTK.Core.Tracking.Modification;
    using VRTK.Core.Visual;
    using VRTK.Core.Data.Type;
    using VRTK.Core.Extension;

    /// <summary>
    /// Sets up the Teleport Prefab based on the provided user settings.
    /// </summary>
    public class TeleporterInternalSetup : MonoBehaviour
    {
        [Header("Facade Settings")]

        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Tooltip("The public interface facade.")]
        public TeleporterFacade facade;

        [Header("Teleporter Settings")]

        /// <summary>
        /// The <see cref="SurfaceLocator"/> to use for the teleporting event.
        /// </summary>
        [Tooltip("The Surface Locator to use for the teleporting event.")]
        public SurfaceLocator surfaceTeleporter;
        /// <summary>
        /// The <see cref="TransformModifier"/> to use for the teleporting event.
        /// </summary>
        [Tooltip("The Transform Modifier to use for the teleporting event.")]
        public TransformModifier modifyTeleporter;

        [Header("Alias Settings")]

        /// <summary>
        /// The <see cref="SurfaceLocator"/> to set aliases on.
        /// </summary>
        [Tooltip("The Surface Locators to set aliases on.")]
        public List<SurfaceLocator> surfaceLocatorAliases = new List<SurfaceLocator>();
        /// <summary>
        /// The <see cref="TransformModifier"/> to set aliases on.
        /// </summary>
        [Tooltip("The Transform Modifiers to set aliases on.")]
        public List<TransformModifier> transformModifierAliases = new List<TransformModifier>();
        /// <summary>
        /// The scene <see cref="Camera"/>s to set the <see cref="CameraColorOverlay"/>s to affect.
        /// </summary>
        [Tooltip("The scene Cameras to set the CameraColorOverlays to affect.")]
        public List<CameraColorOverlay> cameraColorOverlays = new List<CameraColorOverlay>();

        /// <summary>
        /// Sets up the Teleporter prefab with the specified settings.
        /// </summary>
        public virtual void Setup()
        {
            if (InvalidParameters())
            {
                return;
            }

            ApplySettings(facade.playAreaAlias, facade.headsetAlias, facade.sceneCameras);
        }

        /// <summary>
        /// Clears all of the settings from the Teleporter prefab.
        /// </summary>
        public virtual void Clear()
        {
            if (InvalidParameters())
            {
                return;
            }

            ApplySettings(null, null, null);
        }

        /// <summary>
        /// Attempts to teleport the <see cref="playAreaAlias"/>.
        /// </summary>
        /// <param name="destination">The location to attempt to teleport to.</param>
        public virtual void Teleport(TransformData destination)
        {
            if (surfaceTeleporter != null)
            {
                surfaceTeleporter.Locate(destination);
            }

            if (modifyTeleporter != null)
            {
                modifyTeleporter.SetSource(destination);
                modifyTeleporter.Modify();
            }
        }

        /// <summary>
        /// Notifies that the teleporter is about to initiate.
        /// </summary>
        /// <param name="data">The location data.</param>
        public virtual void NotifyTeleporting(TransformModifier.EventData data)
        {
            facade?.Teleporting.Invoke(data);
        }

        /// <summary>
        /// Notifies that the teleporter has completed.
        /// </summary>
        /// <param name="data">The location data.</param>
        public virtual void NotifyTeleported(TransformModifier.EventData data)
        {
            facade?.Teleported.Invoke(data);
        }

        /// <summary>
        /// Applies the provided settings to the Teleporter prefab.
        /// </summary>
        /// <param name="playAreaAlias">The PlayArea alias.</param>
        /// <param name="headsetAlias">The Headset alias.</param>
        /// <param name="sceneCameras">The scene cameras.</param>
        protected virtual void ApplySettings(GameObject playAreaAlias, GameObject headsetAlias, CameraList sceneCameras)
        {
            foreach (SurfaceLocator currentLocator in surfaceLocatorAliases.EmptyIfNull())
            {
                currentLocator.searchOrigin = headsetAlias;
            }

            foreach (TransformModifier currentModifier in transformModifierAliases.EmptyIfNull())
            {
                currentModifier.target = playAreaAlias;
                currentModifier.offset = headsetAlias;
            }

            foreach (CameraColorOverlay currentOverlay in cameraColorOverlays.EmptyIfNull())
            {
                currentOverlay.validCameras = sceneCameras;
            }
        }

        protected virtual void OnEnable()
        {
            Setup();
        }

        protected virtual void OnDisable()
        {
            Clear();
        }

        /// <summary>
        /// Determines if the setup parameters are invalid.
        /// </summary>
        /// <returns><see langword="true"/> if the parameters are invalid.</returns>
        protected virtual bool InvalidParameters()
        {
            return (facade == null || facade.playAreaAlias == null || facade.headsetAlias == null || facade.sceneCameras == null);
        }
    }
}