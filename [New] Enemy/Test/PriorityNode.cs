using System;

public class PriorityNode : IComparable<PriorityNode>
{
    public float priority;
    public NavNode node;

    public PriorityNode(float fScore, NavNode node)
    {
        this.priority = fScore;
        this.node = node;
    }

    public int CompareTo(PriorityNode other)
    {
        return priority.CompareTo(other.priority);
    }
}