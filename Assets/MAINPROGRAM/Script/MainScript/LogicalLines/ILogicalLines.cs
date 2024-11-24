using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    public interface ILogicalLines
    {
        string keyword { get; }

        bool Matches(DailogLine line);

        IEnumerator Execute(DailogLine line  );
    }
}
