using System;
using Oracle.DataAccess.Types;

namespace ozgurtek.framework.driver.oracle.NetTopologySuit.IO.Oracle.UdtBase
{
    public abstract class OracleArrayTypeFactoryBase<T> : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems) => new T[numElems];

        public Array CreateStatusArray(int numElems) => null;
    }
}
