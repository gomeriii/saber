using System.Text;

namespace GraphProject;

//Specify your class\file name and complete implementation.
public class JohnSmithSerializer : IListSerializer
{
    //the constructor with no parameters is required and no other constructors can be used.
    public JohnSmithSerializer()
    {
        //...
    }

    public Task<ListNode> DeepCopy(ListNode head)
    {
        throw new NotImplementedException();
    }

    public Task<ListNode> Deserialize(Stream s)
    {
        var randNodes = new Dictionary<string, ListNode>();
        var previousNodes = new Dictionary<string, ListNode>();
        using var reader = new BinaryReader(s, Encoding.UTF8, false);

        ListNode current = null;

        while (reader.PeekChar() != -1)
        {
            var data = reader.ReadString();
            var randData = reader.ReadString();

            if (current == null)
            {
                current = new ListNode() { Data = data };
            }
            else
            {
                current.Next = new ListNode() { Data = data, Previous = current };
                current = current.Next;

                var isSuccessRandNode = randNodes.TryGetValue(data, out var parentRandNode);
                if (isSuccessRandNode)
                {
                    parentRandNode.Random = current;
                }
                else
                {
                    previousNodes.Add(data, current);
                }

                var isSuccessPrevNode = previousNodes.TryGetValue(randData, out var prevNode);
                if (isSuccessPrevNode)
                {
                    current.Random = prevNode;
                }
                else
                {
                    randNodes.Add(randData, current);
                }
            }
        }

        return Task.FromResult(randNodes.First().Value);
    }

    public Task Serialize(ListNode head, Stream s)
    {
        var currentNode = head;

        using var writer = new BinaryWriter(s, Encoding.UTF8, false);

        while (currentNode.Next != null)
        {
            writer.Write(currentNode.Data);
            writer.Write(currentNode.Random.Data);

            currentNode = currentNode.Next;
        }

        return Task.CompletedTask;
    }
}