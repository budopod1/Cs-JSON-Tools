using System.Text;

public class BinJSONEnv {
    readonly List<string> strings = [];

    BinJSONEnv() {}

    public static IEnumerable<byte> ToVWInt(uint val) {
        List<byte> bytes = [];
        do {
            byte b = (byte)(0x7F & val);
            val >>= 7;
            if (val != 0) b |= 0x80;
            bytes.Add(b);
        } while (val != 0);
        return bytes;
    }

    public static uint FromVWInt(BinaryReader bytes) {
        uint result = 0;
        byte curByte;
        uint i = 0;
        do {
            curByte = ReadByte(bytes);
            result += (uint)((curByte & 0x7F) << (int)i);
            i += 7;
        } while ((curByte & 0x80) != 0);
        return result;
    }

    public static byte ReadByte(BinaryReader bytes) {
        try {
            return bytes.ReadByte();
        } catch (EndOfStreamException) {
            throw new InvalidBinJSONException("Expected additional byte, found EOF");
        }
    }

    public static byte[] ReadN(BinaryReader bytes, uint n) {
        byte[] result = bytes.ReadBytes((int)n);
        if (result.Length < n) {
            throw new InvalidBinJSONException("Expected additional bytes, found EOF");
        }
        return result;
    }

    public IEnumerable<byte> RegisterString(string str) {
        int idx = strings.IndexOf(str);
        if (idx == -1) {
            idx = strings.Count;
            strings.Add(str);
        }
        return ToVWInt((uint)idx);
    }

    public string GetString(BinaryReader bytes) {
        uint i = FromVWInt(bytes);
        if (i >= strings.Count) {
            throw new InvalidBinJSONException($"{i} is not an acceptable string index, only {strings.Count} strings are specified in the string table");
        }
        return strings[(int)i];
    }

    public IEnumerable<byte> CreateStringTable() {
        IEnumerable<byte> result = ToVWInt((uint)strings.Count);
        foreach (string str in strings) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            result = result.Concat(ToVWInt((uint)bytes.Length));
            result = result.Concat(bytes);
        }
        return result;
    }

    public void ReadStringTable(BinaryReader bytes) {
        uint strCount = FromVWInt(bytes);
        for (int i = 0; i < strCount; i++) {
            uint strLen = FromVWInt(bytes);
            strings.Add(Encoding.UTF8.GetString(ReadN(bytes, strLen)));
        }
    }

    public JSONBool ReadBinJSONBool(BinaryReader bytes) {
        return new JSONBool(ReadByte(bytes) != 0);
    }

    public JSONDouble ReadBinJSONDouble(BinaryReader bytes) {
        byte[] raw = ReadN(bytes, 8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(raw);
        return new JSONDouble(BitConverter.ToDouble(raw));
    }

    public JSONInt ReadBinJSONInt(BinaryReader bytes) {
        int raw = (int)FromVWInt(bytes);
        return new JSONInt((raw >> 1) * ((raw & 1) == 1 ? -1 : 1));
    }

    public JSONList ReadBinJSONList(BinaryReader bytes) {
        JSONList result = [];
        uint count = FromVWInt(bytes);
        if (count == 0) return result;
        byte ID = ReadByte(bytes);
        for (uint i = 0; i < count; i++) {
            result.Add(ReadBinJSONValue(ID, bytes));
        }
        return result;
    }

    public JSONList ReadBinJSONMultitypedList(BinaryReader bytes) {
        JSONList result = [];
        uint count = FromVWInt(bytes);
        for (uint i = 0; i < count; i++) {
            byte ID = ReadByte(bytes);
            result.Add(ReadBinJSONValue(ID, bytes));
        }
        return result;
    }

    public JSONObject ReadBinJSONObject(BinaryReader bytes) {
        JSONObject result = [];
        uint count = FromVWInt(bytes);
        for (uint i = 0; i < count; i++) {
            string key = GetString(bytes);
            byte ID = ReadByte(bytes);
            IJSONValue value = ReadBinJSONValue(ID, bytes);
            result[key] = value;
        }
        return result;
    }

    public JSONString ReadBinJSONString(BinaryReader bytes) {
        return new JSONString(GetString(bytes));
    }

    public IJSONValue ReadBinJSONValue(byte ID, BinaryReader bytes) {
        return ID switch {
            1 => ReadBinJSONBool(bytes),
            2 => ReadBinJSONDouble(bytes),
            3 => ReadBinJSONInt(bytes),
            4 => ReadBinJSONList(bytes),
            5 => new JSONNull(),
            6 => ReadBinJSONObject(bytes),
            7 => ReadBinJSONString(bytes),
            8 => ReadBinJSONMultitypedList(bytes),
            _ => throw new InvalidBinJSONException($"{ID} is not a valid BinJSON type ID"),
        };
    }

    public static IEnumerable<byte> Serialize(IJSONValue value) {
        BinJSONEnv env = new();
        IEnumerable<byte> magicByte = [0x42];
        IEnumerable<byte> dataBytes = value.ToBinJSON(env);
        IEnumerable<byte> stringTable = env.CreateStringTable();
        return magicByte.Concat(stringTable).Concat(value.ID).Concat(dataBytes);
    }

    public static void WriteFile(string path, IJSONValue value) {
        byte[] bytes = Serialize(value).ToArray();
        using FileStream file = new(path, FileMode.Create);
        file.Write(bytes, 0, bytes.Length);
    }

    public static IJSONValue Deserialize(BinaryReader bytes) {
        BinJSONEnv env = new();
        if (ReadByte(bytes) != 0x42) {
            throw new InvalidBinJSONException("BinJSON files must start with 0x42");
        }

        env.ReadStringTable(bytes);

        byte ID = ReadByte(bytes);
        return env.ReadBinJSONValue(ID, bytes);
    }
}
