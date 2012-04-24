using System;

namespace MSBuildVersioning
{
    /// <summary>
    /// Provides Mercurial information for a particular file path, by executing and scraping
    /// information from the hg.exe command-line program.
    /// </summary>
    public class HgInfoProvider : SourceControlInfoProvider
    {
        private int? revisionNumber;
        private string revisionId;
        private bool? isWorkingCopyDirty;
        private string branch;
        private string tags;

        public override string SourceControlName
        {
            get { return "Mercurial"; }
        }

        public virtual int GetRevisionNumber()
        {
            if (revisionNumber == null)
            {
                string revisionNumberStr = ExecuteCommand("hg.exe", "identify -n")[0];

                if (revisionNumberStr.Contains("+"))
                {
                    isWorkingCopyDirty = true;
                    revisionNumber = int.Parse(
                        revisionNumberStr.Substring(0, revisionNumberStr.IndexOf("+")));
                }
                else
                {
                    isWorkingCopyDirty = false;
                    revisionNumber = int.Parse(revisionNumberStr);
                }
            }
            return (int)revisionNumber;
        }

        public virtual string GetRevisionId(bool completeHash)
        {
            if (revisionId == null)
            {
                revisionId = ExecuteCommand("hg.exe", completeHash ? "identify -i --debug" : "identify -i")[0];

                if (revisionId.Contains("+"))
                {
                    isWorkingCopyDirty = true;
                    revisionId = revisionId.Substring(0, revisionId.IndexOf("+"));
                }
                else
                {
                    isWorkingCopyDirty = false;
                }
            }
            return revisionId;
        }

        public virtual bool IsWorkingCopyDirty()
        {
            if (isWorkingCopyDirty == null)
            {
                GetRevisionNumber();
            }
            return (bool)isWorkingCopyDirty;
        }

        public virtual string GetBranch()
        {
            if (branch == null)
            {
                branch = ExecuteCommand("hg.exe", "identify -b")[0];
            }
            return branch;
        }

        public virtual string GetTags()
        {
            if (tags == null)
            {
                tags = ExecuteCommand("hg.exe", "identify -t")[0];
            }
            return tags;
        }
    }
}
