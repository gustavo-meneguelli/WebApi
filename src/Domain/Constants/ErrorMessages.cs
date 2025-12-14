namespace Domain.Constants;

/// <summary>
/// Mensagens de erro centralizadas usadas em toda a aplicação.
/// Todas as mensagens estão em inglês - a localização é feita pelo front-end.
/// </summary>
public static class ErrorMessages
{
    // Geral
    public const string NotFound = "{0} not found.";
    public const string AlreadyExists = "A {0} with this {1} already exists.";
    public const string RequiredField = "The {0} field is required.";
    public const string InvalidOperation = "Invalid operation.";
    public const string InvalidOperationWithDetails = "Invalid operation: {0}.";
    public const string AccessDenied = "Access denied.";
    public const string UnhandledError = "An unhandled error occurred.";
    public const string InternalServerError = "An internal server error occurred. Please try again later.";
    
    // Validação - Tamanho
    public const string MinLength = "The {0} must be at least {1} characters.";
    public const string MaxLength = "The {0} must be at most {1} characters.";
    public const string LengthRange = "The {0} must be between {1} and {2} characters.";
    
    // Validação - Numérico
    public const string GreaterThanZero = "The {0} must be greater than zero.";
    public const string InvalidId = "Invalid {0} ID.";
    public const string MaxValue = "The {0} must be less than {1}.";
    public const string RangeValue = "The {0} must be between {1} and {2}.";
    public const string MaxQuantity = "Maximum quantity: {0} units.";
    
    // Validação - Formato
    public const string InvalidUrl = "The {0} must be a valid URL.";
    
    // Autenticação
    public const string InvalidCredentials = "Invalid username or password.";
    public const string UserIdNotFoundInClaims = "User ID not found in claims.";
    
    // Pedido
    public const string EmptyCart = "Cart is empty.";
    public const string OrderNotFound = "Order not found.";
    public const string OnlyPendingOrdersCanBeCancelled = "Only pending orders can be cancelled.";
    public const string OnlyPendingOrdersCanBeConfirmed = "Only pending orders can have their payment confirmed.";
    public const string OrderAlreadyProcessed = "This order has already been processed.";
    
    // Carrinho
    public const string CartNotFound = "Cart not found.";
    public const string CartItemNotFound = "Item not found in cart.";
    public const string ProductNotFound = "Product not found.";
    
    // Categoria
    public const string CategoryNotFound = "Category not found.";
    
    // Avaliação
    public const string ReviewNotFound = "Review not found.";
    public const string ProfanityDetected = "Your comment contains inappropriate words. Please revise your text.";
    public const string AlreadyReviewed = "You have already submitted your first purchase review.";
    public const string AlreadyReviewedProduct = "You have already reviewed this product.";
    public const string MustPurchaseToReview = "You can only review products you have purchased.";
    public const string NoPermissionToReview = "You do not have permission to review this order.";
    
    // Mensagens de Sucesso (usadas em operações que retornam string)
    public const string ItemRemovedFromCart = "Item removed successfully.";
    public const string CartCleared = "Cart cleared successfully.";
    
    // Fallbacks
    public const string ProductUnavailable = "Product unavailable.";
}

