// ***********************************************************************
// Assembly         : DylanTwitch
// Author           : Subtixx
// Created          : 05-19-2017
//
// Last Modified By : Subtixx
// Last Modified On : 05-19-2017
// ***********************************************************************
// <copyright file="CommandController.cs">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using TwitchLib.Events.Client;

namespace DylanTwitch
{
    /// <summary>
    ///     Class to provide a way to register commands
    /// </summary>
    public class CommandController
    {
        /// <summary>
        ///     The registered global commands
        /// </summary>
        private readonly Dictionary<string, Func<OnChatCommandReceivedArgs, bool>> _registeredGlobalCommands =
            new Dictionary<string, Func<OnChatCommandReceivedArgs, bool>>();

        /// <summary>
        ///     The registered whisper commands
        /// </summary>
        private readonly Dictionary<string, Func<OnWhisperCommandReceivedArgs, bool>> _registeredWhisperCommands =
            new Dictionary<string, Func<OnWhisperCommandReceivedArgs, bool>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandController" /> class.
        /// </summary>
        internal CommandController()
        {
        }

        /// <summary>
        ///     Executes the global command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the command was executed successfully, <c>false</c> otherwise.</returns>
        internal bool ExecuteGlobalCommand(string commandName, OnChatCommandReceivedArgs args)
        {
            if (_registeredGlobalCommands.ContainsKey(commandName))
                return _registeredGlobalCommands[commandName](args);
            return false;
        }

        /// <summary>
        ///     Executes the whisper command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the command was executed successfully, <c>false</c> otherwise.</returns>
        internal bool ExecuteWhisperCommand(string commandName, OnWhisperCommandReceivedArgs args)
        {
            if (_registeredWhisperCommands.ContainsKey(commandName))
                return _registeredWhisperCommands[commandName](args);
            return false;
        }

        #region API

        /// <summary>
        ///     Registers the global command.
        ///     Note: Automatically inserts the current command prefix!
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="function">The function to execute.</param>
        /// <param name="aliases">A list of aliases for this command</param>
        /// <returns><c>true</c> if the command was added successfully, <c>false</c> otherwise.</returns>
        public bool RegisterGlobalCommand(string commandName, Func<OnChatCommandReceivedArgs, bool> function,
            List<string> aliases = null)
        {
            if (!_registeredGlobalCommands.ContainsKey(commandName))
            {
                _registeredGlobalCommands.Add(commandName, function);

                // Add all aliases if they exist.
                aliases?.ForEach(a => { _registeredGlobalCommands.Add(a, function); });
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Unregisters the global command.
        ///     Note: Automatically inserts the current command prefix!
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns><c>true</c> if the command was found and removed, <c>false</c> otherwise.</returns>
        public bool UnregisterGlobalCommand(string commandName)
        {
            if (_registeredGlobalCommands.ContainsKey(commandName))
            {
                _registeredGlobalCommands.Remove(commandName);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Registers the whisper command.
        ///     Note: Automatically inserts the current command prefix!
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="function">The function.</param>
        /// <returns><c>true</c> if the command was added sucessfully, <c>false</c> otherwise.</returns>
        public bool RegisterWhisperCommand(string commandName, Func<OnWhisperCommandReceivedArgs, bool> function)
        {
            if (!_registeredWhisperCommands.ContainsKey(commandName))
            {
                _registeredWhisperCommands.Add(commandName, function);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Unregisters the whisper command.
        ///     Note: Automatically inserts the current command prefix!
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns><c>true</c> if was found and removed, <c>false</c> otherwise.</returns>
        public bool UnregisterWhisperCommand(string commandName)
        {
            if (_registeredWhisperCommands.ContainsKey(commandName))
            {
                _registeredWhisperCommands.Remove(commandName);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Utility function to parse arguments
        ///     Can parse arguments surrounded by quotes
        ///     Ex: "ARG 1" "ARG 2" are 2 arguments instead of 4
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public static IList<string> ParseLine(string line)
        {
            var args = new List<string>();
            var quote = false;
            for (int i = 0, n = 0; i <= line.Length; ++i)
            {
                if ((i == line.Length || line[i] == ' ') && !quote)
                {
                    if (i - n > 0)
                        args.Add(line.Substring(n, i - n).Trim(' ', '"'));

                    n = i + 1;
                    continue;
                }

                if (line[i] == '"')
                    quote = !quote;
            }

            return args;
        }

        #endregion
    }
}