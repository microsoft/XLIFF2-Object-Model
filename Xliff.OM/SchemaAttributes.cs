namespace Localization.Xliff.OM
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Localization.Xliff.OM.Attributes;

    /// <summary>
    /// This class contains the attribute information about properties that determine how to transform properties from
    /// .NET to XLIFF. This class is expected to be filled after reflecting on classes.
    /// </summary>
    internal sealed class SchemaAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaAttributes"/> class.
        /// </summary>
        /// <param name="name">The name of the property that contains these attributes.</param>
        /// <param name="converter">The converter that defines how to convert from .NET to XLIFF.</param>
        /// <param name="defaultValue">The default .NET value of the property.</param>
        /// <param name="hasValueIndicator">The indicator that defines how to determine if the value has data.</param>
        /// <param name="schema">Defines how to map the property to XLIFF.</param>
        /// <param name="inheritanceList">A list that defines how to inherit the value from ancestors.</param>
        /// <param name="outputDependencies">A list of property names that, when written, must be accompanied by the
        /// property to which this attribute is applied.</param>
        public SchemaAttributes(
                                string name,
                                ConverterAttribute converter,
                                DefaultValueAttribute defaultValue,
                                HasValueIndicatorAttribute hasValueIndicator,
                                SchemaEntityAttribute schema,
                                IEnumerable<InheritValueAttribute> inheritanceList,
                                IEnumerable<ExplicitOutputDependencyAttribute> outputDependencies)
        {
            this.Converter = converter;
            this.DefaultValue = defaultValue;
            this.ExplicitOutputDependencies = new List<ExplicitOutputDependencyAttribute>(outputDependencies);
            this.HasValueIndicator = hasValueIndicator;
            this.InheritanceList = new List<InheritValueAttribute>(inheritanceList);
            this.Name = name;
            this.Schema = schema;
        }

        #region Properties
        /// <summary>
        /// Gets the attribute that defines how to convert from .NET to XLIFF.
        /// </summary>
        public ConverterAttribute Converter { get; private set; }

        /// <summary>
        /// Gets the attribute that defines the default .NET value of the property.
        /// </summary>
        public DefaultValueAttribute DefaultValue { get; private set; }

        /// <summary>
        /// Gets the list of property names that, when written, must be accompanied by the property to which this
        /// attribute is applied.
        /// </summary>
        public IEnumerable<ExplicitOutputDependencyAttribute> ExplicitOutputDependencies { get; private set; }

        /// <summary>
        /// Gets the attribute that defines how to determine if the value has data.
        /// </summary>
        public HasValueIndicatorAttribute HasValueIndicator { get; private set; }

        /// <summary>
        /// Gets the list of attributes that define how to inherit the value from ancestors.
        /// </summary>
        public IEnumerable<InheritValueAttribute> InheritanceList { get; private set; }
        
        /// <summary>
        /// Gets name of the property that contains these attributes. 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the attribute that defines how to map the property to XLIFF.
        /// </summary>
        public SchemaEntityAttribute Schema { get; private set; }
        #endregion Properties
    }
}
