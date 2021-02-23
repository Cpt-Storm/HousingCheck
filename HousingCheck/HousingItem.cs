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
                        && obj.Size == Size
                        && obj.Price == Price);
        }

    }

    public class HousingItemEX : IEquatable<HousingItemEX>
    {
        public HousingItemEX(string _area, int _slot, int _id, string _size, string _name, bool _isOpen, int _price, string _ownerName, string _tag1, string _tag2, string _tag3,
                             DateTime _fcTimeCreate, int _fcPopulation, int _fcOnline, int _fcLevel, string _fcName, string _fcTag)
        {
            Area = _area;
            Slot = _slot;
            Id = _id;
            Size = _size;
            Name = _name;
            IsOpen = _isOpen;
            Price = _price;
            Owner_Name = _ownerName;
            Tag1 = _tag1;
            Tag2 = _tag2;
            Tag3 = _tag3;
            FC_TimeCreated = _fcTimeCreate;
            FC_Population = _fcPopulation;
            FC_Online = _fcOnline;
            FC_Level = _fcLevel;
            FC_Name = _fcName;
            FC_Tag = _fcTag;
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
        [DisplayName("价格")]
        public int Price { get; set; }
        [DisplayName("持有人/部队长")]
        public string Owner_Name { get; set; }
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
                        && obj.Size == Size
                        && obj.Price == Price);
        }

    }

}
