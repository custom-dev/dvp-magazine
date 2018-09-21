using System;
using System.Collections.Generic;
using System.Text;

namespace Developpez.MagazineTool
{
    public class ContenuState
    {
        public ContenuState()
        {
            this.Stack = new Stack<string>();
            this.Sections = new Stack<int>();
            this.PendingFootnotes = new List<string>();
        }

        public Stack<string> Stack { get; set; }
        public Stack<int> Sections { get; set; }
        public List<string> PendingFootnotes { get; set; }
    }
}
