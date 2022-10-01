using NPoco;
using System;
using System.Linq;
using uSupport.Dtos.Tables;
using System.Collections.Generic;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Extensions
{
	public static class SqlExtensions
	{
		public static Sql GetFullTicket(this Sql sql)
		{
			return sql.InnerJoin(TicketTypeTableAlias)
				.On($"[TypeId] = {TicketTypeTableAlias}.Id")
				.InnerJoin(TicketStatusTableAlias)
				.On($"[StatusId] = {TicketStatusTableAlias}.Id");
		}

		public static string ConvertStatusesToSql(this IEnumerable<uSupportTicketStatus> statuses)
		{
			if (statuses == null) return string.Empty;

			string sql = string.Empty;

			foreach (uSupportTicketStatus status in statuses)
			{
				sql += status.Id.ConvertGuidToSqlString();

				if (status != statuses.LastOrDefault())
					sql += ", ";
			}

			return sql;
		}

		public static string ConvertGuidToSqlString(this List<Guid> guids)
        {
			List<string> sql = new List<string>();
            foreach (Guid guid in guids)
				sql.Add(string.Format("CAST('{0}' as uniqueidentifier)", guid));

			return string.Join(",", sql);
        }
			
		public static string ConvertGuidToSqlString(this Guid guid) => $"CAST('{guid}' as uniqueidentifier)";
	}
}