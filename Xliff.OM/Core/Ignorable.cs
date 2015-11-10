namespace Localization.Xliff.OM.Core
{
    /// <summary>
    /// This class represents content that should be ignored and not translated. This corresponds to a
    /// &lt;ignorable> element in the XLIFF 2.0 specification.
    /// </summary>
    /// <example>A sample XLIFF fragment looks like: &lt;ignorable [id=string] ...>
    /// </example>
    /// <remarks>
    /// See the <a href="http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html#ignorable">XLIFF specification</a>
    /// for more details.
    /// </remarks>
    /// <seealso cref="ContainerResource"/>
    public class Ignorable : ContainerResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ignorable"/> class.
        /// </summary>
        /// <param name="id">The Id of the element.</param>
        public Ignorable(string id)
            : base(id)
        {
            this.RegisterElementInformation(ElementInformationFromReflection.Create(this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ignorable"/> class.
        /// </summary>
        public Ignorable()
            : this(null)
        {
        }
    }
}