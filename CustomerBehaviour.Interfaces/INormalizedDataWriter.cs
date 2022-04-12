using CustomerBehaviour.Definitions;

namespace CustomerBehaviour.Interfaces
{
    public interface INormalizedDataWriter
    {
        public void WriteNormalizedDataToFile(CustomersBehaviourSnapshot snapshot);
    }
}
