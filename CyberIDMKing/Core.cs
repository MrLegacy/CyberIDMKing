using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CyberIDMKing
{
    internal class Core
    {
        private static string setacl = (RuntimeInformation.OSArchitecture == Architecture.X86) ? "SetACLx32.exe" : "SetACLx64.exe";
        //private static bool _is_auto = false;
        //private static int _version = 21;
        private static string[] _all_keys =
        {
            "{6DDF00DB-1234-46EC-8356-27E7B2051192}",
            "{7B8E9164-324D-4A2E-A46D-0165FB2000EC}",
            "{D5B91409-A8CA-4973-9A0B-59F713D25671}",
            "{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}",
            "",
            "{07999AC3-058B-40BF-984F-69EB1E554CA7}"
        };

        private static void SetOwner(string owner)
        {
            switch (owner) {
                case "everyone":
                    owner = "S-1-1-0";
                    break;
                case "nobody":
                    owner = "S-1-0-0";
                    break;
            }
            for (var i = 0; i < _all_keys.Length; i++)
            {
                RunWait($"{setacl} -on HKCU\\Software\\Classes\\CLSID\\{_all_keys[i]} -ot reg -actn setowner -ownr \"n:{owner}\" -silent");
                RunWait($"{setacl} -on HKCU\\Software\\Classes\\Wow6432Node\\CLSID\\{_all_keys[i]} -ot reg -actn setowner -ownr \"n:{owner}\" -silent");
                RunWait($"{setacl} -on HKLM\\Software\\Classes\\CLSID\\{_all_keys[i]} -ot reg -actn setowner -ownr \"n:{owner}\" -silent");
                RunWait($"{setacl} -on HKLM\\Software\\Classes\\Wow6432Node\\CLSID\\{_all_keys[i]} -ot reg -actn setowner -ownr \"n:{owner}\" -silent");
            }
        }

        private static void SetPermission(string permission)
        {
            for (var i = 0; i < _all_keys.Length; i++)
            {
                RunWait($"{setacl} -on HKCU\\Software\\Classes\\CLSID\\{_all_keys[i]} -ot reg -actn ace -ace \"n:everyone;p:{permission}\" -actn setprot -op \"dacl:p_nc;sacl:p_nc\" -silent");
                RunWait($"{setacl} -on HKCU\\Software\\Classes\\Wow6432Node\\CLSID\\{_all_keys[i]} -ot reg -actn ace -ace \"n:everyone;p:{permission}\" -actn setprot -op \"dacl:p_nc;sacl:p_nc\" -silent");
                RunWait($"{setacl} -on HKLM\\Software\\Classes\\CLSID\\{_all_keys[i]} -ot reg -actn ace -ace \"n:everyone;p:{permission}\" -actn setprot -op \"dacl:p_nc;sacl:p_nc\" -silent");
                RunWait($"{setacl} -on HKLM\\Software\\Classes\\Wow6432Node\\CLSID\\{_all_keys[i]} -ot reg -actn ace -ace \"n:everyone;p:{permission}\" -actn setprot -op \"dacl:p_nc;sacl:p_nc\" -silent");
            }
        }

        private static void RunWait(string command)
        {
            var res = GetDOSOutput(command);
            Console.WriteLine(res);
        }


        private static string GetDOSOutput(string command)
        {
            // Start the process
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {command}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                // Read the output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Wait for the process to finish
                process.WaitForExit();

                return output.Trim();
            }
        }
        private static string RegSearch(string value = "")
        {
            var key = "";

            try
            {
                RunWait("reg query hkcr\\clsid /s > reg_query.tmp");

                var res = GetDOSOutput($"findstr /N /I {value} reg_query.tmp");
                var find = res.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                find = GetDOSOutput($"findstr /N . reg_query.tmp | findstr /b {(Convert.ToInt32(find) - 1)}:");

                if (find.Contains("{") && find.Contains("}"))
                {
                    key = "{" + _StringBetween(find, "{", "}")[0] + "}";
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return key;
        }

        private static string[] _StringBetween(string text, string startDelimiter, string endDelimiter)
        {

            // Create a regular expression pattern to capture the content between the delimiters
            string pattern = Regex.Escape(startDelimiter) + @"(.*?)" + Regex.Escape(endDelimiter);

            // Find all matches in the text
            MatchCollection matches = Regex.Matches(text, pattern);

            // Create an array to store the captured group values
            string[] results = new string[matches.Count];

            // Extract the values from each match and store them in the array
            for (int i = 0; i < matches.Count; i++)
            {
                results[i] = matches[i].Groups[1].Value;
            }

            // Return the array of captured values
            return results;

        }

        public static void Trial()
        {
            Reset();

            RunWait("reg import \"idm_trial.reg\"");
            
            SetPermission("read");
            SetOwner("nobody");
        }

        public static void Reset()
        {
            _all_keys[4] = RegSearch("cDTvBFquXk0");
            
            SetOwner("everyone");
            SetPermission("full");

            RunWait($"reg import \"idm_reset.reg\"");

            if (!string.IsNullOrEmpty(_all_keys[4])) {
                RegDelete(@"HKEY_CURRENT_USER\Software\Classes\CLSID\" + _all_keys[4]);
                RegDelete(@"HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\" + _all_keys[4]);
                RegDelete(@"HKEY_LOCAL_MACHINE\Software\Classes\CLSID\" + _all_keys[4]);
                RegDelete(@"HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\" + _all_keys[4]);
            }
        }
        public static void Register(string FName = "CyberTik") {

            Reset();

            //autorun('off');

            RunWait("reg import \"idm_reg.reg\"");

            if (!string.IsNullOrEmpty(_all_keys[4])) {
                RegWrite(@"HKEY_CURRENT_USER\Software\Classes\CLSID\" + _all_keys[4]);
                RegWrite(@"HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\" + _all_keys[4]);
                RegWrite(@"HKEY_LOCAL_MACHINE\Software\Classes\CLSID\" + _all_keys[4]);
                RegWrite(@"HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\" + _all_keys[4]);
            }

            RunWait($"reg add \"HKCU\\Software\\DownloadManager\" /v \"FName\" /t \"REG_SZ\" /d \"{FName}\" /f");

            SetPermission("read");
            SetOwner("nobody");
        }


        private static RegistryKey GetSource(string path)
        {
            if (path.Contains("HKEY_LOCAL_MACHINE"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);

            }
            else if (path.Contains("HKEY_CURRENT_USER"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            }
            return null;
        }

        private static void RegDelete(string keyPath)
        {
            using (var src = GetSource(keyPath))
            {
                src.DeleteSubKey(keyPath.Substring(keyPath.IndexOf("\\") + 1).Trim());
            }
        }
        private static void RegWrite(string keyPath)
        {
            using (var src = GetSource(keyPath))
            {
                src.CreateSubKey(keyPath.Substring(keyPath.IndexOf("\\") + 1).Trim(),true);
            }
        }
    }

    
}
