using System;

namespace Examples.Data {

    /// <summary>
    /// 订单状态集
    /// </summary>
    [Const("订单状态集")]
    public enum OrderStates  {
        /// <summary>
        /// 待支付
        /// </summary>
        [Const("待支付")]
        [Const("Order", "1")]
        PendingPayment=10,
        /// <summary>
        /// 待发货
        /// </summary>
        [Const("待发货")]
        [Const("Order", "2")]
        PendingShipment=20,
        /// <summary>
        /// 运输中
        /// </summary>
        [Const("运输中")]
        [Const("Order", "3")]
        InTransit = 30,
        /// <summary>
        /// 已签收
        /// </summary>
        [Const("已签收")]
        [Const("Order", "2.5")]//测试排序
        Received = 40,
        /// <summary>
        /// 已关闭
        /// </summary>
        [Const("已关闭")]
        [Const("Order", "5")]
        Closed = 256

    }

}
