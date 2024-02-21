using Microsoft.Win32;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberIDMKing
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            Core.Init();
            chbAutoResetTrial.Checked = Core.AutoResetTrial;
        }

        public static RegistryKey GetSource(string path)
        {
            if (path.Contains("HKEY_LOCAL_MACHINE"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);

            } else if (path.Contains("HKEY_CURRENT_USER"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            }
            return null;
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static void ChangePermission(RegistryKey key, string subKeyPath)
        {
            //SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            //NTAccount account = sid.Translate(typeof(NTAccount)) as NTAccount;

            // Get ACL from Windows

            // CHANGED to open the key as writable: using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(key))
            using (RegistryKey rk = key.OpenSubKey(subKeyPath, rights: RegistryRights.ChangePermissions))
            {

                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                RegistryAccessRule rule = new RegistryAccessRule(user,
                       RegistryRights.FullControl,
                       AccessControlType.Allow);
                RegistrySecurity security = new RegistrySecurity();
                security.AddAccessRule(rule);
                //var key2 = key.OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                rk.SetAccessControl(security);
            }
        }
        public static void Import(string regFileContent)
        {

            string[] lines = regFileContent.Replace("\\\r\n", "").Split('\n');
            var path = "";
            foreach (string line in lines)
            {
                // try{
                if (string.IsNullOrWhiteSpace(line)) { continue; }
                string[] parts = line.Split('=');
                if (line.StartsWith("["))
                {
                    path = line.Replace("[", "").Replace("]", "").Trim();
                    if (path.StartsWith("-"))
                    {
                        using (var src = GetSource(path))
                        {

                            if (path.StartsWith("-"))//delete
                            {
                                using (var key = src.OpenSubKey(path.Substring(path.IndexOf("\\")), true))
                                {
                                    key?.DeleteSubKeyTree("");
                                }
                            }

                        }
                    }



                }
                else if (line.Contains("="))//key values
                {
                    using (var src = GetSource(path))
                    {
                        if (parts.Length == 2)
                        {
                            string keyName = parts[0].Replace("\"", "").Trim();
                            string value = parts[1].Replace("\"", "").Trim();

                            try
                            {
                                using (RegistryKey key = src.CreateSubKey(path.Substring(path.IndexOf("\\") + 1).Trim(), true))
                                {
                                    CreateKey(key, keyName, value);
                                }
                            }
                            catch
                            {
                                var subKeyPath = path.Substring(path.IndexOf("\\") + 1).Trim();
                                try
                                {
                                    ChangePermission(src, subKeyPath);
                                }
                                catch { }
                                try
                                {
                                    using (RegistryKey key = src.OpenSubKey(subKeyPath, true))
                                    {
                                        CreateKey(key, keyName, value);
                                    }
                                }
                                catch { }
                            }
                        }
                    }


                }

                //}catch { }

            }
        }
        public static void CreateKey(RegistryKey key, string keyName, string value)
        {
            if (key == null)
                return;
            var hexD = "hex(0):";
            if (value.StartsWith(hexD))
            {
                var hexStr = value.Replace(hexD, "").Replace(" ", "").Replace(",", "").ToUpper();
                byte[] hexBytes = StringToByteArray(hexStr);
                key?.SetValue(keyName, hexBytes, RegistryValueKind.Binary);
            }
            else
            {
                key?.SetValue(keyName, value);
            }
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        picLoading.Visible = true;
                    });

                    Core.Trial();
                    ProccessManager.RestartIDM();
                    MessageBox.Show("IDM has been reset successfully.");
                    Invoke((MethodInvoker)delegate
                    {
                        picLoading.Visible = false;
                    });
                });
            }
            catch { }
            
            /*return;
            var reset = @"﻿Windows Registry Editor Version 5.00

[-HKEY_CURRENT_USER\Software\Classes\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
[-HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
[-HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
[-HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]

[-HKEY_CURRENT_USER\Software\Classes\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
[-HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
[-HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
[-HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]

[-HKEY_CURRENT_USER\Software\Classes\CLSID\{D5B91409-A8CA-4973-9A0B-59F713D25671}]
[-HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{D5B91409-A8CA-4973-9A0B-59F713D25671}]
[-HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{D5B91409-A8CA-4973-9A0B-59F713D25671}]
[-HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{D5B91409-A8CA-4973-9A0B-59F713D25671}]


[-HKEY_CURRENT_USER\Software\Classes\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
[-HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
[-HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
[-HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]

[-HKEY_CURRENT_USER\Software\Classes\CLSID\{07999AC3-058B-40BF-984F-69EB1E554CA7}]
[-HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{07999AC3-058B-40BF-984F-69EB1E554CA7}]
[-HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{07999AC3-058B-40BF-984F-69EB1E554CA7}]
[-HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{07999AC3-058B-40BF-984F-69EB1E554CA7}]


[HKEY_CURRENT_USER\Software\DownloadManager]
""FName""=-
""LName""=-
""Email""=-
""Serial""=-
[HKEY_LOCAL_MACHINE\Software\Internet Download Manager]
""FName""=-
""LName""=-
""Email""=-
""Serial""=-
[HKEY_LOCAL_MACHINE\Software\Wow6432Node\Internet Download Manager]
""FName""=-
""LName""=-
""Email""=-
""Serial""=-";

            Import(reset);



            var trial = @"Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\DownloadManager]
""Serial""=""""

[HKEY_CURRENT_USER\Software\Classes\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
""scansk""=hex(0):91,1d,ac,d6,90,5c,42,ea,ba,1a,ac,08,1a,18,2f,16,2a,a8,0a,aa,24,bf,\
  0c,fc,4e,7b,3b,76,f7,70,93,58,5c,03,03,7e,04,ab,b0,7e,00,00,00,00,00,00,00,\
  00,00,00
[HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
""scansk""=hex(0):91,1d,ac,d6,90,5c,42,ea,ba,1a,ac,08,1a,18,2f,16,2a,a8,0a,aa,24,bf,\
  0c,fc,4e,7b,3b,76,f7,70,93,58,5c,03,03,7e,04,ab,b0,7e,00,00,00,00,00,00,00,\
  00,00,00
[HKEY_CURRENT_USER\Software\DownloadManager]
""scansk""=hex(0):91,1d,ac,d6,90,5c,42,ea,ba,1a,ac,08,1a,18,2f,16,2a,a8,0a,aa,24,bf,\
  0c,fc,4e,7b,3b,76,f7,70,93,58,5c,03,03,7e,04,ab,b0,7e,00,00,00,00,00,00,00,\
  00,00,00
[HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
""scansk""=hex(0):91,1d,ac,d6,90,5c,42,ea,ba,1a,ac,08,1a,18,2f,16,2a,a8,0a,aa,24,bf,\
  0c,fc,4e,7b,3b,76,f7,70,93,58,5c,03,03,7e,04,ab,b0,7e,00,00,00,00,00,00,00,\
  00,00,00
[HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}]
""scansk""=hex(0):91,1d,ac,d6,90,5c,42,ea,ba,1a,ac,08,1a,18,2f,16,2a,a8,0a,aa,24,bf,\
  0c,fc,4e,7b,3b,76,f7,70,93,58,5c,03,03,7e,04,ab,b0,7e,00,00,00,00,00,00,00,\
  00,00,00";

            Import(trial);

            var register = @"Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\DownloadManager]
""FName""=""IDM trial reset""
""LName""=""(http://bit.ly/IDMresetTrialForum)""
""Email""=""your@email.com""
""Serial""=""9QNBL-L2641-Y7WVE-QEN3I""

[HKEY_CURRENT_USER\Software\Classes\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
""MData""=hex(0):21,9e,ac,77,b5,b5,26,3c,9d,ff,86,40,2d,b9,55,6c,13,17,81,2f,93,54,\
  2e,ab,2c,34,ca,dc,32,1f,a4,b0,c6,cc,4c,83,48,84,2c,1e,68,5f,4d,d7,ac,41,2e,\
  52,5c,6a,4a,78,7c,3b,39,8d,b3,d5,62,d6,a0,e8,12,e5,46,8f,3c,f2,5c,68,ee,21,\
  15,a4,0a,99,ab,bf,d8,2c,5c,77,3b,01,33,e9,9b,4f,12,8e,c4,a7,a1,35,9f,eb,15,\
  a4,0a,99,ab,bf,d8,2c,ef,ac,0d,ee,9b,62,b8,89,1c,42,98,d2,36,ce,b3,9e,e7,56,\
  88,5b,cc,7f,1d,40,34,a2,cd,43,fe,e6,97,15,40,11,6c,23,3f,1a,3c,92,0b,f9,20,\
  e6,17,ac,22,68,8f,45,30,16,84,0d,f4,de,9c,e8,e5,a9,15,5d,d9,1c,22,d2,1b,76,\
  2d,b4,c4,bb,e8,84,71,b7,16,8a,2e,35,a0,a8,66,49,b7,1a,ec,38,0b,5f,4e,35,4e,\
  59,31,63,cd,d2,af,85,4e,90,32,ea,15,44,53,e0,8d,7b,af,34,b8,fe,c8,ec,2c,ef,\
  8a,26,01,77,38,5b,df,31,59,65,36,d8,51,ef,7f,20,6d,43,d6,c2,e8,d6,17,18,16,\
  a4,d0,f3,ea,f7,83,c5,55,00
[HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
""MData""=hex(0):21,9e,ac,77,b5,b5,26,3c,9d,ff,86,40,2d,b9,55,6c,13,17,81,2f,93,54,\
  2e,ab,2c,34,ca,dc,32,1f,a4,b0,c6,cc,4c,83,48,84,2c,1e,68,5f,4d,d7,ac,41,2e,\
  52,5c,6a,4a,78,7c,3b,39,8d,b3,d5,62,d6,a0,e8,12,e5,46,8f,3c,f2,5c,68,ee,21,\
  15,a4,0a,99,ab,bf,d8,2c,5c,77,3b,01,33,e9,9b,4f,12,8e,c4,a7,a1,35,9f,eb,15,\
  a4,0a,99,ab,bf,d8,2c,ef,ac,0d,ee,9b,62,b8,89,1c,42,98,d2,36,ce,b3,9e,e7,56,\
  88,5b,cc,7f,1d,40,34,a2,cd,43,fe,e6,97,15,40,11,6c,23,3f,1a,3c,92,0b,f9,20,\
  e6,17,ac,22,68,8f,45,30,16,84,0d,f4,de,9c,e8,e5,a9,15,5d,d9,1c,22,d2,1b,76,\
  2d,b4,c4,bb,e8,84,71,b7,16,8a,2e,35,a0,a8,66,49,b7,1a,ec,38,0b,5f,4e,35,4e,\
  59,31,63,cd,d2,af,85,4e,90,32,ea,15,44,53,e0,8d,7b,af,34,b8,fe,c8,ec,2c,ef,\
  8a,26,01,77,38,5b,df,31,59,65,36,d8,51,ef,7f,20,6d,43,d6,c2,e8,d6,17,18,16,\
  a4,d0,f3,ea,f7,83,c5,55,00
[HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
""MData""=hex(0):21,9e,ac,77,b5,b5,26,3c,9d,ff,86,40,2d,b9,55,6c,13,17,81,2f,93,54,\
  2e,ab,2c,34,ca,dc,32,1f,a4,b0,c6,cc,4c,83,48,84,2c,1e,68,5f,4d,d7,ac,41,2e,\
  52,5c,6a,4a,78,7c,3b,39,8d,b3,d5,62,d6,a0,e8,12,e5,46,8f,3c,f2,5c,68,ee,21,\
  15,a4,0a,99,ab,bf,d8,2c,5c,77,3b,01,33,e9,9b,4f,12,8e,c4,a7,a1,35,9f,eb,15,\
  a4,0a,99,ab,bf,d8,2c,ef,ac,0d,ee,9b,62,b8,89,1c,42,98,d2,36,ce,b3,9e,e7,56,\
  88,5b,cc,7f,1d,40,34,a2,cd,43,fe,e6,97,15,40,11,6c,23,3f,1a,3c,92,0b,f9,20,\
  e6,17,ac,22,68,8f,45,30,16,84,0d,f4,de,9c,e8,e5,a9,15,5d,d9,1c,22,d2,1b,76,\
  2d,b4,c4,bb,e8,84,71,b7,16,8a,2e,35,a0,a8,66,49,b7,1a,ec,38,0b,5f,4e,35,4e,\
  59,31,63,cd,d2,af,85,4e,90,32,ea,15,44,53,e0,8d,7b,af,34,b8,fe,c8,ec,2c,ef,\
  8a,26,01,77,38,5b,df,31,59,65,36,d8,51,ef,7f,20,6d,43,d6,c2,e8,d6,17,18,16,\
  a4,d0,f3,ea,f7,83,c5,55,00
[HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{6DDF00DB-1234-46EC-8356-27E7B2051192}]
""MData""=hex(0):21,9e,ac,77,b5,b5,26,3c,9d,ff,86,40,2d,b9,55,6c,13,17,81,2f,93,54,\
  2e,ab,2c,34,ca,dc,32,1f,a4,b0,c6,cc,4c,83,48,84,2c,1e,68,5f,4d,d7,ac,41,2e,\
  52,5c,6a,4a,78,7c,3b,39,8d,b3,d5,62,d6,a0,e8,12,e5,46,8f,3c,f2,5c,68,ee,21,\
  15,a4,0a,99,ab,bf,d8,2c,5c,77,3b,01,33,e9,9b,4f,12,8e,c4,a7,a1,35,9f,eb,15,\
  a4,0a,99,ab,bf,d8,2c,ef,ac,0d,ee,9b,62,b8,89,1c,42,98,d2,36,ce,b3,9e,e7,56,\
  88,5b,cc,7f,1d,40,34,a2,cd,43,fe,e6,97,15,40,11,6c,23,3f,1a,3c,92,0b,f9,20,\
  e6,17,ac,22,68,8f,45,30,16,84,0d,f4,de,9c,e8,e5,a9,15,5d,d9,1c,22,d2,1b,76,\
  2d,b4,c4,bb,e8,84,71,b7,16,8a,2e,35,a0,a8,66,49,b7,1a,ec,38,0b,5f,4e,35,4e,\
  59,31,63,cd,d2,af,85,4e,90,32,ea,15,44,53,e0,8d,7b,af,34,b8,fe,c8,ec,2c,ef,\
  8a,26,01,77,38,5b,df,31,59,65,36,d8,51,ef,7f,20,6d,43,d6,c2,e8,d6,17,18,16,\
  a4,d0,f3,ea,f7,83,c5,55,00

[HKEY_CURRENT_USER\Software\DownloadManager]
""scansk""=hex(0):6f,4e,79,b5,cc,8b,50,bb,f4,b7,e2,6d,2e,38,d2,8b,ad,10,0b,03,a6,\
  1b,53,30,6b,b8,8b,92,d6,04,22,c7,55,b9,a5,33,4d,a8,4e,9b,00,00,00,00,00,00,\
  00,00,00,00
[HKEY_CURRENT_USER\Software\Classes\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
""scansk""=hex(0):6f,4e,79,b5,cc,8b,50,bb,f4,b7,e2,6d,2e,38,d2,8b,ad,10,0b,03,a6,\
  1b,53,30,6b,b8,8b,92,d6,04,22,c7,55,b9,a5,33,4d,a8,4e,9b,00,00,00,00,00,00,\
  00,00,00,00
[HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
""scansk""=hex(0):6f,4e,79,b5,cc,8b,50,bb,f4,b7,e2,6d,2e,38,d2,8b,ad,10,0b,03,a6,\
  1b,53,30,6b,b8,8b,92,d6,04,22,c7,55,b9,a5,33,4d,a8,4e,9b,00,00,00,00,00,00,\
  00,00,00,00
[HKEY_LOCAL_MACHINE\Software\Classes\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
""scansk""=hex(0):6f,4e,79,b5,cc,8b,50,bb,f4,b7,e2,6d,2e,38,d2,8b,ad,10,0b,03,a6,\
  1b,53,30,6b,b8,8b,92,d6,04,22,c7,55,b9,a5,33,4d,a8,4e,9b,00,00,00,00,00,00,\
  00,00,00,00
[HKEY_LOCAL_MACHINE\Software\Classes\Wow6432Node\CLSID\{7B8E9164-324D-4A2E-A46D-0165FB2000EC}]
""scansk""=hex(0):6f,4e,79,b5,cc,8b,50,bb,f4,b7,e2,6d,2e,38,d2,8b,ad,10,0b,03,a6,\
  1b,53,30,6b,b8,8b,92,d6,04,22,c7,55,b9,a5,33,4d,a8,4e,9b,00,00,00,00,00,00,\
  00,00,00,00";

            Import(register);*/

        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        picLoading.Visible = true;
                    });
                    Core.Register();
                    ProccessManager.RestartIDM();
                    MessageBox.Show("IDM has been registered successfully.");
                    Invoke((MethodInvoker)delegate
                    {
                        picLoading.Visible = false;
                    });
                });
            }
            catch { }
        }


        private readonly string[] _noisyWindowsTitles=new string[]{ 
            "Internet Download Managerتتوفر نسخة جديدة من",
            "New version of Internet Download Manager is available",
            "Internet Download Manager Registration",
            "Internet Download Manager تسجيل",
        };

        private async void tmrWindow_Tick(object sender, EventArgs e)
        {
            //Auto Reset Trial
            try
            {
                if (Core.AutoResetTrial)
                {
                    if ((DateTime.Now - Core.LastReset).Days > 15)
                    {
                        await Task.Run(() => Core.Trial());

                    }
                }
            }
            catch { }
            //Auto Close Noisy Windows
            try
            {
                tmrWindow.Enabled = false;
                foreach (var title in _noisyWindowsTitles)
                {
                    var handle = WindowManager.FindWindowByTitle(title);
                    if (handle != IntPtr.Zero)
                    {
                        if (WindowManager.BringWindowToForeground(handle))
                        {
                            SendKeys.Send("{ESC}");
                            if (WindowManager.CloseWindowByHandle(handle))
                            {
                                //send esc to the window

                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                tmrWindow.Enabled = true;
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.nIcon.ShowBalloonTip(3000);
            this.Hide();
        }

        private void nIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if(this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
            
        }

        private void chbAutoResetTrial_CheckedChanged(object sender, EventArgs e)
        {
            Core.SetAutoResetTrial(chbAutoResetTrial.Checked);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}
