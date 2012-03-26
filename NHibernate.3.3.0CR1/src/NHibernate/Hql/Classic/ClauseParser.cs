using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// Parses the hibernate query into its constituent clauses.
	/// </summary>
	public class ClauseParser : IParser
	{
		private IParser child;
		private IList<string> selectTokens;
		private bool cacheSelectTokens = false;
		private bool byExpected = false;
		private int parenCount = 0;

		public virtual void Token(string token, QueryTranslator q)
		{
			string lcToken = token.ToLowerInvariant();

			if (token.Equals(StringHelper.OpenParen))
			{
				parenCount++;
			}
			else if (token.Equals(StringHelper.ClosedParen))
			{
				parenCount--;
			}

			if (byExpected && !"by".Equals(lcToken))
			{
				throw new QueryException("BY expected after GROUP or ORDER: " + token);
			}

			bool isClauseStart = parenCount == 0; //ignore subselect keywords

			if (isClauseStart)
			{
				if ("select".Equals(lcToken))
				{
					selectTokens = new List<string>();
					cacheSelectTokens = true;
				}
				else if ("from".Equals(lcToken))
				{
					child = new FromParser();
					child.Start(q);
					cacheSelectTokens = false;
				}
				else if ("where".Equals(lcToken))
				{
					EndChild(q);
					child = new WhereParser();
					child.Start(q);
				}
				else if ("order".Equals(lcToken))
				{
					EndChild(q);
					child = new OrderByParser();
					byExpected = true;
				}
				else if ("having".Equals(lcToken))
				{
					EndChild(q);
					child = new HavingParser();
					child.Start(q);
				}
				else if ("group".Equals(lcToken))
				{
					EndChild(q);
					child = new GroupByParser();
					byExpected = true;
				}
				else if ("by".Equals(lcToken))
				{
					if (!byExpected)
					{
						throw new QueryException("GROUP or ORDER expected before BY");
					}
					child.Start(q);
					byExpected = false;
				}
				else
				{
					isClauseStart = false;
				}
			}

			if (!isClauseStart)
			{
				if (cacheSelectTokens)
				{
					selectTokens.Add(token);
				}
				else
				{
					if (child == null)
					{
						throw new QueryException("query must begin with SELECT or FROM: " + token);
					}
					else
					{
						child.Token(token, q);
					}
				}
			}
		}

		private void EndChild(QueryTranslator q)
		{
			if (child == null)
			{
				//null child could occur for no from clause in a filter
				cacheSelectTokens = false;
			}
			else
			{
				child.End(q);
			}
		}

		public virtual void Start(QueryTranslator q)
		{
		}

		public virtual void End(QueryTranslator q)
		{
			EndChild(q);
			if (selectTokens != null)
			{
				child = new SelectParser();
				child.Start(q);
				foreach (string item in selectTokens)
				{
					Token(item, q);
				}
				child.End(q);
			}
			byExpected = false;
			parenCount = 0;
			cacheSelectTokens = false;
		}
	}
}