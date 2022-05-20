namespace OrderService.GraphQL
{
    public record OrderTrackingData
    (
        string Latitude,
        string Longitude,
        int OrderId
    );
    
}
