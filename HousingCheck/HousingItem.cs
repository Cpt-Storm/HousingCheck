using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousingCheck
{
    public class HousingItem : IEquatable<HousingItem>
    {
        public HousingItem(string _area, int _slot, int _id, string _size, int _price)
        {
            Area = _area;
            Slot = _slot;
            Id = _id;
            Size = _size;
            Price = _price;
            AddTime = DateTime.Now;
            ExistenceTime = DateTime.Now;
        }


        [DisplayName("住宅区")]
        public string Area { get; set; }
        [DisplayName("区")]
        public int Slot { get; set; }
        [DisplayName("号")]
        public int Id { get; set; }
        [DisplayName("大小")]
        public string Size { get; set; }
        [DisplayName("价格")]
        public int Price { get; set; }
        [DisplayName("首次记录时间")]
        public DateTime AddTime { get; set; }
        [DisplayName("最后记录时间")]
        public DateTime ExistenceTime { get; set; }

        public bool Equals(HousingItem obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj.Area == Area
                        && obj.Slot == Slot
                        && obj.Id == Id
                        && obj.Size == Size);
        }

    }

    public class HousingItemEX : IEquatable<HousingItemEX>
    {
        public HousingItemEX(string _area, int _slot, int _id, string _size, string _name, bool _isOpen, bool _isFC, int _price, string _ownerName, string _tag1, string _tag2, string _tag3,
                             string _fcLeader, DateTime _fcTimeCreate, int _fcPopulation, int _fcOnline, int _fcLevel, string _fcName, string _fcTag, string _description)
        {
            Area = _area;
            Slot = _slot;
            Id = _id;
            Size = _size;
            Name = _name;
            IsOpen = _isOpen;
            IsFC = _isFC;
            Price = _price;
            Owner_Name = _ownerName;
            Tag1 = _tag1;
            Tag2 = _tag2;
            Tag3 = _tag3;
            FC_Leader = _fcLeader;
            FC_TimeCreated = _fcTimeCreate;
            FC_Population = _fcPopulation;
            FC_Online = _fcOnline;
            FC_Level = _fcLevel;
            FC_Name = _fcName;
            FC_Tag = _fcTag;
            House_Description = _description;
            //AddTime = DateTime.Now;
            //ExistenceTime = DateTime.Now;
        }

        [DisplayName("住宅区")]
        public string Area { get; set; }
        [DisplayName("区")]
        public int Slot { get; set; }
        [DisplayName("号")]
        public int Id { get; set; }
        [DisplayName("大小")]
        public string Size { get; set; }
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayName("TAG1")]
        public string Tag1 { get; set; }
        [DisplayName("TAG2")]
        public string Tag2 { get; set; }
        [DisplayName("TAG3")]
        public string Tag3 { get; set; }
        [DisplayName("是否开放")]
        public bool IsOpen { get; set; }
        [DisplayName("部队房/个人房")]
        public bool IsFC { get; set; }
        [DisplayName("价格")]
        public int Price { get; set; }
        [DisplayName("房屋持有对象")]
        public string Owner_Name { get; set; }
        [DisplayName("部队长")]
        public string FC_Leader { get; set; }
        [DisplayName("部队创建时间")]
        public DateTime FC_TimeCreated { get; set; }
        [DisplayName("部队人口")]
        public int FC_Population { get; set; }
        [DisplayName("部队在线人口")]
        public int FC_Online { get; set; }
        [DisplayName("部队阶级")]
        public int FC_Level { get; set; }
        [DisplayName("部队名")]
        public string FC_Name { get; set; }
        [DisplayName("部队后缀")]
        public string FC_Tag { get; set; }
        [DisplayName("房屋详情")]
        public string House_Description { get; set; }

        public bool Equals(HousingItemEX obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj.Area == Area
                        && obj.Slot == Slot
                        && obj.Id == Id
                        && obj.FC_Tag == FC_Tag);
        }

    }

    public class HousingData
    {
        public string HousingAppeal(uint ID)
        {
            switch (ID)
            {
                case 0:
                    return "无";
                case 1:
                    return "商店";
                case 2:
                    return "服装模特商店";
                case 3:
                    return "内部装潢别致";
                case 4:
                    return "交流簿";
                case 5:
                    return "酒馆";
                case 6:
                    return "餐馆";
                case 7:
                    return "角色扮演";
                case 8:
                    return "咖啡馆";
                case 9:
                    return "水族馆";
                case 10:
                    return "教堂";
                case 11:
                    return "活动会场";
                case 12:
                    return "花店";
                case 14:
                    return "图书馆";
                case 15:
                    return "摄影棚";
                case 16:
                    return "鬼怪屋";
                case 17:
                    return "工房";
                case 18:
                    return "浴场";
                case 19:
                    return "植物园";
                case 20:
                    return "东方样式";
                case 21:
                    return "欢迎来访";
                case 22:
                    return "面包房";
                case 23:
                    return "装修中";
                case 24:
                    return "剧场";
                default:
                    return "无";
            }
        }
           
        public string HousingArea(uint ID)
        {
            switch (ID)
            {
                case 0x53:
                    return "海雾村";
                case 0x54:
                    return "薰衣草苗圃";
                case 0x55:
                    return "高脚孤丘";
                case 0x81:
                    return "白银乡";
                default:
                    return "UnknownArea";
            }
        }
    }
}
