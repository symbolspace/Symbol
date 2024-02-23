namespace Symbol.Tests;

[TestClass()]
public class EntityPropertySetterTests
{
    [TestMethod()]
    public void SetValuesTest()
    {
        var entity = new UserInfo();
        //匿名类方式
        {
            EntityPropertySetter.SetValues(entity, new
            {
                Name = "张三"
            });
            Assert.AreEqual("张三", entity.Name);
        }
        //JSON文本
        {
            var id = Guid.Parse("{30800F56-C078-498C-8968-7B92C0890F2F}");
            EntityPropertySetter.SetValues(entity, "{ 'AttributeID': '30800F56-C078-498C-8968-7B92C0890F2F'  }");
            Assert.AreEqual(id, entity.ID);
        }
        //字典对象
        {
            var values = new Dictionary<string, object>()
            {
                { "AttributeName", "测试" }
            };
            EntityPropertySetter.SetValues(entity, values);
            Assert.AreEqual("测试", entity.Name);
        }
    }

    abstract class BaseInfo
    {
        public abstract Guid ID { get; }
        public abstract string Name { get; }
    }
    class UserInfo : BaseInfo
    {
        private Guid _id;
        private string _name;

        public override Guid ID { get { return _id; } }
        public override string Name { get { return _name; } }

        [PropertySetMethod(nameof(ID))]
        [PropertySetMethod("AttributeID")]
        void SetID(Guid value) { _id = value; }
        [PropertySetMethod(nameof(Name))]
        [PropertySetMethod("AttributeName")]
        void SetName(string value) { _name = value; }
    }
}