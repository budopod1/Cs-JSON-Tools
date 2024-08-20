using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class BJSONEnv {
    readonly List<string> strings = [];

    BJSONEnv() {}

    public static IEnumerable<byte> ToVWInt(int val) {
        List<byte> bytes = [];
        do {
            byte b = (byte)(0x7F & val);
            val >>= 7;
            if (val != 0) b |= 0x80;
            bytes.Add(b);
        } while (val != 0);
        return bytes;
    }

    public static int FromVWInt(BinaryReader bytes) {
        int result = 0;
        byte curByte;
        int i = 0;
        do {
            curByte = ReadByte(bytes);
            result += (curByte & 0x7F) << i;
            i += 7;
        } while ((curByte & 0x80) != 0);
        return result;
    }

    public static byte ReadByte(BinaryReader bytes) {
        try {
            return bytes.ReadByte();
        } catch (EndOfStreamException) {
            throw new InvalidBJSONException("Expected additional byte, found EOF");
        }
    }

    public static byte[] ReadN(BinaryReader bytes, int n) {
        byte[] result = bytes.ReadBytes(n);
        if (result.Length < n) {
            throw new InvalidBJSONException("Expected additional bytes, found EOF");
        }
        return result;
    }

    public IEnumerable<byte> RegisterString(string str) {
        int idx = strings.IndexOf(str);
        if (idx == -1) {
            idx = strings.Count;
            strings.Add(str);
        }
        return ToVWInt(idx);
    }

    public string GetString(BinaryReader bytes) {
        int i = FromVWInt(bytes);
        if (i >= strings.Count) {
            throw new InvalidBJSONException($"{i} is not an acceptable string index, only {strings.Count} strings are specified in the string table");
        }
        return strings[i];
    }

    public IEnumerable<byte> CreateStringTable() {
        IEnumerable<byte> result = ToVWInt(strings.Count);
        foreach (string str in strings) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            result = result.Concat(ToVWInt(bytes.Length));
            result = result.Concat(bytes);
        }
        return result;
    }

    public void ReadStringTable(BinaryReader bytes) {
        int strCount = FromVWInt(bytes);
        for (int i = 0; i < strCount; i++) {
            int strLen = FromVWInt(bytes);
            strings.Add(Encoding.UTF8.GetString(ReadN(bytes, strLen)));
        }
    }

    public JSONBool ReadBJSONBool(BinaryReader bytes) {
        return new JSONBool(ReadByte(bytes) != 0);
    }

    public JSONDouble ReadBJSONDouble(BinaryReader bytes) {
        byte[] raw = ReadN(bytes, 8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(raw);
        return new JSONDouble(BitConverter.ToDouble(raw));
    }

    public JSONInt ReadBJSONInt(BinaryReader bytes) {
        return new JSONInt(FromVWInt(bytes));
    }

    public JSONList ReadBJSONList(BinaryReader bytes) {
        JSONList result = [];
        int count = FromVWInt(bytes);
        if (count == 0) return result;
        byte ID = ReadByte(bytes);
        for (int i = 0; i < count; i++) {
            result.Add(ReadBJSONValue(ID, bytes));
        }
        return result;
    }

    public JSONList ReadBJSONMultitypedList(BinaryReader bytes) {
        JSONList result = [];
        int count = FromVWInt(bytes);
        for (int i = 0; i < count; i++) {
            byte ID = ReadByte(bytes);
            result.Add(ReadBJSONValue(ID, bytes));
        }
        return result;
    }

    public JSONObject ReadBJSONObject(BinaryReader bytes) {
        JSONObject result = [];
        int count = FromVWInt(bytes);
        for (int i = 0; i < count; i++) {
            string key = GetString(bytes);
            byte ID = ReadByte(bytes);
            IJSONValue value = ReadBJSONValue(ID, bytes);
            result[key] = value;
        }
        return result;
    }

    public JSONString ReadBJSONString(BinaryReader bytes) {
        return new JSONString(GetString(bytes));
    }

    public IJSONValue ReadBJSONValue(byte ID, BinaryReader bytes) {
        switch (ID) {
        case 1:
            return ReadBJSONBool(bytes);
        case 2:
            return ReadBJSONDouble(bytes);
        case 3:
            return ReadBJSONInt(bytes);
        case 4:
            return ReadBJSONList(bytes);
        case 5:
            return new JSONNull();
        case 6:
            return ReadBJSONObject(bytes);
        case 7:
            return ReadBJSONString(bytes);
        case 8:
            return ReadBJSONMultitypedList(bytes);
        default:
            throw new InvalidBJSONException($"{ID} is not a valid BJSON type ID");
        }
    }

    public static IEnumerable<byte> Serialize(IJSONValue value) {
        BJSONEnv env = new();
        IEnumerable<byte> magicByte = new List<byte> {0x42};
        IEnumerable<byte> dataBytes = value.ToBJSON(env);
        IEnumerable<byte> stringTable = env.CreateStringTable();
        return magicByte.Concat(stringTable).Concat(value.ID).Concat(dataBytes);
    }

    public static void WriteFile(string path, IJSONValue value) {
        byte[] bytes = Serialize(value).ToArray();
        using (FileStream file = new(path, FileMode.Create)) {
            file.Write(bytes, 0, bytes.Length);
        }
    }

    public static IJSONValue Deserialize(BinaryReader bytes) {
        BJSONEnv env = new();
        if (ReadByte(bytes) != 0x42) {
            throw new InvalidBJSONException("BJSON files must start with 0x42");
        }

        env.ReadStringTable(bytes);

        byte ID = ReadByte(bytes);
        return env.ReadBJSONValue(ID, bytes);
    }
}
