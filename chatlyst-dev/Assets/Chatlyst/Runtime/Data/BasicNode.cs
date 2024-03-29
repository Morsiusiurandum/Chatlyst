﻿using Newtonsoft.Json;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Chatlyst.Runtime.Data
{
    public abstract class BasicNode
    {
        [JsonProperty]
        public readonly string Guid;
        public NodeType NodeType;
        public string   NextGuid;
        public Vector2  NodePos;

        protected BasicNode()
        {
            Guid     = System.Guid.NewGuid().ToString();
            NodeType = 0;
            NextGuid = null;
            NodePos  = Vector2.Zero;
        }

        public void SetRect(Rect rect)
        {
            NodePos.X = rect.position.x;
            NodePos.Y = rect.position.y;
        }

        public Rect GetRect()
        {
            return new Rect(NodePos.X, NodePos.Y, 1, 1);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        private bool Equals(BasicNode other)
        {
            return Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is BasicNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
