using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Environment
{
    /// <summary>
    /// プロセス引数を管理します。
    /// </summary>
    public class ProcessArgs
    {
        #region 定数

        /// <summary>
        /// 引数をキーとみなす正規表現を表します。
        /// </summary>
        private const string REGEX_KEY_PATTERN = @"^[/\-]";

        /// <summary>
        /// キーの識別文字を表します。
        /// </summary>
        private const char ARGS_KEY_CHAR = '/';

        /// <summary>
        /// 長さ 0 を表します。
        /// </summary>
        private const int LENGTH_EMPTY = 0;

        /// <summary>
        /// デフォルトのキー(キーなし)を表します。
        /// </summary>
        public const string DEFAULT_KEY = "";

        #endregion

        #region メンバー

        /// <summary>
        /// プロセス引数を正規化したディクショナリ情報を保持します。
        /// </summary>
        private SortedDictionary<string, List<string>> normalizedArgs = null;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ProcessArgs クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ProcessArgs()
        {
            Clear();
        }

        /// <summary>
        /// ProcessArgs クラスの新しいインスタンスを、指定した string 配列を用いて初期化します。
        /// </summary>
        /// <param name="args">引数の string 配列。</param>
        /// <example>
        /// このコンストラクタの一般的な利用方法を次の例に示します。
        /// <code><![CDATA[
        /// using Hondarersoft.Dpm.Environment;
        /// 
        /// ProcessArgs processArgs = new ProcessArgs(System.Environment.GetCommandLineArgs());
        /// ]]></code>
        /// コマンドライン アプリケーションでの一般的な利用方法を次の例に示します。
        /// <code><![CDATA[
        /// using Hondarersoft.Dpm.Environment;
        /// 
        /// public static void Main(String[] args)
        /// {
        ///     ProcessArgs processArgs = new ProcessArgs(args);
        /// 
        ///     ...
        /// 
        /// }
        /// ]]></code>
        /// </example>
        public ProcessArgs(string[] args)
        {
            ArrayArgs = args;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 引数の string 配列を取得または設定します。
        /// </summary>
        public string[] ArrayArgs
        {
            set
            {
                if (value != null)
                {
                    // ディクショナリをクリアする。
                    Clear();

                    string capturingKey = string.Empty;

                    // 引数でループ
                    for (int index = 0; index < value.Length; index++)
                    {
                        // インデックスが 0 でかつ、値が自身の実行名である場合は
                        // 登録対象外とする。
                        if ((index == 0) && (value[index] == System.Environment.GetCommandLineArgs()[0]))
                        {
                            continue;
                        }

                        // オプションを表す文字が先頭にある
                        if (Regex.Match(value[index], REGEX_KEY_PATTERN).Success == true)
                        {
                            // オプションを表す文字の後ろに何らかの文字列がある
                            if (value[index].Length > 0)
                            {
                                // キーとして確保
                                capturingKey = value[index].Substring(1);

                                // はじめてのキーならばディクショナリを登録
                                AddKey(capturingKey);
                            }
                        }
                        else
                        {
                            // 値を登録
                            AddValue(capturingKey, value[index]);
                        }
                    }
                }
            }
            get
            {
                string[] ret = new string[LENGTH_EMPTY];

                foreach (string key in normalizedArgs.Keys)
                {
                    if (HasValue(key) == true)
                    {
                        if (string.IsNullOrEmpty(key) != true)
                        {
                            Array.Resize(ref ret, ret.Length + 1);
                            ret[ret.Length - 1] = string.Format("{0}{1}", ARGS_KEY_CHAR, key);
                        }
                        foreach (string value in normalizedArgs[key])
                        {
                            Array.Resize(ref ret, ret.Length + 1);
                            ret[ret.Length - 1] = value;
                        }
                    }
                    else
                    {
                        Array.Resize(ref ret, ret.Length + 1);
                        ret[ret.Length - 1] = string.Format("{0}{1}", ARGS_KEY_CHAR, key);
                    }
                }

                return ret;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// キーと値を初期化します。
        /// </summary>
        public void Clear()
        {
            // StringComparer.InvariantCultureIgnoreCase を設定することにより、
            // キーの大文字小文字の違いを無視して比較できるようにする。
            normalizedArgs = new SortedDictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 指定されたキーに対応する値を取得します。値が複数存在する場合は、先頭の値を返します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>値。</returns>
        public string GetValue(string key)
        {
            string ret = string.Empty;

            if (HasValue(key) == true)
            {
                ret = normalizedArgs[key][0];
            }

            return ret;
        }

        /// <summary>
        /// 指定されたキーに対応する値をすべて取得します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>値の列挙。</returns>
        public IEnumerable<string> GetValues(string key)
        {
            IEnumerable<string> ret = null;

            if (HasValue(key) == true)
            {
                ret = normalizedArgs[key].AsEnumerable();
            }

            return ret;
        }

        /// <summary>
        /// 指定されたキーが存在するかどうかを返します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>キーが存在する場合は true を返します。</returns>
        public bool HasKey(string key)
        {
            if (key == null)
            {
                return false;
            }
            return (normalizedArgs.ContainsKey(key) == true);
        }

        /// <summary>
        /// 指定されたキーに値が指定されているかを返します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>
        /// キーが存在し、値が指定されている場合は true を返します。
        /// キーが存在しない場合または値が指定されていない場合は false を返します。
        /// </returns>
        public bool HasValue(string key)
        {
            return ((HasKey(key) == true) && (normalizedArgs[key] != null));
        }

        /// <summary>
        /// 指定されたキーを追加します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <returns>キーがすでに存在していれば false を、追加した場合は true を返します。</returns>
        public bool AddKey(string key)
        {
            // キーの補正
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                key = DEFAULT_KEY;
            }

            if (HasKey(key) == false)
            {
                normalizedArgs.Add(key, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定されたキーに、1つの値を追加します。
        /// キーが存在しない場合は作成します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <param name="value">値。</param>
        public void AddValue(string key, object value)
        {
            if (value == null)
            {
                return;
            }

            string valstr = value.ToString();

            // キーの補正
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                key = DEFAULT_KEY;
            }

            // キーの登録
            AddKey(key);

            // 初回の値登録
            if (HasValue(key) == false)
            {
                normalizedArgs[key] = new List<string>();
            }

            // 引数を登録
            normalizedArgs[key].Add(valstr);
        }

        /// <summary>
        /// 指定されたキーに、複数の値を追加します。
        /// キーが存在しない場合は作成します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <param name="values">値の配列。</param>
        public void AddValues(string key, object[] values)
        {
            if (values == null)
            {
                return;
            }

            // キーの補正
            if (string.IsNullOrWhiteSpace(key) == true)
            {
                key = DEFAULT_KEY;
            }

            for (int index = 0; index < values.Length; index++)
            {
                if (values[index] != null)
                {
                    string valstr = values[index].ToString();

                    // キーの登録
                    AddKey(key);

                    // 初回の値登録
                    if (HasValue(key) == false)
                    {
                        normalizedArgs[key] = new List<string>();
                    }

                    // 引数を登録
                    normalizedArgs[key].Add(valstr);
                }
            }
        }

        /// <summary>
        /// このオブジェクトの文字列表現を返します。この値は <see cref="System.Diagnostics.Process.Start(string, string)"/> メソッドの arguments パラメーターに設定可能です。
        /// </summary>
        /// <returns>このオブジェクトの文字列表現。</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in normalizedArgs.Keys)
            {
                // 値を持つか
                if (HasValue(key) == true)
                {
                    // デフォルトのキーでなければ、キーを出力
                    if (string.IsNullOrEmpty(key) != true)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(" ");
                        }

                        sb.AppendFormat("{0}{1}", ARGS_KEY_CHAR, key);
                    }

                    // 値でループ
                    foreach (string value in normalizedArgs[key])
                    {
                        // 値を出力
                        if (sb.Length != 0)
                        {
                            sb.Append(" ");
                        }

                        sb.AppendFormat("\"{0}\"", value);
                    }
                }
                else
                {
                    // キーのみ
                    if (sb.Length != 0)
                    {
                        sb.Append(" ");
                    }

                    sb.AppendFormat("{0}{1}", ARGS_KEY_CHAR, key);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}