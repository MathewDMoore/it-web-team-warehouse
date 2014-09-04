using ApplicationSource.Models;
using AutoMapper;
using Domain;

namespace ApplicationSource
{
    public static class DomainModelMapper
    {
        public static void Initialize()
        {
            Mapper.CreateMap<Delivery, OrderDeliveryModel>();
            Mapper.CreateMap<SerialNumberItem, DeliveryOrderItemModel>()
                .ForMember(d => d.Verified, s => s.MapFrom(v => v.IsVerified));

        }

        public static TDestination Map<TSource, TDestination>(this TSource value)
        {
            return Mapper.Map<TSource, TDestination>(value);
        }
    }
}