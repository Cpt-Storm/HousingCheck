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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

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
        public ObservableCollection<HousingItemEX> HousingListEX = new ObservableCollection<HousingItemEX>();
        public ObservableCollection<HousingItemLite> HousingListLite = new ObservableCollection<HousingItemLite>();
        public ObservableCollection<HousingItemLite> lites = new ObservableCollection<HousingItemLite>();               //中转存储
        public ObservableCollection<HousingItemLite> ListCache = new ObservableCollection<HousingItemLite>();
        public DateTime CacheCreateTime;
        public int dataCount;
        public BindingSource bindingSource1;
        FFXIV_ACT_Plugin.FFXIV_ACT_Plugin ffxivPlugin;
        bool initialized = false;
        bool OtterUploadFlag = false;       
        string OtterText = "";      //上报队列
        private BackgroundWorker OtterThread;
        Label statusLabel;
        PluginControl control;
        readonly string[] houseID_List = new string[] { "海雾村1", "海雾村2", "海雾村4", "海雾村5", "海雾村6", "海雾村7", "海雾村14", "海雾村15", "海雾村29", "海雾村30", "海雾村31", "海雾村32", "海雾村34", "海雾村35", "海雾村36", "海雾村37", "海雾村44", "海雾村45", "海雾村59", "海雾村60", "薰衣草苗圃1", "薰衣草苗圃3", "薰衣草苗圃5", "薰衣草苗圃6", "薰衣草苗圃11", "薰衣草苗圃16", "薰衣草苗圃21", "薰衣草苗圃27", "薰衣草苗圃28", "薰衣草苗圃30", "薰衣草苗圃31", "薰衣草苗圃33", "薰衣草苗圃35", "薰衣草苗圃36", "薰衣草苗圃41", "薰衣草苗圃46", "薰衣草苗圃51", "薰衣草苗圃57", "薰衣草苗圃58", "薰衣草苗圃60", "高脚孤丘4", "高脚孤丘5", "高脚孤丘6", "高脚孤丘8", "高脚孤丘11", "高脚孤丘12", "高脚孤丘13", "高脚孤丘19", "高脚孤丘25", "高脚孤丘30", "高脚孤丘34", "高脚孤丘35", "高脚孤丘36", "高脚孤丘38", "高脚孤丘41", "高脚孤丘42", "高脚孤丘43", "高脚孤丘49", "高脚孤丘55", "高脚孤丘60", "白银乡1", "白银乡7", "白银乡8", "白银乡13", "白银乡15", "白银乡16", "白银乡19", "白银乡24", "白银乡28", "白银乡30", "白银乡31", "白银乡37", "白银乡38", "白银乡43", "白银乡45", "白银乡46", "白银乡49", "白银乡54", "白银乡58", "白银乡60" };
        readonly string[] teleportList = new string[] { "直接跑过去", "海雾村西北区", "海雾村南区码头", "海雾村南区码头", "海雾村南区码头", "海雾村南区码头", "海雾村东北区", "海雾村东北区", "海雾村东南区", "海雾村东南区", "[扩建区]雾门广场", "[扩建区]海雾村东北区", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村西北区码头", "[扩建区]海雾村东南区", "[扩建区]海雾村东南区", "[扩建区]海雾村西南区", "[扩建区]海雾村西南区", "薰衣草苗圃东区", "树冠商店街（住宅管理人）", "薰衣草苗圃东区", "薰衣草苗圃东区", "直接游过去比较快", "薰衣草苗圃西南区", "芳草商店街", "薰衣草苗圃西北区", "薰衣草苗圃西北区", "树冠商店街（住宅管理人）", "[扩建区] 薰衣草苗圃南区", "[扩建区]树冠商店街（住宅管理人）", "[扩建区] 薰衣草苗圃南区", "[扩建区] 薰衣草苗圃南区", "[扩建区]薰衣草苗圃西南区", "[扩建区]薰衣草苗圃西北区", "[扩建区]芳草商店街", "[扩建区] 薰衣草苗圃东北区", "[扩建区] 薰衣草苗圃东北区", "[扩建区]树冠商店街（住宅管理人）", "高脚市场（住宅管理人）", "高脚孤丘西区", "高脚孤丘西区", "高脚孤丘西区", "娜娜莫大风车", "娜娜莫大风车", "娜娜莫大风车", "雄心广场", "高脚孤丘东区", "高脚孤丘东南区", "[扩建区]高脚市场（住宅管理人）", "[扩建区]高脚孤丘北区", "[扩建区]高脚孤丘北区", "[扩建区]高脚孤丘北区", "[扩建区]娜娜莫大风车", "[扩建区]娜娜莫大风车", "[扩建区]娜娜莫大风车", "[扩建区]雄心广场", "[扩建区]高脚孤丘南区", "[扩建区]高脚孤丘西南区", "白银乡南区", "白银乡东南区", "白银乡东南区", "白银乡东北区", "白银乡东北区", "白银乡东北区", "建议直接跑过去", "白银乡西区", "白银乡西北区", "白银乡西北区", "[扩建区]白银乡西区", "[扩建区]白银乡西南区", "[扩建区]白银乡西南区", "[扩建区]白银乡南区", "[扩建区]白银乡南区", "[扩建区]白银乡南区", "[扩建区]茜云栈桥（住宅管理人）", "[扩建区]白银乡东北区", "[扩建区]白银乡东区", "[扩建区]白银乡东区" };
        readonly char[] charsToTrim = { '\0', '\r', '\n', ' ' };
        readonly Regex regex = new Regex("(\0|\a|\b|\t|\n|\v|\f|\r|[\x01-\x1F])");
        bool isLimitReleased = false;
        bool MoreDetailFlag = false;

        private object GetFfxivPlugin()
        {
            ffxivPlugin = null;

            var plugins = ActGlobals.oFormActMain.ActPlugins;
            foreach (var plugin in plugins)
                if (plugin.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_Plugin".ToUpper()) &&
                    plugin.lblPluginStatus.Text.ToUpper().Contains("FFXIV Plugin Started.".ToUpper()))
                    ffxivPlugin = (FFXIV_ACT_Plugin.FFXIV_ACT_Plugin)plugin.pluginObj;

            if (ffxivPlugin == null)
                throw new Exception("Could not find FFXIV plugin. Make sure that it is loaded before HousingCheck.");

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
                if (control.checkBoxAutoSaveAndLoad.Checked == true)
                {
                    JsonSave();
                }
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
            control.buttonJsonSave.Click += ButtonJsonSave_Click;
            control.buttonJsonLoad.Click += ButtonJsonLoad_Click;
            control.buttonCacheItemList.Click += ButtonCacheItemList_Click;
            control.buttonRestoreListFromCache.Click += ButtonRestoreListFromCache_Click;
            control.buttonClearItemList.Click += ButtonClearItemList_Click;
            control.buttonCompare.Click += ButtonCompare_Click;
            control.buttonSaveCache.Click += ButtonSaveCache_Click;
            control.buttonReadCache.Click += ButtonReadCache_Click;

            control.buttondump.Click += Buttondump_Click;

            if (control.checkBoxAutoSaveAndLoad.Checked == true)
            {
                JsonLoad();
            }
            string tips = "本插件免费，发布及更新地址 https://file.bluefissure.com/FFXIV/ 或 https://bbs.nga.cn/read.php?tid=25465725 ，勿从其他渠道（闲鱼卖家或神秘群友）获取以避免虚拟财产受到损失。";
            MessageBox.Show(tips);
            Log("Info", tips);

            isLimitReleased = control.checkBoxLimitMode.Checked;
            control.buttonSaveDetailToFile.Click += ButtonSaveDetailToFile_Click;
        }

        private void Buttondump_Click(object sender, EventArgs e)
        {
            foreach (var item in HousingListLite)
            {
                if (item != null)
                {
                    lites.Add(item);
                }
            }
            var list = lites.OrderBy(lites => lites.Area).ThenBy(lites => lites.Slot).ThenBy(lites => lites.Id).ToList();
            HousingListLite.Clear();
            foreach (var item in list)
            {
                HousingListLite.Add(item);
            }
            using (StreamWriter file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "dump.json")))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, HousingListLite);
                Log("Debug", $"Lite列表已保存");
            }
            dataCount = HousingListLite.Count;
            control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760 （{((float)dataCount/5760):P}）";
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
            //if (opcode != 284 && message.Length != 2440) return;
            //Log("Debug", $"OPCODE:{opcode}"); return;
            if (isLimitReleased)
            {
                MoreDetailFlag = control.checkBoxDetailRecord.Checked;
            }
            //if (MoreDetailFlag)
            //{
            //    //Log("Info", $"opcode:{opcode}");
            //}
            if (opcode == 284)
            {
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
                        var housingItemLite = new HousingItemLite(
                            area,
                            slot + 1,
                            house_id + 1,
                            size,
                            price.ToString()
                            );
                        if (HousingList.IndexOf(housignItem) == -1)
                        {
                            bindingSource1.Add(housignItem);
                            if (MoreDetailFlag)
                            {
                                var HousingItemEX = new HousingItemEX(area, slot + 1, house_id + 1, "", "", false, false, price, "在售", "", "", "", "", DateTime.MinValue, 0, 0, 0, "", "", $"土地出售中，价格：{price}");
                                if (HousingListEX.IndexOf(HousingItemEX) == -1)
                                {
                                    HousingListEX.Add(HousingItemEX);
                                }
                            }
                        }
                        else
                        {
                            HousingList[HousingList.IndexOf(housignItem)].ExistenceTime = DateTime.Now;     //更新时间
                            HousingList[HousingList.IndexOf(housignItem)].Price = price;                    //更新价格
                            Log("Info", "重复土地，已更新。");
                            control.dataGridView1.Refresh();                                                //刷新控件
                        }
                        if (size == "M" || size == "L")
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
                        if (HousingListLite.IndexOf(housingItemLite) == -1)
                        {
                            HousingListLite.Add(housingItemLite);
                        }
                        else
                        {
                            HousingListLite[HousingListLite.IndexOf(housingItemLite)].Name = price.ToString();
                            HousingListLite[HousingListLite.IndexOf(housingItemLite)].AddTime = DateTime.Now;     //更新时间
                        }
                    }
                    else
                    {
                        string name = Encoding.UTF8.GetString(name_array).Trim(charsToTrim);
                        int flag = name_header[4];
                        bool is_fc = (flag & 0b10000) > 0;
                        if (is_fc)
                        {
                            name = "《" + name + "》";
                        }
                        var housingItemLite = new HousingItemLite(
                            area,
                            slot + 1,
                            house_id + 1,
                            size,
                            name
                            );
                        if (HousingListLite.IndexOf(housingItemLite) == -1)
                        {
                            HousingListLite.Add(housingItemLite);
                        }
                        else
                        {
                            HousingListLite[HousingListLite.IndexOf(housingItemLite)].Name = name;
                            HousingListLite[HousingListLite.IndexOf(housingItemLite)].AddTime = DateTime.Now;
                        }
                    }
                }
                Log("Info", $"查询第{slot + 1}区");     //输出翻页日志
                dataCount = HousingListLite.Count;
                control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760 （{((float)dataCount/5760):P}）";
                return;
            }

            #region 额外信息获取

            if (opcode == 586)      //住房资料页面
            {
                if (!MoreDetailFlag)
                {
                    return;
                }
                var HousingData = new HousingData();
                var data = message.SubArray(32, message.Length - 32);

                uint area = data[4];
                int slot = BitConverter.ToUInt16(data, 2);
                int houseID = BitConverter.ToUInt16(data, 0);
                bool isOpen = (data[20] == 1);
                string size = (data[21] == 2) ? "L" : ((data[21] == 1) ? "M" : "S");
                string houseName = Encoding.UTF8.GetString(data.SubArray(23, 21)).Trim();
                houseName = regex.Replace(houseName, "");
                string houseDescription = Encoding.UTF8.GetString(data.SubArray(46, 192)).Trim();
                houseDescription = regex.Replace(houseDescription, "");
                string ownerName = Encoding.UTF8.GetString(data.SubArray(239, 30)).Trim();
                ownerName = regex.Replace(ownerName, "");
                string ownerNick = Encoding.UTF8.GetString(data.SubArray(270, 7)).Trim();
                ownerNick = regex.Replace(ownerNick, "");

                bool isFC = (BitConverter.ToString(data.SubArray(14, 1)) == "0A");

                string areaName = HousingData.HousingArea(area);
                //var housetag1 = lumina.GetExcelSheet<HousingAppeal>(Lumina.Data.Language.ChineseSimplified).GetRow((uint)data[277]);
                //var housetag2 = lumina.GetExcelSheet<HousingAppeal>(Lumina.Data.Language.ChineseSimplified).GetRow((uint)data[278]);
                //var housetag3 = lumina.GetExcelSheet<HousingAppeal>(Lumina.Data.Language.ChineseSimplified).GetRow((uint)data[279]);
                string housetag1 = HousingData.HousingAppeal((uint)data[277]);
                string housetag2 = HousingData.HousingAppeal((uint)data[278]);
                string housetag3 = HousingData.HousingAppeal((uint)data[279]);

                //var HousingItemEX = new HousingItemEX(areaName, slot + 1, houseID + 1, size, houseName, isOpen, 0, ownerName, $"{ housetag1.Tag}", $"{ housetag2.Tag}", $"{ housetag3.Tag}", DateTime.MinValue, 0, 0, 0, "", ownerNick);
                var HousingItemEX = new HousingItemEX(areaName, slot + 1, houseID + 1, size, houseName, isOpen, isFC, 0, ownerName, $"{ housetag1}", $"{ housetag2}", $"{ housetag3}", "", DateTime.MinValue, 0, 0, 0, "", ownerNick, houseDescription);



                if (HousingListEX.IndexOf(HousingItemEX) == -1)
                {
                    if (isFC)
                    {
                        HousingItemEX.Owner_Name = $"《{ownerNick}》";
                        HousingListEX.Add(HousingItemEX);
                        Log("Info", $"查询{areaName}第{slot + 1}区{houseID + 1}号房，部队房");     //输出翻页日志
                    }
                    else
                    {
                        HousingListEX.Add(HousingItemEX);
                        Log("Info", $"查询{areaName}第{slot + 1}区{houseID + 1}号房，个人房");     //输出翻页日志
                    }
                }
                else
                {
                    if (isFC)
                    {
                        Log("Info", $"查询{areaName}第{slot + 1}区{houseID + 1}号房，部队房，记录已存在");     //输出翻页日志
                    }
                    else
                    {
                        Log("Info", $"查询{areaName}第{slot + 1}区{houseID + 1}号房，个人房，记录已存在");     //输出翻页日志
                    }
                }
                return;
            }

            if (opcode == 170)      //部队信息页面
            {
                if (!MoreDetailFlag)
                {
                    return;
                }

                var HousingData = new HousingData();
                var data = message.SubArray(32, message.Length - 32);

                int houseID = BitConverter.ToUInt16(data, 24);
                int slot = BitConverter.ToUInt16(data, 26);
                uint area = (uint)data[28];
                //Log("Debug", $"Area:{area}");
                long fc_TimeCreatedUNIX = BitConverter.ToInt32(data, 36);
                DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(fc_TimeCreatedUNIX);
                DateTime fcTimeCreated = dto.LocalDateTime;

                int fcPupulation = BitConverter.ToUInt16(data, 44);
                int fcOnline = BitConverter.ToUInt16(data, 46);
                int fcLevel = BitConverter.ToUInt16(data, 56);
                
                string fcName = Encoding.UTF8.GetString(data.SubArray(58, 22)).Trim(charsToTrim);
                fcName = regex.Replace(fcName, "");
                string fcTag = Encoding.UTF8.GetString(data.SubArray(80, 7)).Trim(charsToTrim);
                string fcLeader = Encoding.UTF8.GetString(data.SubArray(87, 32)).Trim(charsToTrim);
                fcLeader = regex.Replace(fcLeader, "");

                string areaName = HousingData.HousingArea(area);

                var HousingItemEX = new HousingItemEX(areaName, slot + 1, houseID + 1, "", "", false, true, 0, "", "", "", "", fcLeader, fcTimeCreated, fcPupulation, fcOnline, fcLevel, fcName, fcTag, "");



                if (HousingListEX.IndexOf(HousingItemEX) == -1)
                {
                    Log("Info", $"所查询部队{fcName}的房屋在{areaName},第{slot + 1}区{houseID + 1}号房，未记录");
                }
                else
                {
                    Log("Info", $"所查询部队{fcName}的房屋在{areaName},第{slot + 1}区{houseID + 1}号房，已记录");
                    int index = HousingListEX.IndexOf(HousingItemEX);
                    HousingListEX[index].FC_Leader = fcLeader;
                    HousingListEX[index].FC_TimeCreated = fcTimeCreated;
                    HousingListEX[index].FC_Population = fcPupulation;
                    HousingListEX[index].FC_Online = fcOnline;
                    HousingListEX[index].FC_Level = fcLevel;
                    HousingListEX[index].FC_Name = fcName;
                }
                //string fileName = $"HousingCheck-{DateTime.Now.ToString("u").Replace(":", "").Replace(" ", "").Replace("-", "")}.txt";
                //File.AppendAllText(
                //    Path.Combine(Environment.CurrentDirectory, fileName),   //ACT根目录
                //    fcName
                //    );
                //Log("Info", $"已保存到{Environment.CurrentDirectory}, {fileName}");
                return;
            }

            #endregion

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

        private void ButtonJsonSave_Click(object sender, EventArgs e)
        {
            JsonSave();
        }
        private void ButtonJsonLoad_Click(object sender, EventArgs e)
        {
            JsonLoad();
        }

        private void ButtonCacheItemList_Click(object sender, EventArgs e)
        {
            Log("Info", "缓存中……");
            lites.Clear();
            foreach (var item in HousingListLite)
            {
                if (item != null)
                {
                    lites.Add(item);
                }
            }
            var list = lites.OrderBy(lites => lites.Area).ThenBy(lites => lites.Slot).ThenBy(lites => lites.Id).ToList();
            HousingListLite.Clear();
            foreach (var item in list)
            {
                HousingListLite.Add(item);
            }
            dataCount = HousingListLite.Count;
            control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760 （{((float)dataCount/5760):P}）";
            if (dataCount != 5760)
            {
                Log("Info", $"当前房屋列表不全（{dataCount}/5760），无法写入缓存，请重新查询房区再试");
                return;
            }
            lites.Clear();
            foreach (var item in HousingListLite)
            {
                if (item != null)
                {
                    lites.Add(item);
                }
            }
            list = lites.OrderBy(lites => lites.Area).ThenBy(lites => lites.Slot).ThenBy(lites => lites.Id).ToList();
            ListCache.Clear();
            foreach (var item in list)
            {
                ListCache.Add(item);
            }
            CacheCreateTime = HousingListLite[100].AddTime;
            control.labelCurrentCacheTime.Text = $"当前缓存版本：{CacheCreateTime:u}";
            Log("Info", "缓存成功");
        }

        private void ButtonRestoreListFromCache_Click(object sender, EventArgs e)
        {
            int cacheCount = ListCache.Count;
            if (cacheCount != 5760)
            {
                Log("Info", $"当前缓存条目不正确（{cacheCount}/5760），无法进行推送");
                return;
            }
            HousingListLite.Clear();
            foreach (var item in ListCache)
            {
                if (item != null)
                {
                    HousingListLite.Add(item);
                }
            }
            dataCount = HousingListLite.Count;
            control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760 （{((float)dataCount / 5760):P}）";
            Log("Info", "缓存推送成功");
        }

        private void ButtonClearItemList_Click(object sender, EventArgs e)
        {
            HousingListLite.Clear();
            lites.Clear();
            dataCount = HousingListLite.Count;
            control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760";
            Log("Info", "列表已清理");
        }

        private void ButtonCompare_Click(object sender, EventArgs e)
        {
            Log("Info", "比对中……");
            lites.Clear();
            foreach (var item in HousingListLite)
            {
                if (item != null)
                {
                    lites.Add(item);
                }
            }
            var list = lites.OrderBy(lites => lites.Area).ThenBy(lites => lites.Slot).ThenBy(lites => lites.Id).ToList();
            HousingListLite.Clear();
            foreach (var item in list)
            {
                HousingListLite.Add(item);
            }
            dataCount = HousingListLite.Count;
            int cacheCount = ListCache.Count;
            control.labelCurrentListCount.Text = $"当前房屋列表：{dataCount}/5760 （{((float)dataCount/5760):P}）";
            if (dataCount != 5760 || cacheCount != 5760)
            {
                Log("Info", $"当前房屋列表不全（{dataCount}/5760）或缓存条目不正确（{cacheCount}/5760），无法进行比对，请重新查询房区或重新缓存");
                return;
            }
            var text = $"-------- {DateTime.Now:G} --------";
            control.textBoxCompare.Text += text + Environment.NewLine;
            control.textBoxCompare.SelectionStart = control.textBoxCompare.TextLength;
            control.textBoxCompare.ScrollToCaret();
            for (int i = 0; i < 5760; i++)
            {
                if (HousingListLite[i].Name != ListCache[i].Name)
                {
                    string area;
                    switch (ListCache[i].Area)
                    {
                        case "海雾村":
                            area = "海雾村\u3000\u3000";
                            break;
                        case "薰衣草苗圃":
                            area = "薰衣草苗圃";
                            break;
                        case "高脚孤丘":
                            area = "高脚孤丘\u3000";
                            break;
                        case "白银乡":
                            area = "白银乡\u3000\u3000";
                            break;
                        default:
                            area = "未知区域\u3000";
                            break;
                    }
                    text = $"[{area}]   [{ListCache[i].Slot,2}区]   [{ListCache[i].Id,2}号]   [{ListCache[i].Size}房]   [{ListCache[i].Name}]==>[{HousingListLite[i].Name}]";
                    control.textBoxCompare.Text += text + Environment.NewLine;
                    control.textBoxCompare.SelectionStart = control.textBoxCompare.TextLength;
                    control.textBoxCompare.ScrollToCaret();
                    Log("Info", $"{text}");
                }
            }
            Log("Info", "比对完成");
        }

        private void ButtonReadCache_Click(object sender, EventArgs e)
        {
            if (control.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filePath = control.openFileDialog1.FileName;
                if (File.Exists(filePath))
                {
                    ListCache.Clear();
                    string s = File.ReadAllText(filePath);
                    ObservableCollection<HousingItemLite> TempList = new ObservableCollection<HousingItemLite>();
                    TempList = JsonConvert.DeserializeObject<ObservableCollection<HousingItemLite>>(s);
                    foreach (var item in TempList)
                    {
                        ListCache.Add(item);
                    }
                    //Log("Debug", $"{item.AddTime}");
                    CacheCreateTime = ListCache[100].AddTime;
                    control.labelCurrentCacheTime.Text = $"当前缓存版本：{CacheCreateTime:u}";
                    Log("Info", "读取缓存成功");
                }
                else
                {
                    Log("Info", $"{filePath}不存在");
                }
            }
        }

        private void ButtonSaveCache_Click(object sender, EventArgs e)
        {
            control.saveFileDialog1.FileName = $"HousingCheck-{CacheCreateTime.ToString("u").Replace(":", "").Replace(" ", "").Replace("-", "")}.item";
            int cacheCount = ListCache.Count;
            if (cacheCount != 5760)
            {
                Log("Info", $"当前缓存条目不正确（{cacheCount}/5760），无法保存文件，请重新查询房区或重新缓存");
                return;
            }
            if (control.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (control.saveFileDialog1.FileName != null)
                {
                    File.WriteAllText(control.saveFileDialog1.FileName, JsonConvert.SerializeObject(ListCache));
                    Log("Info", "缓存保存成功");
                }
            }
        }

        private void ButtonSaveDetailToFile_Click(object sender, EventArgs e)
        {
            Log("Info", "数据提取中……");
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var L in HousingListEX)
            {
                stringBuilder.Append($"{L.Area}\t{L.Slot}\t{L.Id}\t{L.Size}\t{L.Name}\t" +

                                        $"{((L.IsOpen) ? "开放" : "关闭")}\t{L.Tag1}\t{L.Tag2}\t{L.Tag3}\t" +

                                        $"{((L.IsFC) ? "部队房" : "个人房")}\t{L.Owner_Name}\t{L.House_Description} \t" +

                                        $"{((L.IsFC) ? ($"{L.FC_Leader}\t{L.FC_Name}\t{L.FC_Level}\t{L.FC_Population}\t{L.FC_Online}\t{L.FC_TimeCreated:F} ") : "")}" +     //{if(IsFC) {output FC infomation} else {output empty} }

                                        $"{Environment.NewLine}");
            }
            Log("Info", "数据提取完成");
            string fileName = $"HousingCheck-{DateTime.Now.ToString("u").Replace(":", "").Replace(" ", "").Replace("-", "")}.txt";
            File.AppendAllText(
                Path.Combine(Environment.CurrentDirectory, fileName),   //ACT根目录
                stringBuilder.ToString()
                );
            Log("Info", $"已保存到{Environment.CurrentDirectory}, {fileName}");

        }


        private string ListToString()
        {
            ArrayList area = new ArrayList(new string[] { "海雾村", "薰衣草苗圃", "高脚孤丘", "白银乡" });
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (var line in HousingList)
            {
                bool isTimedout = DateTime.Compare(line.ExistenceTime.AddMinutes((double)control.numericUpDownTimeout.Value), DateTime.Now) <= 0;
                if (isTimedout && control.numericUpDownTimeout.Value != 0)
                {
                    continue;
                }

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

        private void JsonSave()
        {
            HousingList.OrderBy(HousingList => HousingList.Area).ThenBy(HousingList => HousingList.Slot).ThenBy(HousingList => HousingList.Id);

            using (StreamWriter file = File.CreateText(control.ItemsFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, HousingList);
                Log("Info", $"列表已保存");
            }
        }

        private void JsonLoad()
        {
            if (File.Exists(control.ItemsFile))
            {
                string s = File.ReadAllText(control.ItemsFile);
                ObservableCollection<HousingItem> TempList = new ObservableCollection<HousingItem>();
                TempList = JsonConvert.DeserializeObject<ObservableCollection<HousingItem>>(s);
                foreach (var item in TempList)
                {
                    if (HousingList.IndexOf(item) == -1)
                    {
                        bindingSource1.Add(item);
                    }
                    //Log("Debug", $"{item.AddTime}");
                }
                Log("Info", $"列表已加载");
            }
            else
            {
                Log("Info", $"{control.ItemsFile}不存在");
            }
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
