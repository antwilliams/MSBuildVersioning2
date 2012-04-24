using System;
using System.Collections.Generic;

namespace MSBuildVersioning
{
    /// <summary>
    /// Provides Subversion information for a particular file path, by executing and scraping
    /// information from the svn.exe command-line program.
    /// </summary>
    public class SvnInfoProvider : SourceControlInfoProvider
    {
        private int? revisionNumber;
        private bool? isMixedRevisions;
        private bool? isWorkingCopyDirty;
        private string repositoryUrl;
        private string repositoryRoot;

        public override string SourceControlName
        {
            get { return "Subversion"; }
        }

        public virtual int GetRevisionNumber()
        {
            if (revisionNumber == null)
            {
                SvnInfoParser parser = new SvnInfoParser();
                ExecuteCommand("svn.exe", "info -R", parser.ReadLine, null);
                revisionNumber = parser.maxRevisionNumber;
                isMixedRevisions = parser.isMixedRevisions;
            }
            return (int)revisionNumber;
        }

        public virtual bool IsMixedRevisions()
        {
            if (isMixedRevisions == null)
            {
                GetRevisionNumber();
            }
            return (bool)isMixedRevisions;
        }

        public virtual bool IsWorkingCopyDirty()
        {
            if (isWorkingCopyDirty == null)
            {
                SvnStatusParser parser = new SvnStatusParser();
                ExecuteCommand("svn.exe", "status", parser.ReadLine, null);
                isWorkingCopyDirty = parser.isWorkingCopyDirty;
            }
            return (bool)isWorkingCopyDirty;
        }

        public virtual string GetRepositoryUrl()
        {
            if (repositoryUrl == null)
            {
                IList<string> svnInfo = ExecuteCommand("svn.exe", "info");
                foreach (string line in svnInfo)
                {
                    if (line.StartsWith("URL: "))
                    {
                        repositoryUrl = line.Substring("URL: ".Length);
                    }
                    else if (line.StartsWith("Repository Root: "))
                    {
                        repositoryRoot = line.Substring("Repository Root: ".Length);
                    }
                }
            }
            return repositoryUrl;
        }

        public virtual string GetRepositoryRoot()
        {
            if (repositoryRoot == null)
            {
                GetRepositoryUrl();
            }
            return repositoryRoot;
        }

        public virtual string GetRepositoryPath()
        {
            string path = GetRepositoryUrl().Substring(GetRepositoryRoot().Length);
            if (path.Length == 0)
            {
                return "/";
            }
            else
            {
                return path;
            }
        }

        public virtual string GetRepositorySubDirectory(string directory)
        {
            string[] pathComponents = GetRepositoryPath().Split('/');
            for (int i = 0; i < pathComponents.Length - 1; i++)
            {
                if (pathComponents[i] == directory)
                {
                    return pathComponents[i + 1];
                }
            }
            return "";
        }

        public virtual string GetBranch()
        {
            return GetRepositorySubDirectory("branches");
        }

        public virtual string GetTag()
        {
            return GetRepositorySubDirectory("tags");
        }

        private class SvnInfoParser
        {
            public int maxRevisionNumber = -1;
            public bool isMixedRevisions = false;

            public void ReadLine(string line)
            {
                if (line.StartsWith("Revision: "))
                {
                    int revision = int.Parse(line.Substring("Revision: ".Length));
                    if (maxRevisionNumber >= 0 && maxRevisionNumber != revision)
                    {
                        isMixedRevisions = true;
                    }
                    maxRevisionNumber = Math.Max(revision, maxRevisionNumber);
                }
            }
        }

        private class SvnStatusParser
        {
            public bool isWorkingCopyDirty = false;

            public void ReadLine(string line)
            {
                isWorkingCopyDirty = true;
            }
        }
    }
}
