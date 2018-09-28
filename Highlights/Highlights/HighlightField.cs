using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlights
{
    public class HighlightField
    {
        public string Text { get; set; }
        public StateEnum State { get; set; }

        public HighlightField()
        {
        }

        public HighlightField(string text, StateEnum state)
        {
            Text = text;
            State = state;
        }

        List<HighlightField> HighlightFields = new List<HighlightField>();
        StateEnum CurState;
        public List<HighlightField> Parse(string x)
        {
            CurState = StateEnum.None;
            StringBuilder sb = new StringBuilder();

            HighlightFields.Clear();

            char prev = ' ';
            foreach ( char c in x )
            {
                if ( Char.IsWhiteSpace(c) )
                {
                    switch (CurState)
                    {
                        case StateEnum.None:    sb.Append(c); break;
                        case StateEnum.Normal:  AddHighlightToList(c, sb, StateEnum.Normal, StateEnum.None, addCharBefore: true); break;
                        case StateEnum.User:    AddHighlightToList(c, sb, StateEnum.User,   StateEnum.None, addCharBefore: false); break;
                        case StateEnum.Hash:    AddHighlightToList(c, sb, StateEnum.Hash,   StateEnum.None, addCharBefore: false); break;
                    }
                }
                else if (c.Equals('@'))
                {
                    bool addBefore = false;
                    StateEnum newstate = StateEnum.User;
                    if (!char.IsWhiteSpace(prev))
                    {
                        newstate = StateEnum.None;
                        addBefore = true;
                    }
                    switch (CurState)
                    {
                        case StateEnum.None:    AddHighlightToList(c, sb, StateEnum.None,   newstate, addBefore); break;
                        case StateEnum.Normal:  AddHighlightToList(c, sb, StateEnum.Normal, newstate, addBefore); break;
                        case StateEnum.User:    AddHighlightToList(c, sb, StateEnum.None,   newstate, addBefore); break;
                        case StateEnum.Hash:    AddHighlightToList(c, sb, StateEnum.None,   newstate, addBefore); break;
                    }
                }
                else if (c.Equals('#'))
                {
                    bool addBefore = false;
                    StateEnum newstate = StateEnum.Hash;
                    if ( !( char.IsWhiteSpace(prev) || prev.Equals('#') || prev.Equals('@')) )
                    {
                        newstate = StateEnum.None;
                        addBefore = (CurState.Equals(StateEnum.User)) ? false :true;
                    }
                    switch (CurState)
                    {
                        case StateEnum.None:    AddHighlightToList(c, sb, StateEnum.None,   newstate, addBefore); break;
                        case StateEnum.Normal:  AddHighlightToList(c, sb, StateEnum.Normal, newstate, addBefore); break;
                        case StateEnum.User:    AddHighlightToList(c, sb, StateEnum.User,   newstate, addBefore); break;
                        case StateEnum.Hash:    AddHighlightToList(c, sb, StateEnum.None,   newstate, addBefore); break;
                    }
                }
                else if ( char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
                else // junk chars
                {
                    sb.Append(c);                    
                }
                prev = c;
            }
            AddHighlightToList( sb, CurState, CurState);
            return HighlightFields;
        }
        public void AddHighlightToList(StringBuilder sb, StateEnum addSstate, StateEnum newstate)
        {
            StateEnum state = addSstate;
            if (sb.Length >= 2) sb.Remove(sb.Length - 2, 2);
            if (sb.Length < 2) state = StateEnum.Normal;
            if (sb.Length < 1) return;

            HighlightFields.Add(new HighlightField(sb.ToString(), state));
            sb.Clear();

            CurState = newstate;
        }

        public void AddHighlightToList(char c, StringBuilder sb, StateEnum addSstate, StateEnum newstate, bool addCharBefore)
        {
            if ( addCharBefore ) sb.Append(c);
            StateEnum state = addSstate;
            if (sb.Length < 2) state = StateEnum.Normal;

            HighlightFields.Add(new HighlightField(sb.ToString(), state));
            sb.Clear();
            if ( ! addCharBefore ) sb.Append(c);
            CurState = newstate;
        }
    }

}
