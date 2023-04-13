using GraphProject;
using Xunit;

namespace NodeTests;

public class JohnSmithSerializerTest
{
    [Theory(DisplayName = "Check Serialization/Deserialization")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Check_Serialization_And_Deserialization(int nodesCount)
    {
        var headNode = ListNodeExtensions.CreateNodes(nodesCount);
        var serializer = new JohnSmithSerializer();

        using var writer = new MemoryStream();
        await serializer.Serialize(headNode, writer);

        using var reader = new MemoryStream(writer.ToArray());
        var deserializedHead = await serializer.Deserialize(reader);

        var deserializedCount = 0;

        for (var newNode = deserializedHead; newNode != null; newNode = newNode.Next)
        {
            deserializedCount++;
        }

        Assert.Equal(deserializedCount, nodesCount);

        for (var (sNode, dNode, index) = (headNode, deserializedHead, 0); sNode != null; sNode = sNode.Next, dNode = dNode.Next, index++)
        {
            if (index < nodesCount-1)
                Assert.NotNull(dNode.Next);
            if (index > 0)
                Assert.NotNull(dNode.Previous);

            Assert.Equal(sNode.Data, dNode.Data);
            Assert.Equal(sNode.Next?.Data, dNode.Next?.Data);
            Assert.Equal(sNode.Previous?.Data, dNode.Previous?.Data);
            Assert.Equal(sNode.Random?.Data, dNode.Random?.Data);
        }
    }

    [Theory(DisplayName = "Check DeepCopy")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Check_DeepCopy(int nodesCount)
    {
        var headNode = ListNodeExtensions.CreateNodes(nodesCount);
        var serializer = new JohnSmithSerializer();

        var newHeadNode = await serializer.DeepCopy(headNode);

        Assert.NotEqual(newHeadNode, headNode);

        for (var (oldNode, newNode, index) = (headNode, newNode: newHeadNode, 0); oldNode != null; oldNode = oldNode.Next, newNode = newNode.Next, index++)
        {
            if (index < nodesCount - 1)
            {
                Assert.NotNull(newNode.Next);
                Assert.NotEqual(newNode.Next, oldNode.Next);
            }

            if (index > 0)
            {
                Assert.NotNull(newNode.Previous);
                Assert.NotEqual(newNode.Previous, oldNode.Previous);
            }

            Assert.Equal(oldNode.Data, newNode.Data);
            Assert.Equal(oldNode.Next?.Data, newNode.Next?.Data);
            Assert.Equal(oldNode.Previous?.Data, newNode.Previous?.Data);
            Assert.Equal(oldNode.Random?.Data, newNode.Random?.Data);

            if(oldNode.Random != null)
                Assert.NotEqual(oldNode.Random, newNode.Random);
        }
    }
}