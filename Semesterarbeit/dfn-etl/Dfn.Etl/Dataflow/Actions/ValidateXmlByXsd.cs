using System;
using System.Xml;
using System.Xml.Schema;
using Common.Logging;
using Dfn.Etl.Core;

namespace Dfn.Etl.Dataflow.Actions
{
    public sealed class ValidateXmlByXsd : ITransformation<Tuple<string, int>, Tuple<string, string, int>>
    {
        private readonly string m_XsdFile;
        private readonly bool m_DoXsdValidation;
        private readonly ILog m_Log;

        public ValidateXmlByXsd(ILog logger, string xsdFile, bool doXsdValidation)
        {
            m_XsdFile = xsdFile;
            m_DoXsdValidation = doXsdValidation;
            m_Log = logger;
        }

        public string Title
        {
            get { return string.Format("Validate item by xsd: {0}", m_XsdFile); }
        }

        public Tuple<string, string, int> Transform(Tuple<string, int> item)
        {
            if (m_DoXsdValidation)
            {   
                m_Log.Info(String.Format("Validate '{0}' sequence '{1}' by '{2}'", item.Item1, item.Item2, m_XsdFile));

                var isValid = true;

                // Set the validation settings. 
                var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
                var schemas = new XmlSchemaSet();
                settings.Schemas = schemas;
                try
                {
                    schemas.Add(null, m_XsdFile);
                }
                catch (XmlException xmlEx)
                {
                    m_Log.Info(string.Format("The Schema '{0}' is not a valid Schema", m_XsdFile));
                    throw new DataflowNetworkRecoverableErrorException(
                        string.Format("The Schema '{0}' is not a valid Schema", m_XsdFile), xmlEx);
                }
                // Not use ValidationEventHandler will result in XmlSchemaValidationException,
                // if the xml is no valid, catch this exception. 
                // To Catch all errors of this xml with defined xsd you have to use 
                // this validation Validation EventHandler. 
                // To not loose the thread context dont create a callable eventHandler Method
                // Use an inline Lambda expression instead. 
                settings.ValidationEventHandler += (sender, args) =>
                {
                    isValid = false;
                    if (args.Severity == XmlSeverityType.Warning)
                    {
                        m_Log.Info("Warning: Matching schema not found.  No validation occurred.");
                        m_Log.Info(String.Format("File: '{0}' XSD: '{1}' Message: {2}", 
                                                 item.Item1, 
                                                 m_XsdFile, 
                                                 args.Message));
                    }
                    else
                    {
                        m_Log.Info(String.Format("File: '{0}' XSD: '{1}' Validation error: {2}", 
                                                 item.Item1, 
                                                 m_XsdFile,
                                                 args.Message));
                    }
                };

                // Create the XmlReader object.
                var validatorReader = XmlReader.Create(item.Item1, settings);

                //Parse the File 
                while (validatorReader.Read())
                {
                }

                m_Log.Info(isValid
                               ? string.Format("File {0}, Sequence {1} is not xsd valid: {2}", item.Item1, item.Item2,
                                               m_XsdFile)
                               : string.Format("File {0}, Sequence {1} is xsd valid: {2}", item.Item1, item.Item2,
                                               m_XsdFile));
            }
            else
            {
                m_Log.Info(string.Format("Skipped XSD validation for File {0}, Sequence {1} XSD: {2}", item.Item1,
                                         item.Item2, m_XsdFile));
            }

            var xmlFileXsdFileSeqnr = new Tuple<string, string, int>(item.Item1, m_XsdFile, item.Item2);
            return xmlFileXsdFileSeqnr;
        }
    }
}
