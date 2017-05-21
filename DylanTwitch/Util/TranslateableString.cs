// ***********************************************************************
// Assembly         : DylanTwitch
// Author           : Subtixx
// Created          : 05-20-2017
//
// Last Modified By : Subtixx
// Last Modified On : 05-20-2017
// ***********************************************************************
// <copyright file="TranslateableString.cs">
//     Copyright © Subtixx 2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace DylanTwitch.Util
{
    /// <summary>
    ///     Provides a function to provide arguments in a string
    /// </summary>
    public static class TranslateableString
    {
        /// <summary>
        ///     Translates the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>System.String.</returns>
        public static string Translate(string message, Dictionary<string, string> variables)
        {
            return variables.Aggregate(message, (current, variable) => current.Replace(variable.Key, variable.Value));
        }
    }
}