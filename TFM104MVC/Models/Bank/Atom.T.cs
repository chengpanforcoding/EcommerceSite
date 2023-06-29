namespace TFM104MVC.Models.Bank
{
    /// <summary>執行結果基元(泛型)</summary>
    public class Atom<T>
    {
        /// <summary>執行是否成功</summary>
        public bool IsSuccess { get; set; }

        /// <summary>是否為錯誤狀態</summary>
        public bool IsError { get; set; }

        /// <summary>訊息</summary>
        public string Message { get; set; }

        /// <summary>項目</summary>
        public T Item { get; set; }
    }
}