using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.Core.SSH
{
    public class NodeBase
    {
        public SSHAuthMdl[] nodes { get; set; }

        public NodeBase(params SSHAuthMdl[] _nodes)
        {
            nodes = _nodes;
        }

        public NodeBase(params string[] _nodeIds)
        {
            nodes = _nodeIds.Select(q=> new SSHAuthMdl(q, true)).ToArray();
        }

        public NodeBase(params Node[] _nodes)
        {
            nodes = _nodes.Select(q => new SSHAuthMdl(q)).ToArray();
        }
        public NodeBase(IEnumerable<Node> _nodes)
        {
            nodes = _nodes.Select(q => new SSHAuthMdl(q)).ToArray();
        }

        public NodeBase(IEnumerable<string> _nodeIds)
        {
            nodes = _nodeIds.Select(q => new SSHAuthMdl(q, true)).ToArray();
        }


    }
}