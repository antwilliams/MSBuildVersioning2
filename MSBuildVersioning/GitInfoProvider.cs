using System;

namespace MSBuildVersioning
{
    /// <summary>
    /// Provides Mercurial information for a particular file path, by executing and scraping
    /// information from the hg.exe command-line program.
    /// </summary>
    public class GitInfoProvider : SourceControlInfoProvider
    {
        private int? revisionNumber;
        private string revisionId;
        private string author;
        private string modifiedDate;
        private bool? isWorkingCopyDirty;
        private string branch;
        private string tags;

        public GitInfoProvider()
        {

        }

        public override string SourceControlName
        {
            get { return "Git"; }
        }

        public virtual int GetRevisionNumber()
        {
            if (revisionNumber == null)
            {
                InitRevision();
            }
            return (int)revisionNumber;
        }

        public virtual string GetRevisionId()
        {
            if (revisionId == null)
            {
                InitRevision();
            }
            return revisionId;
        }

        private void InitRevision()
        {
            ExecuteCommand("git.exe", "log -n 1 --format=\"%H %an %ad\"", output =>
            {
                if (revisionId == null)
                {
                    int firstSpace = output.IndexOf(' ');
                    int secondSpace = output.IndexOf(' ',firstSpace+1);
                    revisionId = output.Substring(0, firstSpace);
                    author = output.Substring(firstSpace+1, secondSpace - firstSpace-1);
                    modifiedDate = output.Substring(secondSpace + 1);
                    revisionNumber = 1;
                }
                else
                {
                    revisionNumber += 1;
                }
            },
            null);
        }

        public virtual bool IsWorkingCopyDirty()
        {
            if (isWorkingCopyDirty == null)
            {
                ExecuteCommand("git.exe", "diff-index --quiet HEAD", (exitCode, error) =>
                {
                    if (exitCode == 0)
                    {
                        isWorkingCopyDirty = false;
                        return false;
                    }
                    else if (exitCode == 1)
                    {
                        isWorkingCopyDirty = true;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
            }
            return (bool)isWorkingCopyDirty;
        }

        public virtual string GetBranch()
        {
            if (branch == null)
            {
                branch = ExecuteCommand("git.exe", "describe --all")[0];
            }
            return branch;
        }

        public virtual string GetTags()
        {
            if (tags == null)
            {
                tags = ExecuteCommand("git.exe", "describe")[0];
            }
            return tags;
        }

        public virtual string GetRevisionDate()
        {
            if (modifiedDate != null)
            {
                InitRevision();
            }
            return modifiedDate;               
        }

        public virtual string GetRevisionAuthor()
        {
            if (author == null)
            {
                InitRevision();
            }
            return author;
        }
    }
}
