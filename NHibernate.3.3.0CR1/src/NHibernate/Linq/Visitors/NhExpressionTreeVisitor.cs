using System;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class NhExpressionTreeVisitor : ExpressionTreeVisitor
	{
		public override Expression VisitExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			switch ((NhExpressionType)expression.NodeType)
			{
				case NhExpressionType.Average:
				case NhExpressionType.Min:
				case NhExpressionType.Max:
				case NhExpressionType.Sum:
				case NhExpressionType.Count:
				case NhExpressionType.Distinct:
					return VisitNhAggregate((NhAggregatedExpression)expression);
				case NhExpressionType.New:
					return VisitNhNew((NhNewExpression)expression);
				case NhExpressionType.Star:
					return VisitNhStar((NhStarExpression)expression);
			}

			return base.VisitExpression(expression);
		}

		protected virtual Expression VisitNhStar(NhStarExpression expression)
		{
			var newExpression = VisitExpression(expression.Expression);

			return newExpression != expression.Expression ? new NhStarExpression(newExpression) : expression;
		}

		protected virtual Expression VisitNhNew(NhNewExpression expression)
		{
			var arguments = VisitAndConvert(expression.Arguments, "VisitNhNew");

			return arguments != expression.Arguments ? new NhNewExpression(expression.Members, arguments) : expression;
		}

		protected virtual Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			switch ((NhExpressionType)expression.NodeType)
			{
				case NhExpressionType.Average:
					return VisitNhAverage((NhAverageExpression)expression);
				case NhExpressionType.Min:
					return VisitNhMin((NhMinExpression)expression);
				case NhExpressionType.Max:
					return VisitNhMax((NhMaxExpression)expression);
				case NhExpressionType.Sum:
					return VisitNhSum((NhSumExpression)expression);
				case NhExpressionType.Count:
					return VisitNhCount((NhCountExpression)expression);
				case NhExpressionType.Distinct:
					return VisitNhDistinct((NhDistinctExpression)expression);
				default:
					throw new ArgumentException();
			}
		}

		protected virtual Expression VisitNhDistinct(NhDistinctExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? new NhDistinctExpression(nx) : expression;
		}

		protected virtual Expression VisitNhCount(NhCountExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? expression.CreateNew(nx) : expression;
		}

		protected virtual Expression VisitNhSum(NhSumExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? new NhSumExpression(nx) : expression;
		}

		protected virtual Expression VisitNhMax(NhMaxExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? new NhMaxExpression(nx) : expression;
		}

		protected virtual Expression VisitNhMin(NhMinExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? new NhMinExpression(nx) : expression;
		}

		protected virtual Expression VisitNhAverage(NhAverageExpression expression)
		{
			Expression nx = VisitExpression(expression.Expression);

			return nx != expression.Expression ? new NhAverageExpression(nx) : expression;
		}
	}
}