﻿using ApplicationSource.Models;
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
                .ForMember(d => d.SerialCode, s => s.MapFrom(v => v.SerialCode??""))
                .ForMember(d => d.ScannedBy, s => s.MapFrom(v => v.ScannedBy??""))
                .ForMember(d => d.Verified, s => s.MapFrom(v => v.IsVerified))
                .ForMember(d => d.ItemCode, s => s.MapFrom(v => !string.IsNullOrEmpty(v.ItemCode)?v.ItemCode.Trim():""))
                .ForMember(d => d.RealItemCode, s => s.MapFrom(v => !string.IsNullOrEmpty(v.RealItemCode) ? v.RealItemCode.Trim() : ""));
        }

        public static TDestination Map<TSource, TDestination>(this TSource value)
        {
            return Mapper.Map<TSource, TDestination>(value);
        }
    }
}