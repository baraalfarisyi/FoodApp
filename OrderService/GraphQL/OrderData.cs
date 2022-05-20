namespace OrderService.GraphQL
{
    public record OrderData
    (
        int courierId,
        List<OrderDetailData> OrderDetailDatas
    );
    
}
