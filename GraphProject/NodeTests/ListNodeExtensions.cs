using GraphProject;

namespace NodeTests;

public static class ListNodeExtensions
{
    //Create ListNodes with random values, for data using HashCode
    public static ListNode CreateNodes(int count)
    {
        var head = new ListNode();

        var list = new List<ListNode>();

        for (var (i, node) = (0, head); i < count; i++, node = node.Next)
        {
            list.Add(node);
            if (i < count - 1)
            {
                node.Next = new ListNode
                {
                    Previous = node
                };
            }
        }
        
        for (int i = 0; i < count; i++)
        {
            list[i].Data = list[i].GetHashCode().ToString();

            if (i <= count / 2) //Add Random for half Nodes
            {
                list[i].Random = list[new Random().Next(0, list.Count - 1)];
            }
        }
        return head;
    }
}