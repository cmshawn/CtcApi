//Copyright (C) 2011 Bellevue College and Peninsula College
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
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using Ctc.Ods.Data;
using System.Configuration;
using Ctc.Ods.Config;

namespace Ctc.Ods
{
	///<summary>
	///</summary>
	public class LateStartFacet : ISectionFacet
	{
		private readonly ushort _days;

		///<summary>
		///</summary>
		public LateStartFacet()
		{
      ApiSettings settings = ConfigurationManager.GetSection(ApiSettings.SectionName) as ApiSettings;
			_days = settings != null ? settings.ClassFlags.LateStartDaysCount : (ushort)0;

			// If required setting does not exist or is set to zero
			if (_days <= 0)
			{
				throw new SettingsPropertyWrongTypeException("Missing a valid value for lateStartDaysCount in the classFlags element of the configuration settings. Either the attribute does not exist, or it is not a positive non-zero integer.");
			}
		}

		/// <summary>
		/// Provides the appropriate anonymous method for a LINQ .Where() call, depending on the option specified
		/// </summary>
		/// <param name="dbContext"></param>
		public Expression<Func<T, bool>> GetFilter <T>(DbContext dbContext) where T : SectionEntity
		{
			if (dbContext != null)
			{
				OdsContext db = dbContext as OdsContext;

				if (db != null)
				{
					return s => s.StartDate.HasValue &&
											SqlFunctions.DateAdd("day", (_days * -1), s.StartDate) >= db.YearQuarters.Where(y => y.YearQuarterID == s.YearQuarterID)
							  																																		 .Select(y => y.FirstClassDay)
																																										 .FirstOrDefault();
				}
				
				throw new ArgumentNullException("dbContext", "Database context is not valid.");
			}
			
			throw new ArgumentNullException("dbContext", "Database context is null.");
		}
	}
}