namespace Zinnia.Data.Type.Transformation
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;

    public class PositionSmoother : Transformer<Vector3, Vector3, PositionSmoother.UnityEvent>
    {
        /// <summary>
        /// Defines the event with the <see cref="Vector3"/> smoothed.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<Vector3>
        {
        }

        /// <summary>
        /// The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Range(0.0001f, 100f)]
        public float MaxAllowedPerFrameDistanceDifference { get; set; } = 0.003f;

        /// <summary>
        /// The position that results by following source.
        /// </summary>
        protected Vector3 targetPosition;

        /// <summary>
        /// Smooth the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input">The value to smooth.</param>
        /// <returns>A new smoothed <see cref="Vector3"/>.</returns>
        protected override Vector3 Process(Vector3 input)
        {
            float alpha = Mathf.Clamp01(Vector3.Distance(targetPosition, input) / MaxAllowedPerFrameDistanceDifference);
            return Vector3.Lerp(targetPosition, input, alpha);
        }
    }
}