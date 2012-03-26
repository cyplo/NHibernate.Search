using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// Parses the ORDER BY clause of a query
	/// </summary>
	public class OrderByParser : IParser
	{
		// This uses a PathExpressionParser but notice that compound paths are not valid,
		// only bare names and simple paths:

		// SELECT p FROM p IN CLASS eg.Person ORDER BY p.Name, p.Address, p

		// The reason for this is SQL doesn't let you sort by an expression you are
		// not returning in the result set.

		private readonly PathExpressionParser pathExpressionParser = new PathExpressionParser();

		public void Token(string token, QueryTranslator q)
		{
			if (q.IsName(StringHelper.Root(token)))
			{
				ParserHelper.Parse(pathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
				q.AppendOrderByToken(pathExpressionParser.WhereColumn);
				pathExpressionParser.AddAssociation(q);
			}
			else if (token.StartsWith(ParserHelper.HqlVariablePrefix))
			{
				var name = token.Substring(1);
				q.AppendOrderByParameter(name);
			}
			else if (StringHelper.SqlParameter.Equals(token))
			{
				q.AppendOrderByParameter();
			}
			else
			{
				q.AppendOrderByToken(token);
			}
		}

		public void Start(QueryTranslator q)
		{
		}

		public void End(QueryTranslator q)
		{
		}

		public OrderByParser()
		{
			pathExpressionParser.UseThetaStyleJoin = true;
		}
	}
}