/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System;

namespace Symbol.Data.SQLite {

    class TableSchemaHelper {
        delegate Symbol.Data.DatabaseTableField ParseFieldDelegate(string block, System.Collections.Generic.List<string> array);
        delegate Symbol.Data.DatabaseTableField NextBlockDelegate();
        public System.Collections.Generic.List<Symbol.Data.DatabaseTableField> ParseFieldsByCreate(string commandText) {
            System.Collections.Generic.List<Symbol.Data.DatabaseTableField> list = new System.Collections.Generic.List<Symbol.Data.DatabaseTableField>();
            if (string.IsNullOrEmpty(commandText)) {
                return list;
            }
            //create table 
            string tableName = FilterTableName(Symbol.Text.StringExtractHelper.StringsStartEnd(commandText, "create table ", "("));
            if (string.IsNullOrEmpty(tableName))
                return list;
            string body;
            int index;
            {
                string s = tableName;
                index = Symbol.Text.StringExtractHelper.StringIndexOf(commandText, ref s, false);
                index += s.Length;
                s = "(";
                index = Symbol.Text.StringExtractHelper.StringIndexOf(commandText, ref s, index, false);
                index++;
                body = commandText.Substring(index);
                index = 0;
            }
            ParseFieldDelegate parseField = (block, array) => {
                if (array.Count < 2)
                    return null;
                string name = FilterTableName(array[0]);
                string type = array[1];
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                    return null;
                string values = array.Count > 2 ? array[2] : "";
                Predicate<string> has = (p11) => {
                    return values.IndexOf(p11, StringComparison.OrdinalIgnoreCase) > -1;
                };
                Symbol.Data.DatabaseTableField field = new Symbol.Data.DatabaseTableField() {
                    TableName = tableName,
                    Position = list.Count + 1,
                    Exists = true,
                    Description = "",
                    Name = name,
                    Type = type,
                    IsIdentity = has("AUTOINCREMENT") || has("identity"),
                    IsPrimary = has("PRIMARY KEY"),
                    Nullable = !has("not null"),
                };
                if (type.StartsWith("decimal(", StringComparison.OrdinalIgnoreCase)
                    || type.StartsWith("number(", StringComparison.OrdinalIgnoreCase)
                    || type.StartsWith("money(", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, "(", ","), 0);
                    field.Scale = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, ",", ")"), 0);
                } else if (type.IndexOf('(') > -1) {
                    field.Type = type.Split('(')[0];
                    field.Length = TypeExtensions.Convert<int>(Symbol.Text.StringExtractHelper.StringsStartEnd(type, "(", ")"), 0);
                } else if (type.Equals("tinyint", StringComparison.OrdinalIgnoreCase)
                    || type.Equals("int8", StringComparison.OrdinalIgnoreCase)
                    || type.Equals("bit", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 1;
                } else if (type.Equals("smallint", StringComparison.OrdinalIgnoreCase) || type.Equals("int16", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 2;
                } else if (type.Equals("int", StringComparison.OrdinalIgnoreCase) || type.Equals("int32", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 4;
                } else if (type.Equals("bigint", StringComparison.OrdinalIgnoreCase)
                        || type.Equals("integer", StringComparison.OrdinalIgnoreCase)
                        || type.Equals("int64", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 8;
                } else if (type.Equals("real", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 4;
                } else if (type.Equals("float", StringComparison.OrdinalIgnoreCase) || type.Equals("double", StringComparison.OrdinalIgnoreCase)) {
                    field.Length = 8;
                }
                //if (has("default")) {
                //    //field.DefaultValue = Symbol.Text.StringExtractHelper.StringsStartEnd(values, "default", ")", " ");
                //}
                return field;
            };
            NextBlockDelegate nextBlock = () => {
                int blockCount = 0;
                bool finded = false;
                int index2 = index;
                System.Collections.Generic.List<string> array = new System.Collections.Generic.List<string>();
                for (int i = index; i < body.Length; i++) {
                    char c = body[i];
                    if (c == '(') {
                        blockCount++;
                        continue;
                    } else if (c == ')') {
                        if (blockCount == 0) {
                            string block = body.Substring(index, i - index);
                            if (finded) {
                                array.Add(body.Substring(index2, i - index2));
                            }
                            index = i + 1;
                            return parseField(block, array);
                        }
                        blockCount--;
                    } else if (c == ',') {
                        if (blockCount > 0)
                            continue;
                        string block = body.Substring(index, i - index);
                        if (finded) {
                            array.Add(body.Substring(index2, i - index2));
                        }
                        index = i + 1;
                        return parseField(block, array);
                    } else if (c == ' ') {
                        if (!finded) {
                            finded = true;
                        }
                        if (array.Count < 2) {
                            array.Add(body.Substring(index2, i - index2));
                            finded = false;
                            index2 = i + 1;
                        }

                    }
                }
                return null;
            };
            while (true) {
                Symbol.Data.DatabaseTableField field = nextBlock();
                if (field == null)
                    break;
                //Console.WriteLine(Symbol.Serialization.Json.ToString(field, false, true));
                list.Add(field);
            }
            return list;
        }
        string FilterTableName(string name) {
            if (string.IsNullOrEmpty(name))
                return null;
            if (name.IndexOf('.') > -1) {
                string[] lines = name.Split('.');
                name = lines[name.Length - 1];
            }
            name = name.Trim().Trim('[', ']', '(', ')', '{', '}', '"', '`', '\'');
            if (string.IsNullOrEmpty(name))
                return null;
            return name;
        }
    }

}