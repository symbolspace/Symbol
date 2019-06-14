/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data {

    /// <summary>
    /// MySql 查询命令构造器基类
    /// </summary>
    public class MySqlSelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {


        #region ctor
        /// <summary>
        /// 创建MySqlSelectCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        /// <param name="commandText"></param>
        public MySqlSelectCommandBuilder(IDataContext dataContext, string tableName, string commandText)
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
            //if (i != 0)
            //    SelectBefore = commandText.Substring(0, i);
            i += "select ".Length;
            int j = commandText.IndexOf(" from", i, System.StringComparison.OrdinalIgnoreCase);
            if (j == -1)//没有from
                throw new System.InvalidOperationException("没有“ from ”：" + commandText);
            bool top = ParseFields(commandText.Substring(i, j - i));//取出列
                                                                    //if (!top) {
            {
                int ix = commandText.IndexOf("limit ", System.StringComparison.OrdinalIgnoreCase);
                if (ix != -1) {
                    //_limitMode = true;
                    var match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+),(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success) {
                        SkipCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                        TakeCount = TypeExtensions.Convert<int>(match.Groups[2].Value, 0);
                        commandText = commandText.Replace(match.Value, "");
                    } else {
                        match = System.Text.RegularExpressions.Regex.Match(commandText, "limit\\s*(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success) {
                            TakeCount = TypeExtensions.Convert<int>(match.Groups[1].Value, 0);
                            commandText = commandText.Replace(match.Value, "");
                        }
                    }
                }

            }

            j += " from ".Length;//推进到form 后面
            i = commandText.IndexOf(" where ", j, System.StringComparison.OrdinalIgnoreCase);//尝试找到where
            if (i != -1) {
                PaseWhereBefore(commandText.Substring(j, i - j));//分析WhereBefore
                i += " where ".Length;
                j = commandText.IndexOf(" order by", i, System.StringComparison.OrdinalIgnoreCase);
                if (j == -1) {
                    j = commandText.Length;
                } else {
                    ParseOrderBy(commandText.Substring(j + " order by".Length));
                }
                ParseWhere(commandText.Substring(i, j - i));
            } else {
                int j2 = commandText.IndexOf(" order by", j, System.StringComparison.OrdinalIgnoreCase);
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
            int i1 = text.IndexOf(' ');
            int i2 = text.IndexOf("\"");
            if (i2 == -1) {
                i2 = text.IndexOf("]");
            }
            int i = System.Math.Max(i1, i2);
            if (i == -1) {
                _tableName = text;
                return;
            }
            i++;
            int j = text.IndexOf('"', i);
            if (j == -1) {
                _tableName = text.Substring(0, i).Trim();
            } else {
                _tableName = text.Substring(i, j - i).Trim();
                i = j + 1;
            }
            if (!IsCustomTable)
                _tableName = _tableName.Trim('[', ']', '"');
            if (i == text.Length)
                return;
            _whereBefores.Add(text.Substring(i));
        }
        private bool ParseFields(string fields) {
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
        private void ParseWhere(string expressions) {
            Where(WhereOperators.And, expressions);
        }
        private void ParseOrderBy(string orderbys) {
            orderbys = orderbys.Trim();
            if (string.IsNullOrEmpty(orderbys))
                return;
            System.Collections.Generic.ICollectionExtensions.AddRange(_orderbys, orderbys.Split(','));
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
            if (SkipCount > 0 || TakeCount > 0) {
                builder.AppendFormat(" limit {0},{1}", SkipCount, TakeCount);
            }
        }

        #endregion

        #endregion
    }
}

