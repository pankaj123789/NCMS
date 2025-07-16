//using System;
//using System.Linq;
//using AutoMapper;

//namespace MyNaati.Ui.Common
//{
//	public static class MapperExtensions
//	{
//		public static void InheritMappingFromBaseType<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
//		{
//			//mappingExpression.IncludeBase<>()
//		}

//		private static bool NotAlreadyMapped(Type sourceType, Type desitnationType, ResolutionContext r)
//		{
//			return !r.IsSourceValueNull &&
//				   Mapper.FindTypeMapFor(sourceType, desitnationType).GetPropertyMaps().Where(
//					   m => m.DestinationProperty.Name.Equals(r.MemberName)).Select(y => !y.IsMapped()).All(b => b);
//		}
//	}
//}