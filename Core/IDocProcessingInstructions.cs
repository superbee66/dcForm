namespace dCForm.Core
{
    internal interface IDocProcessingInstructions
    {
        int DocChecksum
        {
            get;
            set;
        }

        bool? DocStatus
        {
            get;
            set;
        }

        string DocTitle
        {
            get;
            set;
        }

        string href
        {
            get;
            set;
        }

        string name
        {
            get;
            set;
        }
        string DocSubmittedBy
        {
            get;
            set;
        }

        /// <summary>
        ///     alpha-numeric, period & underscore string that sorts (orderby) in a manner that puts the most current version at
        ///     the bottom of a list
        ///     legal strings are
        ///     1.0.0.1
        ///     1.0.0.
        /// </summary>
        string solutionVersion
        {
            get;
            set;
        }
    }
}