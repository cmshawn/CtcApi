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
using Common.Logging.Simple;
using CtcApi.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.CtcApi
{
  [TestClass]
  public class LogElapsedTest
  {
    private const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Trace;
    private static CapturingLoggerFactoryAdapter _adapter;

    #region Initialize/cleanup
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      // http://netcommon.sourceforge.net/docs/2.0.0/api/html/Common.Logging~Common.Logging.Simple.CapturingLoggerFactoryAdapter.html
      _adapter = new CapturingLoggerFactoryAdapter();
      LogManager.Adapter = _adapter;
    }

    [ClassCleanup()]
    public static void MyClassCleanup()
    {
    }

    [TestInitialize()]
    public void MyTestInitialize()
    {
      _adapter.Clear();
    }

    [TestCleanup()]
    public void MyTestCleanup()
    {
    }

    #endregion

    #region Tests
    [TestMethod]
    public void DefaultConstructor()
    {
      using (new LogElapsed())
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(DEFAULT_LOG_LEVEL);
    }

    [TestMethod]
    public void With_Label()
    {
      using (new LogElapsed("Testing label"))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
        AssertLastLogMessageStartsWith("Starting TIMER for: Testing label");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(DEFAULT_LOG_LEVEL);
      AssertLastLogMessageStartsWith("Elapsed TIME for Testing label:");
    }

    [TestMethod]
    public void With_Label_StartLevel()
    {
      using (new LogElapsed("Testing label", LogLevel.Debug))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(LogLevel.Debug);
        AssertLastLogMessageStartsWith("Starting TIMER for: Testing label");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(DEFAULT_LOG_LEVEL);
      AssertLastLogMessageStartsWith("Elapsed TIME for Testing label:");
    }

    [TestMethod]
    public void With_Label_StartLevel_EndLevel()
    {
      using (new LogElapsed("Testing label", LogLevel.Debug, LogLevel.Info))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(LogLevel.Debug);
        AssertLastLogMessageStartsWith("Starting TIMER for: Testing label");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(LogLevel.Info);
      AssertLastLogMessageStartsWith("Elapsed TIME for Testing label:");
    }

    [TestMethod]
    public void With_StartLevel_EndLevel()
    {
      using (new LogElapsed(startLevel: LogLevel.Info, finishLevel: LogLevel.Debug))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(LogLevel.Info);
        AssertLastLogMessageStartsWith("Starting TIMER:");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(LogLevel.Debug);
      AssertLastLogMessageStartsWith("Elapsed TIME:");
    }

    [TestMethod]
    public void With_EndLevel()
    {
      using (new LogElapsed(finishLevel: LogLevel.Debug))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
        AssertLastLogMessageStartsWith("Starting TIMER:");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(LogLevel.Debug);
      AssertLastLogMessageStartsWith("Elapsed TIME:");
    }

    [TestMethod]
    public void With_Label_EndLevel()
    {
      using (new LogElapsed("Testing label", finishLevel: LogLevel.Debug))
      {
        AssertLogEventCount(1);
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
        AssertLastLogMessageStartsWith("Starting TIMER for: Testing label");
      }

      AssertLogEventCount(2);
      AssertLastLogLeven(LogLevel.Debug);
      AssertLastLogMessageStartsWith("Elapsed TIME for Testing label:");
    }

    [TestMethod]
    public void InvalidStartLevels()
    {
      foreach (LogLevel level in new[] {LogLevel.Fatal, LogLevel.Error, LogLevel.Warn})
      {
        _adapter.Clear();
        Console.Out.Write("Testing LogLevel: {0}...\n", level.ToString("F"));

        using (new LogElapsed(startLevel: level))
        {
          // should issue warning...
          AssertLogEventCount(2);
          Assert.AreEqual(LogLevel.Warn, _adapter.LoggerEvents[0].Level);
          // ...then fall back to default
          AssertLastLogLeven(DEFAULT_LOG_LEVEL);
          AssertLastLogMessageStartsWith("Starting TIMER:");
        }

        AssertLogEventCount(3);
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
        AssertLastLogMessageStartsWith("Elapsed TIME:");
      }
    }

    [TestMethod]
    public void InvalidEndLevels()
    {
      foreach (LogLevel level in new[] {LogLevel.Fatal, LogLevel.Error, LogLevel.Warn})
      {
        _adapter.Clear();
        Console.Out.Write("Testing LogLevel: {0}...\n", level.ToString("F"));

        using (new LogElapsed(finishLevel: level))
        {
          AssertLogEventCount(1);
          AssertLastLogLeven(DEFAULT_LOG_LEVEL);
          AssertLastLogMessageStartsWith("Starting TIMER:");
        }

        AssertLogEventCount(3);
        // should issue warning...
        Assert.AreEqual(LogLevel.Warn, _adapter.LoggerEvents[1].Level);
        // ...then fall back to default
        AssertLastLogLeven(DEFAULT_LOG_LEVEL);
        AssertLastLogMessageStartsWith("Elapsed TIME:");
      }
    }

    #endregion

    #region Private methods
    private static void AssertLastLogMessageStartsWith(string msgStart)
    {
      string logMessage = _adapter.LastEvent.RenderedMessage;
      Assert.IsTrue(logMessage.StartsWith(msgStart), "Expected message beginning '{0}', but received '{1}'", msgStart, logMessage);
    }

    private void AssertLastLogLeven(LogLevel level)
    {
      Assert.AreEqual(level, _adapter.LastEvent.Level);
    }

    private static void AssertLogEventCount(int count)
    {
      Assert.AreEqual(count, _adapter.LoggerEvents.Count);
    }

    #endregion
  }
}
