using System;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
    /*
	[CLSCompliant(false)]
    public class HqlSqlWalkerTreeNodeStream : ITreeAdaptor
	{

		/// <summary>
		/// Insert a new node into both the Tree and the Node Array. Add DOWN and UP nodes if needed.
		/// </summary>
		/// <param name="parent">The parent node</param>
		/// <param name="child">The child node</param>
		public void InsertChild(IASTNode parent, IASTNode child)
		{
			if (child.ChildCount > 0)
			{
				throw new InvalidOperationException("Currently do not support adding nodes with children");
			}
            
			int parentIndex = nodes.IndexOf(parent);
			int numberOfChildNodes = NumberOfChildNodes(parentIndex);
			int insertPoint;
			
			if (numberOfChildNodes == 0)
			{
				insertPoint = parentIndex + 1;  // We want to insert immediately after the parent
				nodes.Insert(insertPoint, down);
				insertPoint++;  // We've just added a new node
			}
			else
			{
				insertPoint = parentIndex + numberOfChildNodes;
			}

			parent.AddChild(child);
			nodes.Insert(insertPoint, child);
			insertPoint++;

			if (numberOfChildNodes == 0)
			{
				nodes.Insert(insertPoint, up);
			}
		}

		/// <summary>
		/// Count the number of child nodes (including DOWNs and UPs) of a parent node
		/// </summary>
		/// <param name="parentIndex">The index of the parent in the node array</param>
		/// <returns>The number of child nodes</returns>
		int NumberOfChildNodes(int parentIndex)
		{
			if (nodes.Count -1 == parentIndex)
			{
				// We are at the end
				return 0;
			}
			
			if (nodes[parentIndex + 1] != down)
			{
				// Next node is not a DOWN node, so we have no children
				return 0;
			}

			// Count the DOWNs & UPs
			int downCount = 0;
			int index = 1;
			do
			{
				if (nodes[parentIndex + index] == down)
				{
					downCount++;
				}
				else if (nodes[parentIndex + index] == up)
				{
					downCount--;
				}

				index++;
				
			} while (downCount > 0);

			return index - 1;
		}

        public object Create(Antlr.Runtime.IToken payload)
        {
            throw new NotImplementedException();
        }

        public object Create(int tokenType, Antlr.Runtime.IToken fromToken)
        {
            throw new NotImplementedException();
        }

        public object Create(int tokenType, Antlr.Runtime.IToken fromToken, string text)
        {
            throw new NotImplementedException();
        }

        public object Create(Antlr.Runtime.IToken fromToken, string text)
        {
            throw new NotImplementedException();
        }

        public object Create(int tokenType, string text)
        {
            throw new NotImplementedException();
        }

        public object DupNode(object treeNode)
        {
            throw new NotImplementedException();
        }

        public object DupNode(int type, object treeNode)
        {
            throw new NotImplementedException();
        }

        public object DupNode(object treeNode, string text)
        {
            throw new NotImplementedException();
        }

        public object DupNode(int type, object treeNode, string text)
        {
            throw new NotImplementedException();
        }

        public object DupTree(object tree)
        {
            throw new NotImplementedException();
        }

        public object Nil()
        {
            throw new NotImplementedException();
        }

        public object ErrorNode(Antlr.Runtime.ITokenStream input, Antlr.Runtime.IToken start, Antlr.Runtime.IToken stop, Antlr.Runtime.RecognitionException e)
        {
            throw new NotImplementedException();
        }

        public bool IsNil(object tree)
        {
            throw new NotImplementedException();
        }

        public void AddChild(object t, object child)
        {
            throw new NotImplementedException();
        }

        public object BecomeRoot(object newRoot, object oldRoot)
        {
            throw new NotImplementedException();
        }

        public object RulePostProcessing(object root)
        {
            throw new NotImplementedException();
        }

        public int GetUniqueID(object node)
        {
            throw new NotImplementedException();
        }

        public object BecomeRoot(Antlr.Runtime.IToken newRoot, object oldRoot)
        {
            throw new NotImplementedException();
        }

        public int GetType(object t)
        {
            throw new NotImplementedException();
        }

        public void SetType(object t, int type)
        {
            throw new NotImplementedException();
        }

        public string GetText(object t)
        {
            throw new NotImplementedException();
        }

        public void SetText(object t, string text)
        {
            throw new NotImplementedException();
        }

        public Antlr.Runtime.IToken GetToken(object t)
        {
            throw new NotImplementedException();
        }

        public void SetTokenBoundaries(object t, Antlr.Runtime.IToken startToken, Antlr.Runtime.IToken stopToken)
        {
            throw new NotImplementedException();
        }

        public int GetTokenStartIndex(object t)
        {
            throw new NotImplementedException();
        }

        public int GetTokenStopIndex(object t)
        {
            throw new NotImplementedException();
        }

        public object GetChild(object t, int i)
        {
            throw new NotImplementedException();
        }

        public void SetChild(object t, int i, object child)
        {
            throw new NotImplementedException();
        }

        public object DeleteChild(object t, int i)
        {
            throw new NotImplementedException();
        }

        public int GetChildCount(object t)
        {
            throw new NotImplementedException();
        }

        public object GetParent(object t)
        {
            throw new NotImplementedException();
        }

        public void SetParent(object t, object parent)
        {
            throw new NotImplementedException();
        }

        public int GetChildIndex(object t)
        {
            throw new NotImplementedException();
        }

        public void SetChildIndex(object t, int index)
        {
            throw new NotImplementedException();
        }

        public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
        {
            throw new NotImplementedException();
        }
    }*/
}