using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarNumberChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<CarNumber> CarNumberList { get; set; }

        private void btnDataCreate_Click(object sender, EventArgs e)
        {
            CarUsageCode[] carUsage = (CarUsageCode[])Enum.GetValues(typeof(CarUsageCode));

            var randomizer = new Random(DateTime.Now.Second);
            var randomAlphaNum = new RandomAlphaNumeric(randomizer);

            this.CarNumberList = new List<CarNumber>();
            for (int i = 1; i <= 100; i++)
            {
                string number = randomizer.Next(2, 10).ToString() + randomAlphaNum.ToString() + randomAlphaNum.ToString();
                number = number.Substring(0, randomizer.Next(1, 4));
                this.CarNumberList.Add(new CarNumber(carUsage[randomizer.Next(0, carUsage.Length)], number));
            }

            this.dataGridView1.DataSource = this.CarNumberList;
        }

        private void btnDivideResult_Click(object sender, EventArgs e)
        {
            if (null == this.CarNumberList)
            {
                return;
            }

            var lstOK = this.CarNumberList.FindAll(match => match.IsValidNumber);
            lstOK.Sort((itemA, itemB) => itemA.CarUsage.Equals(itemB.CarUsage) ? itemA.ClassNumber.CompareTo(itemB.ClassNumber) : itemA.CarUsage.CompareTo(itemB.CarUsage));
            this.dataGridView2.DataSource = lstOK;

            var lstNG = this.CarNumberList.FindAll(match => !match.IsValidNumber);
            lstNG.Sort((itemA, itemB) => itemA.CarUsage.Equals(itemB.CarUsage) ? itemA.ClassNumber.CompareTo(itemB.ClassNumber) : itemA.CarUsage.CompareTo(itemB.CarUsage));
            this.dataGridView3.DataSource = lstNG;

            this.CarNumberList.Sort((itemA, itemB) => itemA.CarUsage.Equals(itemB.CarUsage) ? itemB.IsValidNumber.CompareTo(itemA.IsValidNumber) : itemA.CarUsage.CompareTo(itemB.CarUsage));
            this.dataGridView1.Refresh();
        }
    }

    public class RandomAlphaNumeric
    {
        public RandomAlphaNumeric(Random rand)
        {
            this.Randomizer = rand;
        }

        public Random Randomizer { get; set; }

        public override string ToString()
        {
            if (0 == (this.Randomizer.Next(0,9) % 3))
            {
                return ((char)this.Randomizer.Next(65, 90)).ToString();
            }
            else
            {
                return this.Randomizer.Next(0, 9).ToString();
            }
        }

        public static bool IsAlphanumeric(char c)
        {
            return char.IsNumber(c) || (65 <= (int)c && (int)c <= 90);
        }
    }

    /// <summary>
    /// 用途車種
    /// </summary>
    public enum CarUsageCode
    {
        /// <summary>
        /// 自家用普通乗用
        /// </summary>
        JikayouFutuJyouyou1 = 11,
        /// <summary>
        /// 自家用普通乗用()
        /// </summary>
        JikayouFutuJyouyou2 = 12,
        /// <summary>
        /// 自家用小型乗用
        /// </summary>
        JikayouKogataJyouyou1 = 13,
        /// <summary>
        /// 自家用小型乗用()
        /// </summary>
        JikayouKogataJyouyou2 = 14,
        /// <summary>
        /// 自家用軽四輪乗用
        /// </summary>
        JikayouKeiyonrinJouyou1 = 31,
        /// <summary>
        /// 自家用軽四輪乗用()
        /// </summary>
        JikayouKeiyonrinJouyou2 = 32,
        /// <summary>
        /// 自家用軽四輪貨物
        /// </summary>
        JikayouKeiyonrinKamotsu = 33,
        /// <summary>
        /// 自家用小型貨物
        /// </summary>
        JikayouKogataKamotsu = 49
    }

    /// <summary>
    /// 自動車ナンバー
    /// </summary>
    public class CarNumber
    {
        /// <summary>
        /// 分類番号最大長
        /// </summary>
        public const int MAX_LENGTH_CLASS_NUMBER = 3;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="carUsage">用途車種</param>
        /// <param name="classNumber">分類番号</param>
        public CarNumber(CarUsageCode carUsage, string classNumber)
        {
            this.CarUsage = carUsage;
            this.ClassNumber = classNumber;
        }

        /// <summary>
        /// 用途車種
        /// </summary>
        public CarUsageCode CarUsage { get; set; }

        /// <summary>
        /// 分類番号
        /// </summary>
        public string ClassNumber { get; set; }

        /// <summary>
        /// 有効なナンバーか判定する
        /// </summary>
        public bool IsValidNumber
        {
            get
            {
                return this.ValidateClassNumber() && true;
            }
        }

        /// <summary>
        /// 分類番号検証
        /// </summary>
        /// <returns>有効/無効</returns>
        private bool ValidateClassNumber()
        {
            int classNumber = 0;
            if (MAX_LENGTH_CLASS_NUMBER.Equals(this.ClassNumber.Length) & !int.TryParse(this.ClassNumber, out classNumber))
            {
                switch (this.CarUsage)
                {
                    case CarUsageCode.JikayouFutuJyouyou1:
                    case CarUsageCode.JikayouFutuJyouyou2:
                        return '3'.Equals(this.ClassNumber[0]) && 
                                RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[1]) && RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[2]);

                    case CarUsageCode.JikayouKogataJyouyou1:
                    case CarUsageCode.JikayouKogataJyouyou2:
                    case CarUsageCode.JikayouKeiyonrinJouyou1:
                    case CarUsageCode.JikayouKeiyonrinJouyou2:
                        return ('5'.Equals(this.ClassNumber[0]) || '7'.Equals(this.ClassNumber[0])) && 
                                RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[1]) && RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[2]);

                    case CarUsageCode.JikayouKeiyonrinKamotsu:
                    case CarUsageCode.JikayouKogataKamotsu:
                        return ('4'.Equals(this.ClassNumber[0]) || '6'.Equals(this.ClassNumber[0]) || '8'.Equals(this.ClassNumber[0])) && 
                                RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[1]) && RandomAlphaNumeric.IsAlphanumeric(this.ClassNumber[2]);

                    default:
                        break;
                }
            }

            switch (this.CarUsage)
            {
                case CarUsageCode.JikayouFutuJyouyou1:
                case CarUsageCode.JikayouFutuJyouyou2:
                    return (3 == classNumber) || (30 <= classNumber && classNumber <= 39) || (300 <= classNumber && classNumber <= 399);

                case CarUsageCode.JikayouKogataJyouyou1:
                case CarUsageCode.JikayouKogataJyouyou2:
                    return (5 == classNumber) || (50 <= classNumber && classNumber <= 59) || (500 <= classNumber && classNumber <= 599) ||
                           (7 == classNumber) || (70 <= classNumber && classNumber <= 79) || (700 <= classNumber && classNumber <= 799);

                case CarUsageCode.JikayouKeiyonrinJouyou1:
                case CarUsageCode.JikayouKeiyonrinJouyou2:
                    return (50 <= classNumber && classNumber <= 59) || (500 <= classNumber && classNumber <= 599) ||
                           (70 <= classNumber && classNumber <= 79) || (700 <= classNumber && classNumber <= 799);

                case CarUsageCode.JikayouKeiyonrinKamotsu:
                    return (40 <= classNumber && classNumber <= 49) || (400 <= classNumber && classNumber <= 499) ||
                           (600 <= classNumber && classNumber <= 699) ||
                           (80 <= classNumber && classNumber <= 89) || (800 <= classNumber && classNumber <= 899);

                case CarUsageCode.JikayouKogataKamotsu:
                    return (4 == classNumber) || (40 <= classNumber && classNumber <= 49) || (400 <= classNumber && classNumber <= 499) ||
                           (6 == classNumber) || (60 <= classNumber && classNumber <= 69) || (600 <= classNumber && classNumber <= 699) ||
                           (8 == classNumber) || (80 <= classNumber && classNumber <= 89) || (800 <= classNumber && classNumber <= 899);

                default:
                    return true;
            }
        }
    }
}
