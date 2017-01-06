using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinMaxTree
{
    public abstract class MinMaxNode<N, T, S>
        where N : MinMaxNode<N, T, S>
    {
        public T State { get; protected set; }

        public int? CurrentStateEval { get; set; }
        public int? MaxNext { get; set; }
        public int? MinNext { get; set; }

        public bool IsEnemyTurn { get; set; }
        public bool IsClosed { get; set; }

        protected LinkedList<Tuple<N, S>> _children;
        public IEnumerable<Tuple<N,S>> Children
        {
            get
            {
                _children = _children ?? Unfold();
                return _children;
            }
        }

        protected abstract LinkedList<Tuple<N,S>> Unfold();
    }

    public class MinMaxTree<N, T, S>
        where N : MinMaxNode<N, T, S>
    {
        public N Root { get; set; }
        public Func<T,int> Eval { get; set; }

        public void Unfold(int depth, bool ignoreClosed = false)
        {
            Unfold(depth, Root, ignoreClosed);
        }

        private void Unfold(int depth, N node, bool ignoreClosed)
        {
            if (depth == 0)
                return;
            if (ignoreClosed && node.IsClosed) return;

            foreach (var ch in node.Children)
                Unfold(depth - 1, ch.Item1, ignoreClosed);
        }

        public void Evaluate(int depth, bool ignoreClosed = false)
        {
            Root.CurrentStateEval =
                !Root.CurrentStateEval.HasValue ? Eval(Root.State) : Root.CurrentStateEval;
            Evaluate(depth, Root, ignoreClosed);
        }

        private void Evaluate(int depth, N node, bool ignoreClosed)
        {
            if (depth == 0)
                return;
            if (ignoreClosed && node.IsClosed) return;

            node.CurrentStateEval =
                !node.CurrentStateEval.HasValue ? Eval(node.State) : node.CurrentStateEval;

            foreach (var ch in node.Children)
                Evaluate(depth - 1, ch.Item1, ignoreClosed);
        }
    }

}
