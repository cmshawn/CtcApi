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
