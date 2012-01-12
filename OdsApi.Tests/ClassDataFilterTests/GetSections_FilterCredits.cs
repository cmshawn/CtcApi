﻿//Copyright (C) 2011 Bellevue College and Peninsula College
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ctc.Ods.Types;

namespace Ctc.Ods.Tests.ClassDataFilterTests
{
    /// <summary>
    /// Summary description for GetSections_FilterEarlyStart
    /// </summary>
    [TestClass]
    public class GetSections_FilterCredits
    {
        private DataVerificationHelper _dataVerifier;

        public GetSections_FilterCredits()
        {
            _dataVerifier = new DataVerificationHelper();
        }

        ~GetSections_FilterCredits()
        {
            _dataVerifier.Dispose();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetSections_Credits_Success()
        {
            int credits = 2;

            IList<Section> sections = TestHelper.GetSectionsWithFilter(new CreditsFacet(credits));
            int correctSectionsExact = sections.Where(s => s.Credits == credits).ToList().Count;
            int correctSectionsRounded = sections.Where(s => s.Credits > credits).ToList().Count;

            int correctCount = correctSectionsExact + correctSectionsRounded;

            string totalWhereClause = String.Format("Credits between {0} and {1}", credits, credits+0.99);
            int expectedCount = _dataVerifier.GetSectionCount(totalWhereClause);
            Assert.AreEqual(expectedCount, sections.Count); // Did we get the expected amount of results?
            Assert.AreEqual(sections.Count, correctCount);  // And all results are valid?

            int incorrectCount = sections.Where(s => s.Credits < credits).ToList().Count;   // Did section credit amounts that were too SMALL get returned?
            incorrectCount += sections.Where(s => s.Credits >= credits + 1).ToList().Count; // Did section credit amounts that were too LARGE get returned?
            Assert.AreEqual(0, incorrectCount);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetSections_OneOrLessCredits_Success()
        {
            int credits = 1;

            IList<Section> sections = TestHelper.GetSectionsWithFilter(new CreditsFacet(credits));
            int correctSectionsExact = sections.Where(s => s.Credits == credits).ToList().Count;
            int correctSectionsRounded = sections.Where(s => s.Credits > credits).ToList().Count;
            correctSectionsRounded += sections.Where(s => s.Credits >= 0 && s.Credits < credits).ToList().Count;

            int correctCount = correctSectionsExact + correctSectionsRounded;

            string totalWhereClause = String.Format("Credits between 0 and {0}", credits + 0.99);
            int expectedCount = _dataVerifier.GetSectionCount(totalWhereClause);
            Assert.AreEqual(expectedCount, sections.Count); // Did we get the expected amount of results?
            Assert.AreEqual(sections.Count, correctCount);  // And all results are valid?

            int incorrectCount = sections.Where(s => s.Credits >= credits + 1).ToList().Count;
            Assert.AreEqual(0, incorrectCount); // Were any of the results returned incorrect?
        }
    }
}
