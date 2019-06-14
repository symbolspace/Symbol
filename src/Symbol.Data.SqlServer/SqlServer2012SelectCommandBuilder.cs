/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// SqlServer 查询命令构造器基类
    /// </summary>
    public class SqlServer2012SelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {
        #region fields
        private bool _limitMode = false;
        #endregion

        #region ctor
        /// <summary>
        /// 创建SqlServer2012SelectCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        /// <param name="commandText"></param>
        public SqlServer2012SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText)
            : base(dataContext, tableName, commandText) {
        }

        #endregion

        #region methods

        #region Parse
        /// <summary>
        /// 解析命令脚本。
        /// </summary>
        /// <param name="commandText">命令脚本。</param>
        protected override void Parse(string commandText) {
            commandText = StringExtensions.Replace(
                                            StringExtensions.Replace(
                                                commandText, "select*", "select *", true),
                                            "*from ", " * from", true);
            int i = commandText.IndexOf("select ", System.StringComparison.OrdinalIgnoreCase);
            if (i == -1)//没有select ，无效
                throw new System.InvalidOperationException("没有“select ”：" + commandText);
            i += "select ".Length;
            int j = commandText.IndexOf(" from", i, System.StringComparison.OrdinalIgnoreCase);
            if (j == -1)//没有from
                throw new System.InvalidOperationException("没有“ from ”：" + commandText);
            bool top = ParseFields(commandText.Substring(i, j - i));//取出列
            if (!top) {
                int ix = commandText.IndexOf("limit ", System.StringComparison.OrdinalIgnoreCase);
                if (ix != -1) {
                    _limitMode = true;
                    System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+),(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    SkipCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                    TakeCount = TypeExtensions.Convert<int>(match.Groups[2].Value, 0);
                    commandText = commandText.Replace(match.Value, "");
                }
            }
            j += " from ".Length;//推进到form 后面
            i = commandText.IndexOf(" where ", j, System.StringComparison.OrdinalIgnoreCase);//尝试找到where
            if (i != -1) {
                PaseWhereBefore(commandText.Substring(j, i - j));//分析WhereBefore
                i += " where ".Length;
                j = commandText.IndexOf(" order by", i, System.StringComparison.OrdinalIgnoreCase);
                if (j == -1) {
                    j = commandText.IndexOf("\norder by", i, System.StringComparison.OrdinalIgnoreCase);
                }
                if (j == -1) {
                    j = commandText.Length;
                } else {
                    ParseOrderBy(commandText.Substring(j + " order by".Length));
                }
                ParseWhere(commandText.Substring(i, j - i));
            } else {
                int j2 = commandText.IndexOf(" order by", j, System.StringComparison.OrdinalIgnoreCase);
                if (j == -1) {
                    j = commandText.IndexOf("\norder by", i, System.StringComparison.OrdinalIgnoreCase);
                }
                if (j2 != -1) {
                    PaseWhereBefore(commandText.Substring(j, j2 - j));//分析WhereBefore
                    ParseOrderBy(commandText.Substring(j2 + " order by".Length));
                } else {
                    PaseWhereBefore(commandText.Substring(j));
                }
            }
        }
        void PaseWhereBefore(string text) {
            if (string.IsNullOrEmpty(text))
                return;
            bool b = false;
            int i = -1;
            if (text[0] == '(') {
                int x = text.IndexOf(')', 1);
                if (x > -1) {
                    b = true;
                    _tableName = text.Substring(0, x + 1);
                    text = text.Substring(x + 1);
                    i = 0;
                }
            }
            if (!b) {
                int i1 = text.IndexOf(' ');
                int i2 = text.IndexOf("]");
                i = System.Math.Max(i1, i2);
                if (i == -1) {
                    _tableName = text;
                    return;
                }
                i++;
                _tableName = text.Substring(0, i).Trim();
            }
            if (!IsCustomTable)
                _tableName = _tableName.Trim('[', ']');
            if (i == text.Length)
                return;
            WhereBefores.Add(text.Substring(i));
        }
        bool ParseFields(string fields) {
            fields = fields.Trim();
            if (string.IsNullOrEmpty(fields))
                return false;
            int i = fields.IndexOf("top ", System.StringComparison.OrdinalIgnoreCase);
            bool top = false;
            if (i != -1) {
                i += "top ".Length;
                int j = fields.IndexOf(' ', i);
                if (j != -1) {
                    TakeCount = TypeExtensions.Convert<int>(fields.Substring(i, j - i), 0);
                    top = true;
                    fields = fields.Substring(j + 1);
                } else {
                    fields = fields.Substring(i);
                }
            }
            System.Collections.Generic.ICollectionExtensions.AddRange(_fields, fields.Split(','));
            return top;
        }
        void ParseWhere(string expressions) {
            Where(WhereOperators.And, expressions);
        }
        void ParseOrderBy(string orderbys) {
            orderbys = PreOffset(orderbys.Trim());
            if (string.IsNullOrEmpty(orderbys))
                return;

            System.Collections.Generic.ICollectionExtensions.AddRange(OrderBys, orderbys.Split(','));
        }
        string PreOffset(string orderbys) {
            if (string.IsNullOrEmpty(orderbys))
                return null;
            string regex = "offset[\\s\\S]+(?<offset>[0-9]+)[\\s\\S]+rows[\\s\\S]+fetch[\\s\\S]+next[\\s\\S]+(?<next>[0-9]+)[\\s\\S]+rows[\\s\\S]+only[\\s\\S;]*|offset[\\s\\S]+(?<offset>[0-9]+)[\\s\\S]+rows[\\s\\S;]*";
            //string regex2 = "offset[\\s\\S]+([0-9]+)[\\s\\S]+rows[\\s\\S;]*";
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(orderbys, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //if (!match.Success) {
            //    match = System.Text.RegularExpressions.Regex.Match(orderbys, regex2, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //}
            if (!match.Success) {
                return orderbys;
            }
            SkipCount = TypeExtensions.Convert<int>(match.Groups["offset"].Value, 0);
            int takeCount = TypeExtensions.Convert<int>(match.Groups["next"].Value, 0);
            if (takeCount > 0) {
                TakeCount = takeCount;
            }
            //if (match.Groups.Count > 2) {

            //}
            return orderbys.Replace(match.Value, "");
        }
        #endregion

        #region BuilderCommandText
        /// <summary>
        /// 构造命令脚本。
        /// </summary>
        /// <returns>返回命令脚本。</returns>
        protected override string BuilderCommandText() {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            BuildSelect(builder);
            BuildWhereBefore(builder);
            BuildWhere(builder);
            BuildOrderBy(builder);
            BuildSkip(builder);
            return builder.ToString();
        }
        void BuildSkip(System.Text.StringBuilder builder) {
            if (_limitMode) {
                if (SkipCount > 0 || TakeCount > 0) {
                    builder.AppendFormat(" limit {0},{1}", SkipCount, TakeCount);
                }
            } else {
                if (TakeCount < 1 && SkipCount < 1)
                    return;
                if (OrderBys.Count == 0) {
                    builder.AppendLine(" order by 1 ");
                }
                if (TakeCount < 1) {
                    builder.AppendFormat(" offset {0} rows", SkipCount);
                    return;
                }

                builder.AppendFormat(" offset {0} rows fetch next {1} rows only", SkipCount < 0 ? 0 : SkipCount, TakeCount);
            }
        }
        #endregion

        #endregion

    }
}

