using ApplicationSource.Models;
using AutoMapper;
using Domain;

namespace ApplicationSource
{
    public static class DomainModelMapper
    {
        public static void Initialize()
        {
            Mapper.CreateMap<Address, AddressModel>();
            Mapper.CreateMap<Delivery, OrderDeliveryModel>();
            Mapper.CreateMap<SerialNumberItem, DeliveryOrderItemModel>();

        }

        public static TDestination Map<TSource, TDestination>(this TSource value)
        {
            return Mapper.Map<TSource, TDestination>(value);
        }
    }
}