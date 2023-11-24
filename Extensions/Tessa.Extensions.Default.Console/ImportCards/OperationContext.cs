namespace Tessa.Extensions.Default.Console.ImportCards
{
    public class OperationContext
    {
        public bool CanDeleteExistentCards { get; set; }

        public bool IgnoreExistentCards { get; set; }

        public bool IgnoreRepairMessages { get; set; }

        public string Source { get; set; }
    }
}
