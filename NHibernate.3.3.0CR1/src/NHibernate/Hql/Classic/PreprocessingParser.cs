using System.Collections.Generic;
using System.Text;
using IESI = Iesi.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary>HQL lexical analyzer (not really a parser)</summary>
	public class PreprocessingParser : IParser
	{
		private static readonly ISet<string> operators;
		private static readonly IDictionary<string,string> collectionProps;

		static PreprocessingParser()
		{
            operators = new IESI.HashedSet<string>();
			operators.Add("<=");
			operators.Add(">=");
			operators.Add("=>");
			operators.Add("=<");
			operators.Add("!=");
			operators.Add("<>");
			operators.Add("!#");
			operators.Add("!~");
			operators.Add("!<");
			operators.Add("!>");
			operators.Add("is not");
			operators.Add("not like");
			operators.Add("not in");
			operators.Add("not between");
			operators.Add("not exists");

			collectionProps = new Dictionary<string, string>();
			collectionProps.Add("elements", "elements");
			collectionProps.Add("indices", "indices");
			collectionProps.Add("size", "size");
			collectionProps.Add("maxindex", "maxIndex");
			collectionProps.Add("minindex", "minIndex");
			collectionProps.Add("maxelement", "maxElement");
			collectionProps.Add("minelement", "minElement");

			collectionProps.Add("index", "index");
		}

		private readonly IDictionary<string, string> replacements;
		private bool quoted;
		private StringBuilder quotedString;
		private readonly ClauseParser parser = new ClauseParser();
		private string lastToken;
		private string currentCollectionProp;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="replacements"></param>
		public PreprocessingParser(IDictionary<string, string> replacements)
		{
			this.replacements = replacements;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		public void Token(string token, QueryTranslator q)
		{
			//handle quoted strings
			if (quoted)
			{
				quotedString.Append(token);
			}
			if ("'".Equals(token))
			{
				if (quoted)
				{
					token = quotedString.ToString();
				}
				else
				{
					quotedString = new StringBuilder(20).Append(token);
				}
				quoted = !quoted;
			}
			if (quoted)
			{
				return;
			}

			//ignore whitespace
			if (ParserHelper.IsWhitespace(token))
			{
				return;
			}

			//do replacements
			string substoken;
			if(replacements.TryGetValue(token, out substoken))
				token = substoken;

			//handle HQL2 collection syntax
			if (currentCollectionProp != null)
			{
				if (StringHelper.OpenParen.Equals(token))
				{
					return;
				}
				else if (StringHelper.ClosedParen.Equals(token))
				{
					currentCollectionProp = null;
					return;
				}
				else
				{
					token += StringHelper.Dot + currentCollectionProp;
				}
			}
			else
			{
				string prop;
				if (collectionProps.TryGetValue(token.ToLowerInvariant(), out prop))
				{
					currentCollectionProp = prop;
					return;
				}
			}


			//handle <=, >=, !=, is not, not between, not in
			if (lastToken == null)
			{
				lastToken = token;
			}
			else
			{
				string doubleToken = (token.Length > 1) ?
				                     lastToken + ' ' + token :
				                     lastToken + token;
				if (operators.Contains(doubleToken.ToLowerInvariant()))
				{
					parser.Token(doubleToken, q);
					lastToken = null;
				}
				else
				{
					parser.Token(lastToken, q);
					lastToken = token;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public virtual void Start(QueryTranslator q)
		{
			quoted = false;
			parser.Start(q);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public virtual void End(QueryTranslator q)
		{
			if (lastToken != null)
			{
				parser.Token(lastToken, q);
			}
			parser.End(q);
			lastToken = null;
			currentCollectionProp = null;
		}
	}
}