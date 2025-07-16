//using System;
//using System.Data;
//using AutoMapper;

//namespace Ncms.Bl.Mappers
//{
//    public class DataRowToEntity<T> : BaseMapper<DataRow, T>
//    {
//        public override T Map(DataRow source)
//        {
//            Mapper.CreateMap<DataRow, T>().ForAllMembers(m => m.ResolveUsing<CustomResolver>());
//            return Mapper.Map<T>(source);
//        }

//        public override DataRow MapInverse(T source)
//        {
//            throw new NotImplementedException();
//        }

//        public class CustomResolver : IValueResolver
//        {
//            public ResolutionResult Resolve(ResolutionResult source)
//            {
//                var row = source.Context.SourceValue as DataRow;
//                if (!row.Table.Columns.Contains(source.Context.MemberName))
//                {
//                    return source.Ignore();
//                }
//                if (row.IsNull(source.Context.MemberName))
//                {
//                    return source.Ignore();
//                }

//                var value = row[source.Context.MemberName];
//                var property = typeof(T).GetProperty(source.Context.MemberName);
//                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

//                    object safeValue = (value == null) ? null : Convert.ChangeType(value, t);

//                   return source.New(Convert.ChangeType(safeValue, t));
//            }
//        }
//    }
//}
