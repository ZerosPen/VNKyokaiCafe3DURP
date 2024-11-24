using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public interface ILogicalLines
    {
        string keyWord { get; }

        bool Matches(DailogLine line);

        IEnumerator Excute(DailogLine line  );
    }
}
