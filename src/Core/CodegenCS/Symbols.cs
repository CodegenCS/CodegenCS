﻿using CodegenCS.ControlFlow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodegenCS
{
    public class Symbols
    {
        /// <summary>
        /// Writes the specified string without indentation
        /// </summary>
        public static WriteRawSymbol RAW(string text)
        {
            return new WriteRawSymbol(text);
        }

        /// <summary>
        /// Starts a conditional block. If condition is false, everything written to TextWriter until the matching ELSE or ENDIF will be discarded.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IfSymbol IF(bool condition)
        {
            return new IfSymbol(condition);
        }

        /// <summary>
        /// Starts the ELSE part of a conditional block. If the IF condition is true then everything written to TextWriter from the ELSE until the matching ENDIF will be discarded.
        /// </summary>
        public static ElseSymbol ELSE => new ElseSymbol();
        
        /// <summary>
        /// Finishes a conditional block. Everything written to TextWritter after this will get back to normal (won't be discarded, even if the IF condition was false).
        /// </summary>
        public static EndIfSymbol ENDIF => new EndIfSymbol();

        /// <summary>
        /// Immediate IF: Returns one of two objects, depending on the evaluation of an expression.
        /// </summary>
        public static FormattableString IIF(bool condition, FormattableString truePart, FormattableString falsePart = null)
        {
            if (condition)
                return truePart;
            return falsePart;
        }

        /// <summary>
        /// Immediate IF: Returns one of two objects, depending on the evaluation of an expression.
        /// </summary>
        public static Func<FormattableString> IIF(bool condition, Func<FormattableString> truePart, Func<FormattableString> falsePart = null)
        {
            if (condition)
                return truePart;
            return falsePart;
        }

        /// <summary>
        /// Comments will be ignored
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static string COMMENT(string comment) => "";

        /// <summary>
        /// Trims Leading Whitespace:
        /// All previous whitespace (including linebreaks) will be removed.
        /// This means that this command will "backspace until it finds a non-whitespace character" (which is usually in previous lines)
        /// 
        /// Usually this is used before an IF block when (for code clarity) there are linebreak(s) before the IF
        /// but you don't want those linebreak(s) to appear (usually because you'll have a linebreak INSIDE the IF)
        /// </summary>
        public static TrimLeadingWhitespaceSymbol TLW => new TrimLeadingWhitespaceSymbol();

        /// <summary>
        /// Trims Trailing Whitespace:
        /// All subsequent whitespace (including linebreaks) will be removed (ignored).
        /// This means that this command will "backspace until the end of the previous line".
        /// </summary>
        /// <returns></returns>
        public static TrimTrailingWhitespaceSymbol TTW => new TrimTrailingWhitespaceSymbol();



        //TODO: Add something like $"{TWS}" (trim whitespace) that would behave like Jinja templates:
        //https://stackoverflow.com/questions/45719062/jinja-docx-template-avoiding-new-line-in-nested-for
        //https://ttl255.com/jinja2-tutorial-part-3-whitespace-control/

        #region Debugging
        // If you're debugging through visual studio (e.g. Unit Tests, or any other code going through CodegenTextWriter)
        // it can be difficult to debug through CodegenTextWriter because it writes interpolated strings element-by-element
        // If you want to brea in the debugger at any specific point inside an interpolated string (markup mode)
        // all you have to do is interpolate BREAKIF(true)
        public static Action BREAKIF(bool condition) => () => { if (condition) { System.Diagnostics.Debugger.Break(); } };
        
        // If you want to break from inside a method in your template (programmatic mode)
        // then you can just use System.Diagnostics.Debugger.Break() and make sure you
        // disable "Tools - Debugging - General - Enable Just My Code", in order to break and see the template source code
        #endregion
    }
}
