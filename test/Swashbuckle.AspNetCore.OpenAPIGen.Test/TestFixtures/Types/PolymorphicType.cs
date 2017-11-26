namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public abstract class PolymorphicType
    {
        public string BaseProperty { get; set; }
    }

    public class SubType : PolymorphicType
    {
        public int SubTypeProperty { get; set; }
    }
}