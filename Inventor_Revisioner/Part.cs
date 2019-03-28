using Inventor;

namespace Inventor_Revisioner
{
    public class Part : InventorObject
    {
        public Part(Application inventorObject) : base(inventorObject)
        {
        }

        protected override string FileExtension { get; set; } = ".ipt";
        protected override string ObjectType { get; set; } = "Bauteil";
    }
}