using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dfn.Etl.Core;

namespace Dfn.Etl.Dataflow.Actions
{
    /// <summary>
    /// Retrieves the files residing in the giving directory.
    /// </summary>
    public sealed class PrepareAndGetFilesForProgress : ISource<string>
    {
        private readonly string m_Directory;
        private readonly string m_SearchPattern;
        private readonly SearchOption m_SearchOption;
        private readonly string m_InProgressFolder;
        private readonly string m_Prfx;
        private readonly string m_AdvancedRegexCond;

        private readonly String[] m_Files;

        /// <summary>
        /// Move files to an InProgress Folder, which will be created 
        /// at: PathCombine(directory,typeDir,'InProgress') 
        /// moves the files to declared folder with defined prefix
        /// stores all defined files. 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="typeDir"></param>
        /// <param name="inWorkPreFix"></param>
        public PrepareAndGetFilesForProgress(string directory,
                        string searchPattern = "*.*",
                        SearchOption searchOption = SearchOption.TopDirectoryOnly,
                        string typeDir = "",
                        string inWorkPreFix = "")
        {
            m_Directory = directory;
            m_SearchPattern = searchPattern;
            m_SearchOption = searchOption;
            m_InProgressFolder = Path.Combine(m_Directory, typeDir, "InProgress");
            m_Prfx = inWorkPreFix;
            var inputFiles = Directory.GetFiles(m_Directory, m_SearchPattern, m_SearchOption);
            m_Files = MoveToInProgress(inputFiles);
        }

        /// <summary>
        /// Looking for all files matching given seartch pattern in given Directory related on search options
        /// prepares just all files which matches on given advanced Regex.
        /// no move will be done
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="advancedRegexCond"> </param>
        public PrepareAndGetFilesForProgress(string directory,
                       string searchPattern = "*.*",
                       SearchOption searchOption = SearchOption.TopDirectoryOnly,
                       string advancedRegexCond = "")
        {
            m_Directory = directory;
            m_SearchPattern = searchPattern;
            m_SearchOption = searchOption;
            m_AdvancedRegexCond = advancedRegexCond;
            var inputFiles = Directory.GetFiles(m_Directory, m_SearchPattern, m_SearchOption);
            m_Files = inputFiles.Where(e => Regex.IsMatch(Path.GetFileName(e) ?? string.Empty, advancedRegexCond)).ToArray();
        }


        public string Title
        {
            get
            {
                return string.Format("Get Files - Directory: '{0}' - Pattern: '{1}' - Search Option: '{2}' - Advanced Regex conditions: '{3}'", m_Directory, m_SearchPattern, m_SearchOption, m_AdvancedRegexCond);
            }
        }

        public IEnumerable<string> GetFilesAsEnumerable()
        {
            return m_Files.OrderBy(e => e);
        }

        public IEnumerable<string> Pull()
        {
            return GetFilesAsEnumerable();
        }

        public string GetInProgressFolder()
        {
            return m_InProgressFolder;
        }

        public string GetInWorkFilePrefix()
        {
            return m_Prfx;
        }

        private string[] MoveToInProgress(IEnumerable<string> inputFiles)
        {
            if (!Directory.Exists(m_InProgressFolder))
                Directory.CreateDirectory(m_InProgressFolder);

            var inProgressFiles = new List<string>();

            foreach (var file in inputFiles)
            {
                var outputFn = Path.Combine(m_InProgressFolder, m_Prfx + Path.GetFileName(file));
                File.Move(file, outputFn);
                inProgressFiles.Add(outputFn);
            }

            return inProgressFiles.ToArray();
        }
    }
}
