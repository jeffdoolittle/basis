using System;
using System.Data;

namespace Basis.Resource
{
    public class AdaptedDataReader : IDataReader
    {
        private readonly IDbCommand _command;
        private readonly IDataReader _innerReader;

        public AdaptedDataReader(IDbCommand command, IDataReader innerReader)
        {
            _command = command;
            _innerReader = innerReader;
        }

        public void Dispose()
        {
            _innerReader?.Dispose();
            _command?.Dispose();
        }

        public string GetName(int i)
        {
            return _innerReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _innerReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _innerReader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return _innerReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _innerReader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return _innerReader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return _innerReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _innerReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _innerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _innerReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _innerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _innerReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _innerReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _innerReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _innerReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _innerReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _innerReader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _innerReader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _innerReader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _innerReader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _innerReader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _innerReader.IsDBNull(i);
        }

        public int FieldCount => _innerReader.FieldCount;

        object IDataRecord.this[int i] => _innerReader[i];

        object IDataRecord.this[string name] => _innerReader[name];

        public void Close()
        {
            _innerReader.Close();
        }

        public DataTable GetSchemaTable()
        {
            return _innerReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return _innerReader.NextResult();
        }

        public bool Read()
        {
            return _innerReader.Read();
        }

        public int Depth => _innerReader.Depth;
        public bool IsClosed => _innerReader.IsClosed;
        public int RecordsAffected => _innerReader.RecordsAffected;
    }
}
