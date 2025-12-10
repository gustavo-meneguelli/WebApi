namespace Domain.Enums;

public enum OrderStatus
{
    Pending = 1,    // Carrinho fechado, aguardando pagamento
    Paid = 2,       // Pagamento confirmado
    Shipped = 3,    // Saiu para entrega
    Delivered = 4,  // Cliente recebeu
    Cancelled = 5   // Cancelado 
}