﻿using System.Collections.Generic;

namespace dCForm.Core
{
    public interface IDocTerm
    {
        string AsTermTxt();

        string DocId
        {
            get;
            set;
        }

        Dictionary<string, string> DocIdKeys
        {
            get;
            set;
        }

        string DocTypeName
        {
            get;
            set;
        }
    }
}