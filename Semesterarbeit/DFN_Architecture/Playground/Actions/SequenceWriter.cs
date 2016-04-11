using System;
using System.IO;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    public sealed class SequenceWriter : ITransformation<Tuple<UInt64, UInt64, string>, Tuple<UInt64, UInt64, string>>
    {
        private string m_fn;

        public SequenceWriter(string filename)
        {
            m_fn = filename;
            // Delete the file if it exists. 
            if (File.Exists(filename))
            {
                // Note that no lock is put on the 
                // file and the possibility exists 
                // that another process could do 
                // something with it between 
                // the calls to Exists and Delete.
                File.Delete(filename);
            }

            // Create the file. 
            using (FileStream fs = File.Create(filename))
            {
                //Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                //// Add some information to the file.
                //fs.Write(info, 0, info.Length);
            }
        }

        public string Title
        {
            get { return "Map zpimXmlFile content to idasDataCtx info"; }
        }

        public Tuple<UInt64, UInt64, string> Transform(Tuple<UInt64, UInt64, string> input)
        {
            using (StreamWriter sw = File.AppendText(m_fn))
            {
                sw.WriteLine(input.Item3);
            }

            return input;
        }
    }
}
