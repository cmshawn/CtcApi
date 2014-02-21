//Copyright (C) 2012 Bellevue College
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as
//published by the Free Software Foundation, either version 3 of the
//License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License and GNU General Public License along with this program.
//If not, see <http://www.gnu.org/licenses/>.
using System;
using Common.Logging;

namespace CtcApi.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class LogElapsed : IDisposable
    {
        readonly DateTime _start = DateTime.Now;
        readonly String _label = "";

        readonly ILog _log = LogManager.GetCurrentClassLogger();
        private readonly LogLevel _startLevel = LogLevel.Trace;
        private readonly LogLevel _finishLevel = LogLevel.Trace;

        /// <summary>
        /// 
        /// </summary>
        public string TimeFormat
        {
            get
            {
                // TODO: Retrieve value from .config
                return "hh:mm:ss.ffff tt";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="startLevel"></param>
        /// <param name="finishLevel"></param>
        public LogElapsed(String label = "", LogLevel startLevel = LogLevel.Trace, LogLevel finishLevel = LogLevel.Trace)
        {
            string msgStart = "Starting TIMER:";
            if (!string.IsNullOrWhiteSpace(label))
            {
                _label = label;
                msgStart = String.Format("Starting TIMER for: {0}", _label);
            }

            _start = DateTime.Now;
            string message = ConstructMessage(msgStart, _start);

            _startLevel = startLevel;
            _finishLevel = finishLevel;
            WriteToLog(_startLevel, message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            TimeSpan elapsed = DateTime.Now - _start;

            string msgStart = "Elapsed TIME:";
            if (!string.IsNullOrWhiteSpace(_label))
            {
                msgStart = String.Format("Elapsed TIME for {0}:", _label);
            }

            string message = ConstructMessage(msgStart, elapsed);
            WriteToLog(_finishLevel, message);
        }

        #region Private methods
        private void WriteToLog(LogLevel level, string message)
        {
            // unfortunately, Common.Logging doesn't appear to provide a method that allows passing the level
            switch (level)
            {
                case LogLevel.Debug:
                    _log.Debug(message);
                    break;
                case LogLevel.Info:
                    _log.Info(message);
                    break;
                case LogLevel.Trace:
                    _log.Trace(message);
                    break;
                default:
                    _log.Warn("Unsupported LogLevel specified. Supported levels are Info, Debug and Trace. Falling back to Trace.");
                    _log.Trace(message);
                    break;
            }
        }

        private static string ConstructMessage(string text, TimeSpan ts)
        {
            return ConstructMessage(text, ts.ToString());
        }

        private string ConstructMessage(string text, DateTime dt)
        {
            string time = dt.ToString(TimeFormat);
            return ConstructMessage(text, time);
        }

        private static string ConstructMessage(string text, string time)
        {
            return String.Format("{0} ({1})", text, time);
        }

        #endregion
    }
}