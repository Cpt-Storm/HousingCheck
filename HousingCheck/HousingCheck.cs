using System;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using System.Text;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin;
using FFXIV_ACT_Plugin.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Xml;
using System.Collections.ObjectModel;

public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }
}

namespace HousingCheck
{
    
    public class HousingCheck : IActPluginV1
    {
        public ObservableCollection<HousingItem> HousingList = new ObservableCollection<HousingItem>();
        public BindingSource bindingSource1;
        FFXIV_ACT_Plugin.FFXIV_ACT_Plugin ffxivPlugin;
        bool initialized = false;
        bool OtterUploadFlag = false;       
        string OtterText = "";      //上报队列
        private BackgroundWorker OtterThread;
        Label statusLabel;
        PluginControl control;
        string[] houseID_List = new string[] { "海雾村1", "海雾村2", "海雾村4", "海雾村5", "海雾村6", "海雾村7", "海雾村14", "海雾村15", "海雾村29", "海雾村30", "海雾村31", "海雾村32", "海雾村34", "海雾村35", "海雾村36", "海雾村37", "海雾村44", "海雾村45", "海雾村59", "海雾村60", "薰衣草苗圃1", "薰衣草苗圃3", "薰衣草苗圃5", "薰衣草苗圃6", "薰衣草苗圃11", "薰衣草苗圃16", "薰衣草苗圃21", "薰衣草苗圃27", "薰衣草苗圃28", "薰衣草苗圃30", "薰衣草苗圃31", "薰衣草苗圃33", "薰衣草苗圃35", "薰衣草苗圃36", "薰衣草苗圃41", "薰衣草苗圃46", "薰衣草苗圃51", "薰衣草苗圃57", "薰衣草苗圃58", "薰衣草苗圃60", "高脚孤丘4", "高脚孤丘5", "高脚孤丘6", "高脚孤丘8", "高脚孤丘11", "高脚孤丘12", "高脚孤丘13", "高脚孤丘19", "高脚孤丘25", "高脚孤丘30", "高脚孤丘34", "高脚孤丘35", "高脚孤丘36", "高脚孤丘38", "高脚孤丘41", "高脚孤丘42", "高脚孤丘43", "高脚孤丘49", "高脚孤丘55", "高脚孤丘60", "白银乡1", "白银乡7", "白银乡8", "白银乡13", "白银乡15", "白银乡16", "白银乡19", "白银乡24", "白银乡28", "白银乡30", "白银乡31", "白银乡37", "白银乡38", "白银乡43", "白银乡45", "白银乡46", "白银乡49", "白银乡54", "白银乡58", "白银乡60" };
        string[] teleportList = new string[] { "直接跑过去", "海雾村西北区", "海雾村南区码头", "海雾村南区码头", "海雾村南区码头", "海雾村南区码头", "海雾村东北区", "海雾村东北区", "海雾村东南区", "海雾村东南区", "[扩建区]雾门广场", "[扩建区]海雾村东北区", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村东南区", "[扩建区]海雾村东南区", "[扩建区]海雾村西南区", "[扩建区]海雾村西南区", "薰衣草苗圃东区", "树冠商店街（住宅管理人）", "薰衣草苗圃东区", "薰衣草苗圃东区", "直接游过去比较快", "薰衣草苗圃西南区", "芳草商店街", "薰衣草苗圃西北区", "薰衣草苗圃西北区", "树冠商店街（住宅管理人）", "[扩建区] 薰衣草苗圃南区", "[扩建区]树冠商店街（住宅管理人）", "[扩建区] 薰衣草苗圃南区", "[扩建区] 薰衣草苗圃南区", "[扩建区]薰衣草苗圃西南区", "[扩建区]薰衣草苗圃西北区", "[扩建区]芳草商店街", "[扩建区] 薰衣草苗圃东北区", "[扩建区] 薰衣草苗圃东北区", "[扩建区]树冠商店街（住宅管理人）", "高脚市场（住宅管理人）", "高脚孤丘西区", "高脚孤丘西区", "高脚孤丘西区", "娜娜莫大风车", "娜娜莫大风车", "娜娜莫大风车", "雄心广场", "高脚孤丘东区", "高脚孤丘东南区", "[扩建区]高脚市场（住宅管理人）", "[扩建区]高脚孤丘北区", "[扩建区]高脚孤丘北区", "[扩建区]高脚孤丘北区", "[扩建区]娜娜莫大风车", "[扩建区]娜娜莫大风车", "[扩建区]娜娜莫大风车", "[扩建区]雄心广场", "[扩建区]高脚孤丘南区", "[扩建区]高脚孤丘西南区", "白银乡南区", "白银乡东南区", "白银乡东南区", "白银乡东北区", "白银乡东北区", "白银乡东北区", "建议直接跑过去", "白银乡西区", "白银乡西北区", "白银乡西北区", "[扩建区]白银乡西区", "[扩建区]白银乡西南区", "[扩建区]白银乡西南区", "[扩建区]白银乡南区", "[扩建区]白银乡南区", "[扩建区]白银乡南区", "[扩建区]茜云栈桥（住宅管理人）", "[扩建区]白银乡东北区", "[扩建区]白银乡东区", "[扩建区]白银乡东区" };


        private object GetFfxivPlugin()
        {
            ffxivPlugin = null;

            var plugins = ActGlobals.oFormActMain.ActPlugins;
            foreach (var plugin in plugins)
                if (plugin.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    plugin.lblPluginStatus.Text.ToUpper().Contains("FFXIV Plugin Started.".ToUpper()))
                    ffxivPlugin = (FFXIV_ACT_Plugin.FFXIV_ACT_Plugin)plugin.pluginObj;

            if (ffxivPlugin == null)
                throw new Exception("Could not find FFXIV plugin. Make sure that it is loaded before CutsceneSkip.");

            return ffxivPlugin;
        }
        public void DeInitPlugin()
        {
            if (initialized)
            {
                var subs = ffxivPlugin.GetType().GetProperty("DataSubscription").GetValue(ffxivPlugin, null);
                var networkReceivedDelegateType = typeof(NetworkReceivedDelegate);
                var networkReceivedDelegate = Delegate.CreateDelegate(networkReceivedDelegateType, (object)this, "NetworkReceived", true);
                subs.GetType().GetEvent("NetworkReceived").RemoveEventHandler(subs, networkReceivedDelegate);
                OtterThread.CancelAsync();
                control.SaveSettings();
                statusLabel.Text = "Exit :|";
            }
            else
            {
                statusLabel.Text = "Error :(";
            }
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            statusLabel = pluginStatusText;
            GetFfxivPlugin();
            control = new PluginControl();
            pluginScreenSpace.Text = "Housing Check";
            bindingSource1 = new BindingSource { DataSource = HousingList };
            control.dataGridView1.DataSource = bindingSource1;
            pluginScreenSpace.Controls.Add(control);

            var subs = ffxivPlugin.GetType().GetProperty("DataSubscription").GetValue(ffxivPlugin, null);
            var networkReceivedDelegateType = typeof(NetworkReceivedDelegate);
            var networkReceivedDelegate = Delegate.CreateDelegate(networkReceivedDelegateType, (object)this, "NetworkReceived", true);
            subs.GetType().GetEvent("NetworkReceived").AddEventHandler(subs, networkReceivedDelegate);
            initialized = true;
            OtterThread = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            OtterThread.DoWork += OtterUpload;
            OtterThread.RunWorkerAsync();
            statusLabel.Text = "Working :D";
            control.LoadSettings();
            control.buttonUploadOnce.Click += ButtonUploadOnce_Click;
            control.buttonCopyToClipboard.Click += ButtonCopyToClipboard_Click;
            control.buttonSaveToFile.Click += ButtonSaveToFile_Click;
        }


        void Log(string type, string message)
        {
            var time = (DateTime.Now).ToString("HH:mm:ss");
            var text = $"[{time}] [{type}] {message.Trim()}";
            control.textBoxLog.Text += text + Environment.NewLine;
            control.textBoxLog.SelectionStart = control.textBoxLog.TextLength;
            control.textBoxLog.ScrollToCaret(); 
            text = $"00|{DateTime.Now.ToString("O")}|0|HousingCheck-{message}|";        //解析插件数据格式化
            ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, $"{text}");    //插入ACT日志
        }

        void NetworkReceived(string connection, long epoch, byte[] message)
        {
            var opcode = BitConverter.ToUInt16(message, 18);
            if (opcode != 0x164 && message.Length != 2440) return;
            var data_list = message.SubArray(32, message.Length - 32);
            var data_header = data_list.SubArray(0, 8);
            string area = "";
            if (data_header[4] == 0x53)
                area = "海雾村";
            else if (data_header[4] == 0x54)
                area = "薰衣草苗圃";
            else if (data_header[4] == 0x55)
                area = "高脚孤丘";
            else if (data_header[4] == 0x81)
                area = "白银乡";
            int slot = data_header[2];
            for (int i = 8; i < data_list.Length; i += 40)
            {
                var house_id = (i - 8) / 40;
                var name_header = data_list.SubArray(i, 8);
                int price = BitConverter.ToInt32(name_header, 0);
                string size = (price > 30000000) ? "L" : ((price > 10000000) ? "M" : "S");
                var name_array = data_list.SubArray(i + 8, 32);
                if (name_array[0] == 0)
                {
                    string text = $"{area} 第{slot + 1}区 {house_id + 1}号 {size}房在售 当前价格:{price} {Environment.NewLine}";
                    Log("Info", text);
                    var housignItem = new HousingItem(
                            area,
                            slot + 1,
                            house_id + 1,
                            size,
                            price
                        );
                    if(HousingList.IndexOf(housignItem) == -1)
                    {
                        bindingSource1.Add(housignItem);
                    }
                    else
                    {
                        HousingList[HousingList.IndexOf(housignItem)].ExistenceTime = DateTime.Now;     //更新时间
                        HousingList[HousingList.IndexOf(housignItem)].Price = price;                    //更新价格
                        Log("Info", "重复土地，已更新。");
                    }
                    if(size == "M" || size == "L")
                    {
                        string teleport = teleportList[Array.IndexOf(houseID_List, $"{area}{house_id + 1}")];
                        OtterText = "[CQ:at,qq=all]" + "传送区域推荐：" + teleport + Environment.NewLine;
                        Console.Beep(3000, 1000);
                    }
                    if (control.upload)
                    {
                        if (!control.checkBoxML.Checked || (size == "M" || size == "L"))
                        {
                            OtterText += text;
                            OtterUploadFlag = true;
                        }
                    }
                }
            }
            Log("Info", $"查询第{slot + 1}区");     //输出翻页日志
        }

        private void ButtonUploadOnce_Click(object sender, EventArgs e)
        {
            Log("Info", $"准备上报");
            OtterText = ListToString();
            OtterUploadFlag = true;
        }

        private void ButtonCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ListToString());
            Log("Info", $"复制成功");
        }

        private void ButtonSaveToFile_Click(object sender, EventArgs e)
        {
            string fileName = $"HousingCheck-{DateTime.Now.ToString("u").Replace(":", "").Replace(" ", "").Replace("-", "")}.txt";
            File.AppendAllText(
                Path.Combine(Environment.CurrentDirectory, fileName),   //ACT根目录
                ListToString()
                );
            Log("Info", $"已保存到{Environment.CurrentDirectory}, {fileName}");
            //Log("Debug", fileName);
        }

        private string ListToString()
        {
            ArrayList area = new ArrayList(new string[] { "海雾村", "薰衣草苗圃", "高脚孤丘", "白银乡" });
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var line in HousingList)
            {
                stringBuilder.Append($"{line.Area} 第{line.Slot}区 {line.Id}号{line.Size}房在售，当前价格:{line.Price} {Environment.NewLine}");

                if (line.Area == "海雾村" && area.IndexOf("海雾村") != -1)
                {
                    area.Remove("海雾村");
                }
                else if (line.Area == "薰衣草苗圃" && area.IndexOf("薰衣草苗圃") != -1)
                {
                    area.Remove("薰衣草苗圃");
                }
                else if (line.Area == "高脚孤丘" && area.IndexOf("高脚孤丘") != -1)
                {
                    area.Remove("高脚孤丘");
                }
                else if (line.Area == "白银乡" && area.IndexOf("白银乡") != -1)
                {
                    area.Remove("白银乡");
                }
            }
            foreach (var line in area)
            {
                stringBuilder.Append($"{line} 无空房 {Environment.NewLine}");
            }

            return stringBuilder.ToString();
        }

        private void OtterUpload(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (OtterThread.CancellationPending)
                {
                    break;
                }
                if (OtterUploadFlag)
                {
                    try
                    {
                        string urls = control.textBoxUpload.Text;
                        foreach (string url in urls.Split('\n'))
                        {
                            string post_url = url.Trim();
                            if (post_url == "") continue;
                            Log("Info", $"上报消息给 {post_url}");
                            var wb = new WebClient();
                            var data = new NameValueCollection
                            {
                                { "text", OtterText }
                            };
                            var response = wb.UploadValues(post_url, "POST", data);
                            string responseInString = Encoding.UTF8.GetString(response);
                            if (responseInString == "OK")
                            {
                                OtterText = "";
                                OtterUploadFlag = false;
                                Log("Info", "上报成功");
                            }
                            else
                            {
                                Log("Error", "上报失败");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Log("Error", "上报失败: " + ex.Message);
                    }
                    Thread.Sleep(5100);
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }

    }
}
