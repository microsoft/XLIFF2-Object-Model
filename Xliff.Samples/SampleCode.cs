namespace Localization.Xliff.Samples
{
    using System;
    using Localization.Xliff.OM;
    using Localization.Xliff.OM.Core;
    using Localization.Xliff.OM.Exceptions;
    using Localization.Xliff.OM.Extensibility;
    using Localization.Xliff.OM.Modules.Glossary;
    using Localization.Xliff.OM.Modules.Metadata;
    using Localization.Xliff.OM.Modules.TranslationCandidates;
    using Localization.Xliff.OM.Serialization;
    using IO = System.IO;
    using System.Text;

    /// <summary>
    /// This class contains the entry point to the application that demonstrates how to use the XLIFF 2.0 object model.
    /// </summary>
    public class SampleCode
    {
        /// <summary>
        /// Demonstrates how to create a blank XLIFF document.
        /// </summary>
        public static void BlankDocument()
        {
            XliffDocument document;

            document = new XliffDocument("en-us");
        }


        /// <summary>
        /// Demonstrates how to disable validation when writing an XLIFF document.
        /// </summary>
        /// <param name="document">The document to write.</param>
        /// <param name="file">The path to the document to write.</param>
        public static void DisableValidationOnWrite(XliffDocument document, string file)
        {
            using (IO.FileStream stream = new IO.FileStream(file, IO.FileMode.Create, IO.FileAccess.Write))
            {
                XliffWriter writer;
                XliffWriterSettings settings;

                settings = new XliffWriterSettings();
                settings.Validators.Clear();

                writer = new XliffWriter(settings);
                writer.Serialize(stream, document);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            string path;

            path = IO.Path.GetTempFileName();
            try
            {
                XliffDocument document;
                Segment segment;
                Unit unit;

                document = new XliffDocument("en-us");
                document.Files.Add(new File("f1"));
                unit = new Unit("u1");
                document.Files[0].Containers.Add(unit);
                segment = new Segment("s1");
                segment.Source = new Source();
                segment.Source.Text.Add(new PlainText("text"));
                unit.Resources.Add(segment);

                SampleCode.BlankDocument();
                SampleCode.DisableValidationOnWrite(document, path);
                SampleCode.ReadDocument(path);
                SampleCode.StoreCustomExtension();
                SampleCode.StoreGenericExtension();
                SampleCode.StoreGlossary();
                SampleCode.StoreMatches();
                SampleCode.StoreMetadata();
                SampleCode.ViewValidations(new XliffDocument("en-us"), path);
                SampleCode.WriteDocument(document, path);
                SampleCode.WhiteSpaces();
            }
            finally
            {
                IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Demonstrates how to read an XLIFF document from a file.
        /// </summary>
        /// <param name="file">The path to the document to read.</param>
        public static void ReadDocument(string file)
        {
            using (IO.FileStream stream = new IO.FileStream(file, IO.FileMode.Open, IO.FileAccess.Read))
            {
                XliffDocument document;
                XliffReader reader;

                reader = new XliffReader();
                document = reader.Deserialize(stream);
            }
        }

        /// <summary>
        /// Demonstrates how to store custom attributes and elements on a <see cref="File"/> element using a custom
        /// extension and element types.
        /// </summary>
        public static void StoreCustomExtension()
        {
            TestExtension extension;
            IExtensible extensible;
            Segment segment;
            XliffDocument document;
            XliffReader reader;
            Unit unit;
            string path;

            // This namespace will be stored on the document element like: <xliff xmlns:pre1="urn:custom:extension:1.0"
            const string customNamespace = "urn:custom:extension:1.0";
            const string customPrefix = "customPrefix";

            extension = new TestExtension();

            document = new XliffDocument("en-us");
            document.Files.Add(new File("f1"));

            unit = new Unit("u1");
            document.Files[0].Containers.Add(unit);

            segment = new Segment("s1");
            unit.Resources.Add(segment);

            segment.Source = new Source();
            segment.Source.Text.Add(new PlainText("text"));

            extensible = document.Files[0];

            // Create custom attributes that look like: <file id="f1" pre1:testattr1="testvalue1" pre1:testattr2="testvalue2">
            if (extensible.SupportsAttributeExtensions)
            {
                extension.AddAttribute(new TestAttribute(customPrefix, customNamespace, "testattr1", "testvalue1"));
                extension.AddAttribute(new TestAttribute(customPrefix, customNamespace, "testattr2", "testvalue2"));
                extensible.Extensions.Add(extension);
            }

            // Create a custom element that looks like: <pre1:testelement1 pre1:testattr1="testvalue1" />
            if (extensible.SupportsElementExtensions)
            {
                ElementInfo info;
                TestElement element;

                element = new TestElement();
                element.SetAttribute(customPrefix, customNamespace, "testattr1", "testvalue1");
                info = new ElementInfo(new XmlNameInfo(customPrefix, customNamespace, "testelement1"), element);
                extension.AddChild(info);
            }

            // Write the file just like any other file.
            path = IO.Path.GetTempFileName();
            SampleCode.WriteDocument(document, path);

            // Read the file using an custom extension handler so the custom types are loaded. The loaded File will
            // have the custom extension and attributes and elements on it just like it was created above.
            reader = new XliffReader();
            reader.RegisterExtensionHandler(customNamespace, new TestExtensionHandler());
            using (IO.FileStream stream = new IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read))
            {
                document = reader.Deserialize(stream);
            }
        }

        /// <summary>
        /// Demonstrates how to store custom attributes and elements on a <see cref="File"/> element using the built in
        /// generic extension support.
        /// </summary>
        public static void StoreGenericExtension()
        {
            IExtensible extensible;
            GenericExtension extension;
            File file;

            // This namespace will be stored on the document element like: <xliff xmlns:pre1="urn:custom:extension:1.0"
            const string customNamespace = "urn:custom:extension:1.0";
            const string customPrefix = "pre1";

            file = new File("f1");
            extensible = file;
            extension = new GenericExtension("custom");

            // Create custom attributes that look like: <file id="f1" pre1:attr1="value1" pre1:attr2="value2">
            if (extensible.SupportsAttributeExtensions)
            {
                extension.AddAttribute(new GenericExtensionAttribute(customPrefix, customNamespace, "attr1", "value1"));
                extension.AddAttribute(new GenericExtensionAttribute(customPrefix, customNamespace, "attr2", "value2"));
                extensible.Extensions.Add(extension);
            }

            // Create a custom element that looks like: <pre1:element1 pre1:attr1="value1" />
            if (extensible.SupportsElementExtensions)
            {
                ElementInfo info;
                GenericElement element;

                element = new GenericElement();
                element.SetAttribute(customPrefix, customNamespace, "attr1", "value1");
                info = new ElementInfo(new XmlNameInfo(customPrefix, customNamespace, "element1"), element);
                extension.AddChild(info);
            }
        }

        /// <summary>
        /// Demonstrates how to store <see cref="Glossary"/> data in a <see cref="Unit"/> element.
        /// </summary>
        public static void StoreGlossary()
        {
            GlossaryEntry entry;
            MarkedSpan span;
            Segment segment;
            Translation translation;
            Unit unit;

            // Create a unit to hold the glossary.
            unit = new Unit("id");
            unit.Glossary = new Glossary();
            entry = new GlossaryEntry();
            entry.Reference = "#m1";
            unit.Glossary.Entries.Add(entry);

            // Create a term that looks like: <gls:term source="publicTermbase">TAB key</gls:term>
            entry.Term.Source = "publicTermbase";
            entry.Term.Text = "TAB key";

            // Create a translation that looks like: <gls:translation id="1" source="myTermbase">Tabstopptaste</gls:translation>
            translation = new Translation("1");
            translation.Source = "myTermbase";
            translation.Text = "Tabstopptaste";
            entry.Translations.Add(translation);

            // Create a translation that looks like: <gls:translation ref="#m2" source="myTermbase">TAB-TASTE</gls:translation>
            translation = new Translation();
            translation.Reference = "#m1";
            translation.Source = "myTermbase";
            translation.Text = "TAB-TASTE";
            entry.Translations.Add(translation);

            // Create a definition that looks like:
            //  <gls:definition source="publicTermbase">A keyboard key that is traditionally used to insert tab"
            //      characters into a document.</gls:definition>
            entry.Definition = new Definition();
            entry.Definition.Source = "publicTermbase";
            entry.Definition.Text = "A keyboard key that is traditionally used to insert tab characters into a document.";

            // Create a segment to which the glossary refers.
            segment = new Segment();
            segment.Source = new Source();
            segment.Source.Text.Add(new PlainText("Press the "));
            span = new MarkedSpan("m1");
            span.Type = "term";
            span.Text.Add(new PlainText("TAB key"));
            segment.Source.Text.Add(span);

            segment.Target = new Target();
            segment.Target.Text.Add(new PlainText("Drücken Sie die "));
            span = new MarkedSpan("m1");
            span.Type = "term";
            span.Text.Add(new PlainText("TAB-TASTE"));
            segment.Target.Text.Add(span);

            unit.Resources.Add(segment);
        }

        /// <summary>
        /// Demonstrates how to store <see cref="Match"/> data in a <see cref="Unit"/> element.
        /// </summary>
        public static void StoreMatches()
        {
            Match match;
            Segment segment;
            Unit unit;

            unit = new Unit("id");

            segment = new Segment("seg1");
            segment.Source = new Source("text");
            segment.Target = new Target("best translation");
            unit.Resources.Add(segment);

            match = new Match("#seg1");
            match.MatchQuality = 90.0f;
            match.MatchSuitability = 87.0f;
            match.Origin = "custom tool";
            match.Similarity = 10.0f;
            match.Source = new Source("text");
            match.SubType = "ms:type";
            match.Target = new Target("translation 1");
            match.Type = MatchType.TranslationMemory;
            unit.Matches.Add(match);

            match = new Match("#seg1");
            match.MatchQuality = 80.0f;
            match.MatchSuitability = 75.0f;
            match.Origin = "custom tool";
            match.Similarity = 8.0f;
            match.Source = new Source("text");
            match.SubType = "ms:type";
            match.Target = new Target("translation 2");
            match.Type = MatchType.TranslationMemory;
            unit.Matches.Add(match);

            match = new Match("#seg1");
            match.MatchQuality = 60.0f;
            match.MatchSuitability = 57.0f;
            match.Origin = "custom tool";
            match.Similarity = 7.0f;
            match.Source = new Source("text");
            match.SubType = "ms:type";
            match.Target = new Target("translation 3");
            match.Type = MatchType.TranslationMemory;
            unit.Matches.Add(match);
        }

        /// <summary>
        /// Demonstrates how to store metadata in a <see cref="File"/> element.
        /// </summary>
        public static void StoreMetadata()
        {
            File file;
            Meta meta;
            MetaGroup metaGroup;

            file = new File("id");

            file.Metadata = new MetadataContainer();

            metaGroup = new MetaGroup();
            metaGroup.AppliesTo = MetaGroupSubject.Source;
            metaGroup.Category = "document_state";
            metaGroup.Id = "g1";
            file.Metadata.Groups.Add(metaGroup);

            meta = new Meta("draft");
            meta.Type = "phase";
            metaGroup.Containers.Add(meta);
        }

        /// <summary>
        /// Demonstrates how to validate a document on write and display the data where the validation error occurred.
        /// </summary>
        /// <param name="document">The document to validate.</param>
        /// <param name="file">The path to write the document to.</param>
        public static void ViewValidations(XliffDocument document, string file)
        {
            using (IO.FileStream stream = new IO.FileStream(file, IO.FileMode.Create, IO.FileAccess.Write))
            {
                XliffWriter writer;

                writer = new XliffWriter();

                try
                {
                    writer.Serialize(stream, document);
                }
                catch (ValidationException e)
                {
                    Console.WriteLine("ValidationException Details:");
                    Console.WriteLine(e.Message);
                    if (e.Data != null)
                    {
                        foreach (object key in e.Data.Keys)
                        {
                            Console.WriteLine("  '{0}': '{1}'", key, e.Data[key]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Demonstrates how to write an XLIFF document to a file.
        /// </summary>
        /// <param name="document">The document to write.</param>
        /// <param name="file">The path to write the document to.</param>
        public static void WriteDocument(XliffDocument document, string file)
        {
            using (IO.FileStream stream = new IO.FileStream(file, IO.FileMode.Create, IO.FileAccess.Write))
            {
                XliffWriter writer;

                writer = new XliffWriter();
                writer.Serialize(stream, document);
            }
        }

        public static void WhiteSpaces()
        {
            string data = "<xliff srcLang='en' version='2.0' xmlns='urn:oasis:names:tc:xliff:document:2.0'>"
            + "<file id='f1'><unit id='u1'>"
            + "<segment><source>Sentence 1.</source></segment>"
            + "<ignorable><source> </source></ignorable>"
            + "<segment><source>Sentence 2.</source></segment>"
            + "</unit></file></xliff>";
            using (IO.MemoryStream ms = new IO.MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                XliffReader reader = new XliffReader();
                XliffDocument doc = reader.Deserialize(ms);
                foreach (XliffElement e in doc.CollapseChildren<XliffElement>())
                {
                    Console.WriteLine("Type: " + e.GetType().ToString());
                    if (e is PlainText)
                    {
                        PlainText pt = (PlainText)e;
                        Console.WriteLine("Content: '" + pt.Text + "'");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
