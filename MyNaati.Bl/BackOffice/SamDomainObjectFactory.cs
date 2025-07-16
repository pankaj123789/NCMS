using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;

namespace MyNaati.Bl.BackOffice
{
    //public interface ISamDomainObjectFactory
    //{
    //    T Construct<T>() where T : LegacyEntityBase;
    //}

    //public class SamDomainObjectFactory : ISamDomainObjectFactory
    //{
    //    private ISamKeyRepository mSamKeyRepository;

    //    private Dictionary<string, string> mDomainToTableNameMappings;
    //    private Dictionary<string, string> DomainToTableNameMappings
    //    {
    //        get
    //        {
    //            // todo make this hook-in to nhibernate mappings to correct the discrepancy between 
    //            //the name of the table and the name of the domain class
    //            //the purpose of this piece of code is to solve this discrepancy problem
    //            if (mDomainToTableNameMappings == null)
    //            {
    //                mDomainToTableNameMappings = new Dictionary<string, string>();

    //                mDomainToTableNameMappings["NaatiEntity"] = "Entity";
    //            }
    //            return mDomainToTableNameMappings;
    //        }
    //    }


    //    public SamDomainObjectFactory(ISamKeyRepository keyRepository)
    //    {
    //        mSamKeyRepository = keyRepository;
    //    }

    //    public T Construct<T>() where T : LegacyEntityBase
    //    {
    //        int key = GetKey<T>();
    //        return (T)Activator.CreateInstance(typeof(T), key);
    //    }

    //    private int GetKey<T>()
    //    {
    //        var typeName = typeof(T).Name;

    //        if (DomainToTableNameMappings.ContainsKey(typeName))
    //        {
    //            typeName = DomainToTableNameMappings[typeName];
    //        }

    //        return GetKeyFromName(typeName);
    //    }

    //    private static Dictionary<string, List<int>> mKeyDictionary = new Dictionary<string, List<int>>();
    //    private int GetKeyFromName(string samTableName)
    //    {
           
    //        //We don't want multiple threads getting the same keys
    //        lock (mKeyDictionary)
    //        {
    //            if (!mKeyDictionary.ContainsKey(samTableName))
    //                mKeyDictionary.Add(samTableName, new List<int>());
              
    //            var keys = mKeyDictionary[samTableName];
    //            if (keys.Count == 0)
    //                keys.AddRange(mSamKeyRepository.AllocateKeys(samTableName));

    //            int newKey = keys.First();
    //            keys.Remove(newKey);
    //            return newKey;
    //        }
    //    }
    //}
}
