namespace Zinnia.Process.Moment
{
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using UnityEngine;
    using Zinnia.Process.Moment.Collection;

    /// <summary>
    /// Treats a given <see cref="MomentProcess"/> collection as an <see cref="IProcessable"/> process.
    /// </summary>
    public class CompositeProcess : MonoBehaviour, IProcessable
    {
        /// <summary>
        /// A collection of <see cref="MomentProcess"/> to process.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public MomentProcessObservableList Processes { get; set; }

        /// <summary>
        /// Iterates through the given <see cref="MomentProcess"/> and calls <see cref="MomentProcess.Process"/> on each one.
        /// </summary>
        public void Process()
        {
            if (Processes == null)
            {
                return;
            }

            foreach (MomentProcess currentProcess in Processes.NonSubscribableElements)
            {
                currentProcess.Process();
            }
        }
    }
}
