using Inventor;

namespace Inventor_Revisioner
{
    public class Assembly : InventorObject
    {
        public Assembly(Application inventorObject) : base(inventorObject)
        {
        }

        protected override string FileExtension { get; set; } = ".iam";
        protected override string ObjectType { get; set; } = "Baugruppe";
    }
}