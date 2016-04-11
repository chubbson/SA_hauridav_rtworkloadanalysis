using System;
using System.Collections.Generic;
using System.IO;
using Dfn.Etl.Core;

namespace DFN_Architecture.Playground.Actions
{
    [Obsolete]
    public sealed class GetFiles : ISource<Tuple<string, int>>
    {
        private readonly string m_InputFolder;
        private readonly string m_FilePattern;
        private readonly SearchOption m_SearchOption;
        private int m_SequenceCnt;

        public GetFiles(string inputFolder, string filePattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            m_InputFolder = inputFolder;
            m_FilePattern = filePattern;
            m_SearchOption = searchOption;
            m_SequenceCnt = 0;
        }

        public string Title
        {
            get { return string.Format("Get Files from {0}, FilePattern = {1}, SearchOption = {2}", m_InputFolder, m_FilePattern, m_SearchOption); }
        }

        public IEnumerable<Tuple<string, int>> Pull()
        {
            foreach (var file in Directory.EnumerateFiles(m_InputFolder, m_FilePattern, m_SearchOption))
            {
                var outItem = new Tuple<string, int>(file, m_SequenceCnt);
                m_SequenceCnt++;
                yield return outItem;
            }
        }
    }
}