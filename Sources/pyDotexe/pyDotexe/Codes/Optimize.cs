using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace pyDotexe.Codes
{
    public class Optimize
    {
        /// <summary>
        /// Start optimize the selected source code.
        /// </summary>
        /// <param name="path">Source code path</param>
        public static void Start(string path)
        {
            List<string> list_data = new List<string>();
            string raw_data = "";

            // Read source code data.
            using (StreamReader sr = new StreamReader(path))
            {
                raw_data = sr.ReadToEnd();
                while (sr.Peek() > -1)
                    list_data.Add(sr.ReadLine());
            }
            // Start optimizing.
            start_optimize_code(ref list_data, raw_data);

            // Write optimized data.
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (string line in list_data)
                {
                    sw.WriteLine(line);
                }
            }
        }

        private static void start_optimize_code(ref List<string> list_data, string raw_data)
        {
            list_data = new List<string>(optcode_trim(raw_data));  // Optimize source code comments.
            list_data = new List<string>(remove_blanks(list_data)); // Optimize blanks.
            list_data = new List<string>(remove_spaces(list_data)); // Optimize spaces.
            //list_data = new List<string>(add_pass(list_data)); // Add 'pass' functions.
            GC.Collect(); // Free memory.
        }

        /// <summary>
        /// Delete sources code comment
        /// </summary>
        /// <param name="raw_data"></param>
        /// <returns></returns>
        private static List<string> optcode_trim(string raw_data)
        {
            //raw_data = Regex.Replace(raw_data, "(\"\"\"|''').*?(\"\"\"|''')", "", RegexOptions.Singleline);
            raw_data = Regex.Replace(raw_data, "(?m)^ *#.*\n?", "", RegexOptions.Multiline); // Delete '#' comments
            return new List<string>(raw_data.Split(new string[] { "\n" }, StringSplitOptions.None));
        }

        /// <summary>
        /// Delete blanks.
        /// </summary>
        /// <param name="list_data"></param>
        /// <returns></returns>
        private static List<string> remove_blanks(List<string> list_data)
        {
            List<string> tmp_list = new List<string>();
            foreach (string line in list_data)
            {
                // Delete blanks.
                string tr_str = line.Trim('\r').Trim('\n');
                if (!(tr_str.Replace(" ", "") == ""))
                    tmp_list.Add(tr_str.Trim('\r').Trim('\n'));
            }
            return tmp_list;
        }

        /// <summary>
        /// Compress indents.
        /// </summary>
        /// <param name="list_data"></param>
        /// <returns></returns>
        private static List<string> remove_spaces(List<string> list_data)
        {
            List<string> tmp_list = new List<string>();
            List<int> sp_num = new List<int>();
            foreach (string line in list_data)
                sp_num.Add(get_space_num(line));

            List<int> tmp_num = new List<int>(sp_num);
            tmp_num.Sort();
            HashSet<int> hs = new HashSet<int>(tmp_num); // Sorted
            int[] sort_num = new int[hs.Count];
            hs.CopyTo(sort_num, 0);

            if (sort_num.Length > 1)
            {
                // Start replaced the spaces.
                Dictionary<int, int> dic_num = new Dictionary<int, int>();
                for (int i = 0; i < sort_num.Length; i++)
                    dic_num.Add(sort_num[i], i);
                for (int p = 0; p < list_data.Count; p++)
                {
                    int index = dic_num[sp_num[p]];
                    string dist_data = list_data[p].Substring(sp_num[p]);
                    tmp_list.Add(new string(' ', index) +dist_data);
                }
            }
            else
                return list_data;
            return tmp_list;
        }

        private static int get_space_num(string line)
        {
            // Get space charactors.
            int num = 0;
            for (num = 0; num < line.Length; num++)
            {
                if (line[num] != ' ') break;
            }
            return num;
        }

        private static bool is_alphabet(string str)
        {
            if (string.IsNullOrEmpty(str)) { return false; }

            foreach (char c in str)
            {
                if (!Char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        /*
        private static List<string> add_pass(List<string> list_data)
        {
            List<string> tmp_list = new List<string>();
            bool def_flag = false;
            bool class_flag = false;
            int blanks = 0;
            int blanks_flag_num = 0;
            foreach (string data_raw in list_data)
            {
                blanks = get_space_num(data_raw);
                string data = data_raw.Replace(" ", "");
                if ((blanks_flag_num >= blanks) & def_flag)
                {
                    tmp_list.Add(new string(' ', blanks_flag_num+1) +"pass"); // Add 'pass'
                    def_flag = false;
                    class_flag = false;
                }
                else if (data.StartsWith("def") & data.EndsWith(":"))
                {
                    def_flag = true;
                    class_flag = false;
                    blanks_flag_num = get_space_num(data_raw);
                }
                else if ((blanks_flag_num >= blanks) & class_flag)
                {
                    tmp_list.Add(new string(' ', blanks_flag_num + 1) + "pass"); // Add 'pass'
                    def_flag = false;
                    class_flag = false;
                }
                else if (data.StartsWith("class") & data.EndsWith(":"))
                {
                    def_flag = false;
                    class_flag = true;
                    blanks_flag_num = get_space_num(data_raw);
                }
                else if (data.EndsWith("=") & !data.EndsWith("=="))
                {
                    def_flag = false;
                    class_flag = false;
                    tmp_list.Add(data_raw + "\"\"");
                    continue;
                }
                else
                {
                    class_flag = false;
                    def_flag = false;
                }

                if (is_alphabet(data) & (data.Length <= 2)) continue;
                if (data.EndsWith(",)"))
                {
                    tmp_list.Add(data_raw.Remove(data_raw.Length-1) +"\"\")");
                }

                tmp_list.Add(data_raw); // Add list data.
            }

            return tmp_list;
        }
        */
    }
}
