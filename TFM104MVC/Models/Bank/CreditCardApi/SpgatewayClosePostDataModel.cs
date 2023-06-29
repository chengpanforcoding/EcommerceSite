namespace TFM104MVC.Models.Bank.CreditCardApi
{
        /// <summary>
        /// [智付通金流]介接資料模型(Deposit Reversal、Refund、Refund Reversal)
        /// </summary>
        public class SpgatewayClosePostDataModel
        {
            /// <summary>
            /// 回傳格式(String 或是 JSON)
            /// </summary>
            public string RespondType { get; set; }

            /// <summary>
            /// 串接程式版本(請帶 1.0。)
            /// </summary>
            public string Version { get; set; }

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
            public int Amt { get; set; }

            /// <summary>
            /// 商店訂單編號
            /// </summary>
            public string MerchantOrderNo { get; set; }

            /// <summary>
            /// 時間戳記
            /// </summary>
            public string TimeStamp { get; set; }

            /// <summary>
            /// 選用單號類別(1 代表選用商店訂單編號 2 代表選用智付通交易序號)
            /// <para>當選用其中一種單號類別時，該種單號不可空白</para>
            /// </summary>
            public int IndexType { get; set; }

            /// <summary>
            /// 智付通交易序號
            /// </summary>
            public string TradeNo { get; set; }

            /// <summary>
            /// 請款或退款(請款交易時請填 1，退款交易時請填 2)
            /// </summary>
            public int CloseType { get; set; }

            /// <summary>
            /// 取消請款或退款
            /// <para>1.取消請款或退款交易時請填 1</para>
            /// <para>2.當傳送取消請款或退款參數時，系統將會取消該筆請款中或退款中的作業流程。</para>
            /// </summary>
            public int? Cancel { get; set; }
        }
    }