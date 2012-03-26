using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// Parses the GROUP BY clause of an aggregate query
	/// </summary>
	public class GroupByParser : IParser
	{
		//this is basically a copy/paste of OrderByParser ... might be worth refactoring

		// This uses a PathExpressionParser but notice that compound paths are not valid,
		// only bare names and simple paths:

		// SELECT p FROM p IN CLASS eg.Person GROUP BY p.Name, p.Address, p

		// The reason for this is SQL doesn't let you sort by an expression you are
		// not returning in the result set.

		private readonly PathExpressionParser pathExpressionParser = new PathExpressionParser();

		public void Token(string token, QueryTranslator q)
		{
			if (q.IsName(StringHelper.Root(token)))
			{
				ParserHelper.Parse(pathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
				q.AppendGroupByToken(pathExpressionParser.WhereColumn);
				pathExpressionParser.AddAssociation(q);
			}
			else if (token.StartsWith(ParserHelper.HqlVariablePrefix)) //named query parameter
			{
				var name = token.Substring(1);
				q.AppendGroupByParameter(name);
			}
			else if (token.Equals(StringHelper.SqlParameter))
			{
				q.AppendGroupByParameter();
			}
			else
			{
				q.AppendGroupByToken(token);
			}
		}

		public void Start(QueryTranslator q)
		{
		}

		public void End(QueryTranslator q)
		{
		}

		public GroupByParser()
		{
			pathExpressionParser.UseThetaStyleJoin = true;
		}
	}
}
