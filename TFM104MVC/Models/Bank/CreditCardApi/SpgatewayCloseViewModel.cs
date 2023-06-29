namespace TFM104MVC.Models.Bank.CreditCardApi
{
    /// <summary>
    /// [智付通金流]介接資料模型(Deposit Reversal、Refund、Refund Reversal)
    /// </summary>
    public class SpgatewayCloseViewModel
    {
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 請退款金額
        /// <para>1.純數字不含符號</para>
        /// <para>2.一次刷卡請款金額需小於或等於授權金額</para>
        /// <para>3.一次刷卡退款金額需小於或等於請款金額</para>
        /// <para>4.分期付款請款金額須等於授權金額</para>
        /// <para>5.分期付款退款金額須等於請款金額</para>
        /// <para>6.紅利折抵請款金額須等於授權金額</para>
        /// <para>7.紅利折抵退款金額須等於請款金額</para>
        /// </summary>
        public string Amt { get; set; }

        /// <summary>
        /// 執行類型
        /// </summary>
        public string ExeType { get; set; }

    }
}