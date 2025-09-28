namespace InventorySystem_Domain;

public class PaymentHistory
{
    private PaymentHistory() { }
    private PaymentHistory(int _OrderId, int _CustomerId, decimal _AmountPaid, string? _PaymentMethod, string _TransactionRefNo,
        decimal _BalanceRemainingToPay, int _CreatedBy)
    {
        OrderId = _OrderId;
        CustomerId = _CustomerId;
        AmountPaid = _AmountPaid;
        PaymentMethod = _PaymentMethod;
        TransactionRefNo = _TransactionRefNo;
        BalanceRemainingToPay = _BalanceRemainingToPay;
        CreatedBy = _CreatedBy;
    }
    public int PaymentHistoryId { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentAt { get; set; } = DateTime.UtcNow;
    public string? PaymentMethod { get; set; }
    public string? TransactionRefNo { get; set; }
    public decimal BalanceRemainingToPay { get; set; }
    public int CreatedBy { get; set; }

    // Navigation Properties
    public Order Order { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;

    public static PaymentHistory Create(int _OrderId, int _CustomerId, decimal _AmountPaid, string? _PaymentMethod, string _TransactionRefNo,
        decimal _BalanceRemainingToPay, int _CreatedBy)
    {
        return new PaymentHistory(_OrderId, _CustomerId, _AmountPaid, _PaymentMethod, _TransactionRefNo, _BalanceRemainingToPay, _CreatedBy);
    }
}

