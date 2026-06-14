namespace WorkshopFlow.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string itemCode, decimal required, decimal available)
            : base($"Insufficient stock for item {itemCode}. " +
                   $"Required: {required}, Available: {available}")
        {
        }
    }
}
