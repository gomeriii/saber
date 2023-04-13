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
        var nodeIds = new Dictionary<ListNode, int>();
        var newHead = new ListNode();

        var count = 0;

        for (var (current, newNode, key) = (head, newHead, 0); current != null; current = current.Next, newNode = newNode.Next, key++)
        {
            nodeIds.Add(current, key);

            if (current.Next != null)
            {
                newNode.Next = new ListNode
                {
                    Previous = newNode
                };
            }

            newNode.Data = current.Data;

            count++;
        }

        var newListNodes = new List<ListNode>(count);

        for (var newNode = newHead; newNode != null; newNode = newNode.Next)
        {
            newListNodes.Add(newNode);
        }

        for (var (current, newNode) = (head, newHead); current != null; current = current.Next, newNode = newNode.Next)
        {
            if (nodeIds[current] >= 0)
                if (current.Random != null)
                    newNode.Random = newListNodes[nodeIds[current.Random]];
        }

        return Task.FromResult(newHead);
    }

    public Task<ListNode> Deserialize(Stream s)
    {
        var readData = new Dictionary<int, (string data, int randIndex)>();
        var count = 0;

        using var reader = new BinaryReader(s, Encoding.UTF8, false);
        while (reader.PeekChar() != -1)
        {
            var data = reader.ReadString();
            var randIndex = reader.ReadInt32();

            readData.Add(count++, (data, randIndex));
        }

        var head = new ListNode();
        var listNodes = new List<ListNode>(count);

        for (var (current, key) = (head, 0); current != null && key < count; current = current.Next, key++)
        {
            listNodes.Add(current);
            current.Data = readData[key].data;

            if (key < count - 1)
            {
                current.Next = new ListNode
                {
                    Previous = current
                };
            }
        }

        for (var (current, key) = (head, 0); current != null && key < count; current = current.Next, key++)
        {
            if (readData[key].randIndex >= 0)
                current.Random = listNodes[readData[key].randIndex];
        }

        return Task.FromResult(head);
    }

    public Task Serialize(ListNode head, Stream s)
    {
        var nodeIds = new Dictionary<ListNode, int>();

        for (var (current, index) = (head, 0); current != null; current = current.Next, index++)
        {
            nodeIds.Add(current, index);
        }

        using var writer = new BinaryWriter(s, Encoding.UTF8, false);
        for (var current = head; current != null; current = current.Next)
        {
            writer.Write(current.Data);
            writer.Write(current.Random != null ? nodeIds[current.Random] : -1);
        }

        return Task.CompletedTask;
    }
}